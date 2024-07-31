using Microsoft.EntityFrameworkCore; // For Entity Framework Core functionalities
using Microsoft.Extensions.Logging; // For logging functionalities
using System; // For general .NET functionalities
using System.Collections.Generic; // For collections like IEnumerable<T>
using System.Linq; // For LINQ operations such as ToList and OrderByDescending
using System.Threading.Tasks; // For asynchronous programming (not used in the current code)

namespace LoRinoBackend.Models
{
    // Repository class for managing decoded data in the SQL database
    public class SQLDecodedDataRepository : IDecodedDataRepository
    {
        // Private fields for database context and logger
        private readonly AppDbContext context;
        private readonly ILogger<SQLDecodedDataRepository> logger;

        // Constructor to initialize the repository with the necessary services
        public SQLDecodedDataRepository(AppDbContext context, ILogger<SQLDecodedDataRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        // Method to add new decoded data to the database
        public DecodedData Add(DecodedData newData)
        {
            // Add the new decoded data entity to the context
            context.DecodedData.Add(newData);
            // Save changes to the database
            context.SaveChanges();
            return newData;
        }

        // Method to delete decoded data by its ID
        public DecodedData Delete(int id)
        {
            // Find the decoded data entity by its ID
            DecodedData decodedData = context.DecodedData.Find(id);
            if (decodedData != null)
            {
                // Remove the decoded data entity from the context
                context.DecodedData.Remove(decodedData);
                // Save changes to the database
                context.SaveChanges();
            }
            return decodedData;
        }

        // Method to retrieve all decoded data with related entities
        public IEnumerable<DecodedData> GetAllData()
        {
            // Retrieve and return the last 10 decoded data records with related LoRaData and EndDeviceData
            return context.DecodedData
                .Include(c => c.LoRaData)  // Include related LoRaData entities
                .Include(d => d.LoRaData.EndDeviceData)  // Include related EndDeviceData entities
                .ToList()  // Convert the result to a list
                .OrderByDescending(s => s.Id)  // Order the list by Id in descending order
                .Take(10);  // Take the last 10 records
        }

        // Method to retrieve a single decoded data record by its ID
        public DecodedData GetData(int Id)
        {
            // Find and return the decoded data entity by its ID
            return context.DecodedData.Find(Id);
        }
    }
}
