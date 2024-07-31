using Microsoft.Extensions.DependencyInjection; // Importing dependency injection services
using Microsoft.Extensions.Logging; // Importing logging interfaces
using System; // Importing base class library, including fundamental classes and base classes that define commonly-used value and reference data types

namespace LoRinoBackend.Logging
{
    public static class LoRinoBackendFileLoggerExtensions // Static class for extending logging capabilities
    {
        // Extension method to add the custom file logger to the logging builder
        public static ILoggingBuilder AddLoRinoBackendFileLogger(this ILoggingBuilder builder, Action<LoRinoBackendFileLoggerOptions> configure)
        {
            // Register the LoRinoBackendFileLoggerProvider as a singleton service
            builder.Services.AddSingleton<ILoggerProvider, LoRinoBackendFileLoggerProvider>();

            // Configure the LoRinoBackendFileLoggerOptions with the provided configuration action
            builder.Services.Configure(configure);

            // Return the modified logging builder to support method chaining
            return builder;
        }
    }
}
