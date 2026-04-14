namespace Environmental_Monitor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            app.UseCors("AllowFrontend");

            // Default get if no coordinates are provided.
            app.MapGet("/api/weather", async (double? latitude, double? longitude) =>
            {
                // Default coordinates if none are provided
                double defaultLatitude = 36.5951;
                double defaultLongitude = -82.1887;

                if (latitude != null && longitude != null)
                {
                    return Results.Ok(await Monitor(latitude.Value, longitude.Value));
                }
                else
                {
                    return Results.Ok(await Monitor(defaultLatitude, defaultLongitude));
                } 
            });

            app.Run();
        }

        /// <summary>
        /// The main mode of the program.
        /// Continuously gets live weather data from the pi and API, generates reports, and compares them to the last report.
        /// Allows the user to specify the interval between reports (default 1 minute).
        /// </summary>
        static async Task<Report> Monitor(double latitude, double longitude)
        {
            // Try to get live weather from the pi
            UdpReceiverAsync receiver = new UdpReceiverAsync();
            UdpMessage? udpMessage = await receiver.ReceiveAsync();

            // Try to get live weather from the API
            APIHandler handler = new APIHandler();
            WeatherResponse? apiWeather = await handler.CallAPI(latitude, longitude);

            // Null is bad
            if (udpMessage == null) { throw new ArgumentNullException(nameof(udpMessage)); }
            if (apiWeather == null) { throw new ArgumentNullException(nameof(apiWeather)); }

            // Pass objects to the report class to generate
            Report report = new Report(udpMessage, apiWeather);

            report.GenerateReport();
            report.CompareToLastReport();

            return report;
        }
    }
}