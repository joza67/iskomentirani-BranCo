using LoRinoBackend.Models; // Importing the necessary namespaces
using LoRinoBackend.ViewModels; // Importing view model classes
using Microsoft.AspNetCore.Authorization; // For authorization attributes
using Microsoft.AspNetCore.Hosting; // For accessing the web hosting environment
using Microsoft.AspNetCore.Identity; // For user identity management
using Microsoft.AspNetCore.Mvc; // For MVC controller functionality
using Microsoft.AspNetCore.Mvc.Rendering; // For select list functionalities
using Microsoft.EntityFrameworkCore; // For entity framework core functionalities
using System; // For general-purpose classes
using System.IO; // For file handling
using System.Linq; // For LINQ functionalities

namespace LoRinoBackend.Controllers // Defining the namespace for the controller
{
    public class LocationController : Controller // LocationController inherits from the base Controller class
    {
        // Dependencies for user management, repositories, and environment
        private readonly UserManager<ApplicationUser> userManager; // UserManager for managing user-related functionalities
        private readonly ICompanyRepository companyRepository; // Company repository for company-related data access
        private readonly ILocationRepository locationRepository; // Location repository for location-related data access
        private readonly IWebHostEnvironment hostingEnvironment; // Hosting environment for accessing web server environment
        private readonly IMoveeDataRepository moveeDataRepository; // Movee data repository for accessing Movee data
        private readonly IMoveeEventRepository moveeEventRepository; // Movee event repository for accessing Movee events
        private readonly IDeviceRepository deviceRepository; // Device repository for device-related data access
        private readonly IAlarmSoundRepository alarmSoundRepository; // Alarm sound repository for alarm sound functionalities

        // Constructor for dependency injection
        public LocationController(UserManager<ApplicationUser> userManager,
                                  ICompanyRepository companyRepository,
                                  IWebHostEnvironment hostingEnvironment,
                                  ILocationRepository locationRepository,
                                  IMoveeDataRepository moveeDataRepository,
                                  IMoveeEventRepository moveeEventRepository,
                                  IDeviceRepository deviceRepository,
                                  IAlarmSoundRepository alarmSoundRepository)
        {
            this.companyRepository = companyRepository; // Initializing the company repository
            this.locationRepository = locationRepository; // Initializing the location repository
            this.hostingEnvironment = hostingEnvironment; // Initializing the hosting environment
            this.moveeDataRepository = moveeDataRepository; // Initializing the Movee data repository
            this.moveeEventRepository = moveeEventRepository; // Initializing the Movee event repository
            this.deviceRepository = deviceRepository; // Initializing the device repository
            this.userManager = userManager; // Initializing the UserManager
            this.alarmSoundRepository = alarmSoundRepository; // Initializing the alarm sound repository
        }

        // Action method for displaying the Access Denied page
        [HttpGet] // HTTP GET method
        [AllowAnonymous] // Allows access to this method without authentication
        public IActionResult AccessDenied()
        {
            return View(); // Returns the AccessDenied view
        }

        // Action method for displaying the Not Found page
        [HttpGet] // HTTP GET method
        [AllowAnonymous] // Allows access to this method without authentication
        public IActionResult NotFound()
        {
            return View(); // Returns the NotFound view
        }

