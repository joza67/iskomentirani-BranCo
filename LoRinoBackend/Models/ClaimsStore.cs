// Import necessary namespaces
using System; // Provides basic functionalities
using System.Collections.Generic; // Provides collection types like List
using System.Linq; // Provides LINQ functionalities for data querying
using System.Security.Claims; // Provides classes for claims-based identity
using System.Threading.Tasks; // Provides asynchronous programming functionalities

namespace LoRinoBackend.Models
{
    // Static class to store a collection of claims
    public static class ClaimsStore
    {
        // Static list of Claim objects representing different permissions
        public static List<Claim> AllClaims = new List<Claim>()
        {
            // Claim representing permission to create a role
            new Claim("Create Role", "Create Role"),
            // Claim representing permission to edit a role
            new Claim("Edit Role", "Edit Role"),
            // Claim representing permission to delete a role
            new Claim("Delete Role", "Delete Role")
        };
    }
}
