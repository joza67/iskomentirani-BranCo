using LoRinoBackend.Functions; // Imports functions from the LoRinoBackend project
using LoRinoBackend.Hubs; // Imports hubs for SignalR communication
using LoRinoBackend.Models; // Imports models used in the LoRinoBackend project
using LoRinoBackend.ViewModels; // Imports view models used in the LoRinoBackend project
using Microsoft.AspNetCore.Authorization; // Imports authorization attributes
using Microsoft.AspNetCore.Http; // Imports HTTP context features
using Microsoft.AspNetCore.Identity; // Imports identity management
using Microsoft.AspNetCore.Mvc; // Imports MVC controller base class
using Microsoft.AspNetCore.Mvc.Rendering; // Imports select list for dropdowns
using Microsoft.AspNetCore.SignalR; // Imports SignalR hub context
using Microsoft.EntityFrameworkCore; // Imports Entity Framework Core
using Microsoft.Extensions.Logging; // Imports logging functionality
using System; // Imports fundamental types and functions
using System.Collections.Generic; // Imports collection types
using System.Diagnostics; // Imports debugging functionality
using System.Dynamic; // Imports dynamic objects
using System.Linq; // Imports LINQ for data querying
using System.Threading.Tasks; // Imports asynchronous task management

namespace LoRinoBackend.Controllers // Defines the namespace for the controllers
{
    public class DeviceController : Controller // Defines the DeviceController class that inherits from Controller
    {
        // Fields for dependency injection of required services
        private readonly IAlarmSoundRepository alarmSoundRepository; // Repository for alarm sounds
        private readonly AppDbContext context; // Application database context
        private readonly ICompanyRepository companyRepository; // Repository for company data
        private readonly IDeviceRepository _deviceRepository; // Repository for device data
        private readonly IDeviceTypeRepository deviceTypeRepository; // Repository for device types
        private readonly ILogger<DeviceController> logger; // Logger for logging events
        private readonly ILocationRepository locationRepository; // Repository for location data
        private readonly ILoRaDataRepository loRaDataRepository; // Repository for LoRa data
        private readonly IMoveeDataRepository moveeDataRepository; // Repository for Movee data
        private readonly UserManager<ApplicationUser> userManager; // User manager for handling user data

        // Constructor for initializing dependencies via dependency injection
        public DeviceController(AppDbContext context,
                        IAlarmSoundRepository alarmSoundRepository,
                        ICompanyRepository companyRepository,
                        IDeviceRepository deviceRepository,
                        IDeviceTypeRepository deviceTypeRepository,
                        IHubContext<WanesyHub> hubContext,
                        ILogger<DeviceController> logger,
                        ILocationRepository locationRepository,
                        ILoRaDataRepository loRaDataRepository,
                        IMoveeDataRepository moveeDataRepository,
                        UserManager<ApplicationUser> userManager)
        {
            this.context = context; // Assigns context to the class field
            this.alarmSoundRepository = alarmSoundRepository; // Assigns alarmSoundRepository to the class field
            this.companyRepository = companyRepository; // Assigns companyRepository to the class field
            _deviceRepository = deviceRepository; // Assigns deviceRepository to the class field
            this.deviceTypeRepository = deviceTypeRepository; // Assigns deviceTypeRepository to the class field
            this.logger = logger; // Assigns logger to the class field
            this.locationRepository = locationRepository; // Assigns locationRepository to the class field
            this.loRaDataRepository = loRaDataRepository; // Assigns loRaDataRepository to the class field
            this.moveeDataRepository = moveeDataRepository; // Assigns moveeDataRepository to the class field
            this.userManager = userManager; // Assigns userManager to the class field
        }

        // Returns error information as JSON
        public IActionResult returnError(int errorCode, String errorData)
        {
            return Json(new ReturnError { ErrorCode = errorCode, ErrorDescription = errorData }); // Returns error details as JSON
        }

        // Fetches all device data based on Id and filter string
        public IActionResult GetAllData(string Id, string s)
        {
            var model = _deviceRepository.GetAllData(Id, s); // Retrieves all data for the specified device ID and filter
            return Json(model); // Returns the data as JSON
        }

        // Fetches the most recent device data
        public IActionResult GetLastData()
        {
            var model = _deviceRepository.GetLastData(); // Retrieves the latest data for devices
            return Json(model); // Returns the data as JSON
        }

