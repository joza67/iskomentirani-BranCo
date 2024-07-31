using Microsoft.AspNetCore.Authorization; // Provides authorization attributes to secure the controller and its actions.
using Microsoft.AspNetCore.Http; // Provides access to HTTP context, including requests and responses.
using Microsoft.AspNetCore.Identity; // Provides user management and authentication features, including UserManager.
using Microsoft.AspNetCore.Mvc; // Provides the base class for MVC controllers and action results.
using System; // Provides fundamental classes and base classes that define commonly-used value and reference data types.
using System.Collections.Generic; // Provides classes that define generic collections.
using System.Linq; // Provides classes and interfaces that support querying of data structures.

namespace LoRinoBackend.Controllers
{
    [Authorize] // Ensures that the entire controller requires authentication for access.
    public class MVController : Controller
    {
        // Dependency injection for various repositories and services.
        private readonly IMoveeDataRepository _moveeDataRepository; // Repository for accessing movee data.
        private readonly IMoveeEventRepository _moveeEventRepository; // Repository for accessing movee events.
        private readonly IMoveeEventTagRepository _moveeEventTagRepository; // Repository for accessing event tags.
        private readonly IDeviceRepository _deviceRepository; // Repository for accessing device data.
        private readonly ILocationRepository _locationRepository; // Repository for accessing location data.
        private readonly IAlarmSoundRepository _alarmSoundRepository; // Repository for managing alarm sounds.
        private readonly IHttpContextAccessor _httpContextAccessor; // Provides access to the current HTTP context.
        private readonly IMoveeEventCommentRepository _moveeEventCommentRepository; // Repository for managing event comments.
        private readonly UserManager<ApplicationUser> _userManager; // Manages user-related operations.
        private readonly int _deviceType; // Default device type, set to 2.

        // Constructor to initialize dependencies via dependency injection.
        public MVController(
            IMoveeDataRepository moveeDataRepository,
            IMoveeEventRepository moveeEventRepository,
            IMoveeEventTagRepository moveeEventTagRepository,
            IDeviceRepository deviceRepository,
            UserManager<ApplicationUser> userManager,
            ILocationRepository locationRepository,
            IHttpContextAccessor httpContextAccessor,
            IMoveeEventCommentRepository moveeEventCommentRepository,
            IAlarmSoundRepository alarmSoundRepository
        )
        {
            _moveeDataRepository = moveeDataRepository; // Assigns the movee data repository.
            _moveeEventRepository = moveeEventRepository; // Assigns the movee event repository.
            _moveeEventTagRepository = moveeEventTagRepository; // Assigns the event tag repository.
            _deviceRepository = deviceRepository; // Assigns the device repository.
            _userManager = userManager; // Assigns the user manager for user operations.
            _deviceType = 2; // Default device type, can be changed later if needed.
            _locationRepository = locationRepository; // Assigns the location repository.
            _httpContextAccessor = httpContextAccessor; // Assigns the HTTP context accessor.
            _moveeEventCommentRepository = moveeEventCommentRepository; // Assigns the event comment repository.
            _alarmSoundRepository = alarmSoundRepository; // Assigns the alarm sound repository.
        }

        // Method to return a JSON response with error details.
        public IActionResult ReturnError(int errorCode, string errorData)
        {
            return Json(new ReturnError { ErrorCode = errorCode, ErrorDescription = errorData }); // Creates a JSON object with error information.
        }

        // Action method to display the index view with the latest movee data.
        public ActionResult Index()
        {
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user ID from the HTTP context.
            var locationUsers = _locationRepository.GetLocationUsersByUserId(userId); // Retrieves the list of users associated with the current user's location.

            // Fetches user details including company information asynchronously.
            ApplicationUser user = _userManager.Users.Include(x => x.Company).SingleOrDefaultAsync(x => x.Id == userId).Result;

            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(user.Id); // Activates the alarm sound for the user.
            var _companyId = user.Company.Id; // Gets the user's company ID.
            bool isAdmin = false; // Initializes the isAdmin flag.

            // Determine if the user is a "Super Admin".
            if (User.IsInRole("Super Admin"))
            {
                _companyId = -1; // If super admin, sets company ID to -1 to access all data.
            }

            // Determine if the user is an "Admin".
            if (User.IsInRole("Admin"))
            {
                isAdmin = true; // Sets isAdmin flag to true.
            }

            // Retrieves the latest movee data based on device type, company ID, location users, and admin status.
            var model = _moveeDataRepository.GetLastData(_deviceType, _companyId, locationUsers, isAdmin);

            return View(model); // Returns the view with the model containing the latest data.
        }

