// Importing necessary namespaces for .NET functionalities
using System; // Core .NET types and functions
using System.Collections.Generic; // Collections like List and Dictionary
using System.ComponentModel.DataAnnotations.Schema; // Data annotations for configuring entity properties (not used here but typically used for database schema configuration)
using System.Linq; // LINQ queries and operations
using System.Threading.Tasks; // Asynchronous programming support

namespace LoRinoBackend.Models
{
    // Class representing a data frame from Movee system
    public class MoveeDataFrame
    {
        // Unique identifier for the data frame
        public int Id { get; set; }

        // Timestamp of when the data was received (in Unix time format)
        public long RecvTime { get; set; }

        // Temperature reading associated with the data frame
        public double Temperature { get; set; }

        // Battery level associated with the data frame
        public double Battery { get; set; }

        // Type of data (e.g., sensor type or category)
        public int DataType { get; set; }

        // Accelerometer readings along the X, Y, and Z axes
        public int Gx { get; set; } // X-axis acceleration
        public int Gy { get; set; } // Y-axis acceleration
        public int Gz { get; set; } // Z-axis acceleration

        // Acknowledgment message flag indicating whether the message was acknowledged
        public bool AckMsg { get; set; }

        // Foreign key referencing the Device entity associated with this data frame
        public int DeviceId { get; set; }

        // Navigation property for the related Device entity
        public Device Device { get; set; }

        // Unique acknowledgment ID for the message
        public string AckId { get; set; }

        // Timestamp of when the acknowledgment was received (in Unix time format)
        public long AckTime { get; set; }

        // Unique identifier for the data frame, used for various tracking purposes
        public string Guid { get; set; }

        // Foreign key referencing the MoveeEventFrame entity associated with this data frame
        public int MoveeEventFrameId { get; set; }

        // Default constructor initializing the properties with default values
        public MoveeDataFrame()
        {
            Id = 0; // Default ID value
            RecvTime = 0; // Default timestamp
            Temperature = 0; // Default temperature
            Battery = 0; // Default battery level
            DataType = 0; // Default data type
            Gx = 0; // Default X-axis acceleration
            Gy = 0; // Default Y-axis acceleration
            Gz = 0; // Default Z-axis acceleration
            AckMsg = true; // Default acknowledgment flag
            Device = new Device(); // Default initialization of the Device navigation property
            AckId = ""; // Default acknowledgment ID
            AckTime = 0; // Default acknowledgment time
            MoveeEventFrameId = 0; // Default MoveeEventFrame ID
            Guid = ""; // Default GUID value
        }
    }
}
