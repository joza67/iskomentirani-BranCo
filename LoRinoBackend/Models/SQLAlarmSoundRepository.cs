using Microsoft.AspNetCore.Http; // For IHttpContextAccessor to access HTTP context
using Microsoft.AspNetCore.Identity; // For UserManager to manage application users
using Microsoft.EntityFrameworkCore; // For Entity Framework Core functionalities
using System.Collections.Generic; // For collections like List<T> and IEnumerable<T>
using System.Linq; // For LINQ operations such as Where and Join

namespace LoRinoBackend.Models
{
    // Repository class for managing alarm sounds related to events in the SQL database
    public class SQLAlarmSoundRepository : IAlarmSoundRepository
    {
        // Private fields for database context, user manager, and HTTP context accessor
        private readonly AppDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private IHttpContextAccessor httpContextAccessor;

        // Constructor to initialize the repository with the necessary services
        public SQLAlarmSoundRepository(AppDbContext contex, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.context = contex;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        // Method to get all locations from the database, including related users
        public IEnumerable<Location> GetAllLocations()
        {
            // Fetch locations and include related users for each location
            return context.Location.Include(a => a.LocationUserList).ToList();
        }

        // Method to get locations associated with a specific user
        public List<Location> GetLocationsByUserId(string userId)
        {
            // Fetch the list of locations for the given user ID
            var userLocations = context.LocationUser.Where(c => c.UserId == userId).ToList();
            var locations = GetAllLocations(); // Get all locations

            // Join the locations with the user's locations and select the matching ones
            var onlyUserLocations = (from l in locations
                                     join ul in userLocations
                                     on l.Id equals ul.LocationId
                                     select l).ToList();
            return onlyUserLocations;
        }

        // Method to get all active events that are not acknowledged and have a valid location ID
        public List<MoveeEventFrame> GetAllActiveEvents()
        {
            // Fetch active events that have not been acknowledged
            var activeEvents = context.MoveeEventFrame.Where(a => a.IsAcked == false && a.LocationId != 0).ToList();
            return activeEvents;
        }

        // Method to get all active events associated with a specific company ID
        public List<MoveeEventFrame> GetEventsWithCompanyId(int companyId)
        {
            var locations = context.Location.ToList(); // Fetch all locations
            var moveeEventFrames = GetAllActiveEvents(); // Get all active events

            // Join events with locations and filter by company ID
            var data = (from e in moveeEventFrames
                        join l in locations on e.LocationId equals l.Id
                        where l.CompanyId == companyId
                        select e).ToList();

            return data;
        }

        // Method to get all active, non-silent events based on user role and associated locations
        public List<MoveeEventFrame> GetAllActiveNotSilentEvents(string userId)
        {
            // Fetch the user and their associated company
            ApplicationUser user = userManager.Users.Include(x => x.Company).SingleOrDefaultAsync(x => x.Id == userId).Result;
            var locationsByUser = GetLocationsByUserId(userId); // Get locations for the user

            // Check user roles
            var isSuperAdmin = userManager.IsInRoleAsync(user, "Super Admin").Result;
            var isAdmin = userManager.IsInRoleAsync(user, "Admin").Result;
            var currentUserCompanyId = user.Company.Id;
            List<MoveeEventFrame> events = new List<MoveeEventFrame>();

            if (isSuperAdmin)
            {
                // Return all active events if the user is a Super Admin
                events = GetAllActiveEvents();
                return events;
            }
            else if (isAdmin)
            {
                // Return active events for the current user's company if the user is an Admin
                events = GetEventsWithCompanyId(currentUserCompanyId);
                return events;
            }
            else
            {
                // Return active events for the locations associated with the user
                events = (from l in locationsByUser
                          join e in events on l.Id equals e.LocationId
                          select e).ToList();
                return events;
            }
        }

        // Method to check if there are any active, non-silent events for a given user
        public bool ActivateSoundAlarm(string userId)
        {
            // Return true if there are active, non-silent events; otherwise, return false
            return GetAllActiveNotSilentEvents(userId).Count() > 0;
        }
    }
}
