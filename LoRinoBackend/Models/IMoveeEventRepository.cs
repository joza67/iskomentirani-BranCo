// Import necessary namespaces
using LoRinoBackend.ViewModels; // Provides access to view models used in the application
using System; // Provides basic functionalities
using System.Collections.Generic; // Provides collection types like ICollection and List
using System.Linq; // Provides LINQ functionalities for data querying
using System.Threading.Tasks; // Provides asynchronous programming functionalities

namespace LoRinoBackend.Models
{
    // Interface defining the contract for Movee event repository operations
    public interface IMoveeEventRepository
    {
        // Method to get all Movee event data
        ICollection<MoveeEventFrame> GetAllData();

        // Method to add a new Movee event and return its ID
        int AddEvent(MoveeEventFrame newData);

        // Method to update the alarm count for a specific event identified by its GUID
        void UpdateAlarmCount(string guid, int count);

        // Method to get the location name associated with a specific event ID
        string GetLocationNameFromEventId(int id);

        // Method to get a Movee event by its ID
        MoveeEventFrame GetMoveeEventById(int id);

        // Method to update an acknowledged Movee event
        void UpdateAckedMoveeEvent(int id, MoveeEventFrame moveeEventFrame);

        // Method to update a cleared Movee event
        void UpdateClearedMoveeEvent(int id, MoveeEventFrame moveeEventFrame);

        // Method to acknowledge alarms by event ID
        void AckMoveeAlarmsByEventId(int id, MoveeEventFrame moveeEventFrame);

        // Method to get the count of active alarms by location ID
        int GetActiveAlarmsByLocationId(int locationId);

        // Method to get all active Movee events
        List<MoveeEventFrame> GetAllActiveEvents();

        // Method to get all confirmed Movee events
        List<MoveeEventFrame> GetAllConfirmedEvents();

        // Method to count all active events
        int CountAllActiveEvents();

        // Method to count all confirmed events
        int CountAllConfirmedEvents();

        // Method to filter Movee events based on a query string and other parameters
        List<MoveeEventFrame> QueryStringFilter(string s, int recordsPerPage, int recordsForPageNo, bool orderByDesc, int[] tagIds, int companyId);

        // Method to get events associated with a specific tag ID
        List<MoveeEventFrame> GetEventsWithTagId(int tagId);

        // Method to get events associated with multiple tag IDs
        List<MoveeEventFrame> GetEventsWithTagIds(int[] tagIds);

        // Method to get the total count of events
        int GetEventsCount();

        // Method to get events from a specific location ID
        List<MoveeEventFrame> GetEventsFromLocationId(int id);

        // Method to get events associated with a specific company ID
        List<MoveeEventFrame> GetEventsWithCompanyId(int companyId);

        // Method to get the location associated with a specific event ID
        Location GetLocationFromEventId(int id);

        // Method to get the event ID associated with a specific data frame ID
        int GetEventIdFromDataFrameId(int id);

        // Method to get the event ID associated with a specific GUID
        int GetEventIdFromGuid(string guid);

        // Method to get the GUID associated with a specific event ID
        string GetGuidFromEventId(int eventId);
    }
}
