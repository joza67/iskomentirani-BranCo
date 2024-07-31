using LoRinoBackend.ViewModels; // For ViewModels used in the application
using Microsoft.EntityFrameworkCore; // For Entity Framework Core functionalities
using Microsoft.Extensions.Logging; // For logging functionalities
using System; // For basic .NET types and operations
using System.Collections.Generic; // For collections such as IEnumerable<T>
using System.Linq; // For LINQ operations like Where and FirstOrDefault
using System.Threading.Tasks; // For asynchronous programming

namespace LoRinoBackend.Models
{
    // Repository class for managing device types in the SQL database, implementing IDeviceTypeRepository
    public class SQLDeviceTypeRepository : IDeviceTypeRepository
    {
        // Private fields for database context and logger
        private readonly AppDbContext context;
        private readonly ILogger<SQLDeviceTypeRepository> logger;

        // Constructor to initialize the repository with the database context and logger
        public SQLDeviceTypeRepository(AppDbContext context, ILogger<SQLDeviceTypeRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        // Method to add a new DeviceType to the database
        public DeviceType Add(DeviceType newData)
        {
            context.DeviceType.Add(newData);
            context.SaveChanges();
            return newData;
        }

        // Method to retrieve all DeviceTypes from the database
        public IEnumerable<DeviceType> GetAllData()
        {
            return context.DeviceType;
        }

        // Method to retrieve all DeviceTypes from the database
        // (this method is essentially a duplicate of GetAllData)
        public IEnumerable<DeviceType> GetLastData()
        {
            return context.DeviceType;
        }

        // Method to retrieve all DeviceTypes from the database
        // (this method is essentially a duplicate of GetAllData and GetLastData)
        public IEnumerable<DeviceType> GetAllDeviceType()
        {
            return context.DeviceType;
        }

        // Method to retrieve a DeviceType by its ID
        public DeviceType GetDeviceType(int Id)
        {
            return context.DeviceType.Find(Id);
        }

        // Method to retrieve a DeviceType by its ID, using FirstOrDefault
        public DeviceType GetData(int id)
        {
            return context.DeviceType.FirstOrDefault(i => i.Id == id);
        }

        // Method to update an existing DeviceType in the database
        public DeviceType Update(DeviceType deviceTypeChanges)
        {
            var device = context.DeviceType.Attach(deviceTypeChanges);
            device.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return deviceTypeChanges;
        }
    }
}
