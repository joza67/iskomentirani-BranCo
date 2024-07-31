using LoRinoBackend.Hubs; // Provides hub functionality for SignalR
using LoRinoBackend.Models; // Contains models used in the application
using Microsoft.AspNetCore.Identity; // Provides classes for identity management
using Microsoft.AspNetCore.Mvc; // Provides classes for MVC controller base
using Microsoft.AspNetCore.Mvc.Rendering; // Provides classes for rendering HTML in MVC views
using Microsoft.AspNetCore.SignalR; // Provides classes for SignalR hubs
using System; // Provides base classes and fundamental types
using System.Collections.Generic; // Provides generic collection types
using System.Linq; // Provides LINQ functionality

namespace LoRinoBackend.Controllers
{
    // Controller responsible for managing event tags
    public class EventTagController : Controller
    {
        // Dependencies injected through the constructor
        private readonly IMoveeDataRepository _moveeDataRepository; // Repository for Movee data
        private readonly IMoveeEventRepository _moveeEventRepository; // Repository for Movee events
        private readonly IMoveeEventTagRepository _moveeEventTagRepository; // Repository for Movee event tags
        private readonly IDeviceRepository _deviceRepository; // Repository for devices
        private readonly ILocationRepository _locationRepository; // Repository for locations
        private readonly IAlarmSoundRepository _alarmSoundRepository; // Repository for alarm sounds
        private readonly UserManager<ApplicationUser> _userManager; // Manager for user-related functionality

        // Constructor to initialize dependencies
        public EventTagController(IHubContext<WanesyHub> hubContext,
                                  IMoveeDataRepository moveeDataRepository,
                                  IMoveeEventRepository moveeEventRepository,
                                  IMoveeEventTagRepository moveeEventTagRepository,
                                  IDeviceRepository deviceRepository,
                                  UserManager<ApplicationUser> userManager,
                                  ILocationRepository locationRepository,
                                  IAlarmSoundRepository alarmSoundRepository)
        {
            _moveeDataRepository = moveeDataRepository;
            _moveeEventRepository = moveeEventRepository;
            _moveeEventTagRepository = moveeEventTagRepository;
            _deviceRepository = deviceRepository;
            _userManager = userManager;
            _locationRepository = locationRepository;
            _alarmSoundRepository = alarmSoundRepository;
        }

        // Returns a JSON response with error details
        public IActionResult returnError(int errorCode, string errorData)
        {
            return Json(new ReturnError { ErrorCode = errorCode, ErrorDescription = errorData });
        }

        // Displays a list of event tags associated with a specific event
        public IActionResult Index(int eventId)
        {
            var currentUserId = _userManager.GetUserId(HttpContext.User); // Gets the current user's ID
            // Activates alarm sound for the current user
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(currentUserId);

            // Retrieves event tags for the given event ID
            string eId = HttpContext.Request.Query["eventId"].ToString(); // Retrieves the event ID from the query string
            var eventTags = (from et in _moveeEventTagRepository.GetAllMoveeEventTags() // Retrieves all event tags
                             where et.MoveeEventFrameId == eventId // Filters tags by the specified event ID
                             select new MoveeEventTag // Selects relevant properties for the view
                             {
                                 Id = et.Id,
                                 MoveeEventFrameId = et.MoveeEventFrameId,
                                 MoveeTagId = et.MoveeTagId,
                                 TagName = (from u in _moveeEventTagRepository.GetMoveeTags() where u.Id == et.MoveeTagId select u.Name).FirstOrDefault()
                             }).ToList();

            ViewBag.EventId = eId; // Sets the event ID in the ViewBag for use in the view
            return View(eventTags); // Returns the view with the list of event tags
        }

        // Displays the view for creating a new event tag
        public IActionResult Create(int eventId)
        {
            var currentUserId = _userManager.GetUserId(HttpContext.User); // Gets the current user's ID
            // Activates alarm sound for the current user
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(currentUserId);
            ViewBag.EventId = HttpContext.Request.Query["eventId"].ToString(); // Sets the event ID in the ViewBag
            ViewBag.Tags = GetTags(); // Retrieves a list of tags for selection
            return View(); // Returns the view for creating a new event tag
        }