        // Action method for listing locations based on user role and company
        [HttpGet] // HTTP GET method
        [AllowAnonymous] // Allows access to this method without authentication
        public IActionResult ListLocations()
        {
            // Get the current user ID
            var userId = userManager.GetUserId(HttpContext.User); // Retrieves the current user's ID

            // Activate alarm sound based on user ID
            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(userId); // Activates alarm sound for the user

            // Get the user details including their company
            ApplicationUser user = userManager.Users
                .Include(x => x.Company)
                .SingleOrDefaultAsync(x => x.Id == userId)
                .Result; // Retrieves the current user including company details

            // Retrieve locations associated with the current user
            var locationsByUser = locationRepository.GetLocationsByUserId(userId); // Retrieves locations associated with the current user

            // Check user roles to determine location access
            var isSuperAdmin = userManager.IsInRoleAsync(user, "Super Admin").Result; // Checks if the user is a Super Admin
            var isAdmin = userManager.IsInRoleAsync(user, "Admin").Result; // Checks if the user is an Admin
            var currentUserCompanyId = user.Company.Id; // Gets the user's company ID
            var sAdminLocations = locationRepository.GetAllLocations(); // Retrieves all locations for Super Admin
            var adminLocations = locationRepository.GetAllLocations().Where(a => a.CompanyId == currentUserCompanyId); // Retrieves locations for the user's company

            // Return different views based on user roles
            if (isSuperAdmin == true)
            {
                return View(sAdminLocations); // Returns all locations if Super Admin
            }
            if (isAdmin == true)
            {
                return View(adminLocations); // Returns company-specific locations if Admin
            }
            else if (locationsByUser == null)
            {
                return View("AccessDenied"); // Returns Access Denied view if no locations are found
            }
            else
            {
                return View(locationsByUser); // Returns locations associated with the user
            }
        }

        // GET action method for displaying the Create Location view
        [HttpGet] // HTTP GET method
        [Authorize(Policy = "SuperAdmin")] // Requires Super Admin role
        public IActionResult CreateLocation()
        {
            // Get the current user ID
            var userId = userManager.GetUserId(HttpContext.User); // Retrieves the current user's ID

            // Activate alarm sound based on user ID
            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(userId); // Activates alarm sound for the user

            // Prepare the view model with available companies for dropdown list
            LocationCreateViewModel model = new LocationCreateViewModel();
            model.CompaniesDDL = locationRepository.GetAllCompanies()
                    .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                    .ToList(); // Populates the dropdown list with company names

            return View(model); // Returns the CreateLocation view with the model
        }

        // POST action method for handling the creation of a new location
        [HttpPost] // HTTP POST method
        [Authorize(Policy = "SuperAdmin")] // Requires Super Admin role
        public IActionResult CreateLocation(LocationCreateViewModel model)
        {
            if (ModelState.IsValid) // Checks if the model state is valid
            {
                // Create a new Location object from the model data
                Location newLocation = new Location
                {
                    Name = model.Name,
                    Road = model.Road,
                    RoadSection = model.RoadSection,
                    Long = model.Longitude,
                    Lat = model.Latitude,
                    TimerLenght = model.TimerLenght,
                    MapZoom = model.MapZoom,
                    CompanyId = model.CompanyId
                };

                // Add the new location to the repository
                locationRepository.Add(newLocation); // Adds the new location to the repository

                // Redirect to the location details page
                return RedirectToAction("LocationDetails", new { id = newLocation.Id }); // Redirects to the details of the newly created location
            }

            // Reload the companies dropdown list if model state is invalid
            model.CompaniesDDL = locationRepository.GetAllCompanies()
                    .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                    .ToList(); // Repopulates the dropdown list with company names

            return View(model); // Returns the CreateLocation view with the model
        }