        // Displays the index view with a list of all devices
        [HttpGet] // Specifies that this action handles GET requests
        public async Task<ViewResult> Index()
        {
            var userId = userManager.GetUserId(HttpContext.User); // Gets the ID of the currently logged-in user
            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(userId); // Activates sound alarm for the user
            var model = await _deviceRepository.GetAllDataAsync(); // Asynchronously fetches all device data
            return View(model); // Returns the view with the model
        }

        // Displays the device administration view
        [HttpGet] // Specifies that this action handles GET requests
        public ViewResult Devices()
        {
            return View("DeviceAdmin"); // Returns the "DeviceAdmin" view
        }

        // Retrieves device information with various details for Super Admin
        [HttpGet] // Specifies that this action handles GET requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public async Task<ActionResult<IEnumerable<ExpandoObject>>> DevicesInfo(long ts)
        {
            List<ExpandoObject> l = new List<ExpandoObject>(); // Initializes a list to hold dynamic objects
            try
            {
                var userId = userManager.GetUserId(HttpContext.User); // Gets the ID of the currently logged-in user
                ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(userId); // Activates sound alarm for the user
                var allDevs = _deviceRepository.GetAllData(); // Retrieves all device data

                foreach (Device dev in allDevs) // Iterates through each device
                {
                    dynamic a = new ExpandoObject(); // Creates a dynamic object to hold device info
                    a.Id = dev.Id; // Sets the device ID
                    a.DevEui = dev.DevEui; // Sets the device EUI
                    a.Name = dev.Name; // Sets the device name
                    a.CompanyName = dev.Company.Name; // Sets the company name
                    a.Location = dev.Location.Name; // Sets the location name
                    a.Lat = dev.Lat; // Sets the latitude
                    a.Long = dev.Long; // Sets the longitude

                    var x = loRaDataRepository.GetLastData(dev.DevEui, null); // Gets the last LoRa data for the device

                    a.Datum = null; // Initializes the datum field
                    a.RSSI = null; // Initializes the RSSI field
                    a.SNR = null; // Initializes the SNR field
                    if (x != null) // Checks if there is LoRa data available
                    {
                        a.Datum = x.RecvTime.ToDateTime(); // Sets the received time
                        if (x.GwInfoData != null) // Checks if gateway info is available
                        {
                            a.RSSI = x.GwInfoData[0].Rssi; // Sets the RSSI value
                            a.SNR = x.GwInfoData[0].Snr; // Sets the SNR value
                        }
                    }

                    // Fetches the last temperature and battery data for the device
                    a.Temp = context.MoveeDataFrame.Where(d => d.Device.DevEui == dev.DevEui).OrderBy(r => r.RecvTime).Select(b => b.Temperature).LastOrDefault();
                    a.Bat = context.MoveeDataFrame.Where(d => d.Device.DevEui == dev.DevEui).OrderBy(r => r.RecvTime).Select(b => b.Battery).LastOrDefault();

                    // Counts the number of LoRa data records for the device
                    int count = loRaDataRepository.FindByCondition(x => x.EndDeviceData.DevEui.ToUpper().Equals(dev.DevEui)).Count();
                    if (count != null) // Checks if count is not null
                    {
                        a.Count = count; // Sets the count
                    }
                    l.Add(a); // Adds the dynamic object to the list
                }
            }
            catch (Exception ex) // Catches any exceptions
            {
                Debug.WriteLine(ex.Message); // Logs the exception message
                logger.LogInformation(ex.Message); // Logs the exception message
            }

            return l; // Returns the list of dynamic objects
        }

