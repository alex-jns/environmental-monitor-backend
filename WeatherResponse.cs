using System.Net;
using System.Runtime.ConstrainedExecution;

namespace Environmental_Monitor
{
    /// <summary>
    /// Matches the structure of the JSON object.
    /// </summary>
    internal class WeatherResponse
    {
        /// <summary>
        /// Matches the object contained within the JSON response.
        /// </summary>
        public CurrentWeather? current { get; set; }
    }

    /// <summary>
    /// Matches the structure of the object in the JSON response.
    /// </summary>
    public class CurrentWeather
    {
        /// <summary>
        /// Represented in the format YYYY-MM-DDTHH:mm (ISO 8601).
        /// Where T is the separator between date and time.
        /// </summary>
        public string? time { get; set; }

        /// <summary>
        /// Current temperature in celsius.
        /// </summary>
        public double temperature_2m { get; set; }

        /// <summary>
        /// Converts temperature from celsius to fahrenheit using the formula F = (C * 9/5) + 32.
        /// </summary>
        public double temperature_2m_fahrenheit
        {
            get { return (temperature_2m * 9 / 5) + 32; }
            set { }
        }

        /// <summary>
        /// Current relative humidity in percentage.
        /// </summary>
        public double relative_humidity_2m { get; set; }

        /// <summary>
        /// Apparent temperature is the perceived feels-like temperature.
        /// combining wind chill factor, relative humidity and solar radiation.
        /// This project represents apparent temperature in celsius.
        /// </summary>
        public double apparent_temperature { get; set; }

        /// <summary>
        /// Converts apparent temperature from celsius to farenheit.
        /// </summary>
        public double apparent_temperature_farenheit
        {
            get { return (apparent_temperature * 9 / 5) + 32;  }
            set { }
        }

        /// <summary>
        /// 1 if the current time step has daylight, 0 at night.
        /// </summary>
        public int is_day { get; set; }

        /// <summary>
        /// Converts is_day into a string representation.
        /// </summary>
        public string is_day_yesorno
        {
            get { if (is_day == 1) { return "Yes"; } else { return "No"; } }
            set { }
        }

        /// <summary>
        /// Weather condition as a numeric code.
        /// Follow WMO weather interpretation codes.
        /// See table below for details.
        /// 
        /// Code	    Description
        /// ----        -----------
        ///	0           Clear sky
        ///	1, 2, 3	    Mainly clear, partly cloudy, and overcast
        ///	45, 48	    Fog and depositing rime fog
        ///	51, 53, 55	Drizzle: Light, moderate, and dense intensity
        ///	56, 57	    Freezing Drizzle: Light and dense intensity
        ///	61, 63, 65	Rain: Slight, moderate and heavy intensity
        ///	66, 67	    Freezing Rain: Light and heavy intensity
        ///	71, 73, 75	Snow fall: Slight, moderate, and heavy intensity
        ///	77	        Snow grains
        ///	80, 81, 82	Rain showers: Slight, moderate, and violent
        ///	85, 86	    Snow showers slight and heavy
        ///	95 *	    Thunderstorm: Slight or moderate
        ///	96, 99 *	Thunderstorm with slight and heavy hail
        ///	
        ///	(*) Thunderstorm forecast with hail is only available in Central Europe
        /// </summary>
        public int weather_code { get; set; }

        /// <summary>
        /// Represents a string representation of the weather code.
        /// </summary>
        public string weather_name
        {
            get
            {
                switch (weather_code)
                {
                    case 0: return "Clear sky";
                    case 1: return "Mainly clear";
                    case 2: return "Partly cloudy";
                    case 3: return "Overcast";
                    case 45: return "Fog";
                    case 48: return "Depositing rime fog";
                    case 51: return "Light drizzle";
                    case 53: return "Moderate drizzle";
                    case 55: return "Dense drizzle";
                    case 56: return "Light freezing drizzle";
                    case 57: return "Dense freezing drizzle";
                    case 61: return "Slight rain";
                    case 63: return "Moderate rain";
                    case 65: return "Heavy rain";
                    case 66: return "Light freezing rain";
                    case 67: return "Heavy freezing rain";
                    case 71: return "Slight snow fall";
                    case 73: return "Moderate snow fall";
                    case 75: return "Heavy snow fall";
                    case 77: return "Snow grains";
                    case 80: return "Slight rain showers";
                    case 81: return "Moderate rain showers";
                    case 82: return "Violent rain showers";
                    case 85: return "Slight snow showers";
                    case 86: return "Heavy snow showers";
                    case 95: return "Thunderstorm";
                    case 96: return "Thunderstorm with slight hail";
                    case 99: return "Thunderstorm with heavy hail";
                }

                return "Unknown";
            }
            set { }
        }

        /// <summary>
        /// Total cloud cover as an area fraction (percentage).
        /// </summary>
        public double cloud_cover { get; set; }

        /// <summary>
        /// Total precipitation (rain, showers, snow) sum of the preceding hour.
        /// This project represents precipitation in inches.
        /// </summary>
        public double precipitation { get; set; }

        /// <summary>
        /// Rain from large scale weather systems of the preceding hour in millimeter.
        /// This project represents rain in inches.
        /// </summary>
        public double rain { get; set; }

        /// <summary>
        /// Showers from convective precipitation in millimeters from the preceding hour.
        /// This project represents showers in inches.
        /// </summary>
        public double showers { get; set; }

        /// <summary>
        /// Snowfall amount of the preceding hour in centimeters.
        /// For the water equivalent in millimeter, divide by 7.
        /// E.g. 7 cm snow = 10 mm precipitation water equivalent
        /// This project represents snowfall in inches.
        /// </summary>
        public double snowfall { get; set; }

        /// <summary>
        /// Wind speed at 10, 80, 120 or 180 meters above ground.
        /// This project uses wind speed at 10 meters above ground in miles per hour.
        /// </summary>
        public double wind_speed_10m { get; set; }

        /// <summary>
        /// Wind direction at 10, 80, 120 or 180 meters above ground.
        /// This project uses wind speed at 10 meters above ground using degrees for direction.
        /// </summary>
        public double wind_direction_10m { get; set; }
    }
}
