using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Linq;

namespace LoRinoBackend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<LoRaData> LoraData { get; set; }
        public DbSet<DecodedData> DecodedData { get; set; }

        public DbSet<MoveeDataFrame> MoveeDataFrame { get; set; }
        public DbSet<Device> Device { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<LocationUser> LocationUser { get; set; }

        public DbSet<DeviceType> DeviceType { get; set; }
        public DbSet<LobaroDataFrame> LobaroDataFrame { get; set; }
        public DbSet<MoveeEventFrame> MoveeEventFrame { get; set; }
        public DbSet<MoveeTag> MoveeTag { get; set; }
        public DbSet<MoveeEventTag> MoveeEventTag { get; set; }
        public DbSet<MoveeEventComment> MoveeEventComment { get; set; }
        public DbSet<EventTagLog> EventTagLog { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            base.OnModelCreating(modelBuilder);
            //modelBuilder.Seed();
            modelBuilder.Entity<MoveeEventFrame>().HasData
                (
                new MoveeEventFrame() { Id = 1, EventCreationTime = 0, EventAckBy = "", EventAckTime = 0, AlarmCount = 0, Guid = "", LocationId = 0, AckMessage = "DummyEvent", ClearMessage = "DummyEvent", EventClearBy = "", EventClearTime = 0, IsAcked = false, IsCleared = false, TimerIsEnded = false }
                );

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

    }
}