        // HTTP GET method to display the events index with filters.
        [HttpGet]
        public IActionResult EventsIndex(string filter, string state, int? page, int? perPage, string tagIds)
        {
            int[] tagIdsArray = null; // Initializes the array for tag IDs.
            if (!string.IsNullOrEmpty(tagIds))
            {
                tagIdsArray = tagIds.Split(',').Select(int.Parse).ToArray(); // Parses the tag IDs from the string.
            }
            else
            {
                tagIdsArray = Array.Empty<int>(); // Defaults to an empty array if no tags are provided.
            }

            var queryString = Request.QueryString.ToString(); // Retrieves the query string from the request.
            if (queryString.Length > 0)
            {
                if (queryString == "?" || queryString == "?filter=")
                {
                    ViewBag.Card = false; // Sets the card view flag based on the query string.
                }
                else
                {
                    ViewBag.Card = true;
                }
            }
            else
            {
                ViewBag.Card = false;
            }
            perPage = 50; // Sets the default number of items per page.
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user ID.

            // Fetches user details including company information asynchronously.
            ApplicationUser user = _userManager.Users.Include(x => x.Company).SingleOrDefaultAsync(x => x.Id == userId).Result;

            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(user.Id); // Activates the alarm sound for the user.
            var locationsByUser = _locationRepository.GetLocationsByUserId(userId); // Retrieves the locations associated with the user.

            // Asynchronously checks if the user has a "Super Admin" role.
            var isSuperAdmin = _userManager.IsInRoleAsync(user, "Super Admin").Result;
            // Asynchronously checks if the user has an "Admin" role.
            var isAdmin = _userManager.IsInRoleAsync(user, "Admin").Result;
            var currentUserCompanyId = user.Company.Id; // Gets the user's company ID.

            int pageInput = 1; // Default page number.
            if (page.HasValue)
            {
                pageInput = page.Value; // Sets the page number if provided.
            }
            ViewBag.Page = pageInput;

            string filterInput = ""; // Default filter input.
            if (!string.IsNullOrEmpty(filter))
            {
                filterInput = filter; // Sets the filter input if provided.
            }
            ViewBag.Filter = filter;

            int perPageInput = 1000; // Default items per page.
            if (perPage.HasValue && perPage > 0)
            {
                perPageInput = perPage.Value; // Sets items per page if provided.
            }
            ViewBag.PerPage = perPage;

            ViewBag.EventState = state ?? "all"; // Sets the event state, defaults to "all" if not provided.

            // Retrieves all tags for the dropdown list based on the user's company ID.
            ViewBag.Tags = _moveeEventTagRepository.AllTagsForDropDownList(user.Company.Id);
            List<MoveeEventFrame> events = new(); // Initializes the list for events.
            List<MoveeEventFrame> allevents = new(); // Initializes the list for all events.

            if (isSuperAdmin)
            {
                // Retrieves events with the specified filter and pagination options for super admins.
                events = _moveeEventRepository.QueryStringFilter(filterInput, perPageInput, pageInput, false, tagIdsArray, 0);
            }
            else if (isAdmin)
            {
                // Retrieves events with the specified filter and pagination options for admins.
                events = _moveeEventRepository.QueryStringFilter(filterInput, perPageInput, pageInput, false, tagIdsArray, user.Company.Id);
            }
            else if (locationsByUser.Count == 0)
            {
                return View("AccessDenied"); // Returns access denied if the user has no associated locations.
            }
            else
            {
                // Retrieves events for non-admin users and filters based on locations.
                events = _moveeEventRepository.QueryStringFilter(filterInput, perPageInput, pageInput, false, tagIdsArray, 0);
                events = (from l in locationsByUser
                          join e in events on l.Id equals e.LocationId
                          select e).ToList();
            }

            if (filter != null || perPage != 0 || tagIdsArray != null)
            {
                if (isSuperAdmin)
                {
                    // Retrieves all events for super admins based on the filter, ordered by ID.
                    allevents = _moveeEventRepository.QueryStringFilter(filterInput, 0, 0, true, tagIdsArray, 0).OrderByDescending(a => a.Id).Where(a => a.Id != 1).ToList();
                }
                else if (isAdmin)
                {
                    // Retrieves all events for admins based on the filter, ordered by ID.
                    events = _moveeEventRepository.QueryStringFilter(filterInput, 0, 0, true, tagIdsArray, user.Company.Id).OrderByDescending(a => a.Id).Where(a => a.Id != 1).ToList();
                }
                else
                {
                    // Retrieves all events for non-admin users based on the filter, ordered by ID.
                    allevents = _moveeEventRepository.QueryStringFilter(filterInput, 0, 0, true, tagIdsArray, 0).OrderByDescending(a => a.Id).Where(a => a.Id != 1).ToList();
                    allevents = (from l in locationsByUser
                                 join e in events on l.Id equals e.LocationId
                                 select e).ToList();
                }

                if (allevents != null)
                {
                    int recordCount = allevents.Count; // Counts the total number of events.
                    int totalPageCount = recordCount / perPageInput; // Calculates the total number of pages.
                    if (recordCount % perPageInput > 0)
                    {
                        totalPageCount++; // Increments the total page count if there are remaining events.
                    }

                    ViewBag.TotalPageCount = totalPageCount; // Sets the total page count.
                }
            }

            ViewBag.EventCount = allevents.Count; // Sets the total event count.

            // Filters events based on the specified state.
            if (state == "active")
            {
                events = events.Where(e => e.IsAcked == false && e.IsCleared == false).ToList();
            }
            else if (state == "acked")
            {
                events = events.Where(e => e.IsAcked == true && e.IsCleared == false).ToList();
            }
            else if (state == "cleared")
            {
                events = events.Where(e => e.IsAcked == true && e.IsCleared == true).ToList();
            }

            return View(events); // Returns the view with the filtered events.
        }

