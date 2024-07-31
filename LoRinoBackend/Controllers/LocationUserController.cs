using LoRinoBackend.Hubs; // Importing necessary namespaces for the backend
using LoRinoBackend.Models; // Importing models used in the backend
using Microsoft.AspNetCore.Identity; // For managing user identities
using Microsoft.AspNetCore.Mvc; // For MVC controller functionalities
using Microsoft.AspNetCore.Mvc.Rendering; // For creating select lists
using Microsoft.AspNetCore.SignalR; // For real-time communication
using System; // For general-purpose functionalities
using System.Collections.Generic; // For handling collections
using System.Linq; // For LINQ functionalities
using System.Threading.Tasks; // For handling asynchronous tasks

namespace LoRinoBackend.Controllers // Defining the namespace for the controllers
{
    public class LocationUserController : Controller // The LocationUserController inherits from Controller
    {
        // Dependencies for location repository, user manager, and alarm sound repository
        private readonly ILocationRepository _locationRepository; // Repository for managing locations
        private readonly UserManager<ApplicationUser> _userManager; // Manager for handling user-related functionalities
        private readonly IAlarmSoundRepository _alarmSoundRepository; // Repository for managing alarm sounds

        // Constructor for dependency injection
        public LocationUserController(ILocationRepository locationRepositor, // Constructor to initialize dependencies
                                      UserManager<ApplicationUser> userManager,
                                      IAlarmSoundRepository alarmSoundRepository)
        {
            _locationRepository = locationRepositor; // Initializing the location repository
            _userManager = userManager; // Initializing the user manager
            _alarmSoundRepository = alarmSoundRepository; // Initializing the alarm sound repository
        }

        // GET action method for displaying the list of users for a specific location
        public IActionResult Index(int locationId)
        {
            // Get the current user ID
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user's ID

            // Activate alarm sound based on user ID
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(userId); // Activates alarm sound for the user

            // Retrieve users associated with the given location
            var locationUsers = (from cu in _locationRepository.GetAllLocationUsers()
                                 where cu.LocationId == locationId
                                 select new LocationUser
                                 {
                                     Id = cu.Id,
                                     LocationId = cu.LocationId,
                                     LocationName = (from c in _locationRepository.GetAllLocations()
                                                     where c.Id == cu.LocationId
                                                     select c.Name).FirstOrDefault(),
                                     UserId = cu.UserId,
                                     UserName = (from u in _userManager.Users
                                                 where u.Id == cu.UserId
                                                 select u.UserName).FirstOrDefault()
                                 }).ToList(); // Retrieves users associated with the given location

            // Pass the location ID to the view
            ViewBag.LocationId = locationId; // Sets the location ID in the view bag

            return View(locationUsers); // Returns the view with the list of location users
        }

        // GET action method for displaying the Create LocationUser view
        public IActionResult Create(int locationId)
        {
            // Get the current user ID
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user's ID

            // Activate alarm sound based on user ID
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(userId); // Activates alarm sound for the user

            // Pass the location ID and list of users to the view
            ViewBag.LocationId = locationId; // Sets the location ID in the view bag
            ViewBag.Users = GetUsers(); // Sets the list of users in the view bag

            return View(); // Returns the Create view
        }

        // POST action method for creating a new LocationUser
        [HttpPost] // Specifies that this action handles POST requests
        public IActionResult Create(LocationUser locationUser)
        {
            if (ModelState.IsValid) // Checks if the model state is valid
            {
                // Add the new LocationUser to the repository
                _locationRepository.CreateLocationUser(locationUser); // Adds the new LocationUser to the repository
                // Redirect to the Index action with the current location ID
                return RedirectToAction(nameof(Index), new { locationId = locationUser.LocationId }); // Redirects to the Index action
            }

            // Return the view with the model if validation fails
            return View(locationUser); // Returns the Create view with the model
        }

        // GET action method for displaying the Edit LocationUser view
        public IActionResult Edit(int id)
        {
            // Retrieve the LocationUser to be edited
            var locationUser = _locationRepository.GetLocationUserById(id); // Retrieves the LocationUser to be edited

            // Get the current user ID
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user's ID

            // Activate alarm sound based on user ID
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(userId); // Activates alarm sound for the user

            // Pass the list of users to the view
            ViewBag.Users = GetUsers(); // Sets the list of users in the view bag

            return View(locationUser); // Returns the Edit view with the LocationUser
        }

        // POST action method for updating a LocationUser
        [HttpPost] // Specifies that this action handles POST requests
        public IActionResult Edit(LocationUser locationUser)
        {
            if (ModelState.IsValid) // Checks if the model state is valid
            {
                // Update the LocationUser in the repository
                _locationRepository.UpdateLocationUser(locationUser); // Updates the LocationUser in the repository

                // Redirect to the Index action with the current location ID
                return RedirectToAction(nameof(Index), new { locationId = locationUser.LocationId }); // Redirects to the Index action
            }

            // Return the view with the model if validation fails
            return View(locationUser); // Returns the Edit view with the model
        }

        // GET action method for displaying the Delete LocationUser view
        public IActionResult Delete(int id)
        {
            // Get the current user ID
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user's ID

            // Activate alarm sound based on user ID
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(userId); // Activates alarm sound for the user

            // Retrieve the LocationUser to be deleted
            var locationUsers = (from cu in _locationRepository.GetAllLocationUsers()
                                 where cu.Id == id
                                 select new LocationUser
                                 {
                                     Id = cu.Id,
                                     LocationId = cu.LocationId,
                                     LocationName = (from c in _locationRepository.GetAllLocations()
                                                     where c.Id == cu.LocationId
                                                     select c.Name).FirstOrDefault(),
                                     UserId = cu.UserId,
                                     UserName = (from u in _userManager.Users
                                                 where u.Id == cu.UserId
                                                 select u.UserName).FirstOrDefault()
                                 }).SingleOrDefault(); // Retrieves the LocationUser to be deleted

            return View(locationUsers); // Returns the Delete view with the LocationUser
        }

        // POST action method for deleting a LocationUser
        [HttpPost] // Specifies that this action handles POST requests
        public IActionResult DeleteLocationUser(int id)
        {
            // Retrieve the LocationUser to be deleted
            var locationUser = _locationRepository.GetAllLocationUsers().FirstOrDefault(a => a.Id == id); // Retrieves the LocationUser to be deleted

            // Delete the LocationUser from the repository
            _locationRepository.DeleteLocationUser(id); // Deletes the LocationUser from the repository

            // Redirect to the Index action with the current location ID
            return RedirectToAction(nameof(Index), new { locationId = locationUser.LocationId }); // Redirects to the Index action
        }

        // Helper method for getting the list of users for dropdown list
        private List<SelectListItem> GetUsers()
        {
            return _userManager.Users.Select(c => new SelectListItem()
            {
                Value = c.Id.ToString(), // Sets the value to the user's ID
                Text = c.UserName // Sets the text to the user's username
            }).ToList(); // Returns the list of users as SelectListItem objects
        }
    }
}
