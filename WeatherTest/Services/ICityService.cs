using System.Collections.Generic;
using WeatherTest.Models;

namespace WeatherTest.Services
{
    public interface ICityService
    {
        List<City> GetCities();
    }
}
