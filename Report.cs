using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Environmental_Monitor
{
    /// <summary>
    /// This class represents the combined information from the UDP message received from the pi and the API response from the weather API.
    /// </summary>
    public class Report
    {
        /// <summary>
        /// Represents the weather data received from the pi via UDP message.
        /// </summary>
        public UdpMessage? Message { get; set; }

        /// <summary>
        /// Represents the weather data received from the API response.
        /// </summary>
        public WeatherResponse? ApiWeather { get; set; }

        /// <summary>
        /// Represents the time parsed from the API response.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Represents the returned value from the InsideWeatherSummary method, which is a quick and concise summary of the inside weather for the dashboard.
        /// </summary>
        public string InsideSummary => InsideWeatherSummary();

        /// <summary>
        /// Represents the returned value from the OutsideWeatherSummary method, which is a quick and concise summary of the outside weather for the dashboard.
        /// </summary>
        public string OutsideSummary => OutsideWeatherSummary();

        /// <summary>
        /// Constructor for the Report class, takes in a UDP message and an API response.
        /// </summary>
        /// <param name="udpMessage">Represents the weather data received from the pi via UDP message.</param>
        /// <param name="weather">Represents the weather data received from the API response.</param>
        public Report(UdpMessage udpMessage, WeatherResponse weather)
        {
            Logger logger = new Logger("logs/report.log");

            // Guard null values
            try
            {
                if (udpMessage == null) { throw new NullReferenceException(nameof(udpMessage)); }
                if (weather == null) { throw new NullReferenceException(nameof(weather)); }
                if (weather.current == null) { throw new NullReferenceException(nameof(weather.current)); }
                if (weather.current.time == null) { throw new NullReferenceException(nameof(weather.current.time)); }
            }
            catch (NullReferenceException ex)
            {
                logger.Error($"Null reference exception in Report constructor: {ex.Message}");
                throw;
            }

            // Assign to instance fields instead of declaring new local variables
            this.Message = udpMessage;
            this.ApiWeather = weather;

            // Attempt to parse time from ISO 8601 to something readable
            try { this.Time = DateTime.Parse(weather.current.time); }
            catch (FormatException ex) { logger.Error($"Format exception in Report constructor: {ex.Message}"); return; }
            catch (ArgumentNullException ex) { logger.Error($"Argument null exception in Report constructor: {ex.Message}"); return; }
            catch (Exception ex) { logger.Error($"Exception in Report constructor: {ex.Message}"); return; }
        }

        /// <summary>
        /// The main purpose of the Report class is to generate a human readable report and a JSON report based on the data from the UDP message and the API response.
        /// </summary>
        /// <param name="udpMessage">Represents the weather data received from the pi via UDP message.</param>
        /// <param name="weather">Represents the weather data received from the API response.</param>
        public void GenerateReport()
        {
            Logger logger = new Logger("logs/report.log");

            // Guard null values
            try
            {
                if (Message == null) { throw new NullReferenceException(nameof(Message)); }
                if (ApiWeather == null) { throw new NullReferenceException(nameof(ApiWeather)); }
                if (ApiWeather.current == null) { throw new NullReferenceException(nameof(ApiWeather.current)); }
                if (ApiWeather.daily == null) { throw new NullReferenceException(nameof(ApiWeather.daily)); }
                if (ApiWeather.daily.temperature_2m_max == null) { throw new NullReferenceException(nameof(ApiWeather.daily.temperature_2m_max)); }
                if (ApiWeather.daily.temperature_2m_min == null) { throw new NullReferenceException(nameof(ApiWeather.daily.temperature_2m_min)); }
                if (ApiWeather.daily.precipitation_probability_max == null) { throw new NullReferenceException(nameof(ApiWeather.daily.precipitation_probability_max)); }
            }
            catch (NullReferenceException ex)
            {
                logger.Error($"Message or ApiWeather values are null: {ex.Message}");
                return;
            }

            // The file name for the report should have the date and time
            string reportFilePath = $"reports/report_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";

            // Create the report directory if none exists
            string? directory = Path.GetDirectoryName(reportFilePath);

            // Guard null directory, throw ex if null
            try
            {
                if (directory == null)
                {
                    throw new NullReferenceException(nameof(directory));
                }
            }
            catch (NullReferenceException ex)
            {
                logger.Error($"Null reference exception when getting directory name: {ex.Message}");
                return;
            }

            // Create directory if it does not exist
            if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

            // If there is no report for the time slot make one
            if (!File.Exists(reportFilePath))
            {
                logger.Info($"Report for this time slot does not exist. Generating report...");

                // String builder is efficient for appending
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Report generated on {DateTime.Now:f}");
                sb.AppendLine($"Inside Temp: {Message.temperatureF} F");
                sb.AppendLine($"Inside Temp: {Message.temperatureC} C");
                sb.AppendLine($"Inside Humidity: {Message.humidity}%");
                sb.AppendLine($"Outside Temp: {ApiWeather.current.temperature_2m} C");
                sb.AppendLine($"Outside Temp: {ApiWeather.current.temperature_2m_fahrenheit} F");
                sb.AppendLine($"Outside Humidity: {ApiWeather.current.relative_humidity_2m}%");
                sb.AppendLine($"Apparent Temp: {ApiWeather.current.apparent_temperature}%");
                sb.AppendLine($"Apparent Temp: {ApiWeather.current.apparent_temperature_fahrenheit}%");
                sb.AppendLine($"Is Day: {ApiWeather.current.is_day_yesorno}");
                sb.AppendLine($"Weather Code: {ApiWeather.current.weather_code}");
                sb.AppendLine($"Weather Name: {ApiWeather.current.weather_name}");
                sb.AppendLine($"Cloud Cover: {ApiWeather.current.cloud_cover}");
                sb.AppendLine($"Precipitation: {ApiWeather.current.precipitation} inches");
                sb.AppendLine($"Rain: {ApiWeather.current.rain} inches");
                sb.AppendLine($"Showers: {ApiWeather.current.showers} inches");
                sb.AppendLine($"Snowfall: {ApiWeather.current.snowfall} inches");
                sb.AppendLine($"Precipitation: {ApiWeather.current.precipitation} inches");
                sb.AppendLine($"Wind Speed: {ApiWeather.current.wind_speed_10m} miles per hour");
                sb.AppendLine($"Wind Direction: {ApiWeather.current.wind_direction_10m_compass} ({ApiWeather.current.wind_direction_10m} degrees)");
                sb.AppendLine($"Inside Weather Summary: {InsideSummary}");
                sb.AppendLine($"Outside Max Temp: {ApiWeather.daily.temperature_2m_max[0]} C");
                sb.AppendLine($"Outside Max Temp: {ApiWeather.daily.temperature_2m_max_fahrenheit} F");
                sb.AppendLine($"Outside Min Temp: {ApiWeather.daily.temperature_2m_min[0]} C");
                sb.AppendLine($"Outside Min Temp: {ApiWeather.daily.temperature_2m_min_fahrenheit} F");
                sb.AppendLine($"Outside Chance of Precipitation: {ApiWeather.daily.precipitation_probability_max[0]}%");

                // Try to catch file I/O exceptions when writing the report, log any errors that occur
                try { File.AppendAllText(reportFilePath, sb.ToString()); }
                catch (ArgumentException ex) { logger.Error($"Invalid path for monthly report: {ex.Message}"); return; }
                catch (IOException ex) { logger.Error($"IO error writing monthly report: {ex.Message}"); return; }
                catch (UnauthorizedAccessException ex) { logger.Error($"Access denied writing monthly report: {ex.Message}"); return; }
                catch (NotSupportedException ex) { logger.Error($"Unsupported path for monthly report: {ex.Message}"); return; }
                catch (Exception ex) { logger.Error($"Failed to write monthly report: {ex.Message}"); return; }

                logger.Info($"Wrote readable report to {reportFilePath}");

                // Also write a JSON version of the report
                string baseName = Path.GetFileNameWithoutExtension(reportFilePath);
                string jsonPath = Path.Combine(directory, baseName + ".json");

                // Anonymous object to hold the report data, this will be serialized to JSON and written to a file
                // This will later be deseralized to the ReportModel class for comparison, so the property names and structure need to match for that to work properly  
                var reportObject = new
                {
                    generated = DateTime.Now,
                    measurementTime = Time,
                    inside = new
                    {
                        temperatureF = Message.temperatureF,
                        temperatureC = Message.temperatureC,
                        humidity = Message.humidity,
                        inside_summary = InsideSummary
                    },
                    outside = new
                    {
                        temperature_2m = ApiWeather.current.temperature_2m,
                        temperature_2m_fahrenheit = ApiWeather.current.temperature_2m_fahrenheit,
                        relative_humidity_2m = ApiWeather.current.relative_humidity_2m,
                        time = ApiWeather.current.time,
                        apparent_temperature = ApiWeather.current.apparent_temperature,
                        apparent_temperature_fahrenheit = ApiWeather.current.apparent_temperature_fahrenheit,
                        is_day_yesorno = ApiWeather.current.is_day_yesorno,
                        weather_code = ApiWeather.current.weather_code,
                        weather_name = ApiWeather.current.weather_name,
                        cloud_cover = ApiWeather.current.cloud_cover,
                        precipitation = ApiWeather.current.precipitation,
                        rain = ApiWeather.current.rain,
                        showers = ApiWeather.current.showers,
                        snowfall = ApiWeather.current.snowfall,
                        wind_speed_10m = ApiWeather.current.wind_speed_10m,
                        wind_direction_10m = ApiWeather.current.wind_direction_10m,
                        wind_direction_10m_compass = ApiWeather.current.wind_direction_10m_compass,
                        outside_summary = OutsideSummary
                    },
                    daily = new
                    {
                        temperature_2m_max = ApiWeather.daily.temperature_2m_max[0],
                        temperature_2m_max_fahrenheit = ApiWeather.daily.temperature_2m_max_fahrenheit,
                        temperature_2m_min = ApiWeather.daily.temperature_2m_min[0],
                        temperature_2m_min_fahrenheit = ApiWeather.daily.temperature_2m_min_fahrenheit,
                        precipitation_probability_max = ApiWeather.daily.precipitation_probability_max[0]
                    }
                };

                var jsonOptions = new JsonSerializerOptions { WriteIndented = true };

                // Try to catch file I/O exceptions when writing the JSON report, log any errors that occur
                try { File.WriteAllText(jsonPath, JsonSerializer.Serialize(reportObject, jsonOptions)); }
                catch (ArgumentException ex) { logger.Error($"Invalid path for monthly report: {ex.Message}"); return; }
                catch (IOException ex) { logger.Error($"IO error writing monthly report: {ex.Message}"); return; }
                catch (UnauthorizedAccessException ex) { logger.Error($"Access denied writing monthly report: {ex.Message}"); return; }
                catch (NotSupportedException ex) { logger.Error($"Unsupported path for monthly report: {ex.Message}"); return; }
                catch (Exception ex) { logger.Error($"Failed to write monthly report: {ex.Message}"); return; }

                logger.Info($"Wrote JSON report to {jsonPath}");
            }
            else
            {
                logger.Info($"Log for this time slot already exists.");
            }
        }

        /// <summary>
        /// This attempts to find the change in temperature and humidity compared to the last two JSON reports.
        /// The problem is finding out which two reports are the "most recent" ones.
        /// Once we have that, we simply need to subtract to find the difference.
        /// </summary>
        public void CompareToLastReport()
        {
            // Compare the two most recent JSON reports in the reports directory
            Logger logger = new Logger("logs/report.log");

            // The directory should exist since we create it when generating reports, but we should check just in case
            // Does not create it since we are only comparing existing reports, if there is no directory or not enough reports to compare, we can just log that and return
            string reportsDir = "reports";
            if (!Directory.Exists(reportsDir))
            {
                logger.Info("No reports directory found to compare.");
                return;
            }

            // Get all JSON report files and ensure there are at least 2 to compare
            // Exit early if there are not enough reports to compare, we need at least 2 to find a difference
            var jsonFiles = Directory.GetFiles(reportsDir, "report_*.json");
            if (jsonFiles.Length < 2)
            {
                logger.Info("Not enough JSON reports to compare (need at least 2).");
                return;
            }

            // Order by last write time and take the last two
            var ordered = jsonFiles.OrderBy(f => File.GetLastWriteTimeUtc(f)).ToArray();
            string previousPath = ordered[ordered.Length - 2];
            string latestPath = ordered[ordered.Length - 1];

            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                string prevJson = File.ReadAllText(previousPath);
                string latestJson = File.ReadAllText(latestPath);

                var prevReport = JsonSerializer.Deserialize<ReportModel>(prevJson, options);
                var latestReport = JsonSerializer.Deserialize<ReportModel>(latestJson, options);

                // Guard against deserialization failure
                if (prevReport == null || latestReport == null)
                {
                    logger.Info("Failed to deserialize one or both reports.");
                    return;
                }

                logger.Info($"Comparing reports: {Path.GetFileName(previousPath)} -> {Path.GetFileName(latestPath)}");

                // Inside comparisons
                // Delta refers to the change in value from the previous report to the latest report, a positive delta means an increase and a negative delta means a decrease
                double insideTempFDelta = latestReport.inside.temperatureF - prevReport.inside.temperatureF;
                double insideTempCDelta = latestReport.inside.temperatureC - prevReport.inside.temperatureC;
                double insideHumidityDelta = latestReport.inside.humidity - prevReport.inside.humidity;

                // "+0.##;-0.##;0" refers to the format for positive, negative, and zero values.
                // Positive values will have a "+" sign and up to 2 decimal places, negative values will have a "-" sign and up to 2 decimal places, and zero will be displayed as "0".
                logger.Info($"Inside temp F: {prevReport.inside.temperatureF} -> {latestReport.inside.temperatureF} ({insideTempFDelta:+0.##;-0.##;0})");
                logger.Info($"Inside temp C: {prevReport.inside.temperatureC} -> {latestReport.inside.temperatureC} ({insideTempCDelta:+0.##;-0.##;0})");
                logger.Info($"Inside humidity: {prevReport.inside.humidity} -> {latestReport.inside.humidity} ({insideHumidityDelta:+0.##;-0.##;0})");

                // Outside comparisons
                double outsideTempFDelta = latestReport.outside.temperature_2m_fahrenheit - prevReport.outside.temperature_2m_fahrenheit;
                double outsideTempCDelta = latestReport.outside.temperature_2m - prevReport.outside.temperature_2m;
                double outsideHumidityDelta = latestReport.outside.relative_humidity_2m - prevReport.outside.relative_humidity_2m;

                logger.Info($"Outside temp F: {prevReport.outside.temperature_2m_fahrenheit} -> {latestReport.outside.temperature_2m_fahrenheit} ({outsideTempFDelta:+0.##;-0.##;0})");
                logger.Info($"Outside temp C: {prevReport.outside.temperature_2m} -> {latestReport.outside.temperature_2m} ({outsideTempCDelta:+0.##;-0.##;0})");
                logger.Info($"Outside humidity: {prevReport.outside.relative_humidity_2m} -> {latestReport.outside.relative_humidity_2m} ({outsideHumidityDelta:+0.##;-0.##;0})");

                // Update the latest report file
                string canonicalLatest = Path.Combine(reportsDir, "report_latest.json");
                File.Copy(latestPath, canonicalLatest, true);
                logger.Info($"Updated canonical latest report: {canonicalLatest}");
            }
            catch (ArgumentException ex) { logger.Error($"Invalid path: {ex.Message}"); return; }
            catch (IOException ex) { logger.Error($"IO error: {ex.Message}"); return; }
            catch (JsonException ex) { logger.Error($"JSON error: {ex.Message}"); return; }
            catch (UnauthorizedAccessException ex) { logger.Error($"Access denied: {ex.Message}"); return; }
            catch (NotSupportedException ex) { logger.Error($"Unsupported path: {ex.Message}"); return; }
            catch (Exception ex) { logger.Error($"Error comparing reports: {ex.Message}"); }
        }

        /// <summary>
        /// Used to create a montly report (human readable and JSON) based on the previously generated reports.
        /// Will only be created when specified by the user.
        /// User provides a date range from available reports to create a unified report.
        /// Shows the change in the tracked variables over the specified time period.
        /// </summary>
        public static void GenerateMontlyReport()
        {
            Logger logger = new Logger("logs/report.log");

            string reportsDir = "reports";
            if (!Directory.Exists(reportsDir))
            {
                logger.Info("No reports directory found for monthly report.");
                return;
            }

            // Check if we have a monthly reports direct
            string monthlyReportsDir = "monthly";
            if (!Directory.Exists(monthlyReportsDir))
            {
                Directory.CreateDirectory(monthlyReportsDir);
                logger.Info("Created monthly reports directory.");
            }

            var jsonFiles = Directory.GetFiles(reportsDir, "report_*.json");
            if (jsonFiles.Length == 0)
            {
                logger.Info("No JSON reports found to include in monthly report.");
                return;
            }

            // Show the user the earliest available report date for reference when entering the start date
            string earliestReportDate = CheckEarliestReportDate(jsonFiles);

            // Prompt the user to enter a start date for the report, with the option to use the earliest available report if they just press Enter
            Console.WriteLine("Enter start date (yyyy-MM-dd) or press Enter for earliest:");
            string? startInput = Console.ReadLine();
            DateTime? startDate = null;
            if (!string.IsNullOrWhiteSpace(startInput))
            {
                // Try to parse the user input for the start date, if it's invalid log that and exit, otherwise assign it to the startDate variable
                if (DateTime.TryParse(startInput, out var s)) startDate = s.Date;
                else
                {
                    Console.WriteLine("Invalid start date format.");
                    return;
                }

                // Convert the earliest report date from the file name to a DateTime for comparison
                DateTime availableDate;

                // If parse is successful and the user provided start date is earlier than the earliest available report date, adjust the start date to the earliest available report date
                if (DateTime.TryParse(earliestReportDate, out availableDate) && startDate < availableDate)
                {
                    Console.WriteLine($"Start date is earlier than the earliest available report date ({availableDate:yyyy-MM-dd}). Adjusting start date to {availableDate:yyyy-MM-dd}.");

                    // This sets the start date to the beginning of the day of the earliest available report date, since we want to include that entire day in the report
                    startDate = availableDate;
                }
            }

            // Show the user the latest available report date for reference when entering the end date
            string latestReportDate = CheckLatestReportDate(jsonFiles);

            // Prompt the user to enter an end date for the report, with the option to use the latest available report if they just press Enter
            Console.WriteLine("Enter end date (yyyy-MM-dd) or press Enter for latest:");
            string? endInput = Console.ReadLine();
            DateTime? endDate = null;
            if (!string.IsNullOrWhiteSpace(endInput))
            {
                // Try to parse the user input for the end date, if it's invalid log that and exit, otherwise assign it to the endDate variable
                if (DateTime.TryParse(endInput, out var e)) endDate = e.Date.AddDays(1).AddTicks(-1);
                else
                {
                    Console.WriteLine("Invalid end date format.");
                    return;
                }

                // Just like before, we need to parse the date
                DateTime availableDate;

                // If parse is successful and the user provided end date is later than the latest available report date, adjust the end date to the latest available report date
                if (DateTime.TryParse(latestReportDate, out availableDate) && endDate > availableDate)
                {
                    Console.WriteLine($"End date is later than the latest available report date ({availableDate:yyyy-MM-dd}). Adjusting end date to {availableDate:yyyy-MM-dd}.");

                    // This sets the end date to the end of the day of the latest available report date, since we want to include that entire day in the report
                    endDate = availableDate.Date.AddDays(1).AddTicks(-1); 
                }
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var reports = new List<ReportModel>();

            // Iterate over each JSON file in the folder
            foreach (var path in jsonFiles)
            {
                // Try to catch file I/O exceptions
                try
                {
                    // Read and deserialize the JSON
                    string json = File.ReadAllText(path);
                    var deserializedReport = JsonSerializer.Deserialize<ReportModel>(json, options);

                    // Make sure it isn't null
                    if (deserializedReport != null)
                    {
                        // If measurement time isn't present, fall back to file write time
                        if (deserializedReport.measurementTime == default)
                        {
                            deserializedReport.measurementTime = File.GetLastWriteTimeUtc(path);
                        }

                        // Apply date filters if provided
                        if (startDate.HasValue && deserializedReport.measurementTime < startDate.Value.ToUniversalTime()) continue;
                        if (endDate.HasValue && deserializedReport.measurementTime > endDate.Value.ToUniversalTime()) continue;

                        reports.Add(deserializedReport);
                    }
                }
                catch (ArgumentException ex) { logger.Error($"Invalid path for JSON report: {ex.Message}"); return; }
                catch (IOException ex) { logger.Error($"IO error reading JSON report: {ex.Message}"); return; }
                catch (JsonException ex)  { logger.Error($"JSON deserialization error: {ex.Message}"); return; }
                catch (UnauthorizedAccessException ex) { logger.Error($"Access denied trying to read JSON report: {ex.Message}"); return; }
                catch (NotSupportedException ex) { logger.Error($"Unsupported path for JSON report: {ex.Message}"); return; }
                catch (Exception ex) { logger.Error($"Failed to read JSON report: {ex.Message}"); return; }
            }

            if (reports.Count == 0)
            {
                logger.Info("No reports matched the specified date range.");
                return;
            }

            // Use the provided start and end dates for the report labels, if they are not provided use "earliest" and "latest" as appropriate
            string startLabel = startDate?.ToString("yyyy-MM-dd") ?? "earliest";
            string endLabel = endDate?.ToString("yyyy-MM-dd") ?? "latest";

            // String builder is always good for appending lines
            var sb = new StringBuilder();
            sb.AppendLine($"Monthly report for {startLabel} -> {endLabel}");
            sb.AppendLine($"Samples: {reports.Count}");
            sb.AppendLine("-- Inside --");
            sb.AppendLine($"Starting Temp F: {reports.First().inside.temperatureF}");
            sb.AppendLine($"Starting Temp C: {reports.First().inside.temperatureC}");
            sb.AppendLine($"Starting Humidity: {reports.First().inside.humidity}%");

            sb.AppendLine($"Ending Temp F: {reports.Last().inside.temperatureF}");
            sb.AppendLine($"Ending Temp C: {reports.Last().inside.temperatureC}");
            sb.AppendLine($"Ending Humidity: {reports.Last().inside.humidity}%");

            double insideDeltaTempF = reports.Last().inside.temperatureF - reports.First().inside.temperatureF;
            double insideDeltaTempC = reports.Last().inside.temperatureC - reports.First().inside.temperatureC;
            double insideDeltaHumidity = reports.Last().inside.humidity - reports.First().inside.humidity;

            sb.AppendLine($"Delta Temp F: {insideDeltaTempF:+0.##;-0.##;0}");
            sb.AppendLine($"Delta Temp C: {insideDeltaTempC:+0.##;-0.##;0}");
            sb.AppendLine($"Delta Humidity: {insideDeltaHumidity:+0.##;-0.##;0}%");

            double avgInsideF = reports.Average(x => x.inside.temperatureF);
            double avgInsideC = reports.Average(x => x.inside.temperatureC);
            double avgInsideHumidity = reports.Average(x => x.inside.humidity);

            sb.AppendLine($"Average Temp F: {avgInsideF:F2}");
            sb.AppendLine($"Average Temp C: {avgInsideC:F2}");
            sb.AppendLine($"Average Humidity: {avgInsideHumidity:F2}%");

            sb.AppendLine("-- Outside --");
            sb.AppendLine($"Starting Temp F: {reports.First().outside.temperature_2m_fahrenheit}");
            sb.AppendLine($"Starting Temp C: {reports.First().outside.temperature_2m}");
            sb.AppendLine($"Starting Humidity: {reports.First().outside.relative_humidity_2m}%");

            sb.AppendLine($"Ending Temp F: {reports.Last().outside.temperature_2m_fahrenheit}");
            sb.AppendLine($"Ending Temp C: {reports.Last().outside.temperature_2m}");
            sb.AppendLine($"Ending Humidity: {reports.Last().outside.relative_humidity_2m}%");

            double outsideDeltaTempF = reports.Last().outside.temperature_2m_fahrenheit - reports.First().outside.temperature_2m_fahrenheit;
            double outsideDeltaTempC = reports.Last().outside.temperature_2m - reports.First().outside.temperature_2m;
            double outsideDeltaHumidity = reports.Last().outside.relative_humidity_2m - reports.First().outside.relative_humidity_2m;

            sb.AppendLine($"Delta Temp F: {outsideDeltaTempF:+0.##;-0.##;0}");
            sb.AppendLine($"Delta Temp C: {outsideDeltaTempC:+0.##;-0.##;0}");
            sb.AppendLine($"Delta Humidity: {outsideDeltaHumidity:+0.##;-0.##;0}%");

            double avgOutsideF = reports.Average(x => x.outside.temperature_2m_fahrenheit);
            double avgOutsideC = reports.Average(x => x.outside.temperature_2m);
            double avgOutsideHumidity = reports.Average(x => x.outside.relative_humidity_2m);

            sb.AppendLine($"Average Temp F: {avgOutsideF:F2}");
            sb.AppendLine($"Average Temp C: {avgOutsideC:F2}");
            sb.AppendLine($"Average Humidity: {avgOutsideHumidity:F2}%");

            // Goes into the monthly report folder with a file name that includes the date range of the report
            string outPath = Path.Combine(monthlyReportsDir, $"monthly_{startLabel}_{endLabel}.txt");

            // Try to catch file I/O exceptions when writing the report, log any errors that occur
            try { File.WriteAllText(outPath, sb.ToString()); }
            catch (ArgumentException ex) { logger.Error($"Invalid path for monthly report: {ex.Message}"); return; }
            catch (IOException ex) { logger.Error($"IO error writing monthly report: {ex.Message}"); return; }
            catch (UnauthorizedAccessException ex) { logger.Error($"Access denied writing monthly report: {ex.Message}"); return; }
            catch (NotSupportedException ex) { logger.Error($"Unsupported path for monthly report: {ex.Message}"); return; }
            catch (Exception ex) { logger.Error($"Failed to write monthly report: {ex.Message}"); return; }

            logger.Info($"Wrote monthly report: {outPath}");
            Console.WriteLine(sb.ToString());

            string monthlyReportFilePath = "monthly/latest_monthly.json";

            var monthlyReportObject = new
            {
                starting_date = startLabel,
                ending_date = endLabel,
                sample_count = reports.Count,
                inside = new
                {
                    starting_temperatureF = reports.First().inside.temperatureF,
                    starting_temperatureC = reports.First().inside.temperatureC,
                    starting_humidity = reports.First().inside.humidity,
                    ending_temperatureF = reports.Last().inside.temperatureF,
                    ending_temperatureC = reports.Last().inside.temperatureC,
                    ending_humidity = reports.Last().inside.humidity,
                    delta_temperatureF = insideDeltaTempF,
                    delta_temperatureC = insideDeltaTempC,
                    delta_humidity = insideDeltaHumidity,
                    average_temperatureF = avgInsideF,
                    average_temperatureC = avgInsideC,
                    average_humidity = avgInsideHumidity
                },
                outside = new
                {
                    starting_temperatureF = reports.First().outside.temperature_2m_fahrenheit,
                    starting_temperatureC = reports.First().outside.temperature_2m,
                    starting_humidity = reports.First().outside.relative_humidity_2m,
                    ending_temperatureF = reports.Last().outside.temperature_2m_fahrenheit,
                    ending_temperatureC = reports.Last().outside.temperature_2m,
                    ending_humidity = reports.Last().outside.relative_humidity_2m,
                    delta_temperatureF = outsideDeltaTempF,
                    delta_temperatureC = outsideDeltaTempC,
                    delta_humidity = outsideDeltaHumidity,
                    average_temperatureF = avgOutsideF,
                    average_temperatureC = avgOutsideC,
                    average_humidity = avgOutsideHumidity
                }
            };

            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };

            // Try to catch file I/O exceptions when writing the JSON report, log any errors that occur
            try { File.WriteAllText(monthlyReportFilePath, JsonSerializer.Serialize(monthlyReportObject, jsonOptions)); }
            catch (ArgumentException ex) { logger.Error($"Invalid path for monthly report: {ex.Message}"); return; }
            catch (IOException ex) { logger.Error($"IO error writing monthly report: {ex.Message}"); return; }
            catch (UnauthorizedAccessException ex) { logger.Error($"Access denied writing monthly report: {ex.Message}"); return; }
            catch (NotSupportedException ex) { logger.Error($"Unsupported path for monthly report: {ex.Message}"); return; }
            catch (Exception ex) { logger.Error($"Failed to write monthly report: {ex.Message}"); return; }

            logger.Info($"Wrote JSON report to {monthlyReportFilePath}");
        }

        /// <summary>
        /// Takes a directory of *.json files and returns the earliest date in the file name.
        /// </summary>
        /// <param name="jsonFiles">Directory of *.json files which holds the reports.</param>
        /// <returns>The earliest date found in the file names.</returns>
        public static string CheckEarliestReportDate(string[] jsonFiles)
        {
            // Show the user the earliest available report date for reference when entering the start date
            string earliestReport = jsonFiles.OrderBy(f => File.GetLastWriteTimeUtc(f)).First();
            Match earliestReportDate = Regex.Match(earliestReport, @"\d{4}-\d{2}-\d{2}");
            return earliestReportDate.Value;
        }

        /// <summary>
        /// Takes a directory of *.json files and returns the latest date in the file name.
        /// </summary>
        /// <param name="jsonFiles">Directory of *.json files which holds the reports.</param>
        /// <returns>The latest date found in the file names.</returns>
        public static string CheckLatestReportDate(string[] jsonFiles)
        {
            string latestReport = jsonFiles.OrderByDescending(f => File.GetLastWriteTimeUtc(f)).First();
            Match latestReportDate = Regex.Match(latestReport, @"\d{4}-\d{2}-\d{2}");
            return latestReportDate.Value;
        }

        /// <summary>
        /// Provides a quick and concise summary of the inside weather for the dashboard.
        /// </summary>
        /// <returns>A string summary of the inside weather for the dashboard.</returns>
        public string InsideWeatherSummary()
        {
            StringBuilder sb = new StringBuilder();

            try { if (Message == null) { throw new ArgumentNullException("Message"); } }
            catch (ArgumentNullException ex)
            {
                Logger logger = new Logger("logs/report.log");
                logger.Error($"Message is null in InsideWeatherSummary: {ex.Message}");
                return "No inside weather data available.";
            }

            // Truncate decimals
            int tempF = (int)Message.temperatureF;

            if (tempF >= 80) // 80+ F range - hot
            {
                sb.Append($"It's {tempF} inside, it's pretty hot!");
                sb.Append(" Consider wearing light and breathable clothes, and stay hydrated.");
            }
            else if (tempF >= 65) // 65-80 F range - warm/comfortable
            {
                sb.Append($"It's {tempF} inside, it's a comfortable temperature.");
                sb.Append(" Enjoy your day!");
            }
            else if (tempF >= 50) // 50-65 F range - cool
            {
                sb.Append($"It's {tempF} inside, it's a bit cool.");
                sb.Append(" Consider wearing long sleeves with jeans along with a sweater or hoodie.");
            }
            else if (tempF >= 35) // 35-50 F range - chilly
            {
                sb.Append($"It's {tempF} inside, it's quite chilly.");
                sb.Append(" Consider wearing a sweater and long pants with a jacket or a coat.");
            }
            else if (tempF >= 20) // 20-25 F range - cold
            {
                sb.Append($"It's {tempF} inside, it's very cold.");
                sb.Append(" Stay warm with heavy layers! A coat, hat, and gloves are recommended.");
            }
            else // Below 20 F - freezing
            {
                sb.Append($"It's {tempF} inside, it's freezing!");
                sb.Append(" Make sure to bundle up with heavy layers, a coat, hat, and gloves.");
            }

            if (Message.humidity >= 80) // 80%+ humidity - very humid
            {
                sb.Append(" It's also very humid! Stay cool, hydrate, and avoid dark or heavy clothes.");
            }
            else if (Message.humidity >= 60) // 60-80% humidity - humid
            {
                sb.Append(" It's also quite humid. Consider wearing breathable fabrics and stay hydrated.");
            }
            else if (Message.humidity >= 30) // 30-60% humidity - comfortable
            {
                sb.Append(" The humidity level is comfortable.");
            }
            else // Below 30% humidity - dry
            {
                sb.Append(" Also the air is quite dry. Consider using a moisturizer and staying hydrated.");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Represents a quick and concise summary of the outside weather for the dashboard.
        /// </summary>
        /// <returns>A string containing the outside weather summary.</returns>
        public string OutsideWeatherSummary()
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                if (ApiWeather == null) { throw new ArgumentNullException("ApiWeather"); }
                if (ApiWeather.current == null) { throw new ArgumentNullException("ApiWeather.current"); }
                if (ApiWeather.daily == null) { throw new ArgumentNullException("ApiWeather.daily"); }
                if (ApiWeather.daily.temperature_2m_max == null) { throw new ArgumentNullException("ApiWeather.daily.temperature_2m_max"); }
                if (ApiWeather.daily.temperature_2m_min == null) { throw new ArgumentNullException("ApiWeather.daily.temperature_2m_min"); }
                if (ApiWeather.daily.precipitation_probability_max == null) { throw new ArgumentNullException("ApiWeather.daily.precipitation_probability_max"); }
            }
            catch (ArgumentNullException ex)
            {
                Logger logger = new Logger("logs/report.log");
                logger.Error($"ApiWeather is null in OutsideWeatherSummary: {ex.Message}");
                return "No outside weather data available.";
            }


            
            sb.Append($"{ApiWeather.current.weather_name} with a high of {ApiWeather.daily.temperature_2m_max_fahrenheit:F0} and a low of {ApiWeather.daily.temperature_2m_min_fahrenheit:F0} today. ");
            sb.Append($"It's currently {ApiWeather.current.temperature_2m_fahrenheit:F0} outside with {ApiWeather.current.relative_humidity_2m:F0}% humidity. ");
            sb.Append($"There is a {ApiWeather.daily.precipitation_probability_max[0]:0.##}% chance of precipitation.");

            return sb.ToString();
        }

        /// <summary>
        /// Used to deseralize the JSON report files for comparison.
        /// </summary>
        private class ReportModel
        {
            // I was going to use the Report class for this,
            // it didn't make sense to use a WeatherResponse since
            // this isn't technically a UDP message or an API response,
            // it's a report that contains information from both.

            public DateTime measurementTime { get; set; }
            public InsideModel inside { get; set; } = new InsideModel();
            public OutsideModel outside { get; set; } = new OutsideModel();
        }

        /// <summary>
        /// Represents the weather inside received from the UDP message
        /// </summary>
        private class InsideModel
        {
            public double temperatureF { get; set; }
            public double temperatureC { get; set; }
            public double humidity { get; set; }
        }

        /// <summary>
        /// Represents the weather outside received from the API response
        /// </summary>
        private class OutsideModel
        {
            public double temperature_2m { get; set; }
            public double temperature_2m_fahrenheit { get; set; }
            public double relative_humidity_2m { get; set; }
            public string? time { get; set; }
        }
    }
}
