// Importing necessary namespaces for .NET functionalities and data annotations
using System.ComponentModel; // Provides attributes for controlling the display of properties
using System.ComponentModel.DataAnnotations; // Provides attributes for data validation
using System.ComponentModel.DataAnnotations.Schema; // Provides attributes for mapping entities to the database schema

namespace LoRinoBackend.Models
{
    // Class representing a comment associated with a Movee event
    public class MoveeEventComment
    {
        // Unique identifier for each comment
        public int Id { get; set; }

        // Comment text with display name and validation attributes
        [DisplayName("Komentar")] // Display name used in UI for this property
        [Required(ErrorMessage = "Tekst komentara je potreban!")] // Marks the property as required with a custom error message
        [StringLength(1000, MinimumLength = 2)] // Specifies the maximum and minimum length of the string
        public string Comment { get; set; }

        // Timestamp for when the comment was made, in Unix time format
        [DisplayName("Vrijeme")] // Display name used in UI for this property
        public long EventCommentTime { get; set; }

        // Name or identifier of the person who made the comment
        [DisplayName("Komentirao/la")] // Display name used in UI for this property
        public string EventCommentBy { get; set; }

        // Foreign key reference to the related MoveeEventFrame entity
        public int MoveeEventFrameId { get; set; }

        // Indicates whether the comment is active or not
        public bool Active { get; set; }

        // Navigation property for accessing the related MoveeEventFrame entity
        // The NotMapped attribute indicates that this property is not mapped to a database column
        [NotMapped]
        public MoveeEventFrame MoveeEventFrame { get; set; }
    }
}