        // HTTP GET method to return a partial view of events based on filters.
        [HttpGet]
        public IActionResult EventsIndexPartial(string filter, string state, int? page, int? perPage, string tagIds)
        {
            int[] tagIdsArray = null; // Initializes the array for tag IDs.
            if (!string.IsNullOrEmpty(tagIds))
            {
                tagIdsArray = tagIds.Split(',').Select(int.Parse).ToArray(); // Parses the tag IDs from the string.
            }
            else
            {
                tagIdsArray = Array.Empty<int>(); // Defaults to an empty array if no tags are provided.
            }

            var queryString = Request.QueryString.ToString(); // Retrieves the query string from the request.
            if (queryString.Length > 0)
            {
                if (queryString == "?" || queryString == "?filter=")
                {
                    ViewBag.Card = false; // Sets the card view flag based on the query string.
                }
                else
                {
                    ViewBag.Card = true;
                }
            }
            else
            {
                ViewBag.Card = false;
            }
            perPage = 50; // Sets the default number of items per page.
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user ID.

            // Fetches user details including company information asynchronously.
            ApplicationUser user = _userManager.Users.Include(x => x.Company).SingleOrDefaultAsync(x => x.Id == userId).Result;

            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(user.Id); // Activates the alarm sound for the user.
            var locationsByUser = _locationRepository.GetLocationsByUserId(userId); // Retrieves the locations associated with the user.

            // Asynchronously checks if the user has a "Super Admin" role.
            var isSuperAdmin = _userManager.IsInRoleAsync(user, "Super Admin").Result;
            // Asynchronously checks if the user has an "Admin" role.
            var isAdmin = _userManager.IsInRoleAsync(user, "Admin").Result;
            var currentUserCompanyId = user.Company.Id; // Gets the user's company ID.

            int pageInput = 1; // Default page number.
            if (page.HasValue)
            {
                pageInput = page.Value; // Sets the page number if provided.
            }
            ViewBag.Page = pageInput;

            string filterInput = ""; // Default filter input.
            if (!string.IsNullOrEmpty(filter))
            {
                filterInput = filter; // Sets the filter input if provided.
            }
            ViewBag.Filter = filter;

            int perPageInput = 1000; // Default items per page.
            if (perPage.HasValue && perPage > 0)
            {
                perPageInput = perPage.Value; // Sets items per page if provided.
            }
            ViewBag.PerPage = perPage;

            ViewBag.EventState = state ?? "all"; // Sets the event state, defaults to "all" if not provided.

            // Retrieves all tags for the dropdown list based on the user's company ID.
            ViewBag.Tags = _moveeEventTagRepository.AllTagsForDropDownList(user.Company.Id);
            List<MoveeEventFrame> events = new(); // Initializes the list for events.
            List<MoveeEventFrame> allevents = new(); // Initializes the list for all events.

            if (isSuperAdmin)
            {
                // Retrieves events with the specified filter and pagination options for super admins.
                events = _moveeEventRepository.QueryStringFilter(filterInput, perPageInput, pageInput, false, tagIdsArray, 0);
            }
            else if (isAdmin)
            {
                // Retrieves events with the specified filter and pagination options for admins.
                events = _moveeEventRepository.QueryStringFilter(filterInput, perPageInput, pageInput, false, tagIdsArray, user.Company.Id);
            }
            else if (locationsByUser.Count == 0)
            {
                return View("AccessDenied"); // Returns access denied if the user has no associated locations.
            }
            else
            {
                // Retrieves events for non-admin users and filters based on locations.
                events = _moveeEventRepository.QueryStringFilter(filterInput, perPageInput, pageInput, false, tagIdsArray, 0);
                events = (from l in locationsByUser
                          join e in events on l.Id equals e.LocationId
                          select e).ToList();
            }

            if (filter != null || perPage != 0 || tagIdsArray != null)
            {
                if (isSuperAdmin)
                {
                    // Retrieves all events for super admins based on the filter, ordered by ID.
                    allevents = _moveeEventRepository.QueryStringFilter(filterInput, 0, 0, true, tagIdsArray, 0).OrderByDescending(a => a.Id).Where(a => a.Id != 1).ToList();
                }
                else if (isAdmin)
                {
                    // Retrieves all events for admins based on the filter, ordered by ID.
                    events = _moveeEventRepository.QueryStringFilter(filterInput, 0, 0, true, tagIdsArray, user.Company.Id).OrderByDescending(a => a.Id).Where(a => a.Id != 1).ToList();
                }
                else
                {
                    // Retrieves all events for non-admin users based on the filter, ordered by ID.
                    allevents = _moveeEventRepository.QueryStringFilter(filterInput, 0, 0, true, tagIdsArray, 0).OrderByDescending(a => a.Id).Where(a => a.Id != 1).ToList();
                    allevents = (from l in locationsByUser
                                 join e in events on l.Id equals e.LocationId
                                 select e).ToList();
                }

                if (allevents != null)
                {
                    int recordCount = allevents.Count; // Counts the total number of events.
                    int totalPageCount = recordCount / perPageInput; // Calculates the total number of pages.
                    if (recordCount % perPageInput > 0)
                    {
                        totalPageCount++; // Increments the total page count if there are remaining events.
                    }

                    ViewBag.TotalPageCount = totalPageCount; // Sets the total page count.
                }
            }

            ViewBag.EventCount = allevents.Count; // Sets the total event count.

            // Filters events based on the specified state.
            if (state == "active")
            {
                events = events.Where(e => e.IsAcked == false && e.IsCleared == false).ToList();
            }
            else if (state == "acked")
            {
                events = events.Where(e => e.IsAcked == true && e.IsCleared == false).ToList();
            }
            else if (state == "cleared")
            {
                events = events.Where(e => e.IsAcked == true && e.IsCleared == true).ToList();
            }

            return PartialView(events); // Returns the partial view with the filtered events.
        }

