// Import necessary namespaces
using LoRinoBackend.ViewModels; // Provides access to view models used in the application
using System; // Provides basic functionalities
using System.Collections.Generic; // Provides collection types like IEnumerable
using System.Linq; // Provides LINQ functionalities for data querying
using System.Threading.Tasks; // Provides asynchronous programming functionalities

namespace LoRinoBackend.Models
{
    // Interface defining the contract for Movee data repository operations
    public interface IMoveeDataRepository : IRepositoryBase<MoveeDataFrame>
    {
        // Asynchronous method to serialize data to a string format
        Task<string> SerializeAsync<T>(T data);

        // Method to serialize data to a string format
        string Serialize<T>(T data);

        // Method to get data frames by a specific ID
        IEnumerable<MoveeDataFrame> GetData(string Id);

        // Method to get all data frames from a specific location ID
        IEnumerable<MoveeDataFrame> GetAllDataFromLocationId(int id);

        // Method to get the last set of data frames for a specific device type and company, including location users and admin status
        IEnumerable<MoveeDataFrameViewModel> GetLastData(int deviceType, int companyId, List<LocationUser> locationUsers, bool isAdmin);

        // Method to get a single data frame by its ID
        MoveeDataFrame GetSingleData(int id);

        // Method to update an existing data frame
        MoveeDataFrame Update(MoveeDataFrame frameChanges);

        // Method to update the first occurrence of a specific event frame by ID
        void UpdateFirst(int id, MoveeEventFrame moveeEventFrame);

        // Method to update the last occurrence of a specific event frame by ID
        void UpdateLast(int id, MoveeEventFrame moveeEventFrame);

        // Method to find an event frame by its GUID
        MoveeEventFrame FindEventByGuid(string guid);

        // Method to get the last unread data frames by device EUI
        IEnumerable<MoveeDataFrame> GetLastUnreadData(string devEui);

        // Method to get data frames by device EUI
        IEnumerable<MoveeDataFrame> GetDataByEui(string devEui);

        // Method to get data frames from the last day for a specific device EUI
        IEnumerable<MoveeDataFrame> GetLastDay(string devEui);

        // Method to get data frames from the last hour for a specific device EUI
        IEnumerable<MoveeDataFrame> GetLastHour(string devEui);

        // Method to get event data frames with filtering and pagination
        IEnumerable<MoveeDataFrame> GetEventDatas(string filter, int pageNumber, int companyId);

        // Method to get event data frames with filtering and pagination for admins
        IEnumerable<MoveeDataFrame> GetEventDatasAdmin(string filter, int pageNumber);

        // Method to get event data frames by device EUI
        IEnumerable<MoveeDataFrame> GetEventData(string devEui);

        // Method to get all data frames
        IEnumerable<MoveeDataFrame> GetAllData();

        // Method to get index data for the home view
        IEnumerable<HomeDataFrameViewModel> GetIndexData();

        // Method to add a new data frame
        MoveeDataFrame Add(MoveeDataFrame newData);

        // Method to get the current status of devices by device EUI
        IEnumerable<MoveeDataFrame> GetCurrentStatus(string devEui);

        // Asynchronous method to add a new event frame
        Task<MoveeEventFrame> Add(MoveeEventFrame newData);

        // Method to get alarms associated with a specific event ID
        IEnumerable<MoveeDataFrame> GetAlarmsByEventId(int id);
    }
}
