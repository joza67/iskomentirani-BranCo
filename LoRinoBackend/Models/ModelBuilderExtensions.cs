// Import namespaces required for Entity Framework and core .NET functionalities
using Microsoft.EntityFrameworkCore; // For Entity Framework Core functionalities
using System; // Core .NET types and functions
using System.Collections.Generic; // Collections like List and Dictionary
using System.Linq; // LINQ queries and operations
using System.Threading.Tasks; // Asynchronous programming support

namespace LoRinoBackend.Models
{
    // Static class for extending the ModelBuilder with seed data
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Seed method to add initial data to the database.
        /// </summary>
        /// <param name="modelBuilder">The ModelBuilder instance used to configure the model.</param>
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // Seed data for the Company entity
            modelBuilder.Entity<Company>().HasData(
                new Company
                {
                    Name = "Micro-Link d.o.o.", // Company name
                    Email = "microlink@microlink.hr", // Company email
                    Street = "Jaruscica 9a", // Company street address
                    City = "Zagreb", // Company city
                    Country = "Croatia" // Company country
                }
            );

            // Seed data for the ApplicationUser entity
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = "40da960e-999f-41b4-8b07-a76ba5278153", // Unique identifier for the user
                    UserName = "admin@microlink.hr", // Username for the user
                    NormalizedUserName = "ADMIN@MICROLINK.HR", // Normalized form of the username (used for indexing)
                    Email = "admin@microlink.hr", // Email address of the user
                    NormalizedEmail = "ADMIN@MICROLINK.HR", // Normalized form of the email (used for indexing)
                    EmailConfirmed = true, // Indicates whether the email address is confirmed
                    PasswordHash = "AQAAAAEAACcQAAAAELVbKlZBbf90kQKIJ1NFOqWAWGUTVDqtcz6lZc/7xub3wJ+bLyCLAlyR44nDAKOFeQ==", // Password hash for the user
                    SecurityStamp = "5ASNZAW7T7EOPQCHDFV4Y77MTKV3Y75P", // Security stamp for the user (used for security checks)
                    ConcurrencyStamp = "7f1bdade-4fdc-45ff-b98d-fa7ed6e54ec8", // Concurrency stamp for managing concurrent operations
                    PhoneNumber = null, // Phone number of the user (optional)
                    PhoneNumberConfirmed = false, // Indicates whether the phone number is confirmed
                    TwoFactorEnabled = false, // Indicates whether two-factor authentication is enabled
                    LockoutEnd = null, // End date and time for the lockout (if any)
                    LockoutEnabled = true, // Indicates whether the user can be locked out
                    AccessFailedCount = 0, // Count of failed access attempts
                    Company = new Company() { Id = 1 }, // Reference to the Company entity (assumes a Company with Id = 1 exists)
                    Streeet = null, // Street address (optional, not used)
                    City = null, // City (optional, not used)
                    Country = null, // Country (optional, not used)
                    FirstName = "Admin", // First name of the user
                    LastName = "Admin" // Last name of the user
                }
            );
        }
    }
}