        // Returns all movee data in JSON format.
        public IActionResult GetAllData(string Id, string s)
        {
            var model = _moveeDataRepository.GetAllData(); // Retrieves all movee data.
            return Json(model); // Returns the data in JSON format.
        }

        [Authorize(Policy = "SuperAdmin")] // Authorizes only super admins to access this method.
        public IActionResult EventList(string Id, int s)
        {
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user ID.
            ApplicationUser user = _userManager.Users.Include(x => x.Company).SingleOrDefaultAsync(x => x.Id == userId).Result; // Retrieves user details asynchronously.
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(user.Id); // Activates the alarm sound for the user.
            var companyId = user.Company.Id; // Gets the user's company ID.

            ViewBag.Page = s; // Sets the current page number.

            return View(_moveeDataRepository.GetEventDatasAdmin(Id, s)); // Returns the view with event data for admins.
        }

        // Returns event data in JSON format.
        public IActionResult EventListJson(string Id, int s)
        {
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user ID.
            ApplicationUser user = _userManager.Users.Include(x => x.Company).SingleOrDefaultAsync(x => x.Id == userId).Result; // Retrieves user details asynchronously.
            var companyId = user.Company.Id; // Gets the user's company ID.
            var model = _moveeDataRepository.GetEventDatas(Id, s, companyId); // Retrieves event data based on the specified parameters.
            return Json(model); // Returns the data in JSON format.
        }

        // Returns a view with the list of devices.
        public IActionResult GetDevices()
        {
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user ID.

            ApplicationUser user = _userManager.Users.Include(x => x.Company).SingleOrDefaultAsync(x => x.Id == userId).Result; // Retrieves user details asynchronously.
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(user.Id); // Activates the alarm sound for the user.
            var _companyId = user.Company.Id; // Gets the user's company ID.
            var locationUsers = _locationRepository.GetLocationUsersByUserId(userId).ToList(); // Retrieves the list of location users.

            bool isAdmin = false; // Initializes the isAdmin flag.

            // Determines if the user is a "Super Admin".
            if (User.IsInRole("Super Admin"))
            {
                _companyId = -1; // Sets the company ID to -1 for super admins.
            }

            // Determines if the user is an "Admin".
            if (User.IsInRole("Admin"))
            {
                isAdmin = true; // Sets the isAdmin flag to true.
            }

            // Retrieves the latest movee data based on the device type, company ID, location users, and admin status.
            var model = _moveeDataRepository.GetLastData(_deviceType, _companyId, locationUsers, isAdmin);

            return View(model); // Returns the view with the list of devices.
        }

        // Returns specific data by ID in JSON format.
        public IActionResult GetData(string id)
        {
            var model = _moveeDataRepository.GetData(id); // Retrieves data based on the specified ID.
            return Json(model); // Returns the data in JSON format.
        }

        [AllowAnonymous] // Allows access to this method without authentication.
        public IActionResult GetCurrentStatus(string Id)
        {
            var model = _moveeDataRepository.GetCurrentStatus(Id); // Retrieves the current status based on the specified ID.
            if (model == null)
            {
                Response.StatusCode = 201; // Sets the status code to 201 (No Content).
                return ReturnError(204, "No Content"); // Returns an error response indicating no content.
            }
            return Json(model); // Returns the current status in JSON format.
        }

        // HTTP POST method to set acknowledgment for a single movee data frame.
        [HttpPost]
        public IActionResult SetAckSingle(MoveeDataFrame moveeData)
        {
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user ID.
            DateTimeOffset now = DateTimeOffset.UtcNow; // Gets the current time in UTC.
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds(); // Converts the current time to Unix time in milliseconds.

            MoveeDataFrame _moveeData = _moveeDataRepository.GetSingleData(moveeData.Id); // Retrieves the specific movee data based on ID.

            // Updates the acknowledgment message, ID, and time.
            _moveeData.AckMsg = moveeData.AckMsg;
            _moveeData.AckId = userId;
            _moveeData.AckTime = unixTimeMilliseconds;

            _moveeDataRepository.Update(_moveeData); // Updates the movee data in the repository.

            this.HttpContext.Response.StatusCode = 201; // Sets the status code to 201 (Created).
            return Json(_moveeData); // Returns the updated data in JSON format.
        }

        // HTTP POST method to set acknowledgment for multiple movee data frames.
        [HttpPost]
        public IActionResult setAck(string Id)
        {
            List<MoveeDataFrame> _moveeData = new List<MoveeDataFrame>(); // Initializes a list to store movee data frames.
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user ID.
            DateTimeOffset now = DateTimeOffset.UtcNow; // Gets the current time in UTC.
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds(); // Converts the current time to Unix time in milliseconds.

            _moveeData = _moveeDataRepository.GetEventData(Id).ToList(); // Retrieves the list of movee data frames based on the specified ID.

            foreach (var data in _moveeData)
            {
                // Updates the acknowledgment message, ID, and time for each data frame.
                data.AckMsg = true;
                data.AckId = userId;
                data.AckTime = unixTimeMilliseconds;
                _moveeDataRepository.Update(data); // Updates the movee data in the repository.
            }

            this.HttpContext.Response.StatusCode = 201; // Sets the status code to 201 (Created).
            return Json(_moveeData); // Returns the updated list of movee data frames in JSON format.
        }

