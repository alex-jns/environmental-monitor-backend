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

            app.MapGet("/api/weather", async () =>
            {
                return Results.Ok(await Monitor());
            });

            app.Run();
        }

        /// <summary>
        /// The main mode of the program.
        /// Continuously gets live weather data from the pi and API, generates reports, and compares them to the last report.
        /// Allows the user to specify the interval between reports (default 1 minute).
        /// </summary>
        static async Task<Report> Monitor()
        {
            // Try to get live weather from the pi
            UdpReceiverAsync receiver = new UdpReceiverAsync();
            UdpMessage? udpMessage = await receiver.ReceiveAsync();

            // Try to get live weather from the API
            APIHandler handler = new APIHandler();
            WeatherResponse? apiWeather = await handler.CallAPI();

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