        // Retrieves device information with details using DeviceDto for Super Admin
        [HttpGet] // Specifies that this action handles GET requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public async Task<ActionResult<IEnumerable<DeviceDto>>> DevicesInfos(long ts)
        {
            List<DeviceDto> l = new List<DeviceDto>(); // Initializes a list to hold device DTOs
            try
            {
                var userId = userManager.GetUserId(HttpContext.User); // Gets the ID of the currently logged-in user
                ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(userId); // Activates sound alarm for the user
                var allDevs = _deviceRepository.GetAllData(); // Retrieves all device data

                foreach (Device dev in allDevs) // Iterates through each device
                {
                    DeviceDto a = new DeviceDto(); // Creates a new DeviceDto object
                    a.Id = dev.Id; // Sets the device ID
                    a.DevEui = dev.DevEui; // Sets the device EUI
                    a.Name = dev.Name; // Sets the device name
                    a.CompanyName = dev.Company.Name; // Sets the company name
                    a.Location = dev.Location.Name; // Sets the location name
                    a.Lat = dev.Lat; // Sets the latitude
                    a.Long = dev.Long; // Sets the longitude

                    var x = loRaDataRepository.GetLastData(dev.DevEui, null); // Gets the last LoRa data for the device

                    a.Datum = null; // Initializes the datum field
                    a.RSSI = null; // Initializes the RSSI field
                    a.SNR = null; // Initializes the SNR field
                    if (x != null) // Checks if there is LoRa data available
                    {
                        a.Datum = x.RecvTime.ToDateTime(); // Sets the received time
                        if (x.GwInfoData != null) // Checks if gateway info is available
                        {
                            a.RSSI = x.GwInfoData[0].Rssi; // Sets the RSSI value
                            a.SNR = x.GwInfoData[0].Snr; // Sets the SNR value
                        }
                    }

                    // Fetches the last temperature and battery data for the device
                    a.Temp = context.MoveeDataFrame.Where(d => d.Device.DevEui == dev.DevEui).OrderBy(r => r.RecvTime).Select(b => b.Temperature).LastOrDefault();
                    a.Bat = context.MoveeDataFrame.Where(d => d.Device.DevEui == dev.DevEui).OrderBy(r => r.RecvTime).Select(b => b.Battery).LastOrDefault();
                    var TEST = context.MoveeDataFrame.Where(d => d.Device.DevEui == dev.DevEui).OrderByDescending(r => r.RecvTime).LastOrDefault();
                    var TEST2 = context.MoveeDataFrame.Where(d => d.Device.DevEui == dev.DevEui).OrderBy(r => r.RecvTime).LastOrDefault();

                    // Counts the number of LoRa data records for the device
                    int count = loRaDataRepository.FindByCondition(x => x.EndDeviceData.DevEui.ToUpper().Equals(dev.DevEui)).Count();
                    if (count != null) // Checks if count is not null
                    {
                        a.Count = count; // Sets the count
                    }
                    l.Add(a); // Adds the DeviceDto object to the list
                }
            }
            catch (Exception ex) // Catches any exceptions
            {
                Debug.WriteLine(ex.Message); // Logs the exception message
                logger.LogInformation(ex.Message); // Logs the exception message
            }

            return l; // Returns the list of DeviceDto objects
        }

        // Displays device details for a specific device id
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public ViewResult Details(int id)
        {
            var currentUserId = userManager.GetUserId(HttpContext.User); // Gets the ID of the currently logged-in user
            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(currentUserId); // Activates sound alarm for the user
            ApplicationUser currentUser = userManager.Users.Include(x => x.Company).SingleOrDefaultAsync(x => x.Id == currentUserId).Result; // Retrieves the current user's information

            var isSuperAdmin = userManager.IsInRoleAsync(currentUser, "Super Admin").Result; // Checks if the user is a Super Admin
            var currentUserCompanyId = currentUser.Company.Id; // Gets the current user's company ID

            Device device = _deviceRepository.GetDevice(id); // Retrieves the device with the specified ID

            // Handle case where device is not found
            if (device == null)
            {
                Response.StatusCode = 404; // Sets the response status code to 404
                return View("DeviceNotFound", id); // Returns the "DeviceNotFound" view with the device ID
            }

            var deviceCompanyId = device.Company.Id; // Gets the company ID associated with the device

            // Check if the user has permission to view the device
            if (deviceCompanyId != currentUserCompanyId && !isSuperAdmin)
            {
                Response.StatusCode = 404; // Sets the response status code to 404
                return View("DeviceNotFound", id); // Returns the "DeviceNotFound" view with the device ID
            }

            DeviceDetailsViewModel deviceDetailsViewModel = new DeviceDetailsViewModel() // Creates a new DeviceDetailsViewModel
            {
                Device = device, // Sets the device
                PageTitle = "Device Details" // Sets the page title
            };

            return View(deviceDetailsViewModel); // Returns the view with the device details model
        }

