using Microsoft.Extensions.Logging; // Importing logging interfaces from Microsoft.Extensions.Logging
using System; // Importing base class library, including fundamental classes and base classes that define commonly-used value and reference data types
using System.Diagnostics.CodeAnalysis; // Importing tools for code analysis and suppressions
using System.IO; // Importing input/output functionalities, including file and stream handling

namespace LoRinoBackend.Logging
{
    public class LoRinoBackendFileLogger : ILogger // Implementing the ILogger interface to create a custom file logger
    {
        protected readonly LoRinoBackendFileLoggerProvider _loRinoBackendFileLoggerProvider; // Reference to the file logger provider

        // Constructor for initializing the logger with a provider
        public LoRinoBackendFileLogger([NotNull] LoRinoBackendFileLoggerProvider loRinoBackendFileloggerProvider)
        {
            _loRinoBackendFileLoggerProvider = loRinoBackendFileloggerProvider; // Assigning the provided logger provider
        }

        // Method for beginning a logging scope, not used in this implementation
        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            return null; // Returning null as scope management is not implemented
        }

        // Method to check if the specified log level is enabled
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None; // Logger is enabled if the log level is not set to None
        }

        // Method to log a message with the specified parameters
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) // If logging is not enabled for the specified log level, return early
            {
                return;
            }

            // Construct the full file path for the log file
            var fullFilePath = string.Format("{0}/{1}", _loRinoBackendFileLoggerProvider.Options.FolderPath,
                _loRinoBackendFileLoggerProvider.Options.FilePath.Replace("{date}", DateTime.UtcNow.ToString("ddMMyyyy")));

            // Format the log record including the date, log level, message, and exception details if present
            var logRecord = string.Format("{0} [{1}] {2} {3}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                logLevel.ToString(), formatter(state, exception), (exception != null ? exception.StackTrace : ""));

            // Write the log record to the specified file
            using (var streamWriter = new StreamWriter(fullFilePath, true))
            {
                streamWriter.WriteLine(logRecord); // Append the log record to the file
            }
        }
    }
}
