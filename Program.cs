namespace Environmental_Monitor
{
    /// <summary>
    /// Connects to the pi and api to generate reports
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Program entry point
        /// </summary>
        static async Task Main()
        {
            Console.WriteLine("1. Monitor");
            Console.WriteLine("2. Monthly Report");
            Console.Write("Enter a mode: ");

            string? mode = Console.ReadLine()?.Trim().ToLower();

            switch (mode)
            {
                case "1":
                    await Monitor();
                    break;
                case "2":
                    Report.GenerateMontlyReport();
                    break;
                default:
                    Console.WriteLine("Invalid mode. Please enter 'monitor'.");
                    break;
            }
        }

        /// <summary>
        /// The main mode of the program.
        /// Continuously gets live weather data from the pi and API, generates reports, and compares them to the last report.
        /// Allows the user to specify the interval between reports (default 1 minute).
        /// </summary>
        static async Task Monitor()
        {
            // Allow the user to enter the interval between reports (in minutes)
            Console.Write("Enter time between reports in minutes (default 1): ");
            string? input = Console.ReadLine();
            int intervalMinutes = 1;

            // Check if user entered anything
            if (!string.IsNullOrWhiteSpace(input))
            {
                // Check if the user entered a valid positive integer
                if (!int.TryParse(input, out intervalMinutes) || intervalMinutes <= 0)
                {
                    Console.WriteLine("Invalid input. Using default 1 minute.");
                    intervalMinutes = 1;
                }
            }

            // Main monitoring loop; input Ctrl+C to stop
            while (true)
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

                // User specified delay between reports, default 1 minute
                await Task.Delay(TimeSpan.FromMinutes(intervalMinutes));
            }
        }
    }
}
