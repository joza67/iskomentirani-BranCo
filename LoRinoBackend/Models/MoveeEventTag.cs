// Importing necessary namespaces for .NET functionalities and data annotations
using System.ComponentModel; // Provides attributes for controlling the display of properties
using System.ComponentModel.DataAnnotations.Schema; // Provides attributes for mapping entities to the database schema

namespace LoRinoBackend.Models
{
    // Class representing a tag associated with a Movee event
    public class MoveeEventTag
    {
        // Unique identifier for the Movee event tag
        public int Id { get; set; }

        // Foreign key referencing the associated Movee event frame
        public int MoveeEventFrameId { get; set; }

        // Foreign key referencing the associated Movee tag
        public int MoveeTagId { get; set; }

        // Boolean indicating whether the tag is active
        public bool Active { get; set; }

        // NotMapped attribute indicates that this property is not mapped to a database column
        // TagName is used for display purposes and is not stored in the database
        [NotMapped]
        public string TagName { get; set; }
    }
}
