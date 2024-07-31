using LoRinoBackend.Models; // Importing necessary namespaces for the backend
using Microsoft.AspNetCore.Authorization; // For authorization functionalities
using Microsoft.AspNetCore.Http; // For accessing HTTP context
using Microsoft.AspNetCore.Identity; // For managing user identities
using Microsoft.AspNetCore.Mvc; // For MVC controller functionalities
using Microsoft.Extensions.Logging; // For logging
using System; // For general-purpose functionalities

namespace LoRinoBackend.Controllers // Defining the namespace for the controllers
{
    public class MoveeEventCommentController : Controller // The MoveeEventCommentController inherits from Controller
    {
        // Logger for the SQLMoveeEventRepository, UserManager for user-related operations,
        // IHttpContextAccessor to access HTTP context, and repository for movee event comments
        private readonly ILogger<SQLMoveeEventRepository> _logger; // Logger for MoveeEventRepository
        private readonly UserManager<ApplicationUser> _userManager; // UserManager for handling user-related operations
        private IHttpContextAccessor _httpContextAccessor; // For accessing HTTP context
        private IMoveeEventCommentRepository _moveeEventCommentRepository; // Repository for handling event comments
        private readonly IAlarmSoundRepository _alarmSoundRepository; // Repository for managing alarm sounds

        // Constructor for dependency injection
        public MoveeEventCommentController(IMoveeEventCommentRepository moveeEventCommentRepository,
                                           UserManager<ApplicationUser> userManager,
                                           IHttpContextAccessor httpContextAccessor,
                                           ILogger<SQLMoveeEventRepository> logger,
                                           IAlarmSoundRepository alarmSoundRepository)
        {
            _userManager = userManager; // Initializing UserManager
            _logger = logger; // Initializing Logger
            _httpContextAccessor = httpContextAccessor; // Initializing HttpContextAccessor
            _moveeEventCommentRepository = moveeEventCommentRepository; // Initializing MoveeEventCommentRepository
            _alarmSoundRepository = alarmSoundRepository; // Initializing AlarmSoundRepository
        }

        // GET action method to display all comments for a specific event
        public IActionResult Index(int eventId)
        {
            // Retrieve the current user ID
            var userId = _userManager.GetUserId(HttpContext.User); // Getting the current user ID

            // Activate alarm sound based on the current user ID
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(userId); // Activating alarm sound for the user

            // Get the current time in Unix time milliseconds
            DateTimeOffset now = DateTimeOffset.UtcNow; // Getting the current time
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds(); // Converting to Unix time in milliseconds

            // Pass event ID, user ID, and current time to the view
            ViewBag.EventId = eventId; // Passing event ID to the view
            ViewBag.CommentBy = userId; // Passing user ID to the view
            ViewBag.CommentTime = unixTimeMilliseconds; // Passing current time to the view

            // Retrieve and display all comments associated with the specified event ID
            var moveeComments = _moveeEventCommentRepository.GetAllCommentsByEventId(eventId); // Getting all comments for the event
            return View(moveeComments); // Returning the view with the comments
        }

        // GET action method to display the details of a specific comment
        public IActionResult CommentDetails(int id)
        {
            // Retrieve the current user ID
            var userId = _userManager.GetUserId(HttpContext.User); // Getting the current user ID

            // Activate alarm sound based on the current user ID
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(userId); // Activating alarm sound for the user

            // Retrieve the details of the specified comment
            var comment = _moveeEventCommentRepository.GetCommentDetails(id); // Getting the details of the comment
            return View(comment); // Returning the view with the comment details
        }

        // GET action method to display the form for creating a new comment
        [HttpGet]
        [Authorize] // Only authorized users can access this method
        public IActionResult CreateComment(int eventId)
        {
            // Retrieve the current user ID
            var userId = _userManager.GetUserId(HttpContext.User); // Getting the current user ID

            // Activate alarm sound based on the current user ID
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(userId); // Activating alarm sound for the user

            // Get the current time in Unix time milliseconds
            DateTimeOffset now = DateTimeOffset.UtcNow; // Getting the current time
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds(); // Converting to Unix time in milliseconds

            // Pass various details to the view for the create comment form
            ViewBag.Active = true; // Setting active status
            ViewBag.EventId = _httpContextAccessor.HttpContext.Request.Query["eventId"].ToString(); // Passing event ID from the query string
            ViewBag.CommentBy = userId; // Passing user ID to the view
            ViewBag.CommentTime = unixTimeMilliseconds; // Passing current time to the view

            return View(); // Returning the CreateComment view
        }