        // HTTP POST method to set acknowledgment for all movee data frames.
        [HttpPost]
        public IActionResult setAckAll([FromBody] List<int> ids)
        {
            List<MoveeDataFrame> frames = new List<MoveeDataFrame>(); // Initializes a list to store movee data frames.
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user ID.
            long unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // Converts the current time to Unix time in milliseconds.

            if (ids != null)
            {
                foreach (int id in ids)
                {
                    MoveeDataFrame _moveeData = new MoveeDataFrame(); // Initializes a new movee data frame.
                    _moveeData = _moveeDataRepository.GetSingleData(id); // Retrieves the specific movee data based on ID.
                    if (_moveeData != null)
                    {
                        // Updates the acknowledgment message, ID, and time for the data frame.
                        _moveeData.AckMsg = true;
                        _moveeData.AckId = userId;
                        _moveeData.AckTime = unixTimeMilliseconds;
                        _moveeDataRepository.Update(_moveeData); // Updates the movee data in the repository.
                        frames.Add(_moveeData); // Adds the updated data frame to the list.
                    }
                }
                this.HttpContext.Response.StatusCode = 201; // Sets the status code to 201 (Created).
            }
            this.HttpContext.Response.StatusCode = 204; // Sets the status code to 204 (No Content) if no data was updated.
            return Json(frames); // Returns the updated list of movee data frames in JSON format.
        }

        // HTTP POST method to get all active movee events.
        [HttpPost]
        public IActionResult getMoveeEvent()
        {
            var frames = _moveeEventRepository.GetAllActiveEvents(); // Retrieves all active movee events.

            this.HttpContext.Response.StatusCode = 204; // Sets the status code to 204 (No Content).
            return Json(frames); // Returns the list of active movee events in JSON format.
        }

        [AllowAnonymous] // Allows access to this method without authentication.
        public IActionResult GetLastHour(string Id)
        {
            var model = _moveeDataRepository.GetLastHour(Id); // Retrieves the last hour of data based on the specified ID.
            if (model == null)
            {
                Response.StatusCode = 404; // Sets the status code to 404 (Not Found).
                return ReturnError(404, "Data not found"); // Returns an error response indicating data not found.
            }
            return Json(model); // Returns the last hour of data in JSON format.
        }

        // Returns the last unread data in JSON format.
        public IActionResult GetLastUnreadData(string id)
        {
            var model = _moveeDataRepository.GetLastUnreadData(id); // Retrieves the last unread data based on the specified ID.
            if (model == null)
            {
                Response.StatusCode = 404; // Sets the status code to 404 (Not Found).
                return ReturnError(404, "Data not found"); // Returns an error response indicating data not found.
            }
            return Json(model); // Returns the last unread data in JSON format.
        }

        // HTTP GET method to display the details of a specific event based on the GUID.
        [HttpGet]
        public IActionResult EventsDetails(string guid)
        {
            var eventId = _moveeEventRepository.GetEventIdFromGuid(guid); // Retrieves the event ID based on the GUID.
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user ID.
            ViewBag.Alarm = _alarmSoundRepository.ActivateSoundAlarm(userId); // Activates the alarm sound for the user.

            // Retrieves the tags associated with the event ID.
            var myTags = _moveeEventTagRepository.GetAllMoveeEventTags().Where(a => a.MoveeEventFrameId == eventId);
            var deletedTags = _moveeEventTagRepository.GetMyDeletedTagLogs(eventId, userId).Distinct(); // Retrieves the deleted tags for the event.
            var addedTags = _moveeEventTagRepository.GetMyAddedTagLogs(eventId, userId).Distinct(); // Retrieves the added tags for the event.
            var excepted = addedTags.Except(deletedTags); // Calculates the tags that are added but not deleted.

            ApplicationUser user = _userManager.Users.Include(x => x.Company).SingleOrDefaultAsync(x => x.Id == userId).Result; // Retrieves user details asynchronously.

            var location = _moveeEventRepository.GetLocationFromEventId(eventId); // Retrieves the location associated with the event ID.
            List<Device> devicesFromLocation = new List<Device>(); // Initializes a list to store devices from the location.
            devicesFromLocation = _deviceRepository.GetAllDevicesFromLocationId(location.Id).ToList(); // Retrieves the devices based on the location ID.

            List<int> ints = new List<int>(); // Initializes a list to store the tag IDs.
            foreach (var item in myTags)
            {
                // Adds or removes tags based on the added and excepted tags.
                if (addedTags.Contains(item.MoveeTagId))
                {
                    ints.Add(item.MoveeTagId);
                }
                else if (!excepted.Contains(item.MoveeTagId))
                {
                    ints.Remove(item.MoveeTagId);
                }
            }

            ViewBag.Tags = _moveeEventTagRepository.TagsForDropDownList(ints, user.Company.Id); // Sets the tags for the dropdown list.

            var comments = _moveeEventCommentRepository.GetAllCommentsByEventId(eventId); // Retrieves all comments associated with the event ID.
            ViewBag.Comments = comments; // Sets the comments for the view.

            var moveeEvent = _moveeEventRepository.GetMoveeEventById(eventId); // Retrieves the movee event based on the event ID.
            MoveeEventFrameViewModel moveeEventFrameViewModel = new(); // Initializes a new view model for the movee event.

            var logs = _moveeEventCommentRepository.GetLogs(eventId); // Retrieves the logs associated with the event ID.
            moveeEventFrameViewModel.moveeEventFrame = moveeEvent; // Sets the movee event frame in the view model.
            moveeEventFrameViewModel.Devices = devicesFromLocation; // Sets the devices in the view model.
            moveeEventFrameViewModel.moveeDataFrames = _moveeDataRepository.GetAlarmsByEventId(eventId).ToList(); // Sets the movee data frames in the view model.
            moveeEventFrameViewModel.moveeEventFrame.EventAckTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // Sets the acknowledgment time.
            moveeEventFrameViewModel.cntAlarm = moveeEvent.AlarmCount; // Sets the alarm count.
            moveeEventFrameViewModel.MoveeTag = _moveeEventTagRepository.TagsList(); // Sets the tags list.
            moveeEventFrameViewModel.MoveeEventTag = myTags.Where(a => a.Active == true).ToList(); // Sets the active tags.
            moveeEventFrameViewModel.MoveeEventComments = comments; // Sets the event comments.
            moveeEventFrameViewModel.LogViewModel = logs.OrderByDescending(x => x.LogTime).ToList(); // Sets the logs in descending order of log time.

            return View(moveeEventFrameViewModel); // Returns the view with the event details.
        }

