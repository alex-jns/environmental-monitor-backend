namespace Environmental_Monitor
{
    /// <summary>
    /// Constants that represent the level of the log
    /// </summary>
    public enum LogLevel { Info, Warning, Error, Debug }

    /// <summary>
    /// Responsible for logging all events to a log file
    /// </summary>
    internal class Logger
    {
        private readonly string _logFilePath;

        /// <summary>
        /// Instantiates the logger object using the file path.
        /// </summary>
        /// <param name="logFilePath">Path to the log file.</param>
        /// <exception cref="NullReferenceException">Thrown if directory is null.</exception>
        public Logger(string logFilePath)
        {
            _logFilePath = logFilePath;

            // Watch out for I/O exceptions when creating directories and files
            try
            {
                string? directory = Path.GetDirectoryName(logFilePath);
                if (directory == null) { throw new NullReferenceException (nameof(directory)); }
                if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }
            }
            // Also catches argument null exception
            catch (ArgumentException ex) { Console.WriteLine($"Argument exception: {ex.Message}"); }
            catch (PathTooLongException ex) { Console.WriteLine($"Path too long exception: {ex.Message}"); }

            // Also catches directory not found exception
            catch (IOException ex) { Console.WriteLine($"IO exception: {ex.Message}"); }
            catch (UnauthorizedAccessException ex) { Console.WriteLine($"Unauthorized access exception: {ex.Message}"); }
            catch (NotSupportedException ex) { Console.WriteLine($"Argument exception: {ex.Message}"); }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
        }

        /// <summary>
        /// The main log method that formats the log entry and writes it to the console and log file.
        /// </summary>
        /// <param name="message">Message to be displayed on the console and logged.</param>
        /// <param name="level">Enum representing the severity of the log.</param>
        public void Log(string message, LogLevel level = LogLevel.Info)
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";

            Console.WriteLine(logEntry);

            File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
        }

        /// <summary>
        /// Information for the user and developer.
        /// </summary>
        /// <param name="message">Event to be descibred for logging purposes</param>
        public void Info(string message) => Log(message, LogLevel.Info);

        /// <summary>
        /// Recoverable error.
        /// </summary>
        /// <param name="message">Event to be descibred for logging purposes</param>
        public void Warning(string message) => Log(message, LogLevel.Info);

        /// <summary>
        /// Non-recoverable error.
        /// </summary>
        /// <param name="message">Event to be descibred for logging purposes</param>
        public void Error(string message) => Log(message, LogLevel.Info);

        /// <summary>
        /// Information for the developer only.
        /// </summary>
        /// <param name="message">Event to be descibred for logging purposes</param>
        public void Debug(string message) => Log(message, LogLevel.Info);
    }
}