        // GET action method for displaying location details
        [AllowAnonymous] // Allows access to this method without authentication
        public IActionResult LocationDetails(int id)
        {
            // Get the current user ID
            var userId = userManager.GetUserId(HttpContext.User); // Retrieves the current user's ID

            // Activate alarm sound based on user ID
            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(userId); // Activates alarm sound for the user

            // Retrieve the location details
            Location location = locationRepository.GetLocation(id); // Retrieves the location details
            ApplicationUser user = userManager.Users
                .Include(x => x.Company)
                .SingleOrDefaultAsync(x => x.Id == userId)
                .Result; // Retrieves the current user including company details

            // Retrieve locations associated with the current user
            var locationsByUser = locationRepository.GetLocationsByUserId(userId); // Retrieves locations associated with the current user

            var _companyId = user.Company.Id; // Gets the user's company ID

            // Check user roles and access to the location
            var isSuperAdmin = userManager.IsInRoleAsync(user, "Super Admin").Result; // Checks if the user is a Super Admin
            var isAdmin = userManager.IsInRoleAsync(user, "Admin").Result; // Checks if the user is an Admin
            var sAdminLocations = locationRepository.GetAllLocations(); // Retrieves all locations for Super Admin

            // Handle case where location is not found
            if (location == null)
            {
                Response.StatusCode = 404; // Sets the status code to 404 (Not Found)
                return View("NotFound"); // Returns the NotFound view
            }

            // Handle access control for non-super admin users
            if (User.IsInRole("Super Admin"))
            {
                _companyId = -1; // Sets company ID to -1 for Super Admin
            }
            else
            {
                if (locationsByUser.FirstOrDefault(a => a.Id == id) == null && !User.IsInRole("Admin"))
                {
                    Response.StatusCode = 404; // Sets the status code to 404 (Not Found)
                    return View("AccessDenied"); // Returns the AccessDenied view
                }
            }

            // Prepare view model with location details, movee data, and events
            var data = moveeDataRepository.GetAllData().Where(a => a.Device.LocationId == id); // Retrieves Movee data associated with the location
            var events = moveeEventRepository.GetEventsFromLocationId(id).OrderBy(d => d.EventCreationTime).ToList(); // Retrieves events associated with the location
            LocationDetailsViewModel locationDetailsViewModel = new()
            {
                moveeEventFrames = events,
                Location = location,
                PageTitle = "Location Details",
                moveeDataFrames = data,
                Devices = deviceRepository.GetAllDevicesFromLocationId(id).ToList() // Retrieves devices associated with the location
            };

            return View(locationDetailsViewModel); // Returns the LocationDetails view with the model
        }

        // GET action method for displaying the Location Edit view
        [HttpGet] // HTTP GET method
        [Authorize(Policy = "SuperAdmin")] // Requires Super Admin role
        public ViewResult LocationEdit(int Id)
        {
            // Get the current user ID
            var userId = userManager.GetUserId(HttpContext.User); // Retrieves the current user's ID

            // Activate alarm sound based on user ID
            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(userId); // Activates alarm sound for the user

            // Retrieve the location to be edited
            Location location = locationRepository.GetLocation(Id); // Retrieves the location to be edited
            LocationEditViewModel locationEditViewModel = new LocationEditViewModel
            {
                Id = location.Id,
                Name = location.Name,
                Road = location.Road,
                RoadSection = location.RoadSection,
                Longitude = location.Long,
                Latitude = location.Lat,
                TimerLenght = location.TimerLenght,
                MapZoom = location.MapZoom,
                CompanyId = location.CompanyId,
                CompaniesDDL = locationRepository.GetAllCompanies()
                    .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                    .ToList() // Populates the dropdown list with company names
            };

            return View(locationEditViewModel); // Returns the LocationEdit view with the model
        }

        // POST action method for handling updates to a location
        [HttpPost] // HTTP POST method
        [Authorize(Policy = "SuperAdmin")] // Requires Super Admin role
        public IActionResult LocationEdit(LocationEditViewModel model)
        {
            if (ModelState.IsValid) // Checks if the model state is valid
            {
                // Retrieve the location from the repository and update its details
                Location location = locationRepository.GetLocation(model.Id); // Retrieves the location to be updated
                location.Name = model.Name;
                location.Road = model.Road;
                location.RoadSection = model.RoadSection;
                location.Long = model.Longitude;
                location.Lat = model.Latitude;
                location.TimerLenght = model.TimerLenght;
                location.MapZoom = model.MapZoom;
                location.CompanyId = model.CompanyId;

                // Update the location in the repository
                locationRepository.Update(location); // Updates the location in the repository

                // Redirect to the location details page
                return RedirectToAction("LocationDetails", new { id = location.Id }); // Redirects to the details of the updated location
            }

            // Reload the companies dropdown list if model state is invalid
            return View(model); // Returns the LocationEdit view with the model
        }

        // POST action method for deleting a location
        [HttpPost] // HTTP POST method
        [Authorize(Policy = "SuperAdmin")] // Requires Super Admin role
        public IActionResult LocationDelete(int id)
        {
            // Delete the location from the repository
            Location location = locationRepository.GetLocation(id); // Retrieves the location to be deleted
            locationRepository.Delete(id); // Deletes the location from the repository

            // Redirect to the list of locations
            return RedirectToAction("ListLocations"); // Redirects to the list of locations
        }
    }
}
