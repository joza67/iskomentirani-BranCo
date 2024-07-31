// Import necessary namespaces
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Provides the base class for ASP.NET Core Identity's Entity Framework Core integration
using Microsoft.EntityFrameworkCore; // Provides Entity Framework Core functionalities
using System; // Provides basic functionalities
using System.Collections.Generic; // Provides collection types like List
using System.Linq; // Provides LINQ functionalities for data querying
using System.Threading.Tasks; // Provides asynchronous programming functionalities

namespace LoRinoBackend.Models
{
    // AppDbContext class inheriting from IdentityDbContext for ASP.NET Core Identity
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        // Constructor that accepts DbContextOptions and passes it to the base class
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        // DbSet properties representing tables in the database
        public DbSet<LoRaData> LoraData { get; set; } // Table for LoRaData
        public DbSet<DecodedData> DecodedData { get; set; } // Table for DecodedData

        public DbSet<MoveeDataFrame> MoveeDataFrame { get; set; } // Table for MoveeDataFrame
        public DbSet<Device> Device { get; set; } // Table for Device
        public DbSet<Company> Company { get; set; } // Table for Company
        public DbSet<Location> Location { get; set; } // Table for Location
        public DbSet<LocationUser> LocationUser { get; set; } // Table for LocationUser

        public DbSet<DeviceType> DeviceType { get; set; } // Table for DeviceType
        public DbSet<MoveeEventFrame> MoveeEventFrame { get; set; } // Table for MoveeEventFrame
        public DbSet<MoveeTag> MoveeTag { get; set; } // Table for MoveeTag
        public DbSet<MoveeEventTag> MoveeEventTag { get; set; } // Table for MoveeEventTag
        public DbSet<MoveeEventComment> MoveeEventComment { get; set; } // Table for MoveeEventComment
        public DbSet<EventTagLog> EventTagLog { get; set; } // Table for EventTagLog

        // Method to configure the model and relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the base method from IdentityDbContext to ensure proper setup
            base.OnModelCreating(modelBuilder);

            // Seed initial data into the MoveeEventFrame table
            modelBuilder.Entity<MoveeEventFrame>().HasData
                (
                new MoveeEventFrame()
                {
                    Id = 1,
                    EventCreationTime = 0,
                    EventAckBy = "",
                    EventAckTime = 0,
                    AlarmCount = 0,
                    Guid = "",
                    LocationId = 0,
                    AckMessage = "DummyEvent",
                    ClearMessage = "DummyEvent",
                    EventClearBy = "",
                    EventClearTime = 0,
                    IsAcked = false,
                    IsCleared = false,
                    TimerIsEnded = false
                }
                );

            // Configure all foreign keys to have a delete behavior of Restrict
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
