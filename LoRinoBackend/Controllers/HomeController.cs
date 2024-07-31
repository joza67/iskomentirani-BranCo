using LoRinoBackend.Functions; // Provides custom functions used in the backend
using LoRinoBackend.Hubs; // Provides hub functionality for SignalR
using LoRinoBackend.ViewModels; // Contains view models used in the application
using Microsoft.AspNetCore.Authorization; // Provides classes for authorization
using Microsoft.AspNetCore.DataProtection; // Provides data protection services
using Microsoft.AspNetCore.Hosting; // Provides hosting environment details
using Microsoft.AspNetCore.Mvc; // Provides classes for MVC controller base
using Microsoft.AspNetCore.SignalR; // Provides classes for SignalR hubs
using Microsoft.Extensions.Logging; // Provides logging functionality
using NuGet.Protocol; // Provides protocol utilities for NuGet
using System; // Provides base classes and fundamental types
using System.Collections.Generic; // Provides generic collection types
using System.Globalization; // Provides globalization support
using System.Linq; // Provides LINQ functionality
using System.Threading.Tasks; // Provides types for asynchronous programming

namespace LoRinoBackend.Controllers
{
    // Controller for handling home-related requests
    public class HomeController : Controller
    {
        // Dependencies for various repositories and services
        private readonly IAlarmSoundRepository _alarmSoundRepository; // Repository for alarm sounds
        private readonly IDecodedDataRepository _decodedDataRepository; // Repository for decoded data
        private readonly IDeviceRepository _deviceRepository; // Repository for devices
        private readonly IHubContext<WanesyHub> _hubContext; // Context for SignalR hub
        private readonly ILocationRepository _locationRepository; // Repository for locations
        private readonly ILogger<HomeController> logger; // Logger for logging information
        private readonly ILoRaDataRepository _loraRepository; // Repository for LoRa data
        private readonly MailTimer _mailTimer; // Timer for handling mail-related operations
        private readonly IMoveeDataRepository _moveeDataRepository; // Repository for Movee data
        private readonly IMoveeEventRepository _moveeEventRepository; // Repository for Movee events
        private readonly IWebHostEnvironment hostingEnvironment; // Provides environment details for web hosting

        // Constructor for dependency injection
        public HomeController(IAlarmSoundRepository alarmSoundRepository,
                                IDecodedDataRepository decodedDataRepository,
                                IDeviceRepository deviceRepository,
                                IDataProtectionProvider dataProtectionProvider,
                                IHubContext<WanesyHub> hubContext,
                                IWebHostEnvironment hostingEnvironment,
                                ILocationRepository locationRepository,
                                ILogger<HomeController> logger,
                                ILoRaDataRepository loraRepository,
                                MailTimer mailTimer,
                                IMoveeDataRepository moveeDataRepository,
                                IMoveeEventRepository moveeEventRepository)
        {
            _alarmSoundRepository = alarmSoundRepository;
            _decodedDataRepository = decodedDataRepository;
            _deviceRepository = deviceRepository;
            _hubContext = hubContext;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
            _locationRepository = locationRepository;
            _loraRepository = loraRepository;
            _mailTimer = mailTimer;
            _moveeDataRepository = moveeDataRepository;
            _moveeEventRepository = moveeEventRepository;
        }

        // Action method for displaying the home page with index data
        [Authorize] // Ensures the user is authenticated
        public ViewResult Index()
        {
            var model = _moveeDataRepository.GetIndexData(); // Retrieves index data from the repository
            return View(model); // Returns the view with the index data
        }

        // Action method for returning a JSON error response
        public IActionResult returnError(int errorCode, String errorData)
        {
            return Json(new ReturnError { ErrorCode = errorCode, ErrorDescription = errorData }); // Returns a JSON object with error details
        }

        // Action method for getting the last data based on ID and an optional string parameter
        [AllowAnonymous] // Allows access without authentication
        public IActionResult GetLastData(string Id, string s)
        {
            var model = _loraRepository.GetLastData(Id, s); // Retrieves the last data from the repository
            if (model == null)
            {
                Response.StatusCode = 201; // Sets the HTTP status code to 201 (Created)
                return returnError(204, "No Content for: " + Id); // Returns an error response if no data is found
            }
            return Json(model); // Returns the data as a JSON response
        }

        // Action method for getting data for the last day based on ID
        [AllowAnonymous] // Allows access without authentication
        public IActionResult GetLastDay(string Id)
        {
            var model = _loraRepository.GetLastDay(Id); // Retrieves the last day's data from the repository
            if (model == null)
            {
                Response.StatusCode = 404; // Sets the HTTP status code to 404 (Not Found)
                return returnError(404, "Data not found"); // Returns an error response if no data is found
            }
            return Json(model); // Returns the data as a JSON response
        }