        // Displays the view for creating a new device
        [HttpGet] // Specifies that this action handles GET requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public ViewResult Create(int id)
        {
            var currentUserId = userManager.GetUserId(HttpContext.User); // Gets the ID of the currently logged-in user
            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(currentUserId); // Activates sound alarm for the user
            ApplicationUser currentUser = userManager.Users.Include(x => x.Company).SingleOrDefaultAsync(x => x.Id == currentUserId).Result; // Retrieves the current user's information

            var isSuperAdmin = userManager.IsInRoleAsync(currentUser, "Super Admin").Result; // Checks if the user is a Super Admin
            var currentUserCompanyId = currentUser.Company.Id; // Gets the current user's company ID

            DeviceCreateViewModel dm = new DeviceCreateViewModel(); // Creates a new DeviceCreateViewModel

            // Populate dropdown lists for the create view
            dm.DeviceTypeList = deviceTypeRepository.GetAllDeviceType()
                                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                                .ToList();
            dm.CompanyList = companyRepository.GetAllCompanies()
                               .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                               .ToList();
            dm.LocationList = locationRepository.GetAllLocations()
                               .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                               .ToList();

            dm.CompanyId = locationRepository.GetLocationById(id).Company.Id; // Sets the company ID for the selected location
            dm.LocationId = id; // Sets the location ID
            dm.Company = locationRepository.GetLocationById(id).Company; // Sets the company for the selected location
            dm.Location = locationRepository.GetLocationById(id); // Sets the location
            dm.Devices = _deviceRepository.GetAllDevicesFromLocationId(id).ToList(); // Gets all devices from the location

            return View(dm); // Returns the create view with the DeviceCreateViewModel
        }

        // Handles POST request to create a new device
        [HttpPost] // Specifies that this action handles POST requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public IActionResult Create(DeviceCreateViewModel model)
        {
            Company cm = companyRepository.GetCompany(model.CompanyId); // Retrieves the company with the specified ID
            Location cu = locationRepository.GetLocation(model.LocationId); // Retrieves the location with the specified ID

            if (ModelState.IsValid) // Checks if the model state is valid
            {
                DeviceType dt = deviceTypeRepository.GetDeviceType(model.DeviceTypeId); // Retrieves the device type with the specified ID

                Device newDevice = new Device // Creates a new Device object
                {
                    Name = model.Name, // Sets the device name
                    DevEui = model.DevEui, // Sets the device EUI
                    DeviceType = dt, // Sets the device type
                    Lat = model.Latitude, // Sets the latitude
                    Long = model.Longitude, // Sets the longitude
                    Company = cm, // Sets the company
                    Location = cu, // Sets the location
                    Expires = model.Expires // Sets the expiration date
                };

                _deviceRepository.Create(newDevice); // Creates the new device in the repository
                return RedirectToAction("Details", new { id = newDevice.Id }); // Redirects to the device details view
            }

            // Repopulate dropdown lists in case of invalid model state
            model.DeviceTypeList = deviceTypeRepository.GetAllDeviceType()
                                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                                .ToList();
            model.CompanyList = companyRepository.GetAllCompanies()
                               .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                               .ToList();

            model.LocationList = locationRepository.GetAllLocations()
                               .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                               .ToList();

            model.Location = cu; // Sets the location
            model.Company = cm; // Sets the company
            model.Devices = _deviceRepository.GetAllDevicesFromLocationId(model.LocationId).ToList(); // Gets all devices from the location

            return View(model); // Returns the create view with the DeviceCreateViewModel
        }

