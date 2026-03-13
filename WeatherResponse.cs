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
        /// Represented in the format YYYY-MM-DDTHH:mm (ISO 8601).
        /// Where T is the separator between date and time.
        /// </summary>
        public string? time { get; set; }
    }
}
