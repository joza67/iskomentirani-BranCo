// Import necessary namespaces
using System; // Provides basic functionalities
using System.Collections.Generic; // Provides collection types like List
using System.ComponentModel.DataAnnotations; // Provides attributes for data validation
using System.ComponentModel.DataAnnotations.Schema; // Provides attributes for database schema management
using System.Linq; // Provides LINQ functionalities for data querying
using System.Threading.Tasks; // Provides asynchronous programming functionalities

namespace LoRinoBackend.Models
{
    // Class representing a company entity
    public class Company
    {
        // Property representing the unique identifier for the company
        [Display(Name = "ID tvrtke")] // Display name for the property
        public int Id { get; set; }

        // Property representing the name of the company
        [Required] // Data annotation to indicate that this field is required
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")] // Limit the maximum length of the name to 50 characters
        [Display(Name = "Naziv Tvrtke")] // Display name for the property
        public string Name { get; set; }

        // Property representing the company's email address
        [Display(Name = "Email")] // Display name for the property
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid email format")] // Regular expression for validating email format
        [Required] // Data annotation to indicate that this field is required
        public string Email { get; set; }

        // Property representing the street address of the company
        [Required] // Data annotation to indicate that this field is required
        [Display(Name = "Ulica")] // Display name for the property
        public string Street { get; set; }

        // Property representing the city where the company is located
        [Required] // Data annotation to indicate that this field is required
        [Display(Name = "Grad")] // Display name for the property
        public string City { get; set; }

        // Property representing the country where the company is located
        [Required] // Data annotation to indicate that this field is required
        [Display(Name = "Država")] // Display name for the property
        public string Country { get; set; }

        // Property representing the file path of the company's logo
        [Display(Name = "Logo")] // Display name for the property
        public string PhotoPath { get; set; }

        // Navigation property representing the list of locations associated with the company
        public List<Location> Location { get; set; }

        // Default constructor initializing default values
        public Company()
        {
            Id = 0; // Default value for Id
            Name = null; // Default value for Name
            Email = null; // Default value for Email
            Street = null; // Default value for Street
            City = null; // Default value for City
            Country = null; // Default value for Country
            PhotoPath = null; // Default value for PhotoPath
        }
    }
}