        // Action method for getting data for the last hour based on ID
        [AllowAnonymous] // Allows access without authentication
        public IActionResult GetLastHour(string Id)
        {
            var model = _loraRepository.GetLastHour(Id); // Retrieves the last hour's data from the repository
            if (model == null)
            {
                Response.StatusCode = 404; // Sets the HTTP status code to 404 (Not Found)
                return returnError(404, "Data not found"); // Returns an error response if no data is found
            }
            return Json(model); // Returns the data as a JSON response
        }

        // Action method for getting all data
        [AllowAnonymous] // Allows access without authentication
        public IActionResult GetAllData()
        {
            var model = _loraRepository.GetAllData(); // Retrieves all data from the repository
            return Json(model); // Returns the data as a JSON response
        }

        // Action method for getting details of data based on ID
        [AllowAnonymous] // Allows access without authentication
        public IActionResult Details(int Id)
        {
            LoRaData loRaData = _loraRepository.GetData(Id); // Retrieves data by ID from the repository
            if (loRaData == null)
            {
                Response.StatusCode = 404; // Sets the HTTP status code to 404 (Not Found)
                return returnError(404, "Data not found"); // Returns an error response if no data is found
            }

            HomeDetailsViewModel loRaDetailsViewModel = new HomeDetailsViewModel()
            {
                CurrentLoRaData = loRaData, // Sets the current LoRa data in the view model
                PageTitle = "Data Details" // Sets the page title
            };

            return Json(loRaData); // Returns the data as a JSON response
        }

        // GET action method for rendering the create view
        [HttpGet] // Specifies this method handles GET requests
        public ViewResult Create()
        {
            return View(); // Returns the view for creating new data
        }

