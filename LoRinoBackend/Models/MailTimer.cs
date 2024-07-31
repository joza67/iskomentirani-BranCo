// Import namespaces required for the code
using LoRinoBackend.Hubs; // Custom namespace for SignalR hubs used in the application
using Microsoft.AspNetCore.Identity; // ASP.NET Core Identity for user management
using Microsoft.AspNetCore.SignalR; // ASP.NET Core SignalR for real-time web functionality
using Microsoft.Extensions.Logging; // Logging support for ASP.NET Core
using System; // Core .NET types and functions
using System.Collections.Generic; // Collections like Dictionary and List
using System.Diagnostics; // Debugging functions
using System.IO; // File input/output operations
using System.Linq; // LINQ queries and operations
using System.Net; // Network operations
using System.Net.Http; // HTTP client operations
using System.Net.Mail; // Email operations
using System.Threading.Tasks; // Asynchronous programming support
using System.Timers; // Timer functionality
using System.Web; // Web-related utilities
using Timer = System.Timers.Timer; // Alias Timer to avoid conflicts with other Timer classes

namespace LoRinoBackend.Models
{
    // Custom timer class extending the built-in Timer class
    public class MyTimer : Timer
    {
        // Property to store a GUID for the timer
        public string Guid { get; set; }

        // Constructor to initialize the GUID property
        public MyTimer(string guid)
        {
            Guid = guid;
        }
    }

    // Main class for managing mail notifications with timers
    public class MailTimer : IdentityUser
    {
        // Private readonly fields for dependencies injected via constructor
        private readonly AppDbContext _context; // Database context
        private readonly IDeviceRepository _deviceRepository; // Repository for device data
        private readonly IEmailSender _emailSender; // Service for sending emails
        private readonly IHubContext<WanesyHub> _hubContext; // SignalR hub context for real-time notifications
        private readonly ILocationRepository _locationRepository; // Repository for location data
        private readonly ILogger<MailTimer> logger; // Logger for logging information
        private readonly IMoveeEventRepository _moveeEventRepository; // Repository for movee events
        private readonly IMoveeDataRepository _moveeDataframe; // Repository for movee data

        // Static dictionaries and lists to manage timers and alarms
        private static Dictionary<int, Timer> _timers = new();
        private static Dictionary<int, string> _timerGuid = new();
        private static List<int> _alarmCounter = new();
        private readonly List<string> ids = new();
        private static MyTimer myTimer;

