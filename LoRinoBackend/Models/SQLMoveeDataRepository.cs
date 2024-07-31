using LoRinoBackend.ViewModels; // For view models used in the repository
using Microsoft.EntityFrameworkCore; // For Entity Framework Core functionalities
using Microsoft.Extensions.Logging; // For logging
using System; // For basic .NET types and operations
using System.Collections.Generic; // For collections like List<T> and IEnumerable<T>
using System.Linq; // For LINQ operations such as Where, OrderBy, and GroupBy
using System.Threading.Tasks; // For asynchronous programming
using System.Text.Json; // For JSON serialization
using System.Text.Json.Serialization; // For JSON serialization settings
using LoRinoBackend.Functions; // For utility functions, likely including Unix time conversion
using LoRinoBackend.Repository; // For base repository classes

namespace LoRinoBackend.Models
{
    // Repository class for managing MoveeDataFrame entities, inheriting from RepositoryBase and implementing IMoveeDataRepository
    public class SQLMoveeDataRepository : RepositoryBase<MoveeDataFrame>, IMoveeDataRepository
    {
        private readonly AppDbContext context; // Database context for accessing MoveeDataFrame entities
        private readonly ILogger<SQLMoveeDataRepository> logger; // Logger for logging information, warnings, and errors

        // Constructor to initialize the repository with the database context and logger
        public SQLMoveeDataRepository(AppDbContext context, ILogger<SQLMoveeDataRepository> logger) : base(context)
        {
            this.context = context;
            this.logger = logger;
        }

        // Asynchronous method to serialize data to a JSON string
        public async Task<string> SerializeAsync<T>(T data)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve // Preserve object references during serialization
            };

