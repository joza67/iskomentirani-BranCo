// Import necessary namespaces
using System; // Provides basic functionalities
using System.Collections.Generic; // Provides collection types like List
using System.ComponentModel.DataAnnotations; // Provides attributes for data validation
using System.Linq; // Provides LINQ functionalities for data querying
using System.Threading.Tasks; // Provides asynchronous programming functionalities

namespace LoRinoBackend.Models
{
    // Class representing a type of device
    public class DeviceType
    {
        // Property representing the unique identifier for the device type
        public int Id { get; set; }

        // Property representing the name of the device type
        [Required] // Data annotation to enforce that this field is required
        public string Name { get; set; }

        // Property representing the file path to a photo representing the device type
        public string PhotoPath { get; set; }

        // Navigation property representing the list of devices associated with this device type
        // public List<Device> Devices { get; set; } // Uncomment if needed to represent a collection of devices

        // Default constructor initializing default values
        public DeviceType()
        {
            Id = 0; // Default value for Id
            Name = null; // Default value for Name
            PhotoPath = null; // Default value for PhotoPath
        }
    }
}
