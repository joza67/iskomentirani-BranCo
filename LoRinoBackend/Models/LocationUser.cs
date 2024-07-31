// Import namespace for attributes used in entity framework mappings
using System.ComponentModel.DataAnnotations.Schema;

namespace LoRinoBackend.Models
{
    // Class representing the association between a user and a location
    public class LocationUser
    {
        // Unique identifier for the LocationUser entity
        public int Id { get; set; }

        // Foreign key for the associated location
        public int LocationId { get; set; }

        // Foreign key for the associated user
        public string UserId { get; set; }

        // Not mapped property for displaying the location's name
        // This property is not mapped to the database
        [NotMapped]
        public string LocationName { get; set; }

        // Not mapped property for displaying the user's name
        // This property is not mapped to the database
        [NotMapped]
        public string UserName { get; set; }
    }
}