        // POST action method to handle the submission of a new comment
        [HttpPost]
        [Authorize] // Only authorized users can access this method
        public IActionResult CreateComment(MoveeEventComment moveeEventComment)
        {
            if (ModelState.IsValid) // Checking if the model state is valid
            {
                // Retrieve the current user ID
                var userId = _userManager.GetUserId(HttpContext.User); // Getting the current user ID

                // Get the current time in Unix time milliseconds
                DateTimeOffset now = DateTimeOffset.UtcNow; // Getting the current time
                long unixTimeMilliseconds = now.ToUnixTimeMilliseconds(); // Converting to Unix time in milliseconds

                // Create a new comment object
                MoveeEventComment newComment = new MoveeEventComment
                {
                    Comment = moveeEventComment.Comment, // Setting the comment text
                    EventCommentBy = userId, // Setting the user ID who made the comment
                    EventCommentTime = unixTimeMilliseconds, // Setting the time the comment was made
                    MoveeEventFrameId = moveeEventComment.MoveeEventFrameId, // Setting the event ID
                    Active = true // Setting the comment as active
                };

                // Add the new comment to the repository
                _moveeEventCommentRepository.AddComment(newComment); // Adding the new comment to the repository

                // Redirect to the Index action to display comments for the event
                return RedirectToAction("Index", new { eventId = moveeEventComment.MoveeEventFrameId }); // Redirecting to the Index action
            }

            // Return the view with the model if validation fails
            return View(); // Returning the view with the model if validation fails
        }

        // GET action method to display the form for editing an existing comment
        [HttpGet]
        [Authorize] // Only authorized users can access this method
        public ViewResult CommentEdit(int id)
        {
            // Retrieve the current user ID
            var userId = _userManager.GetUserId(HttpContext.User); // Getting the current user ID

            // Activate alarm sound based on the current user ID
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(userId); // Activating alarm sound for the user

            // Retrieve the comment details to be edited
            MoveeEventComment commentForUpdate = _moveeEventCommentRepository.GetCommentDetails(id); // Getting the comment details

            // Create a new comment object for the edit form
            MoveeEventComment newComment = new MoveeEventComment
            {
                Id = commentForUpdate.Id, // Setting the comment ID
                Comment = commentForUpdate.Comment, // Setting the comment text
                EventCommentBy = commentForUpdate.EventCommentBy, // Setting the user ID who made the comment
                EventCommentTime = commentForUpdate.EventCommentTime, // Setting the time the comment was made
                MoveeEventFrameId = commentForUpdate.MoveeEventFrameId // Setting the event ID
            };

            return View(newComment); // Returning the view with the comment details for editing
        }

        // POST action method to handle the submission of an edited comment
        [HttpPost]
        [Authorize] // Only authorized users can access this method
        public IActionResult CommentEdit(MoveeEventComment moveeEventComment)
        {
            if (ModelState.IsValid) // Checking if the model state is valid
            {
                // Retrieve the current user ID
                var userId = _userManager.GetUserId(HttpContext.User); // Getting the current user ID

                // Get the current time in Unix time milliseconds
                DateTimeOffset now = DateTimeOffset.UtcNow; // Getting the current time
                long unixTimeMilliseconds = now.ToUnixTimeMilliseconds(); // Converting to Unix time in milliseconds

                // Retrieve the existing comment to be updated
                MoveeEventComment comment = _moveeEventCommentRepository.GetCommentDetails(moveeEventComment.Id); // Getting the comment details

                // Update the comment object with new details
                comment.Id = moveeEventComment.Id; // Updating the comment ID
                comment.Comment = moveeEventComment.Comment; // Updating the comment text
                comment.EventCommentBy = userId; // Updating the user ID who made the comment
                comment.EventCommentTime = unixTimeMilliseconds; // Updating the time the comment was made
                comment.MoveeEventFrameId = moveeEventComment.MoveeEventFrameId; // Updating the event ID

                // Update the comment in the repository
                _moveeEventCommentRepository.Update(comment); // Updating the comment in the repository

                // Redirect to the CommentDetails action to view the updated comment
                return RedirectToAction("CommentDetails", new { id = comment.Id }); // Redirecting to the CommentDetails action
            }

            // Return the view with the model if validation fails
            return View(); // Returning the view with the model if validation fails
        }

        // POST action method to handle the deletion of a comment
        [HttpPost]
        [Authorize] // Only authorized users can access this method
        public IActionResult CommentDelete(int id)
        {
            int eventId = 0; // Initializing eventId to 0
            if (id != 0) // Checking if the comment ID is not 0
            {
                // Retrieve the comment to be deleted
                MoveeEventComment moveeEventComment = _moveeEventCommentRepository.GetCommentDetails(id); // Getting the comment details
                eventId = moveeEventComment.MoveeEventFrameId; // Setting the event ID

                // Delete the comment from the repository
                _moveeEventCommentRepository.DeleteComment(id); // Deleting the comment from the repository
            }

            // Redirect to the Index action to display comments for the event
            return RedirectToAction("Index", new { eventId = eventId }); // Redirecting to the Index action
        }
    }
}
