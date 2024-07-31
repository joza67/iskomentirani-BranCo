using LoRinoBackend.Hubs; // Importing the namespace for Hubs
using LoRinoBackend.Models; // Importing the namespace for Models
using LoRinoBackend.ViewModels; // Importing the namespace for ViewModels
using Microsoft.AspNetCore.Identity; // Importing the namespace for Identity
using Microsoft.AspNetCore.Mvc; // Importing the namespace for MVC
using Microsoft.AspNetCore.SignalR; // Importing the namespace for SignalR
using Microsoft.EntityFrameworkCore; // Importing the namespace for Entity Framework Core
using System; // Importing the System namespace
using System.Linq; // Importing the Linq namespace

namespace LoRinoBackend.Controllers
{
    // Controller for managing Movee Tags
    public class MoveeTagController : Controller
    {
        // Dependencies for interacting with repositories and user management
        private readonly IMoveeDataRepository _moveeDataRepository;
        private readonly IMoveeEventRepository _moveeEventRepository;
        private readonly IMoveeEventTagRepository _moveeEventTagRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IAlarmSoundRepository _alarmSoundRepository;

        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor for dependency injection
        public MoveeTagController(IHubContext<WanesyHub> hubContext,
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

        // Returns a JSON object with an error code and description
        public IActionResult returnError(int errorCode, String errorData)
        {
            return Json(new ReturnError { ErrorCode = errorCode, ErrorDescription = errorData });
        }

        // GET action method to display the list of Movee Tags
        [HttpGet]
        public IActionResult Index()
        {
            // Retrieve the current user ID
            var userId = _userManager.GetUserId(HttpContext.User);

            // Fetch the user including their company information
            ApplicationUser user = _userManager.Users.Include(x => x.Company)
                .SingleOrDefaultAsync(x => x.Id == userId).Result;

            // Activate alarm sound based on the current user ID
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(userId);

            // Check if the user has the "Super Admin" role
            if (User.IsInRole("Super Admin"))
            {
                // Return all Movee Tags if the user is a Super Admin
                return View(_moveeEventTagRepository.GetMoveeTags());
            }

            // Return only active Movee Tags associated with the user's company
            return View(_moveeEventTagRepository.GetActiveMoveeTags().Where(a => a.CompanyId == user.Company.Id));
        }

        // GET action method to display the form for creating a new Movee Tag
        [HttpGet]
        public IActionResult Create()
        {
            // Retrieve the current user ID
            var userId = _userManager.GetUserId(HttpContext.User);

            // Activate alarm sound based on the current user ID
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(userId);

            // Pass a flag to indicate that the tag will be created as active
            ViewBag.Active = true;

            return View();
        }

        // POST action method to handle the submission of a new Movee Tag
        [HttpPost]
        public IActionResult Create(MoveeTagCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the current user ID
                var userId = _userManager.GetUserId(HttpContext.User);

                // Fetch the user including their company information
                ApplicationUser user = _userManager.Users.Include(x => x.Company)
                    .SingleOrDefaultAsync(x => x.Id == userId).Result;

                // Create a new Movee Tag based on the provided model
                MoveeTag moveeTag = new MoveeTag
                {
                    Name = model.Name,
                    Active = model.Active,
                    CompanyId = user.Company.Id
                };

                // Add the new Movee Tag to the repository
                _moveeEventTagRepository.CreateMoveeTag(moveeTag);

                // Redirect to the Index action to display the updated list of tags
                return RedirectToAction("Index");
            }

            // Return the view with the model if validation fails
            return View();
        }

        // GET action method to display the form for editing an existing Movee Tag
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Retrieve the current user ID
            var userId = _userManager.GetUserId(HttpContext.User);

            // Fetch the user including their company information
            ApplicationUser user = _userManager.Users.Include(x => x.Company)
                .SingleOrDefaultAsync(x => x.Id == userId).Result;

            // Activate alarm sound based on the current user ID
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(userId);

            // Fetch the tag to be edited based on the provided ID
            var tagForEdit = _moveeEventTagRepository.GetMoveeTagById(id);

            // Create a view model for the edit form
            MoveeTagEditViewModel moveeTagEditViewModel = new MoveeTagEditViewModel
            {
                Id = tagForEdit.Id,
                Name = tagForEdit.Name,
                Active = tagForEdit.Active,
                CompanyId = tagForEdit.CompanyId
            };

            return View(moveeTagEditViewModel);
        }

        // POST action method to handle the submission of an edited Movee Tag
        [HttpPost]
        public IActionResult Edit(MoveeTagEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the existing Movee Tag based on the provided ID
                MoveeTag moveeTag = _moveeEventTagRepository.GetMoveeTagById(model.Id);

                // Update the Movee Tag details
                moveeTag.Id = model.Id;
                moveeTag.Name = model.Name;
                moveeTag.Active = model.Active;
                moveeTag.CompanyId = model.CompanyId;

                // Update the Movee Tag in the repository
                _moveeEventTagRepository.UpdateMoveeTag(moveeTag);

                // Redirect to the Index action to display the updated list of tags
                return RedirectToAction("Index");
            }

            // Return the view with the model if validation fails
            return View(model);
        }

        // GET action method to display the confirmation view for deleting a Movee Tag
        public IActionResult Delete(int id)
        {
            // Retrieve the current user ID
            var userId = _userManager.GetUserId(HttpContext.User);

            // Activate alarm sound based on the current user ID
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(userId);

            // Fetch the tag to be deleted based on the provided ID
            var tag = _moveeEventTagRepository.GetMoveeTagById(id);

            return View(tag);
        }

        // POST action method to handle the deletion of a Movee Tag
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTag(MoveeTag moveeTag)
        {
            // Retrieve the Movee Tag to be deleted
            var tag = _moveeEventTagRepository.GetMoveeTagById(moveeTag.Id);

            // Delete the Movee Tag from the repository
            _moveeEventTagRepository.DeleteMoveeTag(moveeTag);

            // Redirect to the Index action to display the updated list of tags
            return RedirectToAction(nameof(Index));
        }

        // Uncommented code for an alternative Delete method (commented out)
        /*
        [HttpPost]
        [ActionName("Delete")]
        public IActionResult Delete(int id)
        {
            // Retrieve the Movee Tag by ID
            MoveeTag moveeTag = _moveeEventTagRepository.GetMoveeTagById(id);

            // Delete the Movee Tag from the repository
            _moveeEventTagRepository.DeleteMoveeTag(id);

            // Redirect to the Index action to display the updated list of tags
            return RedirectToAction("Index");
        }
        */
    }
}
