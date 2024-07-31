using LoRinoBackend.Repository; // For repository base class and interface
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectListItem class used in dropdown lists
using Microsoft.Extensions.Logging; // For logging functionality
using System.Collections.Generic; // For collections such as IEnumerable<T> and List<T>
using System.Linq; // For LINQ operations like Where and FirstOrDefault
using System.Threading.Tasks; // For asynchronous programming

namespace LoRinoBackend.Models
{
    // Repository class for managing devices in the SQL database, inheriting from RepositoryBase
    public class SQLDeviceRepository : RepositoryBase<Device>, IDeviceRepository
    {
        // Private fields for database context and logger
        private readonly AppDbContext context;
        private readonly ILogger<SQLDeviceRepository> logger;

        // Constructor to initialize the repository with the database context and logger
        public SQLDeviceRepository(AppDbContext context, ILogger<SQLDeviceRepository> logger)
            : base(context)
        {
            this.context = context;
            this.logger = logger;
        }

        // Method to retrieve devices based on devEui and group; if devEui is null, return all devices
        public IEnumerable<Device> GetAllData(string devEui, string group)
        {
            if (devEui != null)
                // Return devices where DevEui starts with the provided value and Id is greater than 0
                return context.Device
                    .Where(d => d.DevEui.StartsWith(devEui) && d.Id > 0)
                    .AsEnumerable();

            // Return all devices with related entities: DeviceType, Company, and Location
            return context.Device
                .Include(dt => dt.DeviceType)
                .Include(c => c.Company)
                .Include(a => a.Location)
                .AsEnumerable();
        }

        // Method to retrieve all devices
        public IEnumerable<Device> GetAllData()
        {
            return context.Device.ToList();
        }

        // Asynchronous method to retrieve all devices
        public async Task<List<Device>> GetAllDataAsync()
        {
            return await context.Device.ToListAsync();
        }

        // Method to retrieve devices based on their location ID
        public IEnumerable<Device> GetAllDevicesFromLocationId(int id)
        {
            return context.Device
                .Where(a => a.LocationId == id)
                .ToList();
        }

        // Method to retrieve devices of a specific type (type ID = 1) with related DeviceType
        public IEnumerable<Device> GetLastData()
        {
            return context.Device
                .Where(di => di.DeviceType.Id == 1)
                .Include(dt => dt.DeviceType);
        }

        // Method to retrieve a device by its DevEui, including related Company and Location entities
        Device IDeviceRepository.GetData(string Id)
        {
            return context.Device
                .Include(c => c.Company)
                .Include(a => a.Location)
                .OrderByDescending(i => i.DevEui)
                .FirstOrDefault(i => i.DevEui == Id); // Find the device by DevEui
        }

        // Method to retrieve a device by its ID, including related Company, Location, and DeviceType entities
        Device IDeviceRepository.GetDevice(int Id)
        {
            return context.Device
                .Include(c => c.Company)
                .Include(a => a.Location)
                .Include(e => e.DeviceType)
                .FirstOrDefault(i => i.Id == Id); // Find the device by ID
        }

        // Method to retrieve devices belonging to the current user's company
        IEnumerable<Device> IDeviceRepository.GetMineDevice(int currentUserCompanyId)
        {
            return context.Device
                .Include(c => c.Company)
                .Include(a => a.Location)
                .Include(e => e.DeviceType)
                .Where(c => c.Company.Id == currentUserCompanyId)
                .ToList(); // Return devices for the specified company ID
        }

        // Method to get devices as a list of SelectListItem for dropdowns
        public List<SelectListItem> DevicesForDropDownList()
        {
            return context.Device
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.DevEui
                })
                .ToList(); // Return a list of SelectListItem
        }

        // Method to retrieve DevEui from a device ID
        public string GetDevEuiFromId(int id)
        {
            return context.Device
                .FirstOrDefault(a => a.Id == id)
                .DevEui; // Find and return DevEui by device ID
        }

        // Method to get all users from the database
        public List<ApplicationUser> GetUsers()
        {
            var allUsers = context.Users.ToList(); // Retrieve and return all users
            return allUsers;
        }
    }
}
