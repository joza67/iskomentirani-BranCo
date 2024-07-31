// Import necessary namespaces
using System; // Provides basic functionalities
using System.Collections.Generic; // Provides collection types like List
using System.Linq; // Provides LINQ functionalities for data querying
using System.Threading.Tasks; // Provides asynchronous programming functionalities

namespace LoRinoBackend.Models
{
    // Class representing database information
    public class DB_Info
    {
        // Property representing the return identifier
        public int ReturnId { get; set; }

        // Property representing the Movee event frame ID
        public int _MoveeEventFrameId { get; set; }

        // Property representing the device EUI (unique identifier)
        public string _DevEui { get; set; }

        // Property representing the data type
        public int _DataType { get; set; }

        // Property representing the alarm count
        public int _AlarmCount { get; set; }

        // Property indicating whether to send email notifications
        public bool _EmailNotify { get; set; }

        // Property representing the company ID
        public int _CompanyId { get; set; }

        // Property representing the location ID
        public int _LocationId { get; set; }

        // Property representing the GUID (Globally Unique Identifier)
        public string _Guid { get; set; }

        // Property representing the time the timer started (in milliseconds)
        public long _TimerStarted { get; set; }

        // Property representing the name of the location
        public string _LocationName { get; set; }

        // List of ApplicationUser objects representing the users to notify via email
        public List<ApplicationUser> _MailUsers { get; set; }

        // Enumerable of Location objects representing the locations associated with the company
        public IEnumerable<Location> _Locations { get; set; }
    }
}