            return await Task.Run(() => JsonSerializer.Serialize(data, options));
        }

        // Synchronous method to serialize data to a JSON string
        public string Serialize<T>(T data)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve // Preserve object references during serialization
            };

            return JsonSerializer.Serialize(data, options);
        }

        // Method to add a new MoveeDataFrame entity to the database
        public MoveeDataFrame Add(MoveeDataFrame newData)
        {
            context.MoveeDataFrame.Add(newData); // Add the new data frame
            context.SaveChanges(); // Save changes to the database
            return newData; // Return the added data frame
        }

        // Method to get event data with filtering and pagination
        public IEnumerable<MoveeDataFrame> GetEventDatas(string filter, int pageNumber, int companyId)
        {
            if (pageNumber < 0)
            {
                pageNumber = 0; // Ensure page number is non-negative
            }

            switch (filter)
            {
                case "all":
                    // Get all event data for a specific company
                    return context.MoveeDataFrame
                        .Where(e => (e.DataType == 4 && e.Device.Company.Id == companyId))
                        .Include(ed => ed.Device)
                        .ToList()
                        .OrderBy(i => i.Id);
                case "active":
                    // Get active event data for a specific company
                    return context.MoveeDataFrame
                        .Where(e => (e.AckMsg == false && e.DataType == 4 && e.Device.Company.Id == companyId))
                        .Include(ed => ed.Device)
                        .ToList()
                        .OrderBy(i => i.Id);
                case "confirmed":
                    // Get confirmed event data for a specific company
                    return context.MoveeDataFrame
                        .Where(e => (e.AckMsg == true && e.DataType == 4 && e.Device.Company.Id == companyId))
                        .Include(ed => ed.Device)
                        .ToList()
                        .OrderBy(i => i.Id);
                case "info":
                    // Get informational data for a specific company
                    return context.MoveeDataFrame
                        .Where(e => (e.DataType == 1 && e.Device.Company.Id == companyId))
                        .Include(ed => ed.Device)
                        .ToList()
                        .OrderBy(i => i.Id);
                default:
                    // Default case: get active event data if filter is unknown
                    return context.MoveeDataFrame
                        .Where(e => (e.AckMsg == false && e.DataType == 4 && e.Device.Company.Id == companyId))
                        .Include(ed => ed.Device)
                        .ToList()
                        .OrderBy(i => i.Id);
            }
        }

        // Method to get event data for admins with filtering and pagination
        public IEnumerable<MoveeDataFrame> GetEventDatasAdmin(string filter, int pageNumber)
        {
            if (pageNumber < 0)
            {
                pageNumber = 0; // Ensure page number is non-negative
            }

            switch (filter)
            {
                case "all":
                    // Get all event data
                    return context.MoveeDataFrame
                        .Where(e => (e.DataType == 4))
                        .Include(ed => ed.Device)
                        .ToList()
                        .OrderBy(i => i.Id);
                case "active":
                    // Get active event data
                    return context.MoveeDataFrame
                        .Where(e => (e.AckMsg == false && e.DataType == 4))
                        .Include(ed => ed.Device)
                        .ToList()
                        .OrderBy(i => i.Id);
                case "confirmed":
                    // Get confirmed event data
                    return context.MoveeDataFrame
                        .Where(e => (e.AckMsg == true && e.DataType == 4))
                        .Include(ed => ed.Device)
                        .ToList()
                        .OrderBy(i => i.Id);
                case "info":
                    // Get informational data
                    return context.MoveeDataFrame
                        .Where(e => (e.DataType == 1))
                        .Include(ed => ed.Device)
                        .ToList()
                        .OrderBy(i => i.Id);
                default:
                    // Default case: get active event data if filter is unknown
                    return context.MoveeDataFrame
                        .Where(e => (e.AckMsg == false && e.DataType == 4))
                        .Include(ed => ed.Device)
                        .ToList()
                        .OrderBy(i => i.Id);
            }
        }

        // Method to get event data for a specific device EUI
        public IEnumerable<MoveeDataFrame> GetEventData(string devEui)
        {
            return context.MoveeDataFrame
                .Where(e => e.AckMsg == false && e.Device.DevEui == devEui)
                .Include(ed => ed.Device)
                .ToList()
                .OrderBy(i => i.Id);
        }

        // Method to get all data, grouped by device EUI and selecting the latest record for each device
        public IEnumerable<MoveeDataFrame> GetAllData()
        {
            return context.MoveeDataFrame
                .Include(d => d.Device)
                .OrderBy(d => d.RecvTime)
                .GroupBy(d => d.Device.DevEui)
                .Select(d => d.OrderBy(d => d.Id).Last());
        }

        // Method to get all data for devices at a specific location
        public IEnumerable<MoveeDataFrame> GetAllDataFromLocationId(int id)
        {
            return context.MoveeDataFrame
                .Include(d => d.Device)
                .Where(d => d.Device.LocationId == id)
                .OrderBy(d => d.RecvTime)
                .GroupBy(d => d.Device.DevEui)
                .Select(d => d.Last());
        }

        // Method to get alarms by event ID
        public IEnumerable<MoveeDataFrame> GetAlarmsByEventId(int id)
        {
            var alarms = context.MoveeDataFrame.Where(a => a.MoveeEventFrameId == id);
            return alarms;
        }

        // Method to get index data for home view
        public IEnumerable<HomeDataFrameViewModel> GetIndexData()
        {
            return context.MoveeDataFrame
                .Include(d => d.Device)
                .Include(ct => ct.Device.Company)
                .Include(dt => dt.Device.DeviceType)
                .OrderBy(d => d.RecvTime)
                .ToList()
                .GroupBy(d => d.Device.DevEui)
                .Select(g => new HomeDataFrameViewModel
                {
                    moveeDataFrame = g.AsQueryable().OrderBy(a => a.Id).LastOrDefault(),
                    cntAlarm = g.Count(a => a.AckMsg == false),
                    msgToday = g.Count(t => Unix.ToDateTime(t.RecvTime) >= DateTime.Today),
                    msgYesterDay = g.Count(t => Unix.ToDateTime(t.RecvTime) > DateTime.Today.AddDays(-1) && Unix.ToDateTime(t.RecvTime) < DateTime.Today),
                    totalMsg = g.Count()
                });
        }

        // Method to get the last data for a specific device type, company, and location users
        public IEnumerable<MoveeDataFrameViewModel> GetLastData(int deviceType, int companyId, List<LocationUser> locationUsers, bool isAdmin)
        {
            if (companyId == -1)
            {
                return context.MoveeDataFrame
                    .Where(dev => dev.Device.DeviceType.Id == deviceType)
                    .OrderBy(i => i.Id)
                    .Include(d => d.Device)
                    .Include(dt => dt.Device.DeviceType)
                    .Include(cp => cp.Device.Company)
                    .GroupBy(d => d.Device.DevEui)
                    .Select(g => new MoveeDataFrameViewModel
                    {
                        moveeDataFrame = g.OrderBy(a => a.Id).LastOrDefault(),
                        cntAlarm = g.Where(a => a.AckMsg == false).Count()
                    });
            }

            if (isAdmin)
            {
                return context.MoveeDataFrame
                    .Where(dev => dev.Device.DeviceType.Id == deviceType && dev.Device.Company.Id == companyId)
                    .OrderBy(i => i.Id)
                    .Include(d => d.Device)
                    .Include(dt => dt.Device.DeviceType)
                    .Include(cp => cp.Device.Company)
                    .GroupBy(d => d.Device.DevEui)
                    .Select(g => new MoveeDataFrameViewModel
                    {
                        moveeDataFrame = g.OrderBy(i => i.Id).LastOrDefault(),
                        cntAlarm = g.Where(a => a.AckMsg == false).Count()
                    });
            }

            var data = context.MoveeDataFrame
                .Include(dt => dt.Device.DeviceType)
                .Include(cp => cp.Device.Company)
                .Include(cu => cu.Device.Location)
                .Include(d => d.Device)
                .ToList();

            foreach (var item in locationUsers)
            {
                data = data.Where(a => a.Device.Location.Id == item.LocationId).ToList();
            }

            return data
                .Where(dev => dev.Device.DeviceType.Id == deviceType && dev.Device.Company.Id == companyId)
                .OrderBy(i => i.Id)
                .GroupBy(d => d.Device.DevEui)
                .Select(g => new MoveeDataFrameViewModel
                {
                    moveeDataFrame = g.LastOrDefault(),
                    cntAlarm = g.Where(a => a.AckMsg == false).Count()
                });
        }

        // Method to get data for a specific device EUI
        public IEnumerable<MoveeDataFrame> GetData(string devEui)
        {
            return context.MoveeDataFrame
                .Where(d => d.Device.DevEui == devEui)
                .Include(d => d.Device)
                .ThenInclude(dt => dt.DeviceType)
                .OrderBy(i => i.Id);
        }

        // Method to get data by device EUI, including the device
        public IEnumerable<MoveeDataFrame> GetDataByEui(string devEui)
        {
            return context.MoveeDataFrame
                .Where(d => d.Device.DevEui == devEui)
                .Include(d => d.Device);
        }

        // Method to get a single MoveeDataFrame entity by ID
        public MoveeDataFrame GetSingleData(int id)
        {
            return context.MoveeDataFrame
                .Include(d => d.Device)
                .FirstOrDefault(f => f.Id == id);
        }

        // Method to update a MoveeDataFrame entity
        public MoveeDataFrame Update(MoveeDataFrame frameChanges)
        {
            var frame = context.MoveeDataFrame.Attach(frameChanges);
            frame.State = Microsoft.EntityFrameworkCore.EntityState.Modified; // Mark entity as modified
            context.SaveChanges(); // Save changes to the database
            return frameChanges; // Return the updated data frame
        }

        // Method to update the first record with a new MoveeEventFrame
        public void UpdateFirst(int id, MoveeEventFrame moveeEventFrame)
        {
            int mefId = moveeEventFrame.Id;
            string guid = moveeEventFrame.Guid;
            int alarmCount = moveeEventFrame.AlarmCount;
            var moveeDataFrame = context.MoveeDataFrame
                .OrderByDescending(a => a.Id)
                .FirstOrDefault(a => a.Id == id);

            try
            {
                moveeDataFrame.MoveeEventFrameId = mefId;
                moveeDataFrame.Guid = guid;
                context.SaveChanges(); // Save changes to the database
            }
            catch (Exception ex)
            {
                // Log information if there is an issue with updating data
                logger.LogInformation("Problem with updating data");
                logger.LogInformation(moveeEventFrame.ToString());
                logger.LogInformation(message: ex.Message);
            }
        }

        // Method to update the last record with a new MoveeEventFrame
        public void UpdateLast(int id, MoveeEventFrame moveeEventFrame)
        {
            int mefId = moveeEventFrame.Id;
            string guid = moveeEventFrame.Guid;
            int alarmCount = moveeEventFrame.AlarmCount;
            var moveeDataFrame = context.MoveeDataFrame.FirstOrDefault(a => a.Id == id);
            moveeDataFrame.MoveeEventFrameId = mefId;
            moveeDataFrame.Guid = guid;
            context.SaveChanges(); // Save changes to the database
        }

        // Method to find a MoveeEventFrame by its GUID
        public MoveeEventFrame FindEventByGuid(string guid)
        {
            return context.MoveeEventFrame.FirstOrDefault(a => a.Guid == guid);
        }

        // Method to get last unread data for a specific device EUI
        public IEnumerable<MoveeDataFrame> GetLastUnreadData(string devEui)
        {
            if (devEui != null)
                return context.MoveeDataFrame
                    .Where(d => d.Device.DevEui.StartsWith(devEui) && d.AckMsg == false)
                    .AsEnumerable();

            return context.MoveeDataFrame
                .Where(d => (d.AckMsg == false))
                .OrderBy(d => d.RecvTime)
                .GroupBy(d => d.Device.DevEui)
                .Select(d => d.Last())
                .ToList();
        }

        // Method to get data from the last day for a specific device EUI
        public IEnumerable<MoveeDataFrame> GetLastDay(string devEui)
        {
            DateTime utcDate = DateTime.UtcNow;
            var currentHour = utcDate.Hour;
            var currentMinute = utcDate.Minute;
            var currentSecond = utcDate.Second;

            if (devEui != null)
                return context.MoveeDataFrame
                    .Where(d => (d.Device.DevEui == devEui && Unix.ToDateTime(d.RecvTime) > utcDate.AddHours(-1 * currentHour).AddMinutes(-1 * currentMinute).AddSeconds(-1 * currentSecond)));

            return context.MoveeDataFrame
                .Where(d => (Unix.ToDateTime(d.RecvTime) > utcDate.AddHours(-1 * currentHour).AddMinutes(-1 * currentMinute).AddSeconds(-1 * currentSecond)));
        }

        // Method to get data from the last hour for a specific device EUI
        public IEnumerable<MoveeDataFrame> GetLastHour(string devEui)
        {
            DateTime utcDate = DateTime.UtcNow;
            var currentHour = utcDate.Hour;
            var currentMinute = utcDate.Minute;
            var currentSecond = utcDate.Second;

            if (devEui != null)
                return context.MoveeDataFrame
                    .Where(d => (d.Device.DevEui.StartsWith(devEui.ToString()) && Unix.ToDateTime(d.RecvTime) > utcDate.AddHours(-1).AddMinutes(-1 * currentMinute).AddSeconds(-1 * currentSecond)));

            return context.MoveeDataFrame
                .Where(d => (Unix.ToDateTime(d.RecvTime) > utcDate.AddHours(-1).AddMinutes(-1 * currentMinute).AddSeconds(-1 * currentSecond)));
        }

        // Method to get the current status of data grouped by device EUI
        public IEnumerable<MoveeDataFrame> GetCurrentStatus(string Id)
        {
            return context.MoveeDataFrame
                .OrderBy(i => i.Id)
                .GroupBy(d => d.Device.DevEui)
                .Select(d => d.LastOrDefault());
        }

        // Asynchronous method to add a new MoveeEventFrame entity to the database
        public async Task<MoveeEventFrame> Add(MoveeEventFrame newData)
        {
            await context.MoveeEventFrame.AddAsync(newData); // Add the new event frame
            context.SaveChanges(); // Save changes to the database
            return newData; // Return the added event frame
        }
    }
}
