using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherTest.Helpers
{
    public static class WeatherHelper
    {
        public static string GetCondition(string condition) => condition switch
        {
            "clear" => "ясно",
            "partly-cloudy" => "малооблачно",
            "cloudy" => "облачно с прояснениями",
            "overcast" => "пасмурно",
            "drizzle" => "морось",
            "light-rain" => "небольшой дождь",
            "rain" => "дождь",
            "moderate-rain" => "умеренно сильный дождь",
            "heavy-rain" => "сильный дождь",
            "continuous-heavy-rain" => "длительный сильный дождь",
            "showers" => "ливень",
            "wet-snow" => "дождь со снегом",
            "light-snow" => "небольшой снег",
            "snow" => "снег",
            "snow-showers" => "снегопад",
            "hail" => "град",
            "thunderstorm" => "гроза",
            "thunderstorm-with-rain" => "дождь с грозой",
            "thunderstorm-with-hail" => "гроза с градом",
            _ => condition
        };

        public static string GetWindDirection(string direction) => direction switch
        {
            "nw" => "северо-западное",
            "n" => "северное",
            "ne" => "северо-восточное",
            "e" => "восточное",
            "se" => "юго-восточное",
            "s" => "южное",
            "sw" => "юго-западное",
            "w" => "западное",
            "c" => "штиль",
            _ => direction
        };

        public static string GetPrecType(int prectype) => prectype switch
        {
            0 => "без осадков",
            1 => "дождь",
            2 => "дождь со снегом",
            3 => "снег",
            4 => "град",
            _ => prectype.ToString()
        };        

    }


}

