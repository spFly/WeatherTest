using System.Collections.Generic;
using WeatherTest.Models;

namespace WeatherTest.Services
{
    public class CityService: ICityService
    {
        /// <summary>
        /// Метод получения списка доступных городов, при необходимости можно заменить реализацию на любую другую(получение из БД, получения из другого сервиса и т.д.)
        /// </summary>
        /// <returns></returns>
        public List<City> GetCities()
        {
            return new()
            {
                new() { Name = "Krasnodar", Latitude = 38.985679M, Longitude = 45.066115M },
                new() { Name = "Moscow", Latitude = 37.646930M, Longitude = 55.725146M },
                new() { Name = "Orenburg", Latitude = 55.192684M, Longitude = 51.794660M },
                new() { Name = "St.Peretburg", Latitude = 30.304908M, Longitude = 59.918072M },
                new() { Name = "Kaliningrad", Latitude = 20.473801M, Longitude = 54.704529M },
            };
        }
    }
}