        // Displays the view for editing a device
        [HttpGet] // Specifies that this action handles GET requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public ViewResult Edit(int Id)
        {
            ViewBag.List = deviceTypeRepository.GetAllDeviceType()
                                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                                .ToList(); // Populates the device type dropdown list
            Device device = _deviceRepository.GetDevice(Id); // Retrieves the device with the specified ID
            var currentUserId = userManager.GetUserId(HttpContext.User); // Gets the ID of the currently logged-in user
            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(currentUserId); // Activates sound alarm for the user

            // Handle case where device is not found
            if (device == null)
            {
                Response.StatusCode = 404; // Sets the response status code to 404
                return View("DeviceNotFound", Id); // Returns the "DeviceNotFound" view with the device ID
            }

            DeviceEditViewModel deviceEditViewModel = new DeviceEditViewModel // Creates a new DeviceEditViewModel
            {
                Id = device.Id, // Sets the device ID
                Name = device.Name, // Sets the device name
                DevEui = device.DevEui, // Sets the device EUI
                Latitude = device.Lat, // Sets the latitude
                Longitude = device.Long, // Sets the longitude
                DeviceTypeList = deviceTypeRepository.GetAllDeviceType()
                                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                                .ToList(), // Populates the device type dropdown list
                DeviceTypeId = device.DeviceType.Id, // Sets the device type ID
                CompanyId = device.Company.Id, // Sets the company ID
                LocationId = device.Location.Id, // Sets the location ID
                ExistingPhotoPath = device.DeviceType.PhotoPath, // Sets the existing photo path
                Expires = device.Expires // Sets the expiration date
            };

            // Repopulate dropdown lists for the edit view
            deviceEditViewModel.DeviceTypeList = deviceTypeRepository.GetAllDeviceType()
                                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                                .ToList();

            deviceEditViewModel.CompanyList = companyRepository.GetAllCompanies()
                                            .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                                            .ToList();

            deviceEditViewModel.LocationList = locationRepository.GetAllLocations()
                                            .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                                            .ToList();

            deviceEditViewModel.Devices = _deviceRepository.GetAllDevicesFromLocationId(device.Location.Id); // Gets all devices from the location

            return View(deviceEditViewModel); // Returns the edit view with the DeviceEditViewModel
        }

        // Handles POST request to update device details
        [HttpPost] // Specifies that this action handles POST requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public IActionResult Edit(DeviceEditViewModel model)
        {
            ViewBag.List = deviceTypeRepository.GetAllDeviceType()
                                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                                .ToList(); // Populates the device type dropdown list

            DeviceType dt = deviceTypeRepository.GetDeviceType(model.DeviceTypeId); // Retrieves the device type with the specified ID
            Company cp = companyRepository.GetCompany(model.CompanyId); // Retrieves the company with the specified ID
            Location cu = locationRepository.GetLocation(model.LocationId); // Retrieves the location with the specified ID

            if (ModelState.IsValid) // Checks if the model state is valid
            {
                Device device = _deviceRepository.GetDevice(model.Id); // Retrieves the device with the specified ID

                device.Name = model.Name; // Sets the device name
                device.DevEui = model.DevEui; // Sets the device EUI
                device.Lat = model.Latitude; // Sets the latitude
                device.Long = model.Longitude; // Sets the longitude
                device.DeviceType = dt; // Sets the device type
                device.Company = cp; // Sets the company
                device.Location = cu; // Sets the location
                _deviceRepository.Update(device); // Updates the device in the repository

                return RedirectToAction("Details", new { id = device.Id }); // Redirects to the device details view
            }

            // Repopulate dropdown lists in case of invalid model state
            model.Devices = _deviceRepository.GetAllDevicesFromLocationId(model.LocationId); // Gets all devices from the location
            model.DeviceTypeList = deviceTypeRepository.GetAllDeviceType()
                                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                                .ToList();

            model.CompanyList = companyRepository.GetAllCompanies()
                                            .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                                            .ToList();

            model.LocationList = locationRepository.GetAllLocations()
                                            .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                                            .ToList();

            return View(model); // Returns the edit view with the DeviceEditViewModel
        }

        // Displays the view to confirm deletion of a device
        public IActionResult Delete(int id)
        {
            var deviceForDelete = _deviceRepository.GetDevice(id); // Retrieves the device with the specified ID
            return View(deviceForDelete); // Returns the delete view with the device
        }

        // Handles POST request to delete a device
        [HttpPost] // Specifies that this action handles POST requests
        [ActionName("Delete")] // Specifies that this action corresponds to the "Delete" action name
        [ValidateAntiForgeryToken] // Validates the anti-forgery token
        public IActionResult DeleteDevice(Device device)
        {
            var deviceForDelete = _deviceRepository.GetDevice(device.Id); // Retrieves the device with the specified ID
            _deviceRepository.Delete(deviceForDelete); // Deletes the device from the repository

            return RedirectToAction(nameof(Index)); // Redirects to the index view
        }
    }
}