        /// <summary>
        /// Sets a timer for a location if it does not already exist.
        /// </summary>
        /// <param name="dB_Info">Database information object containing details for the timer</param>
        /// <returns>Updated database information object</returns>
        public async Task<DB_Info> SetTimer(DB_Info dB_Info)
        {
            // Check if a timer for the specified location already exists
            if (!_timers.ContainsKey(dB_Info._LocationId))
            {
                // Create a new timer for each location in dB_Info if it doesn't exist
                foreach (var location in dB_Info._Locations)
                {
                    if (dB_Info._LocationId == location.Id)
                    {
                        // Create a new GUID for the timer
                        string guid = Guid.NewGuid().ToString();
                        myTimer = new MyTimer(guid)
                        {
                            Enabled = false, // Timer initially disabled
                            Interval = dB_Info._Locations.FirstOrDefault(a => a.Id == location.Id).TimerLenght * 1000 // Timer interval in milliseconds
                        };
                        // Log the timer setup
                        Debug.WriteLine($"Setting {myTimer.Interval / 1000} second delay for {location.Name} location");
                        logger.LogInformation($"Setting {myTimer.Interval / 1000} second delay for {location.Name} location");

                        // Configure the timer to not auto-reset and add it to the dictionary
                        myTimer.AutoReset = false;
                        _timers.Add(location.Id, myTimer);

                        // Set the event handler for the timer's Elapsed event
                        myTimer.Elapsed += async (sender, e) =>
                            await MyElapsedMethod(sender, e, location.Id, location.LocationUserList.ToList(), location.Name, myTimer.Guid, dB_Info);

                        _timerGuid.Add(location.Id, guid); // Store the GUID for the timer
                        dB_Info._Guid = guid; // Update dB_Info with the new GUID

                        // Log the creation of the timer
                        Debug.WriteLine($"Created timer for {location.Name} location");
                        logger.LogInformation($"Created timer for {location.Name} location");

                        // Check if there are no users associated with the location
                        if (location.LocationUserList.Count == 0)
                        {
                            Debug.WriteLine($"NOT ADDED USER FOR {location.Name.ToUpper()} location!!!");
                            logger.LogInformation($"NOT ADDED USER FOR {location.Name.ToUpper()} location!!!");
                        }
                    }
                }
            }
            else
            {
                // Use existing timer GUID if the timer already exists
                dB_Info._Guid = _timerGuid[dB_Info._LocationId];
            }

            // If the timer is not enabled and email notifications are required
            if (_timers[dB_Info._LocationId].Enabled == false && dB_Info._EmailNotify == true)
            {
                ids.Clear(); // Clear the list of IDs
                _timers[dB_Info._LocationId].AutoReset = true; // Set timer to auto-reset
                DateTimeOffset time = DateTimeOffset.Now; // Get the current time
                long unixTimeMilliseconds = time.ToUnixTimeMilliseconds(); // Convert to Unix time
                dB_Info._TimerStarted = unixTimeMilliseconds; // Set the start time of the timer
            }

            // If email notifications are enabled
            if (dB_Info._EmailNotify == true)
            {
                _alarmCounter.Add(dB_Info._LocationId); // Add the location ID to the alarm counter
                ids.Add(dB_Info.ReturnId.ToString()); // Add the return ID to the list of IDs
                // Start or restart the timer if necessary
                foreach (var timer in _timers)
                {
                    if (timer.Key == dB_Info._LocationId)
                    {
                        if (timer.Value.Enabled == false)
                        {
                            timer.Value.Enabled = true; // Enable the timer
                            timer.Value.Start(); // Start the timer
                            // Log the alarm reception and timer start
                            Debug.WriteLine($"Recived alarm from {dB_Info._DevEui} device @ {dB_Info._LocationName} location");
                            logger.LogInformation($"Recived alarm from {dB_Info._DevEui} device @ {dB_Info._LocationName} location");
                            Debug.WriteLine($"Starting timer for {dB_Info._Locations.FirstOrDefault(a => a.Id == timer.Key).TimerLenght} seconds @ {dB_Info._LocationName} location");
                            logger.LogInformation($"Starting timer for {dB_Info._Locations.FirstOrDefault(a => a.Id == timer.Key).TimerLenght} seconds @ {dB_Info._LocationName} location");
                        }
                        else
                        {
                            // Log a new alarm reception if the timer was already running
                            Debug.WriteLine($"Recived new alarm from {dB_Info._DevEui} device @ {dB_Info._LocationName} location");
                            logger.LogInformation($"Recived new alarm from {dB_Info._DevEui} device @ {dB_Info._LocationName} location");
                        }
                    }
                }
            }
            // Log the GUID of the timer
            Debug.WriteLine($"GUID: {dB_Info._Guid}");
            // Update the alarm count for the location
            int alarmCount = _alarmCounter.Where(a => a == dB_Info._LocationId).Count();
            dB_Info._AlarmCount = alarmCount;

            return dB_Info; // Return the updated database information
        }

