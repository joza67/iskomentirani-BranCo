// Import necessary namespaces
using System; // Provides fundamental classes and types
using System.Collections.Generic; // Provides classes for working with collections
using System.ComponentModel; // Provides classes for components and controls
using System.ComponentModel.DataAnnotations; // Provides attributes for validating data
using System.ComponentModel.DataAnnotations.Schema; // Provides attributes for mapping classes to database tables
using System.Linq; // Provides LINQ (Language Integrated Query) functionality
using System.Threading.Tasks; // Provides types for asynchronous programming

namespace LoRinoBackend.Models
{
    // Class representing a location in the system
    public class Location
    {
        // Unique identifier for the location
        public int Id { get; set; }

        // Name of the location with validation
        [Required, MaxLength(50, ErrorMessage = "Naziv lokacije ne smije biti duže od 50 slova")]
        [Display(Name = "Ime")]
        public string Name { get; set; }

        // Road name with validation
        [Display(Name = "Prometnica")]
        [Required]
        public string Road { get; set; }

        // Road section with validation
        [Display(Name = "Dionica")]
        [Required]
        public string RoadSection { get; set; }

        // Longitude of the location with formatting
        [Display(Name = "Geo. dužina")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:N06}", ApplyFormatInEditMode = true)]
        public double Long { get; set; }

        // Latitude of the location with formatting
        [Display(Name = "Geo. širina")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:N06}", ApplyFormatInEditMode = true)]
        public double Lat { get; set; }

        // Zoom level for the map view
        public double MapZoom { get; set; }

        // Minimum zoom level
        public double MinZoom { get; set; }

        // Maximum zoom level
        public double MaxZoom { get; set; }

        // Duration for event collection with validation
        [Display(Name = "Vrijeme sakupljanja")]
        [Range(1, 60, ErrorMessage = "Trajanje vremena sakupljanja događaja je maksimalno 60 sekundi")]
        [Required]
        public int TimerLenght { get; set; }

        // List of users associated with the location
        public List<LocationUser> LocationUserList { get; set; }

        // Foreign key for the associated company
        [Required]
        public int CompanyId { get; set; }

        // Reference to the associated company
        public Company Company { get; set; }

        // Default constructor initializing properties to default values
        public Location()
        {
            Id = 0;
            Name = null;
            Road = null;
            RoadSection = null;
            Long = 0;
            Lat = 0;
            MapZoom = 0;
            MinZoom = 0;
            MaxZoom = 0;
        }
    }
}
