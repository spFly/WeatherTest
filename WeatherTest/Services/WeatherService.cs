using EventTypes;
using MassTransit;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherTest.Helpers;
using WeatherTest.Models;

namespace WeatherTest.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IConnectionMultiplexer redis;
        private readonly IHttpClientFactory clientFactory;
        private readonly ICityService cityService;
        private readonly IBus bus;
        private readonly IConfiguration configuration;
        private readonly ILogger<TimedHostedService> logger;

        public WeatherService(IConnectionMultiplexer redis, IHttpClientFactory clientFactory, ICityService cityService, IBus bus, IConfiguration configuration, ILogger<TimedHostedService> logger)
        {            
            this.redis = redis;
            this.clientFactory = clientFactory;
            this.cityService = cityService;
            this.bus = bus;
            this.configuration = configuration;
            this.logger = logger;
        }

        /// <summary>
        /// Получение информации о погоде по переданному городу
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public async Task<string> GetWeather(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                logger.LogError($"'{nameof(city)}' cannot be null or whitespace.", nameof(city));
                throw new ArgumentException($"'{nameof(city)}' cannot be null or whitespace.", nameof(city));
            }

            await bus.Publish(new Message { City = city, Time = DateTimeOffset.Now  });
            city = city.ToLower();

            var db = redis.GetDatabase();
            List<string> cities = JsonConvert.DeserializeObject<List<string>>(await db.StringGetAsync("cities"));
            if (!cities.Exists(x => x == city))
            {
                logger.LogError($"There is no weather for the city {city}");
                throw new CityNotExistException($"There is no weather for the city {city}");
            }
            return await db.StringGetAsync(city);
        }

        /// <summary>
        /// Adding weather data to cache
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CachingWeather()
        {
            var db = redis.GetDatabase();
            List<Task> tasks = new();            
            List<City> cities = cityService.GetCities();
            tasks.Add(db.StringSetAsync("cities", JsonConvert.SerializeObject(cities.Select(x => x.Name.ToLower()))));

            using (var client = clientFactory.CreateClient("WeatherClient"))
            {
                foreach (var city in cities)
                {
                    var queryParams = new Dictionary<string, string>()
                    {
                        {"lat", city.Latitude.ToString() },
                        {"lon", city.Longitude.ToString() },
                        {"lang","ru_RU" },
                        {"limit","1" },
                        {"hours","false" },
                    };
                    var url = QueryHelpers.AddQueryString("v2/forecast", queryParams);
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.Add("X-Yandex-API-Key", configuration.GetValue<string>("YandexAPIKey"));

                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseStream = await response.Content.ReadAsStringAsync();
                        var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseStream);
                        var dictionary = JObject.FromObject(values["fact"]).ToObject<Dictionary<string, object>>();
                        try
                        {
                            tasks.Add(db.StringSetAsync(city.Name.ToLower(), JsonConvert.SerializeObject(
                                new Weather()
                                {
                                    Temp = Convert.ToInt32(dictionary["temp"]),
                                    FeelsLike = Convert.ToInt32(dictionary["feels_like"]),
                                    TempWater = dictionary.ContainsKey("temp_water") ? Convert.ToInt32(dictionary["temp_water"]) : null,
                                    Condition = WeatherHelper.GetCondition(dictionary["condition"].ToString()),
                                    WindSpeed = Convert.ToDecimal(dictionary["wind_speed"]),
                                    WindGust = Convert.ToDecimal(dictionary["wind_gust"]),
                                    WindDir = WeatherHelper.GetWindDirection( dictionary["wind_dir"].ToString()),
                                    Pressure = Convert.ToInt32(dictionary["pressure_mm"]),
                                    PrecType = WeatherHelper.GetPrecType(Convert.ToInt32(dictionary["prec_type"]))
                                }
                                )));
                            logger.LogInformation($"Caching Weather at {DateTime.Now}");
                        }
                        catch (Exception e)
                        {
                            logger.LogError($"Mapping exception {e.Message}");
                        }
                    }
                    else
                    {
                        logger.LogError($"Http request exception {response.StatusCode}");
                    }
                }
            }

            Task.WaitAll(tasks.ToArray());
            return true;
        }
    }
}