        /// <summary>
        /// Executes when the timer elapses.
        /// Sends an email and clears the timer.
        /// </summary>
        /// <param name="source">The source of the timer event</param>
        /// <param name="e">Event arguments for the timer</param>
        /// <param name="locationId">ID of the location</param>
        /// <param name="userIds">List of users associated with the location</param>
        /// <param name="locationName">Name of the location</param>
        /// <param name="guid">GUID of the timer</param>
        /// <param name="dB_Info">Database information object</param>
        /// <returns>Task representing the asynchronous operation</returns>
        private async Task MyElapsedMethod(Object source, ElapsedEventArgs e, int locationId, List<LocationUser> userIds, string locationName, string guid, DB_Info dB_Info)
        {
            // Check if the timer for the location exists
            if (!_timers.ContainsKey(locationId))
            {
                return;
            }
            try
            {
                // If the timer is enabled
                if (_timers[locationId].Enabled)
                {
                    _timers[locationId].AutoReset = false; // Disable auto-reset
                    _timers[locationId].Enabled = false; // Disable the timer
                    int alarmCount = _alarmCounter.Where(a => a == locationId).Count(); // Get the alarm count for the location
                    dB_Info._AlarmCount = alarmCount; // Update dB_Info with the alarm count
                    // Notify all clients via SignalR
                    await _hubContext.Clients.All.SendAsync("WanesyNotify", dB_Info.ReturnId, dB_Info._TimerStarted, dB_Info._AlarmCount, dB_Info._LocationId, dB_Info._Guid, dB_Info._MoveeEventFrameId);
                    // Log the timer elapsed event
                    logger.LogInformation($"Timer elapsed with object: {dB_Info.ReturnId}, DevEUI {dB_Info._DevEui}, {dB_Info._AlarmCount} alarms, @ {dB_Info._LocationName}, {dB_Info._Guid}");
                    Debug.WriteLine($"Timer elapsed with object: {dB_Info.ReturnId}, DevEUI {dB_Info._DevEui}, {dB_Info._AlarmCount} alarms, @ {dB_Info._LocationName}, {dB_Info._Guid}");
                    Debug.WriteLine($"Alarm count new: {alarmCount}");

                    // Create a string of IDs
                    string idsStr = "";
                    foreach (var id in ids)
                    {
                        idsStr += id + " - ";
                    }
                    Debug.WriteLine($"GUID eventa: {guid}");
                    // Log the mail sending process
                    logger.LogInformation($"Sending mail to {locationName} with {alarmCount} alarm");
                    Debug.WriteLine($"Sending mail to {locationName} with {alarmCount} alarm");
                    // Remove the timer and alarm data
                    _timers.Remove(locationId);
                    _alarmCounter.RemoveAll(a => a == locationId);
                    _timerGuid.Remove(locationId);
                    // Send email notification
                    await SendMailAsync(userIds, alarmCount, locationId, locationName, dB_Info);
                    // Optional: Send Telegram notification (currently commented out)
                    //await SendTelegramAsync(alarmCount, locationId);

                    // Clean up
                    _alarmCounter.RemoveAll(a => a == locationId);
                    _timerGuid.Remove(locationId);
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the elapsed event
                logger.Log(LogLevel.Information, "GUID: " + dB_Info._Guid + " " + ex.Message + " " + ex.InnerException);
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Sends an email notification.
        /// </summary>
        /// <param name="userIds">List of users to notify</param>
        /// <param name="alarmCount">Number of alarms</param>
        /// <param name="locationId">ID of the location</param>
        /// <param name="locationName">Name of the location</param>
        /// <param name="dB_Info">Database information object</param>
        /// <returns>Task representing the asynchronous operation</returns>
        private async Task SendMailAsync(List<LocationUser> userIds, int alarmCount, int locationId, string locationName, DB_Info dB_Info)
        {
            List<ApplicationUser> allUsers = [];
            List<string> users = [];
            allUsers = dB_Info._MailUsers;

            try
            {
                string devices = string.Empty;
                // Loop through all users and send email notifications
                foreach (var user in allUsers)
                {
                    Debug.WriteLine($"Sending mail to {user}");
                    logger.LogInformation($"Sending mail to {user}");

                    // Create SMTP client for sending email (commented out)
                    var smtp = new SmtpClient("mail.microlink.hr", 465)
                    {
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential("iot", "LdcZea7GV%+Gpa~w]*Y6eMmvZ;U'LR"), // SMTP credentials
                        EnableSsl = false,
                    };
                    // Send alarm email asynchronously
                    await _emailSender.SendAlarmMailAsync(user.Email, alarmCount.ToString(), locationName, user.Email);
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur while sending email
                Debug.WriteLine(ex.ToString());
                logger.LogInformation(ex.Message);
            }
        }

        /// <summary>
        /// Sends a notification via Telegram (currently commented out).
        /// </summary>
        /// <param name="alarmCount">Number of alarms</param>
        /// <param name="locationId">ID of the location</param>
        /// <returns>Task representing the asynchronous operation</returns>
        private async Task SendTelegramAsync(int alarmCount, int locationId)
        {
            try
            {
                var client = new HttpClient();
                // Prepare the Telegram message
                var telegramData = HttpUtility.HtmlEncode("Lokacija: " + _locationRepository.GetLocation(locationId).Name + "Novih alarma:" + alarmCount + ".\n \n" + "Molimo provjerite stanje na https://branco.microlink.hr:8086/");
                var telegramChatId = -724229468; // Chat ID for Telegram notifications
                var telegramToken = "5022058193:AAFUK0d9V-pa9mGsDu9auU1rLmJbue5JeDg"; // Bot token

                // Send the message to Telegram
                var result = await client.GetAsync(String.Format("https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}", telegramToken, telegramChatId, telegramData));
            }
            catch { }
        }

        /// <summary>
        /// Prepares the body of the email using an HTML template.
        /// </summary>
        /// <param name="uName">User name</param>
        /// <param name="alarmCount">Number of alarms</param>
        /// <param name="locationName">Name of the location</param>
        /// <returns>Email body as a string</returns>
        private async Task<string> mailBodyAsync(string uName, string alarmCount, string locationName)
        {
            string body = string.Empty;
            // Read the HTML template from file
            using (StreamReader reader = new StreamReader("./wwwroot/mailTemplateAlarm.html"))
            {
                body = await reader.ReadToEndAsync();
            }

            // Replace placeholders in the template with actual values
            body = body.Replace("{UserName}", uName);
            body = body.Replace("{AlarmCount}", alarmCount);
            body = body.Replace("{LocationName}", locationName);

            return body;
        }
    }
}
