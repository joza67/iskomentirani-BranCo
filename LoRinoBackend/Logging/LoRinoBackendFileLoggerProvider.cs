using Microsoft.Extensions.Logging; // Importing logging interfaces
using Microsoft.Extensions.Options; // Importing options pattern support for configuration
using System.IO; // Importing input/output functionalities, including file and stream handling

namespace LoRinoBackend.Logging
{
    [ProviderAlias("LoRinoBackendFile")] // Alias attribute for the logger provider, used for configuration
    public class LoRinoBackendFileLoggerProvider : ILoggerProvider // Implementing the ILoggerProvider interface to create a custom logger provider
    {
        public readonly LoRinoBackendFileLoggerOptions Options; // Instance of LoRinoBackendFileLoggerOptions to store configuration options

        // Constructor that takes IOptions<LoRinoBackendFileLoggerOptions> for dependency injection
        public LoRinoBackendFileLoggerProvider(IOptions<LoRinoBackendFileLoggerOptions> _options)
        {
            Options = _options.Value; // Assigning the provided options value to the Options property

            // Check if the directory specified in Options.FolderPath exists, create it if it doesn't
            if (!Directory.Exists(Options.FolderPath))
            {
                Directory.CreateDirectory(Options.FolderPath); // Creating the directory for storing log files
            }
        }

        // Method to create a logger instance for a specific category
        public ILogger CreateLogger(string categoryName)
        {
            return new LoRinoBackendFileLogger(this); // Return a new instance of LoRinoBackendFileLogger, passing this provider
        }

        // Dispose method for releasing unmanaged resources, not implemented in this case
        public void Dispose()
        {
            // No resources to dispose in this implementation
        }
    }
}
