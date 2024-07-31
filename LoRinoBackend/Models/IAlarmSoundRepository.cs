// Import necessary namespaces
using Microsoft.AspNetCore.Mvc.Rendering; // Provides classes for rendering HTML elements in MVC views
using System; // Provides basic functionalities
using System.Collections.Generic; // Provides collection types like List
using System.Linq; // Provides LINQ functionalities for data querying
using System.Threading.Tasks; // Provides asynchronous programming functionalities

namespace LoRinoBackend.Models
{
    // Interface defining the contract for alarm sound repository operations
    public interface IAlarmSoundRepository
    {
        // Method to get all active, not silent events for a specific user
        List<MoveeEventFrame> GetAllActiveNotSilentEvents(string userId);

        // Method to activate the sound alarm for a specific user
        bool ActivateSoundAlarm(string userId);
    }
}
