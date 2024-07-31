using LoRinoBackend.Repository; // For base repository class and interface
using LoRinoBackend.Functions; // For utility functions like Unix time conversion
using Microsoft.Extensions.Logging; // For logging
using System; // For basic .NET types and operations
using System.Collections.Generic; // For collections like List<T> and IEnumerable<T>
using System.Linq; // For LINQ operations like Where and OrderBy
using System.Threading.Tasks; // For asynchronous programming

namespace LoRinoBackend.Models
{
    // Repository class for managing LoRaData entities, implementing ILoRaDataRepository
    public class SQLLoRaRepository : RepositoryBase<LoRaData>, ILoRaDataRepository
    {
        // Private fields for database context and logger
        private readonly AppDbContext context;
        private readonly ILogger<SQLLoRaRepository> logger;

        // Constructor to initialize the repository with the database context and logger
        public SQLLoRaRepository(AppDbContext context, ILogger<SQLLoRaRepository> logger) : base(context)
        {
            this.context = context;
            this.logger = logger;
        }

        // Method to retrieve all LoRaData entries with related entities included
        public IEnumerable<LoRaData> GetAllData()
        {
            return context.LoraData
                .Include(e => e.EndDeviceData)
                .Include(c => c.EndDeviceData.ClusterData)
                .Include(g => g.GwInfoData)
                .ToList();
        }

        // Method to retrieve the latest LoRaData entry based on devEui and optional startString
        public LoRaData GetLastData(string devEui, string startString)
        {
            if (startString == null)
            {
                return context.LoraData
                    .Include(e => e.EndDeviceData)
                    .Include(c => c.EndDeviceData.ClusterData)
                    .Include(g => g.GwInfoData)
                    .Where(d => d.EndDeviceData.DevEui.StartsWith(devEui))
                    .OrderBy(r => r.RecvTime)
                    .LastOrDefault();
            }
            return context.LoraData
                .Include(e => e.EndDeviceData)
                .Include(c => c.EndDeviceData.ClusterData)
                .Include(g => g.GwInfoData)
                .Where(d => d.EndDeviceData.DevEui.StartsWith(devEui.ToString()) && d.Payload.StartsWith(startString))
                .OrderBy(r => r.RecvTime)
                .LastOrDefault();
        }

        // Asynchronous method to retrieve the latest LoRaData entry based on devEui and optional startString
        public async Task<LoRaData> GetLastDataAsync(string devEui, string startString)
        {
            if (startString == null)
            {
                return await context.LoraData
                    .Include(e => e.EndDeviceData)
                    .Include(c => c.EndDeviceData.ClusterData)
                    .Include(g => g.GwInfoData)
                    .Where(d => d.EndDeviceData.DevEui.StartsWith(devEui))
                    .OrderBy(r => r.RecvTime)
                    .LastOrDefaultAsync();
            }
            return await context.LoraData
                .Include(e => e.EndDeviceData)
                .Include(c => c.EndDeviceData.ClusterData)
                .Include(g => g.GwInfoData)
                .Where(d => d.EndDeviceData.DevEui.StartsWith(devEui.ToString()) && d.Payload.StartsWith(startString))
                .OrderBy(r => r.RecvTime)
                .LastOrDefaultAsync();
        }

        // Method to retrieve LoRaData entries for the last day based on devEui
        public IEnumerable<LoRaData> GetLastDay(string devEui)
        {
            DateTime utcDate = DateTime.UtcNow;
            var currentHour = utcDate.Hour;
            var currentMinute = utcDate.Minute;
            var currentSecond = utcDate.Second;

            return context.LoraData
                .Include(e => e.EndDeviceData)
                .Include(c => c.EndDeviceData.ClusterData)
                .Include(g => g.GwInfoData)
                .Where(d => d.EndDeviceData.DevEui == devEui.ToString() &&
                            Unix.ToDateTime(d.RecvTime / 1000) > utcDate.AddHours(-1 * currentHour)
                            .AddMinutes(-1 * currentMinute)
                            .AddSeconds(-1 * currentSecond));
        }

        // Method to retrieve LoRaData entries for the last hour based on devEui
        public IEnumerable<LoRaData> GetLastHour(string devEui)
        {
            DateTime utcDate = DateTime.UtcNow;
            var currentHour = utcDate.Hour;
            var currentMinute = utcDate.Minute;
            var currentSecond = utcDate.Second;

            if (devEui == null)
            {
                return context.LoraData
                    .Include(e => e.EndDeviceData)
                    .Include(c => c.EndDeviceData.ClusterData)
                    .Include(g => g.GwInfoData)
                    .Where(d => Unix.ToDateTime(d.RecvTime / 1000) > utcDate.AddHours(-1)
                                .AddMinutes(-1 * currentMinute)
                                .AddSeconds(-1 * currentSecond));
            }
            return context.LoraData
                .Include(e => e.EndDeviceData)
                .Include(c => c.EndDeviceData.ClusterData)
                .Include(g => g.GwInfoData)
                .Where(d => d.EndDeviceData.DevEui.StartsWith(devEui.ToString()) &&
                            Unix.ToDateTime(d.RecvTime / 1000) > utcDate.AddHours(-1)
                            .AddMinutes(-1 * currentMinute)
                            .AddSeconds(-1 * currentSecond));
        }

        // Method to retrieve LoRaData entries with specific battery conditions based on devEui
        public IEnumerable<LoRaData> GetSmilioBattery(string devEui)
        {
            DateTime utcDate = DateTime.UtcNow;
            var currentHour = utcDate.Hour;
            var currentMinute = utcDate.Minute;
            var currentSecond = utcDate.Second;

            return context.LoraData
                .Include(e => e.EndDeviceData)
                .Include(c => c.EndDeviceData.ClusterData)
                .Include(g => g.GwInfoData)
                .Where(d => d.EndDeviceData.DevEui == devEui.ToString() &&
                            d.Payload.Substring(0, 2) == "01" &&
                            d.Payload.Substring(d.Payload.Length - 2) == "64");
        }

        // Method to retrieve a specific LoRaData entry by its ID
        public LoRaData GetData(int Id)
        {
            return context.LoraData
                .Include(e => e.EndDeviceData)
                .Include(c => c.EndDeviceData.ClusterData)
                .Include(g => g.GwInfoData)
                .FirstOrDefault(i => i.Id == Id);
        }

        // Method to check the validity of new LoRaData entry based on its FCntUp and RecvTime
        public bool CheckValidity(LoRaData newData)
        {
            var check = context.LoraData.Where(x => x.EndDeviceData == newData.EndDeviceData);
            if (check.Any(a => a.FCntUp == newData.FCntUp))
            {
                if (check.Any(r => r.RecvTime == newData.RecvTime))
                    return false;
            }
            return true;
        }
    }
}
