// Import necessary namespaces
using System; // Provides basic functionalities
using System.Collections.Generic; // Provides collection types like List
using System.ComponentModel.DataAnnotations; // Provides attributes for data validation
using System.Linq; // Provides LINQ functionalities for data querying
using System.Threading.Tasks; // Provides asynchronous programming functionalities

namespace LoRinoBackend.Models
{
    // Class representing a device entity
    public class Device
    {
        // Property representing the unique identifier for the device
        public int Id { get; set; }

        // Property representing the name of the device
        [Required(ErrorMessage = "Name is required")] // Data annotation to enforce that this field is required
        public string Name { get; set; }

        // Property representing the device EUI (unique identifier)
        [Required(ErrorMessage = "DevEui is required")] // Data annotation to enforce that this field is required
        public string DevEui { get; set; }

        // Property representing the longitude coordinate of the device's location
        public double Long { get; set; }

        // Property representing the latitude coordinate of the device's location
        public double Lat { get; set; }

        // Property representing the minimum zoom level for mapping
        public double MinZoom { get; set; }

        // Property representing the maximum zoom level for mapping
        public double MaxZoom { get; set; }

        // Property representing the expiration date of the device
        [DataType(DataType.Date)] // Specifies that the property should be treated as a date
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)] // Specifies the display format and that it should be applied in edit mode
        public DateTime Expires { get; set; }

        // Navigation property representing the type of the device
        public DeviceType DeviceType { get; set; }

        // Navigation property representing the company associated with the device
        public Company Company { get; set; }

        // Property representing the foreign key to the location
        public int LocationId { get; set; }

        // Navigation property representing the location of the device
        public Location Location { get; set; }

        // Default constructor initializing default values
        public Device()
        {
            Id = 0; // Default value for Id
            Name = null; // Default value for Name
            DevEui = null; // Default value for DevEui
            Long = 0; // Default value for Long
            Lat = 0; // Default value for Lat
            Expires = new DateTime(); // Default value for Expires
            DeviceType = new DeviceType(); // Default initialization for DeviceType
            Company = new Company(); // Default initialization for Company
            Location = new Location(); // Default initialization for Location
            MinZoom = 0; // Default value for MinZoom
            MaxZoom = 0; // Default value for MaxZoom
        }
    }
}
