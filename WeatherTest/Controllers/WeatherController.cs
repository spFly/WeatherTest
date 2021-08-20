using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WeatherTest.Services;

namespace WeatherTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService weatherService;
        public WeatherController(IWeatherService weatherService)
        {
           this.weatherService = weatherService;
        }
        [HttpGet()]
        public async Task<string> Get(string city)
        {
            return await weatherService.GetWeather(city);
        }

    }
}
