// Import necessary namespaces
using System; // Provides basic functionalities
using System.Collections.Generic; // Provides collection types like List
using System.Linq; // Provides LINQ functionalities for data querying
using System.Threading.Tasks; // Provides asynchronous programming functionalities

namespace LoRinoBackend.Models
{
    // Class representing decoded data from a LoRa device
    public class DecodedData
    {
        // Property representing the unique identifier for the decoded data entry
        public int Id { get; set; }

        // Navigation property representing the associated LoRaData entity
        public LoRaData LoRaData { get; set; }

        // Property representing the energy consumption in kWh
        public long Energy { get; set; }    // kWh - Int

        // Property representing the power consumption in kW
        public long Power { get; set; }     // kW - Int

        // Property representing the volume in cubic meters
        public double Volume { get; set; }  // m3 - Float n,3

        // Property representing the flow rate in cubic meters per hour
        public double FlowRate { get; set; }  // m3/h - Float n,3

        // Property representing the forward temperature
        public long FwdTemp { get; set; }   // Int

        // Property representing the return temperature
        public long RetTemp { get; set; }   // Int
    }
}
