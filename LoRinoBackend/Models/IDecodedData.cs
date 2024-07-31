// Import necessary namespaces
using System; // Provides basic functionalities
using System.Collections.Generic; // Provides collection types like IEnumerable
using System.Linq; // Provides LINQ functionalities for data querying
using System.Threading.Tasks; // Provides asynchronous programming functionalities

namespace LoRinoBackend.Models
{
    // Interface defining the contract for decoded data repository operations
    public interface IDecodedDataRepository
    {
        // Method to get a decoded data entry by its ID
        DecodedData GetData(int Id);

        // Method to get all decoded data entries
        IEnumerable<DecodedData> GetAllData();

        // Method to add a new decoded data entry
        DecodedData Add(DecodedData newData);

        // Method to delete a decoded data entry by its ID
        DecodedData Delete(int id);
    }
}