        // POST action method for handling the creation of new data
        [HttpPost] // Specifies this method handles POST requests
        public async Task<IActionResult> Create([FromBody] RecievedLoraData model)
        {
            try
            {
                // Check if the model is null or invalid
                if (model == null)
                {
                    Response.StatusCode = 400; // Sets the HTTP status code to 400 (Bad Request)
                    logger.LogWarning("Home/Create received object to decode. Owner object is null"); // Logs a warning
                    return Json(new { fail = "Owner object is null" }); // Returns an error response
                }
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 406; // Sets the HTTP status code to 406 (Not Acceptable)
                    logger.LogWarning("Home/Create received object to decode. Invalid model object"); // Logs a warning
                    return Json(new { fail = "Invalid model object" }); // Returns an error response
                }

                // Create a new LoRaData object from the received model
                LoRaData loRaData = new LoRaData
                {
                    MsgId = model.Id,
                    EndDeviceData = new EndDevice
                    {
                        DevEui = model.EndDevice.DevEui,
                        DevAddr = model.EndDevice.DevAddr,
                        ClusterData = new Cluster
                        {
                            ClusterId = model.EndDevice.Cluster.Id
                        },
                    },
                    FPort = model.FPort,
                    FCntDown = model.FCntDown,
                    FCntUp = model.FCntUp,
                    Adr = model.Adr,
                    Confirmed = model.Confirmed,
                    Encrypted = model.Encrypted,
                    Payload = model.Payload,
                    RecvTime = model.RecvTime,
                    ClassB = model.ClassB,
                    Delayed = model.Delayed,
                    UlFrequency = model.UlFrequency,
                    Modulation = model.Modulation,
                    DataRate = model.DataRate,
                    CodingRate = model.CodingRate,
                    GwCnt = 1,
                    GwInfoData = new List<GwInfo>()
                };

                // Add gateway info data to the LoRaData object
                foreach (var gwInfo in model.GwInfo)
                {
                    loRaData.GwInfoData.Add(new GwInfo
                    {
                        GwEui = gwInfo.GwEui,
                        RfRegion = gwInfo.RfRegion,
                        Rssi = gwInfo.Rssi,
                        Snr = gwInfo.Snr,
                        Latitude = gwInfo.Latitude,
                        Longitude = gwInfo.Longitude,
                        Altitude = gwInfo.Altitude,
                        Channel = gwInfo.Channel,
                        RadioId = gwInfo.RadioId
                    });
                }

                logger.LogInformation("We received a LoRaWAN message"); // Logs an informational message
                var payloadInHex = HexString.HexStringToByte(model.Payload); // Converts payload from hex string to byte array

                Response.StatusCode = 201; // Sets the HTTP status code to 201 (Created)

                // Check if the LoRaData object is a duplicate
                if (!_loraRepository.CheckValidity(loRaData))
                {
                    Response.StatusCode = 409; // Sets the HTTP status code to 409 (Conflict)
                    logger.LogWarning("Home/Create received object to decode. Object already exists."); // Logs a warning
                    return Json(new { fail = "Duplicate message." }); // Returns an error response for duplicate data
                }

                // Save the LoRaData object if it is not a duplicate
                if (loRaData.EndDeviceData.DevEui == "0000000000000000")
                {
                    Response.StatusCode = 202; // Sets the HTTP status code to 202 (Accepted)
                }
                else
                {
                    _loraRepository.Create(loRaData); // Creates the LoRaData record in the repository
                }

                // Process custom data
                AddCustomData customData = new AddCustomData(
                    _moveeDataRepository,
                    _moveeEventRepository,
                    _deviceRepository,
                    _locationRepository);
                var _retObj = await customData.decodeDevice(model.EndDevice.DevEui, model.FPort, model.RecvTime, model.Payload); // Decodes the device data

                if (_retObj.ReturnId == 0)
                    Response.StatusCode = 406; // Sets the HTTP status code to 406 (Not Acceptable)

                // Handle event frame creation based on custom data
                MoveeEventFrame moveeEventFrame = new MoveeEventFrame();
                var eventGuid = _moveeEventRepository.GetAllData().Select(a => a.Guid).ToList(); // Gets all event GUIDs
                if (_retObj._DataType == 4)
                {
                    DB_Info dba = await _mailTimer.SetTimer(_retObj); // Call to mail timer

                    string result = eventGuid.FirstOrDefault(x => x == dba._Guid); // Checks if the GUID already exists

                    if (result == null && dba._Guid != null)
                    {
                        DateTimeOffset now = DateTimeOffset.UtcNow; // Gets the current time in UTC
                        long unixTimeMilliseconds = now.ToUnixTimeMilliseconds(); // Converts the current time to Unix time in milliseconds
                        moveeEventFrame.EventCreationTime = unixTimeMilliseconds;
                        moveeEventFrame.Guid = dba._Guid;
                        moveeEventFrame.AlarmCount = dba._AlarmCount;
                        moveeEventFrame.LocationId = dba._LocationId;
                        _moveeEventRepository.AddEvent(moveeEventFrame); // Update with new data
                        _moveeDataRepository.UpdateFirst(dba.ReturnId, moveeEventFrame);
                    }
                    else
                    {
                        moveeEventFrame = _moveeDataRepository.FindEventByGuid(dba._Guid); // Finds the event by GUID
                        moveeEventFrame.AlarmCount = dba._AlarmCount;
                        _moveeDataRepository.UpdateFirst(dba.ReturnId, moveeEventFrame); // Updates the event with new data
                    }
                }

                return Json(moveeEventFrame); // Returns the event frame as a JSON response
            }
            catch (Exception ex)
            {
                logger.LogInformation(ex.ToString()); // Logs the exception message
                return Json(new { ex = ex }); // Returns the exception details as a JSON response
            }
        }