        // Handles the POST request to create a new event tag
        [HttpPost]
        [ActionName("Create")] // Specifies that this method handles the "Create" action
        public IActionResult Create(MoveeEventTag moveeEventTag)
        {
            var userId = _userManager.GetUserId(HttpContext.User); // Gets the current user's ID
            DateTimeOffset now = DateTimeOffset.UtcNow; // Gets the current UTC time
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds(); // Converts the current time to Unix time in milliseconds

            if (ModelState.IsValid) // Checks if the model state is valid
            {
                ViewBag.EventId = moveeEventTag.MoveeEventFrameId; // Sets the event ID in the ViewBag
                _moveeEventTagRepository.CreateMoveeEventTag(moveeEventTag, userId, unixTimeMilliseconds); // Creates the event tag
                return RedirectToAction(nameof(Index), new { eventId = moveeEventTag.MoveeEventFrameId }); // Redirects to the index action with the event ID
            }
            return View(moveeEventTag); // Returns the view with the model if the model state is invalid
        }

        // Displays the view for editing an existing event tag
        public IActionResult Edit(int id)
        {
            var currentUserId = _userManager.GetUserId(HttpContext.User); // Gets the current user's ID
            // Activates alarm sound for the current user
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(currentUserId);
            var eventTag = _moveeEventTagRepository.GetMoveeEventTagById(id); // Retrieves the event tag by ID

            ViewBag.Tags = GetTags(); // Retrieves a list of tags for selection
            return View(eventTag); // Returns the view for editing the event tag
        }

        // Handles the POST request to update an existing event tag
        [HttpPost]
        public IActionResult Edit(MoveeEventTag eventTag)
        {
            if (ModelState.IsValid) // Checks if the model state is valid
            {
                _moveeEventTagRepository.UpdateMoveeEventTag(eventTag); // Updates the event tag
                return RedirectToAction(nameof(Index), new { eventId = eventTag.MoveeEventFrameId }); // Redirects to the index action with the event ID
            }
            return View(eventTag); // Returns the view with the model if the model state is invalid
        }

        // Displays the view for confirming the deletion of an event tag
        public IActionResult Delete(int id)
        {
            var currentUserId = _userManager.GetUserId(HttpContext.User); // Gets the current user's ID
            // Activates alarm sound for the current user
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(currentUserId);
            var eventTag = (from et in _moveeEventTagRepository.GetAllMoveeEventTags() // Retrieves all event tags
                            where et.Id == id // Filters tags by the specified ID
                            select new MoveeEventTag // Selects relevant properties for the view
                            {
                                Id = et.Id,
                                MoveeEventFrameId = et.MoveeEventFrameId,
                                MoveeTagId = et.MoveeTagId,
                                TagName = (from u in _moveeEventTagRepository.GetMoveeTags() where u.Id == et.MoveeTagId select u.Name).FirstOrDefault()
                            }).SingleOrDefault(); // Ensures that only one event tag is returned

            return View(eventTag); // Returns the view for confirming the deletion of the event tag
        }

        // Handles the POST request to delete an event tag
        [HttpPost]
        [ActionName("Delete")] // Specifies that this method handles the "Delete" action
        public IActionResult DeleteEventTag(int id)
        {
            var userId = _userManager.GetUserId(HttpContext.User); // Gets the current user's ID
            DateTimeOffset now = DateTimeOffset.UtcNow; // Gets the current UTC time
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds(); // Converts the current time to Unix time in milliseconds

            var eventTag = _moveeEventTagRepository.GetAllMoveeEventTags().FirstOrDefault(a => a.Id == id); // Retrieves the event tag by ID
            _moveeEventTagRepository.RemoveMoveeEventTag(eventTag, userId, unixTimeMilliseconds); // Removes the event tag

            return RedirectToAction(nameof(Index), new { eventId = eventTag.MoveeEventFrameId }); // Redirects to the index action with the event ID
        }

        // Retrieves a list of tags for use in dropdowns or selections
        private List<SelectListItem> GetTags()
        {
            return _moveeEventTagRepository.GetMoveeTags().Select(c => new SelectListItem()
            {
                Value = c.Id.ToString(), // Sets the value attribute of the option
                Text = c.Name // Sets the display text of the option
            }).ToList(); // Converts the IEnumerable to a List
        }
    }
}