        // HTTP POST method to acknowledge a movee event.
        [HttpPost]
        [ActionName("AckMoveeEvent")]
        public IActionResult AckMoveeEvent(string guid, MoveeEventFrameViewModel moveeEventFrameViewModel)
        {
            var id = _moveeEventRepository.GetEventIdFromGuid(guid); // Retrieves the event ID based on the GUID.
            long unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // Converts the current time to Unix time in milliseconds.
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user ID.

            MoveeEventFrame moveeEventFrame = new MoveeEventFrame(); // Initializes a new movee event frame.
            moveeEventFrame = moveeEventFrameViewModel.moveeEventFrame; // Sets the movee event frame in the view model.
            moveeEventFrame.EventAckTime = unixTimeMilliseconds; // Sets the acknowledgment time.
            moveeEventFrame.LocationId = _moveeEventRepository.GetMoveeEventById(id).LocationId; // Sets the location ID.

            ViewBag.Comments = _moveeEventCommentRepository.GetAllCommentsByEventId(id); // Sets the comments for the view.

            var myTags = _moveeEventTagRepository.GetAllMoveeEventTags().Where(a => a.MoveeEventFrameId == id); // Retrieves all tags associated with the event ID.
            ViewBag.MyTags = myTags; // Sets the tags for the view.

            moveeEventFrameViewModel.MoveeEventTag = myTags.ToList(); // Sets the movee event tags in the view model.
            moveeEventFrameViewModel.MoveeTag = _moveeEventTagRepository.GetMoveeTags().ToList(); // Sets the movee tags in the view model.
            moveeEventFrameViewModel.LogViewModel = _moveeEventCommentRepository.GetLogs(id); // Sets the logs in the view model.
            moveeEventFrameViewModel.moveeDataFrames = _moveeDataRepository.GetAllData().ToList(); // Sets the movee data frames in the view model.

            ApplicationUser user = _userManager.Users.Include(x => x.Company).SingleOrDefaultAsync(x => x.Id == userId).Result; // Retrieves user details asynchronously.

            List<int> ints = new List<int>(); // Initializes a list to store the tag IDs.
            var deletedTags = _moveeEventTagRepository.GetMyDeletedTagLogs(id, userId).Distinct(); // Retrieves the deleted tags for the event.
            var addedTags = _moveeEventTagRepository.GetMyAddedTagLogs(id, userId).Distinct(); // Retrieves the added tags for the event.
            var excepted = addedTags.Except(deletedTags); // Calculates the tags that are added but not deleted.

            foreach (var item in myTags)
            {
                // Adds or removes tags based on the added and excepted tags.
                if (addedTags.Contains(item.MoveeTagId))
                {
                    ints.Add(item.MoveeTagId);
                }
                else if (!excepted.Contains(item.MoveeTagId))
                {
                    ints.Remove(item.MoveeTagId);
                }
            }

            ViewBag.Tags = _moveeEventTagRepository.TagsForDropDownList(ints, user.Company.Id); // Sets the tags for the dropdown list.

            if (moveeEventFrame.IsAcked == false)
            {
                if (ModelState.IsValid)
                {
                    // Checks if the acknowledgment message is valid and contains at least 2 letters.
                    if (moveeEventFrame.AckMessage != null && moveeEventFrame.AckMessage.Count(char.IsLetter) >= 2)
                    {
                        _moveeEventRepository.UpdateAckedMoveeEvent(id, moveeEventFrame); // Updates the movee event as acknowledged.
                        return RedirectToAction(nameof(Index)); // Redirects to the index action.
                    }
                    else
                    {
                        TempData["ViewData"] = "Obavezan unos opisa potvrde"; // Sets an error message for missing acknowledgment description.

                        return RedirectToAction(nameof(EventsDetails), new { guid = guid }); // Redirects to the event details.
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Opis je već unesen!"); // Adds a model error for already entered description.
            }

            return View("EventsDetails", moveeEventFrameViewModel); // Returns the view with the event details.
        }

        // HTTP POST method to clear a movee event.
        [HttpPost]
        [ActionName("ClearMoveeEvent")]
        public IActionResult ClearEvent(string guid, MoveeEventFrameViewModel moveeEventFrameViewModel)
        {
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user ID.
            var id = _moveeEventRepository.GetEventIdFromGuid(guid); // Retrieves the event ID based on the GUID.

            MoveeEventFrame moveeEventFrame = new MoveeEventFrame(); // Initializes a new movee event frame.
            moveeEventFrame = moveeEventFrameViewModel.moveeEventFrame; // Sets the movee event frame in the view model.
            moveeEventFrame.EventClearTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // Sets the clear time.
            moveeEventFrame.LocationId = _moveeEventRepository.GetMoveeEventById(moveeEventFrame.Id).LocationId; // Sets the location ID.

            ViewBag.Comments = _moveeEventCommentRepository.GetAllCommentsByEventId(id); // Sets the comments for the view.

            var myTags = _moveeEventTagRepository.GetAllMoveeEventTags().Where(a => a.MoveeEventFrameId == id); // Retrieves all tags associated with the event ID.
            ViewBag.MyTags = myTags; // Sets the tags for the view.

            moveeEventFrameViewModel.MoveeEventTag = myTags.ToList(); // Sets the movee event tags in the view model.
            moveeEventFrameViewModel.MoveeTag = _moveeEventTagRepository.GetMoveeTags().ToList(); // Sets the movee tags in the view model.
            moveeEventFrameViewModel.moveeDataFrames = _moveeDataRepository.GetAllData().ToList(); // Sets the movee data frames in the view model.
            moveeEventFrameViewModel.LogViewModel = _moveeEventCommentRepository.GetLogs(id); // Sets the logs in the view model.

            ApplicationUser user = _userManager.Users.Include(x => x.Company).SingleOrDefaultAsync(x => x.Id == userId).Result; // Retrieves user details asynchronously.

            List<int> ints = new List<int>(); // Initializes a list to store the tag IDs.
            var deletedTags = _moveeEventTagRepository.GetMyDeletedTagLogs(id, userId).Distinct(); // Retrieves the deleted tags for the event.
            var addedTags = _moveeEventTagRepository.GetMyAddedTagLogs(id, userId).Distinct(); // Retrieves the added tags for the event.
            var excepted = addedTags.Except(deletedTags); // Calculates the tags that are added but not deleted.

            foreach (var item in myTags)
            {
                // Adds or removes tags based on the added and excepted tags.
                if (addedTags.Contains(item.MoveeTagId))
                {
                    ints.Add(item.MoveeTagId);
                }
                else if (!excepted.Contains(item.MoveeTagId))
                {
                    ints.Remove(item.MoveeTagId);
                }
            }

            ViewBag.Tags = _moveeEventTagRepository.TagsForDropDownList(ints, user.Company.Id); // Sets the tags for the dropdown list.

            if (ModelState.IsValid)
            {
                // Checks if the clear message is valid and contains at least 2 letters.
                if (!string.IsNullOrEmpty(moveeEventFrame.ClearMessage) && moveeEventFrame.ClearMessage.Count(char.IsLetter) >= 2)
                {
                    _moveeEventRepository.UpdateClearedMoveeEvent(id, moveeEventFrame); // Updates the movee event as cleared.
                    _moveeEventRepository.AckMoveeAlarmsByEventId(id, moveeEventFrame); // Acknowledges the alarms associated with the event ID.
                    return RedirectToAction(nameof(Index)); // Redirects to the index action.
                }
                else
                {
                    TempData["ViewData"] = "Obavezan unos opisa zatvaranja"; // Sets an error message for missing clear description.

                    return RedirectToAction(nameof(EventsDetails), new { guid = guid }); // Redirects to the event details.
                }
            }

            return View("EventsDetails", moveeEventFrameViewModel); // Returns the view with the event details.
        }

        // HTTP POST method to add a tag for acknowledgment.
        [HttpPost]
        public IActionResult AddTagForAck(string guid, int tagId)
        {
            var eventId = _moveeEventRepository.GetEventIdFromGuid(guid); // Retrieves the event ID based on the GUID.
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user ID.
            long unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // Converts the current time to Unix time in milliseconds.

            if (ModelState.IsValid)
            {
                // Initializes a new movee event tag.
                MoveeEventTag moveeEventTag = new MoveeEventTag()
                {
                    MoveeEventFrameId = eventId,
                    MoveeTagId = tagId,
                    Active = true
                };

                ViewBag.EventId = moveeEventTag.MoveeEventFrameId; // Sets the event ID for the view.
                _moveeEventTagRepository.CreateMoveeEventTag(moveeEventTag, userId, unixTimeMilliseconds); // Creates the new movee event tag.
                return RedirectToAction(nameof(EventsDetails), new { guid = guid }); // Redirects to the event details.
            }

            return View(); // Returns the default view if the model state is not valid.
        }

        // HTTP POST method to add a tag for clearing.
        [HttpPost]
        public IActionResult AddTagForClear(int eventId, int tagId)
        {
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user ID.
            long unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // Converts the current time to Unix time in milliseconds.

            if (ModelState.IsValid)
            {
                // Initializes a new movee event tag.
                MoveeEventTag moveeEventTag = new MoveeEventTag()
                {
                    MoveeEventFrameId = eventId,
                    MoveeTagId = tagId,
                    Active = true
                };

                ViewBag.EventId = moveeEventTag.MoveeEventFrameId; // Sets the event ID for the view.
                _moveeEventTagRepository.CreateMoveeEventTag(moveeEventTag, userId, unixTimeMilliseconds); // Creates the new movee event tag.
                return RedirectToAction(nameof(EventsDetails), new { id = moveeEventTag.MoveeEventFrameId }); // Redirects to the event details.
            }

            return View(); // Returns the default view if the model state is not valid.
        }

        // HTTP POST method to add a tag for details.
        [HttpPost]
        public IActionResult AddTagForDetails(int eventId, int tagId)
        {
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user ID.

            if (ModelState.IsValid)
            {
                // Initializes a new movee event tag.
                MoveeEventTag moveeEventTag = new MoveeEventTag()
                {
                    MoveeEventFrameId = eventId,
                    MoveeTagId = tagId,
                    Active = true
                };

                ViewBag.EventId = moveeEventTag.MoveeEventFrameId; // Sets the event ID for the view.
                _moveeEventTagRepository.CreateMoveeEventTag(moveeEventTag, userId, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()); // Creates the new movee event tag.
                return RedirectToAction(nameof(EventsDetails), new { id = moveeEventTag.MoveeEventFrameId }); // Redirects to the event details.
            }

            return View(); // Returns the default view if the model state is not valid.
        }

        // HTTP POST method to disable a tag for details.
        [HttpPost]
        public IActionResult DisableTagForDetails(int eventTagId)
        {
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user ID.
            DateTimeOffset now = DateTimeOffset.UtcNow; // Gets the current time in UTC.
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds(); // Converts the current time to Unix time in milliseconds.

            var tagForUpdate = _moveeEventTagRepository.GetMoveeEventTagById(eventTagId); // Retrieves the specific movee event tag based on the ID.
            var guid = _moveeEventRepository.GetGuidFromEventId(tagForUpdate.MoveeEventFrameId); // Retrieves the GUID based on the event ID.

            if (ModelState.IsValid)
            {
                ViewBag.EventId = tagForUpdate.MoveeEventFrameId; // Sets the event ID for the view.
                _moveeEventTagRepository.RemoveMoveeEventTag(tagForUpdate, userId, unixTimeMilliseconds); // Removes the specified movee event tag.
                return RedirectToAction(nameof(EventsDetails), new { guid = guid }); // Redirects to the event details.
            }

            return View(); // Returns the default view if the model state is not valid.
        }

        // HTTP POST method to create an acknowledgment comment.
        [HttpPost]
        [Authorize]
        public IActionResult CreateAckComment(MoveeEventComment moveeEventComment)
        {
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user ID.
            DateTimeOffset now = DateTimeOffset.UtcNow; // Gets the current time in UTC.
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds(); // Converts the current time to Unix time in milliseconds.

            var guid = _moveeEventRepository.GetGuidFromEventId(moveeEventComment.MoveeEventFrameId); // Retrieves the GUID based on the event ID.

            // Initializes a new movee event comment.
            MoveeEventComment newComment = new MoveeEventComment
            {
                Comment = moveeEventComment.Comment,
                EventCommentBy = userId,
                EventCommentTime = unixTimeMilliseconds,
                MoveeEventFrameId = moveeEventComment.MoveeEventFrameId,
                Active = true
            };

            if (ModelState.IsValid)
            {
                _moveeEventCommentRepository.AddComment(newComment); // Adds the new comment to the repository.
                return RedirectToAction("EventsDetails", new { guid = guid }); // Redirects to the event details.
            }

            return RedirectToAction("EventsDetails", new { guid = guid }); // Redirects to the event details if the model state is not valid.
        }

        // HTTP POST method to create a clearing comment.
        [HttpPost]
        [Authorize]
        public IActionResult CreateClearComment(MoveeEventComment moveeEventComment)
        {
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user ID.
            DateTimeOffset now = DateTimeOffset.UtcNow; // Gets the current time in UTC.
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds(); // Converts the current time to Unix time in milliseconds.

            // Initializes a new movee event comment.
            MoveeEventComment newComment = new MoveeEventComment
            {
                Comment = moveeEventComment.Comment,
                EventCommentBy = userId,
                EventCommentTime = unixTimeMilliseconds,
                MoveeEventFrameId = moveeEventComment.MoveeEventFrameId,
                Active = true
            };

            if (ModelState.IsValid)
            {
                _moveeEventCommentRepository.AddComment(newComment); // Adds the new comment to the repository.
                return RedirectToAction("EventsDetails", new { id = moveeEventComment.MoveeEventFrameId }); // Redirects to the event details.
            }

            return RedirectToAction("EventsDetails", new { id = moveeEventComment.MoveeEventFrameId }); // Redirects to the event details if the model state is not valid.
        }

        // HTTP POST method to create a comment for details.
        [HttpPost]
        [Authorize]
        public IActionResult CreateCommentDetails(MoveeEventComment moveeEventComment)
        {
            var userId = _userManager.GetUserId(HttpContext.User); // Retrieves the current user ID.
            DateTimeOffset now = DateTimeOffset.UtcNow; // Gets the current time in UTC.
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds(); // Converts the current time to Unix time in milliseconds.

            var guid = _moveeEventRepository.GetGuidFromEventId(moveeEventComment.MoveeEventFrameId); // Retrieves the GUID based on the event ID.

            // Initializes a new movee event comment.
            MoveeEventComment newComment = new MoveeEventComment
            {
                Comment = moveeEventComment.Comment,
                EventCommentBy = userId,
                EventCommentTime = unixTimeMilliseconds,
                MoveeEventFrameId = moveeEventComment.MoveeEventFrameId,
                Active = true
            };

            _moveeEventCommentRepository.AddComment(newComment); // Adds the new comment to the repository.

            return RedirectToAction("EventsDetails", new { guid = guid }); // Redirects to the event details.
        }
    }
}
