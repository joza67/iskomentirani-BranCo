// Import necessary namespaces
using LoRinoBackend.ViewModels; // Provides access to the ViewModels in the application
using System; // Provides basic functionalities
using System.Collections.Generic; // Provides collection types like IEnumerable
using System.Linq; // Provides LINQ functionalities for data querying
using System.Threading.Tasks; // Provides asynchronous programming functionalities

namespace LoRinoBackend.Models
{
    // Interface defining the contract for device type repository operations
    public interface IDeviceTypeRepository
    {
        // Method to get a device type by its ID
        DeviceType GetData(int id);

        // Method to get all device types
        IEnumerable<DeviceType> GetAllData();

        // Method to get the last set of device types, possibly for recent data
        IEnumerable<DeviceType> GetLastData();

        // Method to add a new device type
        DeviceType Add(DeviceType newData);

        // Method to get a device type by its integer ID
        DeviceType GetDeviceType(int Id);

        // Method to get all device types
        IEnumerable<DeviceType> GetAllDeviceType();

        // Method to update an existing device type's details
        DeviceType Update(DeviceType deviceTypeChanges);
    }
}
