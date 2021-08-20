using System;

namespace WeatherTest
{
    public class CityNotExistException : Exception
    {
        public CityNotExistException()
        {
        }

        public CityNotExistException(string message)
            : base(message)
        {
        }

        public CityNotExistException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
