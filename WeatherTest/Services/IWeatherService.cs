using System.Threading.Tasks;

namespace WeatherTest.Services
{
    public interface IWeatherService
    {
        Task<string> GetWeather(string city);
        Task<bool> CachingWeather();
    }
}