        // POST action method for handling Chirpstack data
        [HttpPost] // Specifies this method handles POST requests
        public async Task<IActionResult> Chirpstack([FromBody] ChirpstackData model)
        {
            try
            {
                if (model == null)
                {
                    return Json(new { fail = "Owner object is null" }); // Returns an error response if the model is null
                }

                // Convert base64 string data to byte array and then to hex payload
                byte[] b = Convert.FromBase64String(model.data); // Converts base64 string to byte array
                var hexpayload = HexString.ByteToHexString(b); // Converts byte array to hex string
                LoRaData loRaData = new LoRaData
                {
                    MsgId = model.deduplicationId,
                    EndDeviceData = new EndDevice
                    {
                        DevEui = model.deviceInfo.devEui.ToUpper(), // Converts the DevEui to uppercase
                        DevAddr = model.devAddr,
                    },
                    FPort = model.fPort,
                    FCntDown = model.fCnt,
                    Adr = model.adr,
                    Confirmed = model.confirmed,
                    Payload = hexpayload,
                    RecvTime = Unix.ToUnixTimeStamp(DateTime.Parse(model.time, new CultureInfo("hr-HR"))), // Parses the time and converts it to Unix timestamp
                    UlFrequency = (float)model.txInfo.frequency,
                    CodingRate = model.txInfo.modulation.lora.codeRate,
                    GwCnt = model.rxInfo.Count(),
                    GwInfoData = [],
                    FCntUp = model.fCnt,
                    DataRate = model.txInfo.modulation.lora.codeRate,
                    Modulation = model.txInfo.modulation.lora.codeRate + ", BW" + model.txInfo.modulation.lora.bandwidth + ", SF" + model.txInfo.modulation.lora.spreadingFactor,
                };

                // Add gateway info data to the LoRaData object
                foreach (var gwInfo in model.rxInfo)
                {
                    loRaData.GwInfoData.Add(new GwInfo
                    {
                        GwEui = gwInfo.gatewayId,
                        RfRegion = gwInfo.metadata.regionCommonName != null ? gwInfo.metadata.regionCommonName : null,
                        Rssi = (int?)gwInfo.rssi,
                        Snr = (double?)gwInfo.snr,
                        Latitude = gwInfo.location.latitude != null ? (double?)gwInfo.location.latitude : null,
                        Longitude = gwInfo.location.longitude != null ? (double?)gwInfo.location.longitude : null,
                        Channel = gwInfo.channel != null ? (int?)gwInfo.channel : null,
                    });
                }

                // Check if the LoRaData object is a duplicate
                if (!_loraRepository.CheckValidity(loRaData))
                {
                    Response.StatusCode = 409; // Sets the HTTP status code to 409 (Conflict)
                    logger.LogWarning("Home/Create Duplicate LoRa message."); // Logs a warning
                    logger.LogWarning("Already exists."); // Logs an informational message
                    return Json(new { message = "Already exists" }); // Returns an error response for duplicate data
                }

                if (loRaData.EndDeviceData.DevEui == "0000000000000000")
                {
                    Response.StatusCode = 202; // Sets the HTTP status code to 202 (Accepted)
                }
                else
                {
                    _loraRepository.Create(loRaData); // Creates the LoRaData record in the repository
                }

                // Process custom data
                AddCustomData customData = new AddCustomData(
                    _moveeDataRepository,
                    _moveeEventRepository,
                    _deviceRepository,
                    _locationRepository);
                var _retObj = await customData.decodeDevice(model.deviceInfo.devEui.ToUpper(), model.fPort, DateTime.Parse(model.time, new CultureInfo("hr-HR")).ToUnixTimeStamp(), hexpayload); // Decodes the device data

                if (_retObj.ReturnId == 0)
                    Response.StatusCode = 406; // Sets the HTTP status code to 406 (Not Acceptable)

                // Handle event frame creation based on custom data
                MoveeEventFrame moveeEventFrame = new MoveeEventFrame();
                var eventGuid = _moveeEventRepository.GetAllData().Select(a => a.Guid).ToList(); // Gets all event GUIDs
                if (_retObj._DataType == 4)
                {
                    logger.LogInformation("-- Primljena alarm poruka --"); // Logs an informational message

                    DB_Info dba = await _mailTimer.SetTimer(_retObj); // Call to mail timer

                    string result = eventGuid.FirstOrDefault(x => x == dba._Guid); // Checks if the GUID already exists

                    if (result == null && dba._Guid != null)
                    {
                        DateTimeOffset now = DateTimeOffset.UtcNow; // Gets the current time in UTC
                        long unixTimeMilliseconds = now.ToUnixTimeMilliseconds(); // Converts the current time to Unix time in milliseconds
                        moveeEventFrame.EventCreationTime = unixTimeMilliseconds;
                        moveeEventFrame.Guid = dba._Guid;
                        moveeEventFrame.AlarmCount = dba._AlarmCount;
                        moveeEventFrame.LocationId = dba._LocationId;
                        _moveeEventRepository.AddEvent(moveeEventFrame); // Update with new data
                        _moveeDataRepository.UpdateFirst(dba.ReturnId, moveeEventFrame);
                    }
                    else
                    {
                        moveeEventFrame = _moveeDataRepository.FindEventByGuid(dba._Guid); // Finds the event by GUID
                        moveeEventFrame.AlarmCount = dba._AlarmCount;
                        _moveeDataRepository.UpdateFirst(dba.ReturnId, moveeEventFrame); // Updates the event with new data
                    }
                }

                return Json(moveeEventFrame); // Returns the event frame as a JSON response
            }
            catch (Exception ex)
            {
                logger.LogInformation(ex.Message); // Logs the exception message
                logger.LogInformation("Received JSON: "); // Logs an informational message
                logger.LogInformation(model.ToJson().ToString() + "\n"); // Logs the received JSON model
                return Json(new { ex = ex }); // Returns the exception details as a JSON response
            }
        }
    }
}
