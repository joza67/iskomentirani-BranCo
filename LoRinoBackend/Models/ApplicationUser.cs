// Import necessary namespaces
using Microsoft.AspNetCore.Identity; // Provides ASP.NET Core Identity functionalities
using System; // Provides basic functionalities
using System.Collections.Generic; // Provides collection types like List
using System.ComponentModel.DataAnnotations.Schema; // Provides attributes for data annotations, including ForeignKey
using System.Linq; // Provides LINQ functionalities for data querying
using System.Threading.Tasks; // Provides asynchronous programming functionalities

namespace LoRinoBackend.Models
{
    // ApplicationUser class inheriting from IdentityUser for ASP.NET Core Identity
    public class ApplicationUser : IdentityUser
    {
        // Navigation property representing the company associated with the user
        public Company Company { get; set; }

        // Additional properties for the user
        public string Streeet { get; set; } // Property for the street address
        public string City { get; set; } // Property for the city
        public string Country { get; set; } // Property for the country
        public string FirstName { get; set; } // Property for the first name
        public string LastName { get; set; } // Property for the last name

        // Foreign key attribute to define the relationship with LocationUser
        [ForeignKey("UserId")]
        // List of LocationUser entities associated with this user
        public List<LocationUser> LocationUserList { get; set; }
    }
}
