// Import necessary namespaces
using Microsoft.AspNetCore.Mvc.Rendering; // Provides classes for rendering HTML elements in MVC views
using System.Collections.Concurrent; // Provides thread-safe collections
using System.Collections.Generic; // Provides collection types like List and IEnumerable
using System.Threading.Tasks; // Provides asynchronous programming functionalities

namespace LoRinoBackend.Models
{
    // Interface defining the contract for device repository operations, extending a base repository interface
    public interface IDeviceRepository : IRepositoryBase<Device>
    {
        // Method to get a device by its unique identifier (DevEui)
        Device GetData(string id);

        // Method to get a device by its integer ID
        Device GetDevice(int Id);

        // Method to get all devices filtered by DevEui and group
        IEnumerable<Device> GetAllData(string devEui, string group);

        // Method to get all devices
        IEnumerable<Device> GetAllData();

        // Asynchronous method to get all devices
        Task<List<Device>> GetAllDataAsync();

        // Method to get all devices from a specific location by location ID
        IEnumerable<Device> GetAllDevicesFromLocationId(int id);

        // Method to get the last set of devices, possibly for recent data
        IEnumerable<Device> GetLastData();

        // Method to get devices owned by a specific company, identified by the current user's company ID
        IEnumerable<Device> GetMineDevice(int currentUserCompanyId);

        // Method to get a list of devices for use in a dropdown list, suitable for UI rendering
        List<SelectListItem> DevicesForDropDownList();

        // Method to get the DevEui of a device given its integer ID
        string GetDevEuiFromId(int id);

        // Method to get a list of application users
        List<ApplicationUser> GetUsers();
    }
}
