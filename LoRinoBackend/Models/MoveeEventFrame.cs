// Importing necessary namespaces for .NET functionalities and data annotations
using System; // Provides fundamental classes and types
using System.Collections.Generic; // Provides generic collection classes such as List
using System.ComponentModel; // Provides attributes for controlling the display of properties
using System.ComponentModel.DataAnnotations; // Provides attributes for data validation
using System.ComponentModel.DataAnnotations.Schema; // Provides attributes for mapping entities to the database schema
using System.Linq; // Provides LINQ functionality for querying collections
using System.Threading.Tasks; // Provides types for asynchronous programming

namespace LoRinoBackend.Models
{
    // Class representing a frame of a Movee event, including its details and associated data
    public class MoveeEventFrame
    {
        // Constructor to initialize default values for the properties
        public MoveeEventFrame()
        {
            Id = 0; // Default value for event ID
            EventCreationTime = 0; // Default value for event creation time (Unix timestamp)
            EventAckBy = ""; // Default value for the person who acknowledged the event
            EventClearBy = ""; // Default value for the person who cleared the event
            Guid = ""; // Default value for a unique identifier
            EventAckTime = 0; // Default value for the time the event was acknowledged
            EventClearTime = 0; // Default value for the time the event was cleared
            AlarmCount = 0; // Default value for the count of active alarms
            LocationId = 0; // Default value for location ID
            AckMessage = ""; // Default value for acknowledgment message
            ClearMessage = ""; // Default value for clearing message
        }

        // Unique identifier for the event frame
        [DisplayName("ID događaja")] // Display name used in UI for this property
        public int Id { get; set; }

        // Timestamp indicating when the event was created, in Unix time format
        [DisplayName("Vrijeme događaja")] // Display name used in UI for this property
        public long EventCreationTime { get; set; }

        // The person or entity who acknowledged the event
        [DisplayName("Potvrda")] // Display name used in UI for this property
        public string EventAckBy { get; set; }

        // The person or entity who cleared the event
        [DisplayName("Zatvaranje")] // Display name used in UI for this property
        public string EventClearBy { get; set; }

        // Unique identifier for the event frame
        public string Guid { get; set; }

        // Timestamp indicating when the event was acknowledged, in Unix time format
        [DisplayName("Vrijeme potvrde")] // Display name used in UI for this property
        public long EventAckTime { get; set; }

        // Timestamp indicating when the event was cleared, in Unix time format
        [DisplayName("Vrijeme zatvaranja")] // Display name used in UI for this property
        public long EventClearTime { get; set; }

        // Number of active alarms associated with the event
        [DisplayName("Broj aktivnih senzora")] // Display name used in UI for this property
        public int AlarmCount { get; set; }

        // Collection of data frames associated with the event
        public ICollection<MoveeDataFrame> MoveeDataFrames { get; set; }

        // Foreign key referencing the location associated with the event
        [DisplayName("Lokacija")] // Display name used in UI for this property
        public int LocationId { get; set; }

        // Navigation property for accessing the related Location entity
        // The NotMapped attribute indicates that this property is not mapped to a database column
        [NotMapped]
        [DisplayName("Lokacija")] // Display name used in UI for this property
        public Location Location { get; set; }

        // Message describing the acknowledgment of the event
        //[Required(ErrorMessage = "Opis događaja je potreban!")] // Uncomment to make the property required with a custom error message
        //[StringLength(1000, MinimumLength = 2)] // Uncomment to specify maximum and minimum length constraints
        [DisplayName("Opis potvrde: ")] // Display name used in UI for this property
        public string AckMessage { get; set; }

        // Boolean indicating whether the event has been acknowledged
        public bool IsAcked { get; set; }

        // Message describing the clearing of the event
        //[Required(ErrorMessage = "Opis zatvaranja je potreban!")] // Uncomment to make the property required with a custom error message
        //[StringLength(1000, MinimumLength = 2)] // Uncomment to specify maximum and minimum length constraints
        [DisplayName("Opis zatvaranja: ")] // Display name used in UI for this property
        public string ClearMessage { get; set; }

        // Boolean indicating whether the event has been cleared
        public bool IsCleared { get; set; }

        // Boolean indicating whether the timer associated with the event has ended
        public bool TimerIsEnded { get; set; }

        // Collection of tags associated with the event
        // Foreign key attribute specifies the relationship with MoveeEventFrameId
        [ForeignKey("MoveeEventFrameId")]
        public List<MoveeEventTag> MoveeEventTag { get; set; }

        // Collection of comments associated with the event
        // Foreign key attribute specifies the relationship with MoveeEventFrameId
        [ForeignKey("MoveeEventFrameId")]
        public List<MoveeEventComment> MoveeEventComment { get; set; }
    }
}
