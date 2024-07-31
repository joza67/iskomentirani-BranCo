// Import necessary namespaces
using System.Collections.Generic; // Provides collection types like IEnumerable
using System.Threading.Tasks; // Provides asynchronous programming functionalities

namespace LoRinoBackend.Models
{
    // Interface defining the contract for LoRa data repository operations
    public interface ILoRaDataRepository : IRepositoryBase<LoRaData>
    {
        // Method to get LoRa data by its ID
        LoRaData GetData(int Id);

        // Method to get the last LoRa data entry for a specific device (devEui) starting with a specific string
        LoRaData GetLastData(string devEui, string startString);

        // Asynchronous method to get the last LoRa data entry for a specific device (devEui) starting with a specific string
        Task<LoRaData> GetLastDataAsync(string devEui, string startString);

        // Method to get all LoRa data entries from the last day for a specific device (devEui)
        IEnumerable<LoRaData> GetLastDay(string devEui);

        // Method to get all LoRa data entries from the last hour for a specific device (devEui)
        IEnumerable<LoRaData> GetLastHour(string devEui);

        // Method to get all battery-related LoRa data entries for a specific device (devEui)
        IEnumerable<LoRaData> GetSmilioBattery(string devEui);

        // Method to get all LoRa data entries
        IEnumerable<LoRaData> GetAllData();

        // Method to check the validity of new LoRa data
        bool CheckValidity(LoRaData newData);
    }
}
