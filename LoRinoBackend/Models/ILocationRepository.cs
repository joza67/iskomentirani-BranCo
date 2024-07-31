// Import necessary namespaces
using Microsoft.AspNetCore.Mvc.Rendering; // Provides classes for rendering HTML elements in MVC views
using System; // Provides basic functionalities
using System.Collections.Generic; // Provides collection types like IEnumerable and List
using System.Linq; // Provides LINQ functionalities for data querying
using System.Threading.Tasks; // Provides asynchronous programming functionalities

namespace LoRinoBackend.Models
{
    // Interface defining the contract for location repository operations
    public interface ILocationRepository
    {
        // Method to get a location by its ID
        Location GetLocation(int Id);

        // Method to get all locations
        IEnumerable<Location> GetAllLocations();

        // Method to get the company associated with a specific ID
        Company GetCompanyFromId(int id);

        // Asynchronous method to get all mail users from a specific location ID
        Task<List<ApplicationUser>> GetAllMailUsersFromLocationId(int locationId);

        // Method to get all users associated with a specific location ID
        List<LocationUser> GetLocationUsersByLocationId(int locationId);

        // Method to get all location IDs
        IEnumerable<int> GetAllLocationsIds();

        // Method to get all location users
        IEnumerable<LocationUser> GetAllLocationUsers();

        // Method to create a new location user
        void CreateLocationUser(LocationUser locationUser);

        // Method to update an existing location user's details
        void UpdateLocationUser(LocationUser locationUser);

        // Method to get all locations users associated with a specific user ID
        List<LocationUser> GetLocationUsersByUserId(string userId);

        // Method to delete a location user by ID
        void DeleteLocationUser(int id);

        // Method to get a location user by ID
        LocationUser GetLocationUserById(int id);

        // Method to add a new location
        Location Add(Location location);

        // Method to update an existing location's details
        Location Update(Location locationChanges);

        // Method to delete a location by ID
        Location Delete(int id);

        // Method to get a location by its ID (duplicate method with GetLocation)
        Location GetLocationById(int id);

        // Method to get locations associated with a specific user ID
        List<Location> GetLocationsByUserId(string userId);

        // Method to get all companies
        IEnumerable<Company> GetAllCompanies();

        // Method to get location IDs associated with a specific admin ID
        List<int> GetLocationsIdsByAdminId(string userId);

        // Method to get location IDs associated with a specific user ID
        List<int> GetLocationsIdsByUserId(string userId);

        // Method to get the company associated with a specific location ID
        Company GetCompanyFromLocationId(int id);

        // Asynchronous method to get all admins from a specific location ID
        Task<List<ApplicationUser>> GetAllAdminsFromLocationId(int id);

        // Method to delete location users by their GUID
        void DeleteLocationUsers(string guid);
    }
}
