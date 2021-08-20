namespace WeatherTest.Models
{
    public class Weather
    {
        public int Temp { get; set; }
        public int FeelsLike { get; set; }
        public int? TempWater { get; set; }
        public string Condition { get; set; }
        public decimal WindSpeed { get; set; }
        public decimal WindGust { get; set; }
        public string WindDir { get; set; }
        public int Pressure { get; set; }
        public string PrecType { get; set; }

    }
}


