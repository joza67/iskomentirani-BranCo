using Microsoft.AspNetCore.Http; // For accessing HTTP context
using Microsoft.AspNetCore.Identity; // For identity management and role management
using Microsoft.EntityFrameworkCore; // For Entity Framework Core functionalities
using System.Collections.Generic; // For collections such as List<T> and IEnumerable<T>
using System.Linq; // For LINQ operations like Where and FirstOrDefault
using System.Threading.Tasks; // For asynchronous programming
using System.Data; // For ADO.NET functionalities (though not used in this code)
using System; // For basic .NET types and operations

namespace LoRinoBackend.Models
{
    // Repository class for managing locations and related operations, implementing ILocationRepository
    public class SQLLocationRepository : ILocationRepository
    {
        // Private fields for database context, user manager, role manager, and HTTP context accessor
        private readonly AppDbContext contex;
        private readonly UserManager<ApplicationUser> userManager;
        private IHttpContextAccessor httpContextAccessor;
        private RoleManager<IdentityRole> roleManager;

        // Constructor to initialize the repository with the database context, user manager, role manager, and HTTP context accessor
        public SQLLocationRepository(AppDbContext contex, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            this.contex = contex;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.roleManager = roleManager;
        }

        // Method to add a new Location to the database
        public Location Add(Location location)
        {
            contex.Location.Add(location);
            contex.SaveChanges();
            return location;
        }

        // Method to delete a Location from the database by its ID
        public Location Delete(int id)
        {
            Location location = contex.Location.Find(id);
            if (location != null)
            {
                contex.Location.Remove(location);
                contex.SaveChanges();
            }
            return location;
        }

        // Method to retrieve all admin users associated with a specific location ID
        public async Task<List<ApplicationUser>> GetAllAdminsFromLocationId(int id)
        {
            var items = await userManager.GetUsersInRoleAsync("Admin");
            items = (IList<ApplicationUser>)items.Where(a => a.Company.Id == id);

            return (List<ApplicationUser>)items;
        }

        // Method to retrieve all locations from the database with related users and companies
        public IEnumerable<Location> GetAllLocations()
        {
            var locations = contex.Location.Include(a => a.LocationUserList).Include(a => a.Company).ToList();
            return locations;
        }

        // Method to retrieve all users with the role "Email User" from a specific location ID
        public async Task<List<ApplicationUser>> GetAllMailUsersFromLocationId(int locationId)
        {
            List<ApplicationUser> userList = new List<ApplicationUser>();
            var locationUsers = GetLocationUsersByLocationId(locationId);
            var allUsers = contex.Users.Include(a => a.Company).ToList();
            var locations = contex.Location.Include(a => a.LocationUserList).Include(a => a.Company);

            foreach (var user in allUsers)
            {
                if (await userManager.IsInRoleAsync(user, "Email User"))
                {
                    if (await userManager.IsInRoleAsync(user, "SuperAdmin"))
                    {
                        userList.Add(user);
                    }
                    else if (await userManager.IsInRoleAsync(user, "Admin") && user.Company.Id == GetCompanyFromLocationId(locationId).Id)
                    {
                        userList.Add(user);
                    }
                    else if (locationUsers.Any(a => a.UserId.Contains(user.Id)))
                    {
                        userList.Add(user);
                    }
                }
            }

            return userList;
        }

        // Method to retrieve all companies from the database
        public IEnumerable<Company> GetAllCompanies()
        {
            return contex.Company.ToList();
        }

        // Method to retrieve the company associated with a specific location ID
        public Company GetCompanyFromLocationId(int id)
        {
            var location = contex.Location.OrderByDescending(i => i.Id).FirstOrDefault(a => a.Id == id);
            return location.Company;
        }

        // Method to retrieve a company by its ID
        public Company GetCompanyFromId(int id)
        {
            var company = contex.Company.FirstOrDefault(a => a.Id == id);
            return company;
        }

