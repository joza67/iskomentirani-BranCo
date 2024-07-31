using System;

namespace LoRinoBackend.Models
{
    // ViewModel representing a log entry with various details
    public class LogViewModel
    {
        // Timestamp indicating when the log entry was created
        public long LogTime { get; set; }

        // Identifier for the user or system that created the log entry
        public string LogBy { get; set; }

        // Enum specifying the type of message logged
        public MessageType MessageType { get; set; }

        // Description of the action taken, associated with the log entry
        public string Action { get; set; }

        // Detailed message or content of the log entry
        public string Message { get; set; }
    }

    // Enum representing different types of messages that can be logged
    public enum MessageType
    {
        // Comment or note added to the log
        Komentar,

        // Confirmation action in the log
        Potvrda,

        // Closing or resolving action in the log
        Zatvaranje,

        // Tagging action in the log
        Tag,

        // Event action in the log
        Događaj
    }
}
