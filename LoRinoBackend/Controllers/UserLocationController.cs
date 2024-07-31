using LoRinoBackend.Hubs; // Importing namespace for SignalR hubs
using LoRinoBackend.Models; // Importing namespace for models
using Microsoft.AspNetCore.Identity; // Importing namespace for identity management
using Microsoft.AspNetCore.Mvc; // Importing namespace for MVC framework
using Microsoft.AspNetCore.Mvc.Rendering; // Importing namespace for rendering select lists
using Microsoft.AspNetCore.SignalR; // Importing namespace for SignalR
using System; // Importing namespace for system utilities
using System.Collections.Generic; // Importing namespace for generic collections
using System.Linq; // Importing namespace for LINQ
using System.Threading.Tasks; // Importing namespace for asynchronous programming

namespace LoRinoBackend.Controllers
{
    public class UserLocationController : Controller
    {
        // Dependencies injected into the controller
        private readonly ILocationRepository _locationRepository; // Interface for location repository
        private readonly UserManager<ApplicationUser> _userManager; // User manager for identity management
        private readonly IAlarmSoundRepository _alarmSoundRepository; // Interface for alarm sound repository

        // Constructor to initialize the dependencies
        public UserLocationController(ILocationRepository locationRepository, UserManager<ApplicationUser> userManager, IAlarmSoundRepository alarmSoundRepository)
        {
            _locationRepository = locationRepository; // Initializing location repository
            _userManager = userManager; // Initializing user manager
            _alarmSoundRepository = alarmSoundRepository; // Initializing alarm sound repository
        }

        // Action method to display the list of locations for a specific user
        public IActionResult Index(string userId)
        {
            // Get the ID of the currently logged-in user
            var currentUserId = _userManager.GetUserId(HttpContext.User);
            // Activate sound alarm for the current user
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(currentUserId);

            // Query to get the list of locations associated with the user
            var userlocations = (from cu in _locationRepository.GetAllLocationUsers()
                                 where cu.UserId == userId
                                 select new LocationUser
                                 {
                                     Id = cu.Id,
                                     UserId = cu.UserId,
                                     UserName = (from u in _userManager.Users where u.Id == cu.UserId select u.UserName).FirstOrDefault(),
                                     LocationId = cu.LocationId,
                                     LocationName = (from c in _locationRepository.GetAllLocations() where c.Id == cu.LocationId select c.Name).FirstOrDefault()
                                 }).ToList();

            // Pass userId to the view
            ViewBag.UserId = userId;

            return View(userlocations); // Return the view with the list of user locations
        }

        // Action method to render the Create view for adding a new location to a user
        public IActionResult Create(string userId)
        {
            // Get the ID of the currently logged-in user
            var currentUserId = _userManager.GetUserId(HttpContext.User);
            // Activate sound alarm for the current user
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(currentUserId);
            // Pass userId to the view
            ViewBag.UserId = userId;
            // Pass a list of locations to the view
            ViewBag.Locations = GetLocations();

            return View(); // Return the Create view
        }

        // Action method to handle the POST request for creating a new user location
        [HttpPost]
        public IActionResult Create(LocationUser userLocation)
        {
            if (ModelState.IsValid) // Check if the model state is valid
            {
                _locationRepository.CreateLocationUser(userLocation); // Add the new location to the user
                return RedirectToAction(nameof(Index), new { userId = userLocation.UserId }); // Redirect to the Index view
            }
            return View(userLocation); // Return the view with validation errors
        }

        // Action method to render the Edit view for updating an existing user location
        public IActionResult Edit(int id)
        {
            var userLocation = _locationRepository.GetLocationUserById(id); // Get the user location by ID
            var currentUserId = _userManager.GetUserId(HttpContext.User);
            // Activate sound alarm for the current user
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(currentUserId);
            // Pass a list of locations to the view
            ViewBag.Locations = GetLocations();

            return View(userLocation); // Return the Edit view with the user location data
        }

        // Action method to handle the POST request for updating an existing user location
        [HttpPost]
        public IActionResult Edit(LocationUser userLocation)
        {
            if (ModelState.IsValid) // Check if the model state is valid
            {
                _locationRepository.UpdateLocationUser(userLocation); // Update the user location

                return RedirectToAction(nameof(Index), new { userId = userLocation.UserId }); // Redirect to the Index view
            }

            return View(userLocation); // Return the view with validation errors
        }

        // Action method to render the Delete view for confirming the deletion of a user location
        public IActionResult Delete(int id)
        {
            var currentUserId = _userManager.GetUserId(HttpContext.User);
            // Activate sound alarm for the current user
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(currentUserId);
            var userlocation = (from cu in _locationRepository.GetAllLocationUsers()
                                where cu.Id == id
                                select new LocationUser
                                {
                                    Id = cu.Id,
                                    UserId = cu.UserId,
                                    UserName = (from u in _userManager.Users where u.Id == cu.UserId select u.UserName).FirstOrDefault(),
                                    LocationId = cu.LocationId,
                                    LocationName = (from c in _locationRepository.GetAllLocations() where c.Id == cu.LocationId select c.Name).FirstOrDefault()
                                }).FirstOrDefault();

            return View(userlocation); // Return the Delete view with the user location data
        }

        // Action method to handle the POST request for deleting a user location
        [HttpPost]
        public IActionResult DeleteUserLocation(int id)
        {
            var userLocation = _locationRepository.GetLocationUserById(id); // Get the user location by ID
            _locationRepository.DeleteLocationUser(id); // Delete the user location

            return RedirectToAction(nameof(Index), new { userId = userLocation.UserId }); // Redirect to the Index view
        }

        // Private method to get a list of locations for dropdown lists
        private List<SelectListItem> GetLocations()
        {
            return _locationRepository.GetAllLocations().Select(c => new SelectListItem()
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
        }

        // Private method (commented out) to get a list of users for dropdown lists
        //private List<SelectListItem> GetUsers()
        //{
        //    return _userManager.Users.Select(c => new SelectListItem()
        //    {
        //        Value = c.Id.ToString(),
        //        Text = c.UserName
        //    }).ToList();
        //}
    }
}
