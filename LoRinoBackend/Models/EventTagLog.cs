// Import necessary namespaces
using System.ComponentModel; // Provides attributes for customizing the appearance of properties
using System.ComponentModel.DataAnnotations; // Provides attributes for data validation

namespace LoRinoBackend.Models
{
    // Class representing a log entry for event tags
    public class EventTagLog
    {
        // Property representing the unique identifier for the event tag log entry
        public int Id { get; set; }

        // Property representing the ID of the associated Movee event tag
        [Required] // Data annotation to enforce that this field is required
        public int MoveeEventTagId { get; set; }

        // Property representing the action taken, with a display name "Akcija"
        [DisplayName("Akcija")] // Custom display name for UI
        public string Action { get; set; }

        // Property representing who tagged the event, with a display name "Tagirao/la"
        [DisplayName("Tagirao/la")] // Custom display name for UI
        public string EventTagBy { get; set; }

        // Property representing the time the tag was created, with a display name "Vrijeme"
        [DisplayName("Vrijeme")] // Custom display name for UI
        public long EventTagTime { get; set; }

        // Property representing the ID of the associated event
        public int EventId { get; set; }
    }
}