        // Method to retrieve a location by its ID with related company data
        public Location GetLocationById(int id)
        {
            var location = contex.Location.Include(a => a.Company).FirstOrDefault(a => a.Id == id);
            return location;
        }

        // Method to retrieve all location IDs from the database
        public IEnumerable<int> GetAllLocationsIds()
        {
            return contex.Location.Select(a => a.Id);
        }

        // Method to retrieve all location-user associations from the database
        public IEnumerable<LocationUser> GetAllLocationUsers()
        {
            return contex.LocationUser.ToList();
        }

        // Method to create a new location-user association
        public void CreateLocationUser(LocationUser locationUser)
        {
            contex.LocationUser.Add(locationUser);
            contex.SaveChanges();
        }

        // Method to retrieve a location-user association by its ID
        public LocationUser GetLocationUserById(int id)
        {
            var locationUser = contex.LocationUser.FirstOrDefault(c => c.Id == id);
            return locationUser;
        }

        // Method to retrieve location-user associations by user ID
        public List<LocationUser> GetLocationUsersByUserId(string userId)
        {
            var locationUsers = contex.LocationUser.Where(c => c.UserId == userId).ToList();
            return locationUsers;
        }

        // Method to retrieve location-user associations by location ID
        public List<LocationUser> GetLocationUsersByLocationId(int locationId)
        {
            var locationUsers = contex.LocationUser.Where(c => c.LocationId == locationId).ToList();
            return locationUsers;
        }

        // Method to retrieve locations associated with a specific user ID
        public List<Location> GetLocationsByUserId(string userId)
        {
            var userLocations = contex.LocationUser.Where(c => c.UserId == userId).ToList();
            var locations = GetAllLocations();

            var onlyUserLocations = (from l in locations
                                     join ul in userLocations
                                     on l.Id equals ul.LocationId
                                     select l).ToList();
            return onlyUserLocations;
        }

        // Method to retrieve location IDs associated with a specific user ID
        public List<int> GetLocationsIdsByUserId(string userId)
        {
            var userLocations = contex.LocationUser.Where(c => c.UserId == userId).ToList();
            var locations = GetAllLocations();

            var onlyUserLocations = (from l in locations
                                     join ul in userLocations
                                     on l.Id equals ul.LocationId
                                     select l)
                                     .Select(a => a.Id)
                                     .ToList();
            return onlyUserLocations;
        }

        // Method to retrieve location IDs associated with a specific admin user ID
        public List<int> GetLocationsIdsByAdminId(string userId)
        {
            ApplicationUser user = userManager.Users.Include(x => x.Company).SingleOrDefaultAsync(x => x.Id == userId).Result;
            var locations = GetAllLocations().Where(a => a.CompanyId == user.Company.Id);

            return locations.Select(a => a.Id).ToList();
        }

        // Method to update an existing location-user association
        public void UpdateLocationUser(LocationUser locationUser)
        {
            contex.LocationUser.Update(locationUser);
            contex.SaveChanges();
        }

        // Method to delete a location-user association by its ID
        public void DeleteLocationUser(int id)
        {
            var cu = contex.LocationUser.Find(id);
            contex.LocationUser.Remove(cu);
            contex.SaveChanges();
        }

        // Method to delete location-user associations by user ID
        public void DeleteLocationUsers(string guid)
        {
            var listToClean = contex.LocationUser.Where(gid => String.Equals(gid.UserId, guid)).ToList();
            foreach (LocationUser a in listToClean)
            {
                contex.LocationUser.Remove(a);
            }
            contex.SaveChanges();
        }

        // Method to retrieve a location by its ID
        public Location GetLocation(int Id)
        {
            return contex.Location.Find(Id);
        }

        // Method to update an existing location
        public Location Update(Location locationChanges)
        {
            contex.Location.Update(locationChanges);
            contex.SaveChanges();
            return locationChanges;
        }
    }
}
