using System.Text.Json;

namespace Environmental_Monitor
{
    /// <summary>
    /// Responsible for all API related functions.
    /// </summary>
    internal class APIHandler
    {
        /// <summary>
        /// Sends a GET request to the Open-Meteo Uri and deseralizes the returned JSON.
        /// </summary>
        /// <returns>Task representing the async operation.</returns>
        public async Task<WeatherResponse?> CallAPI()
        {
            // Set up the logger
            Logger logger = new Logger("logs/api.log");

            // Setting up the request
            string requestUri = "https://api.open-meteo.com/v1/forecast?" +
                "latitude=36.5951&" +
                "longitude=-82.1887&" +
                "current=temperature_2m," +
                "relative_humidity_2m," +
                "apparent_temperature," +
                "is_day," +
                "weather_code," +
                "cloud_cover," +
                "precipitation," +
                "rain,showers," +
                "snowfall," +
                "wind_speed_10m," +
                "wind_direction_10m&" +
                "timezone=America%2FNew_York&" +
                "forecast_days=1&" +
                "wind_speed_unit=ms&" +
                "precipitation_unit=inch";
            using HttpClient client = new HttpClient();

            // See below for a list of exceptions
            try
            {
                // Sends the request, and handles the response
                logger.Info("Sending a request to the API...");
                HttpResponseMessage response = await client.GetAsync(requestUri);
                logger.Info("Request sent. Awaiting response...");
                string result = await response.Content.ReadAsStringAsync();
                logger.Info("Response received. Validating response...");
                WeatherResponse? weather = JsonSerializer.Deserialize<WeatherResponse>(result);

                // Null value checks
                if (weather == null) { throw new NullReferenceException(nameof(weather)); }
                if (weather.current == null) { throw new NullReferenceException(nameof(weather.current)); }
                if (weather.current.time == null) { throw new NullReferenceException(nameof(weather.current.time)); }

                // Attempt to parse time from ISO 8601 to something readable
                logger.Info("Parsing time...");
                DateTime time = DateTime.Parse(weather.current.time);

                // Show the information
                logger.Info($"Time: {time.ToString("f")}"); // full readable format
                logger.Info($"Temp: {weather.current.temperature_2m} C");
                logger.Info($"Temp: {weather.current.temperature_2m_fahrenheit} F");
                logger.Info($"Humidity: {weather.current.relative_humidity_2m}%");
                logger.Info($"Apparent Temp: {weather.current.apparent_temperature}%");
                logger.Info($"Apparent Temp: {weather.current.apparent_temperature_farenheit}%");
                logger.Info($"Is Day: {weather.current.is_day_yesorno}");
                logger.Info($"Weather Code: {weather.current.weather_code}");
                logger.Info($"Cloud Cover: {weather.current.cloud_cover}");
                logger.Info($"Precipitation: {weather.current.precipitation} inches");
                logger.Info($"Rain: {weather.current.rain} inches");
                logger.Info($"Showers: {weather.current.showers} inches");
                logger.Info($"Snowfall: {weather.current.snowfall} inches");
                logger.Info($"Precipitation: {weather.current.precipitation} inches");
                logger.Info($"Wind Speed: {weather.current.wind_speed_10m} miles per hour");
                logger.Info($"Wind Direction: {weather.current.wind_direction_10m} degrees");

                // Return the deserialized weather response
                return weather;
            }
            // Try GetAsync exceptions
            catch (InvalidOperationException ex) { logger.Warning($"Invalid operation exception: {ex.Message}"); }
            catch (HttpRequestException ex) { logger.Warning($"HTTP request exception: {ex.Message}"); }
            catch (OperationCanceledException ex) { logger.Warning($"Operation canceled exception: {ex.Message}"); }
            catch (UriFormatException ex) { logger.Warning($"URI format exception: {ex.Message}"); }

            // Try Deserialize exceptions
            catch (ArgumentNullException ex) { logger.Warning($"Argument null exception: {ex.Message}"); }
            catch (JsonException ex) { logger.Warning($"JSON exception: {ex.Message}"); }
            catch (NotSupportedException ex) { logger.Error($"Not supported exception: {ex.Message}"); }

            // Try Parse exceptions
            catch (FormatException ex) { logger.Warning($"Format exception: {ex.Message}"); }

            // Null value checks
            catch (NullReferenceException ex) { logger.Error($"Null value exception: {ex.Message}"); }

            // If all else fails
            catch (Exception ex) { logger.Error($"Error: {ex.Message}"); }

            // On error, return null
            return null;
        }
    }
}