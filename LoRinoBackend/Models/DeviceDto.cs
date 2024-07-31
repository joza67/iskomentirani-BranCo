// Import necessary namespaces
using System; // Provides basic functionalities

namespace LoRinoBackend.Controllers
{
    // Data Transfer Object (DTO) class for device information
    public class DeviceDto
    {
        // Property representing the unique identifier for the device
        public int Id { get; set; }

        // Property representing the device's unique identifier (EUI)
        public string DevEui { get; set; }

        // Property representing the name of the device
        public string Name { get; set; }

        // Property representing the name of the company associated with the device
        public string CompanyName { get; set; }

        // Property representing the location name of the device
        public string Location { get; set; }

        // Property representing the latitude coordinate of the device's location
        public double Lat { get; set; }

        // Property representing the longitude coordinate of the device's location
        public double Long { get; set; }

        // Property representing the date and time associated with the device, nullable
        public DateTime? Datum { get; set; }

        // Property representing the Received Signal Strength Indicator, nullable
        public int? RSSI { get; set; }

        // Property representing the Signal-to-Noise Ratio, nullable
        public double? SNR { get; set; }

        // Property representing the temperature value, nullable
        public double? Temp { get; set; }

        // Property representing the battery level, nullable
        public double? Bat { get; set; }

        // Property representing a count value, possibly for events or signals, nullable
        public int? Count { get; set; }
    }
}
