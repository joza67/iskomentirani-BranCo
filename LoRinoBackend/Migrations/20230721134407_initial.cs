using System; // Importing base class library, including fundamental classes and base classes that define commonly-used value and reference data types
using Microsoft.EntityFrameworkCore.Metadata; // Importing metadata features for Entity Framework Core
using Microsoft.EntityFrameworkCore.Migrations; // Importing migration features for Entity Framework Core

#nullable disable // Disabling nullable context warnings

namespace LoRinoBackend.Migrations
{
    // Partial class for initial migration, derived from Migration
    public partial class initial : Migration
    {
        // Method to define the operations to apply to the database during migration
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Alter the database with specific character set
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create AspNetRoles table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for role ID
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for role name
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for normalized role name
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4") // Column for concurrency stamp
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id); // Set primary key
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create Cluster table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "Cluster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn), // Column for primary key with auto-increment
                    ClusterId = table.Column<int>(type: "int", nullable: false) // Column for cluster ID
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cluster", x => x.Id); // Set primary key
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create Company table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn), // Column for primary key with auto-increment
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for company name
                    Email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for company email
                    Street = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for company street address
                    City = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for company city
                    Country = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for company country
                    PhotoPath = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4") // Column for company photo path (nullable)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id); // Set primary key
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create DeviceType table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "DeviceType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn), // Column for primary key with auto-increment
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for device type name
                    PhotoPath = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4") // Column for photo path (nullable)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceType", x => x.Id); // Set primary key
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create EventTagLog table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "EventTagLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn), // Column for primary key with auto-increment
                    MoveeEventTagId = table.Column<int>(type: "int", nullable: false), // Column for event tag ID
                    Action = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for action description (nullable)
                    EventTagBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for who tagged the event (nullable)
                    EventTagTime = table.Column<long>(type: "bigint", nullable: false), // Column for event tag time
                    EventId = table.Column<int>(type: "int", nullable: false) // Column for event ID
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTagLog", x => x.Id); // Set primary key
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create LobaroDataFrame table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "LobaroDataFrame",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn), // Column for primary key with auto-increment
                    DevEui = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for device EUI (nullable)
                    WaterMeterEui = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for water meter EUI (nullable)
                    RecvTime = table.Column<long>(type: "bigint", nullable: false), // Column for receive time
                    Volume = table.Column<int>(type: "int", nullable: false), // Column for volume
                    VolumeFlow = table.Column<int>(type: "int", nullable: false), // Column for volume flow
                    TemperatureFlow = table.Column<int>(type: "int", nullable: false), // Column for temperature flow
                    Battery = table.Column<int>(type: "int", nullable: false) // Column for battery level
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LobaroDataFrame", x => x.Id); // Set primary key
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create MoveeEventFrame table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "MoveeEventFrame",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn), // Column for primary key with auto-increment
                    EventCreationTime = table.Column<long>(type: "bigint", nullable: false), // Column for event creation time
                    EventAckBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for who acknowledged the event (nullable)
                    EventClearBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for who cleared the event (nullable)
                    Guid = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for event GUID (nullable)
                    EventAckTime = table.Column<long>(type: "bigint", nullable: false), // Column for event acknowledgment time
                    EventClearTime = table.Column<long>(type: "bigint", nullable: false), // Column for event clear time
                    AlarmCount = table.Column<int>(type: "int", nullable: false), // Column for alarm count
                    LocationId = table.Column<int>(type: "int", nullable: false), // Column for location ID
                    AckMessage = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for acknowledgment message (nullable)
                    IsAcked = table.Column<bool>(type: "tinyint(1)", nullable: false), // Column for whether the event is acknowledged
                    ClearMessage = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for clear message (nullable)
                    IsCleared = table.Column<bool>(type: "tinyint(1)", nullable: false), // Column for whether the event is cleared
                    TimerIsEnded = table.Column<bool>(type: "tinyint(1)", nullable: false) // Column for whether the timer has ended
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoveeEventFrame", x => x.Id); // Set primary key
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create MoveeTag table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "MoveeTag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn), // Column for primary key with auto-increment
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for tag name (nullable)
                    Active = table.Column<bool>(type: "tinyint(1)", nullable: false), // Column for whether the tag is active
                    CompanyId = table.Column<int>(type: "int", nullable: false) // Column for company ID
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoveeTag", x => x.Id); // Set primary key
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create AspNetRoleClaims table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn), // Column for primary key with auto-increment
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for role ID
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for claim type (nullable)
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4") // Column for claim value (nullable)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id); // Set primary key
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId", // Foreign key constraint linking to AspNetRoles table
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create EndDevice table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "EndDevice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn), // Column for primary key with auto-increment
                    DevEui = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for device EUI (nullable)
                    DevAddr = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for device address (nullable)
                    ClusterDataId = table.Column<int>(type: "int", nullable: true) // Column for cluster data ID (nullable)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndDevice", x => x.Id); // Set primary key
                    table.ForeignKey(
                        name: "FK_EndDevice_Cluster_ClusterDataId", // Foreign key constraint linking to Cluster table
                        column: x => x.ClusterDataId,
                        principalTable: "Cluster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create AspNetUsers table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for user ID
                    CompanyId = table.Column<int>(type: "int", nullable: true), // Column for company ID (nullable)
                    Streeet = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for user street (nullable)
                    City = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for user city (nullable)
                    Country = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for user country (nullable)
                    FirstName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for user first name (nullable)
                    LastName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for user last name (nullable)
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for user name (nullable)
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for normalized user name (nullable)
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for user email (nullable)
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for normalized email (nullable)
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false), // Column for email confirmation status
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for password hash (nullable)
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for security stamp (nullable)
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for concurrency stamp (nullable)
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for phone number (nullable)
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false), // Column for phone number confirmation status
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false), // Column for two-factor authentication status
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true), // Column for lockout end date (nullable)
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false), // Column for lockout enabled status
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false) // Column for access failed count
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id); // Set primary key
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Company_CompanyId", // Foreign key constraint linking to Company table
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create Location table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn), // Column for primary key with auto-increment
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for location name
                    Road = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for road
                    RoadSection = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for road section
                    Long = table.Column<double>(type: "double", nullable: false), // Column for longitude
                    Lat = table.Column<double>(type: "double", nullable: false), // Column for latitude
                    MapZoom = table.Column<double>(type: "double", nullable: false), // Column for map zoom level
                    MinZoom = table.Column<double>(type: "double", nullable: false), // Column for minimum zoom level
                    MaxZoom = table.Column<double>(type: "double", nullable: false), // Column for maximum zoom level
                    TimerLenght = table.Column<int>(type: "int", nullable: false), // Column for timer length
                    CompanyId = table.Column<int>(type: "int", nullable: false) // Column for company ID
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id); // Set primary key
                    table.ForeignKey(
                        name: "FK_Location_Company_CompanyId", // Foreign key constraint linking to Company table
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create MoveeEventComment table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "MoveeEventComment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn), // Column for primary key with auto-increment
                    Comment = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for event comment
                    EventCommentTime = table.Column<long>(type: "bigint", nullable: false), // Column for event comment time
                    EventCommentBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for who commented on the event (nullable)
                    MoveeEventFrameId = table.Column<int>(type: "int", nullable: false), // Column for event frame ID
                    Active = table.Column<bool>(type: "tinyint(1)", nullable: false) // Column for active status
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoveeEventComment", x => x.Id); // Set primary key
                    table.ForeignKey(
                        name: "FK_MoveeEventComment_MoveeEventFrame_MoveeEventFrameId", // Foreign key constraint linking to MoveeEventFrame table
                        column: x => x.MoveeEventFrameId,
                        principalTable: "MoveeEventFrame",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create MoveeEventTag table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "MoveeEventTag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn), // Column for primary key with auto-increment
                    MoveeEventFrameId = table.Column<int>(type: "int", nullable: false), // Column for event frame ID
                    MoveeTagId = table.Column<int>(type: "int", nullable: false), // Column for tag ID
                    Active = table.Column<bool>(type: "tinyint(1)", nullable: false) // Column for active status
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoveeEventTag", x => x.Id); // Set primary key
                    table.ForeignKey(
                        name: "FK_MoveeEventTag_MoveeEventFrame_MoveeEventFrameId", // Foreign key constraint linking to MoveeEventFrame table
                        column: x => x.MoveeEventFrameId,
                        principalTable: "MoveeEventFrame",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                    table.ForeignKey(
                        name: "FK_MoveeEventTag_MoveeTag_MoveeTagId", // Foreign key constraint linking to MoveeTag table
                        column: x => x.MoveeTagId,
                        principalTable: "MoveeTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create LoraData table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "LoraData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn), // Column for primary key with auto-increment
                    MsgId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for message ID (nullable)
                    EndDeviceDataId = table.Column<int>(type: "int", nullable: true), // Column for end device data ID (nullable)
                    FPort = table.Column<int>(type: "int", nullable: false), // Column for FPort
                    FCntDown = table.Column<int>(type: "int", nullable: false), // Column for FCntDown
                    FCntUp = table.Column<int>(type: "int", nullable: false), // Column for FCntUp
                    Adr = table.Column<bool>(type: "tinyint(1)", nullable: false), // Column for ADR status
                    Confirmed = table.Column<bool>(type: "tinyint(1)", nullable: false), // Column for confirmed status
                    Encrypted = table.Column<bool>(type: "tinyint(1)", nullable: false), // Column for encryption status
                    Payload = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for payload (nullable)
                    RecvTime = table.Column<long>(type: "bigint", nullable: false), // Column for receive time
                    ClassB = table.Column<bool>(type: "tinyint(1)", nullable: false), // Column for Class B status
                    Delayed = table.Column<bool>(type: "tinyint(1)", nullable: false), // Column for delayed status
                    UlFrequency = table.Column<float>(type: "float", nullable: false), // Column for uplink frequency
                    Modulation = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for modulation (nullable)
                    DataRate = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for data rate (nullable)
                    CodingRate = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for coding rate (nullable)
                    GwCnt = table.Column<int>(type: "int", nullable: false) // Column for gateway count
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoraData", x => x.Id); // Set primary key
                    table.ForeignKey(
                        name: "FK_LoraData_EndDevice_EndDeviceDataId", // Foreign key constraint linking to EndDevice table
                        column: x => x.EndDeviceDataId,
                        principalTable: "EndDevice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create AspNetUserClaims table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn), // Column for primary key with auto-increment
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for user ID
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for claim type (nullable)
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4") // Column for claim value (nullable)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id); // Set primary key
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId", // Foreign key constraint linking to AspNetUsers table
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create AspNetUserLogins table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for login provider
                    ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for provider key
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for provider display name (nullable)
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4") // Column for user ID
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey }); // Set composite primary key
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId", // Foreign key constraint linking to AspNetUsers table
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create AspNetUserRoles table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for user ID
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4") // Column for role ID
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId }); // Set composite primary key
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId", // Foreign key constraint linking to AspNetRoles table
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId", // Foreign key constraint linking to AspNetUsers table
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create AspNetUserTokens table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for user ID
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for login provider
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for token name
                    Value = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4") // Column for token value (nullable)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name }); // Set composite primary key
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId", // Foreign key constraint linking to AspNetUsers table
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create Device table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn), // Column for primary key with auto-increment
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for device name
                    DevEui = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for device EUI
                    Long = table.Column<double>(type: "double", nullable: false), // Column for longitude
                    Lat = table.Column<double>(type: "double", nullable: false), // Column for latitude
                    MinZoom = table.Column<double>(type: "double", nullable: false), // Column for minimum zoom level
                    MaxZoom = table.Column<double>(type: "double", nullable: false), // Column for maximum zoom level
                    Expires = table.Column<DateTime>(type: "datetime(6)", nullable: false), // Column for expiration date
                    DeviceTypeId = table.Column<int>(type: "int", nullable: true), // Column for device type ID (nullable)
                    CompanyId = table.Column<int>(type: "int", nullable: true), // Column for company ID (nullable)
                    LocationId = table.Column<int>(type: "int", nullable: false) // Column for location ID
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Device", x => x.Id); // Set primary key
                    table.ForeignKey(
                        name: "FK_Device_Company_CompanyId", // Foreign key constraint linking to Company table
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                    table.ForeignKey(
                        name: "FK_Device_DeviceType_DeviceTypeId", // Foreign key constraint linking to DeviceType table
                        column: x => x.DeviceTypeId,
                        principalTable: "DeviceType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                    table.ForeignKey(
                        name: "FK_Device_Location_LocationId", // Foreign key constraint linking to Location table
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create LocationUser table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "LocationUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn), // Column for primary key with auto-increment
                    LocationId = table.Column<int>(type: "int", nullable: false), // Column for location ID
                    UserId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4") // Column for user ID (nullable)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationUser", x => x.Id); // Set primary key
                    table.ForeignKey(
                        name: "FK_LocationUser_AspNetUsers_UserId", // Foreign key constraint linking to AspNetUsers table
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                    table.ForeignKey(
                        name: "FK_LocationUser_Location_LocationId", // Foreign key constraint linking to Location table
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create DecodedData table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "DecodedData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn), // Column for primary key with auto-increment
                    LoRaDataId = table.Column<int>(type: "int", nullable: true), // Column for LoRa data ID (nullable)
                    Energy = table.Column<long>(type: "bigint", nullable: false), // Column for energy
                    Power = table.Column<long>(type: "bigint", nullable: false), // Column for power
                    Volume = table.Column<double>(type: "double", nullable: false), // Column for volume
                    FlowRate = table.Column<double>(type: "double", nullable: false), // Column for flow rate
                    FwdTemp = table.Column<long>(type: "bigint", nullable: false), // Column for forward temperature
                    RetTemp = table.Column<long>(type: "bigint", nullable: false) // Column for return temperature
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DecodedData", x => x.Id); // Set primary key
                    table.ForeignKey(
                        name: "FK_DecodedData_LoraData_LoRaDataId", // Foreign key constraint linking to LoraData table
                        column: x => x.LoRaDataId,
                        principalTable: "LoraData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create GwInfo table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "GwInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn), // Column for primary key with auto-increment
                    GwEui = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for gateway EUI (nullable)
                    RfRegion = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for RF region (nullable)
                    Rssi = table.Column<int>(type: "int", nullable: false), // Column for RSSI
                    Snr = table.Column<double>(type: "double", nullable: false), // Column for SNR
                    Latitude = table.Column<double>(type: "double", nullable: false), // Column for latitude
                    Longitude = table.Column<double>(type: "double", nullable: false), // Column for longitude
                    Altitude = table.Column<int>(type: "int", nullable: false), // Column for altitude
                    Channel = table.Column<int>(type: "int", nullable: false), // Column for channel
                    RadioId = table.Column<int>(type: "int", nullable: false), // Column for radio ID
                    LoRaDataId = table.Column<int>(type: "int", nullable: true) // Column for LoRa data ID (nullable)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GwInfo", x => x.Id); // Set primary key
                    table.ForeignKey(
                        name: "FK_GwInfo_LoraData_LoRaDataId", // Foreign key constraint linking to LoraData table
                        column: x => x.LoRaDataId,
                        principalTable: "LoraData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create MoveeDataFrame table with specified columns and constraints
            migrationBuilder.CreateTable(
                name: "MoveeDataFrame",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn), // Column for primary key with auto-increment
                    RecvTime = table.Column<long>(type: "bigint", nullable: false), // Column for receive time
                    Temperature = table.Column<double>(type: "double", nullable: false), // Column for temperature
                    Battery = table.Column<double>(type: "double", nullable: false), // Column for battery level
                    DataType = table.Column<int>(type: "int", nullable: false), // Column for data type
                    Gx = table.Column<int>(type: "int", nullable: false), // Column for Gx
                    Gy = table.Column<int>(type: "int", nullable: false), // Column for Gy
                    Gz = table.Column<int>(type: "int", nullable: false), // Column for Gz
                    AckMsg = table.Column<bool>(type: "tinyint(1)", nullable: false), // Column for acknowledgment message status
                    DeviceId = table.Column<int>(type: "int", nullable: false), // Column for device ID
                    AckId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for acknowledgment ID (nullable)
                    AckTime = table.Column<long>(type: "bigint", nullable: false), // Column for acknowledgment time
                    Guid = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"), // Column for GUID (nullable)
                    MoveeEventFrameId = table.Column<int>(type: "int", nullable: false) // Column for event frame ID
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoveeDataFrame", x => x.Id); // Set primary key
                    table.ForeignKey(
                        name: "FK_MoveeDataFrame_Device_DeviceId", // Foreign key constraint linking to Device table
                        column: x => x.DeviceId,
                        principalTable: "Device",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                    table.ForeignKey(
                        name: "FK_MoveeDataFrame_MoveeEventFrame_MoveeEventFrameId", // Foreign key constraint linking to MoveeEventFrame table
                        column: x => x.MoveeEventFrameId,
                        principalTable: "MoveeEventFrame",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // On delete, restrict deletion if related records exist
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Insert initial data into MoveeEventFrame table
            migrationBuilder.InsertData(
                table: "MoveeEventFrame",
                columns: new[] { "Id", "AckMessage", "AlarmCount", "ClearMessage", "EventAckBy", "EventAckTime", "EventClearBy", "EventClearTime", "EventCreationTime", "Guid", "IsAcked", "IsCleared", "LocationId", "TimerIsEnded" },
                values: new object[] { 1, "DummyEvent", 0, "DummyEvent", "", 0L, "", 0L, 0L, "", false, false, 0, false });

            // Create indices on specific columns for better query performance
            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DecodedData_LoRaDataId",
                table: "DecodedData",
                column: "LoRaDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_CompanyId",
                table: "Device",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_DeviceTypeId",
                table: "Device",
                column: "DeviceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_LocationId",
                table: "Device",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_EndDevice_ClusterDataId",
                table: "EndDevice",
                column: "ClusterDataId");

            migrationBuilder.CreateIndex(
                name: "IX_GwInfo_LoRaDataId",
                table: "GwInfo",
                column: "LoRaDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Location_CompanyId",
                table: "Location",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationUser_LocationId",
                table: "LocationUser",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationUser_UserId",
                table: "LocationUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LoraData_EndDeviceDataId",
                table: "LoraData",
                column: "EndDeviceDataId");

            migrationBuilder.CreateIndex(
                name: "IX_MoveeDataFrame_DeviceId",
                table: "MoveeDataFrame",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_MoveeDataFrame_MoveeEventFrameId",
                table: "MoveeDataFrame",
                column: "MoveeEventFrameId");

            migrationBuilder.CreateIndex(
                name: "IX_MoveeEventComment_MoveeEventFrameId",
                table: "MoveeEventComment",
                column: "MoveeEventFrameId");

            migrationBuilder.CreateIndex(
                name: "IX_MoveeEventTag_MoveeEventFrameId",
                table: "MoveeEventTag",
                column: "MoveeEventFrameId");

            migrationBuilder.CreateIndex(
                name: "IX_MoveeEventTag_MoveeTagId",
                table: "MoveeEventTag",
                column: "MoveeTagId");
        }

        // Method to define the operations to reverse the migration
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims"); // Drop AspNetRoleClaims table

            migrationBuilder.DropTable(
                name: "AspNetUserClaims"); // Drop AspNetUserClaims table

            migrationBuilder.DropTable(
                name: "AspNetUserLogins"); // Drop AspNetUserLogins table

            migrationBuilder.DropTable(
                name: "AspNetUserRoles"); // Drop AspNetUserRoles table

            migrationBuilder.DropTable(
                name: "AspNetUserTokens"); // Drop AspNetUserTokens table

            migrationBuilder.DropTable(
                name: "DecodedData"); // Drop DecodedData table

            migrationBuilder.DropTable(
                name: "EventTagLog"); // Drop EventTagLog table

            migrationBuilder.DropTable(
                name: "GwInfo"); // Drop GwInfo table

            migrationBuilder.DropTable(
                name: "LobaroDataFrame"); // Drop LobaroDataFrame table

            migrationBuilder.DropTable(
                name: "LocationUser"); // Drop LocationUser table

            migrationBuilder.DropTable(
                name: "MoveeDataFrame"); // Drop MoveeDataFrame table

            migrationBuilder.DropTable(
                name: "MoveeEventComment"); // Drop MoveeEventComment table

            migrationBuilder.DropTable(
                name: "MoveeEventTag"); // Drop MoveeEventTag table

            migrationBuilder.DropTable(
                name: "AspNetRoles"); // Drop AspNetRoles table

            migrationBuilder.DropTable(
                name: "LoraData"); // Drop LoraData table

            migrationBuilder.DropTable(
                name: "AspNetUsers"); // Drop AspNetUsers table

            migrationBuilder.DropTable(
                name: "Device"); // Drop Device table

            migrationBuilder.DropTable(
                name: "MoveeEventFrame"); // Drop MoveeEventFrame table

            migrationBuilder.DropTable(
                name: "MoveeTag"); // Drop MoveeTag table

            migrationBuilder.DropTable(
                name: "EndDevice"); // Drop EndDevice table

            migrationBuilder.DropTable(
                name: "DeviceType"); // Drop DeviceType table

            migrationBuilder.DropTable(
                name: "Location"); // Drop Location table

            migrationBuilder.DropTable(
                name: "Cluster"); // Drop Cluster table

            migrationBuilder.DropTable(
                name: "Company"); // Drop Company table
        }
    }
}
