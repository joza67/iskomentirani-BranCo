// Import necessary namespaces
using Microsoft.Extensions.Logging; // Provides logging functionalities
using System; // Provides basic functionalities
using System.Collections.Generic; // Provides collection types like List
using System.IO; // Provides I/O functionalities for file handling
using System.Linq; // Provides LINQ functionalities for data querying
using System.Net; // Provides networking functionalities
using System.Net.Http; // Provides HTTP client functionalities
using System.Net.Mail; // Provides functionalities for sending email
using System.Threading.Tasks; // Provides asynchronous programming functionalities
using System.Timers; // Provides timer functionalities
using System.Web; // Provides ASP.NET request and response management

namespace LoRinoBackend.Models
{
    // Static class for managing a custom timer
    public static class CustomTimer
    {
        // Static field for the timer instance
        private static System.Timers.Timer aTimer = new System.Timers.Timer();

        // Method to set up the timer
        public static void SetTimer()
        {
            // Debug message to check timer status
            System.Diagnostics.Debug.WriteLine("Checking timer");

            // Check if the timer is not already enabled
            if (aTimer.Enabled == false)
            {
                System.Diagnostics.Debug.WriteLine("Creating new timer"); // Debug message for timer creation

                aTimer.Interval = 5000; // Set the timer interval to 5000 milliseconds (5 seconds)
                aTimer.Elapsed += OnTimedEvent; // Subscribe to the Elapsed event
                aTimer.AutoReset = true; // Set the timer to reset automatically
                aTimer.Enabled = true; // Enable the timer
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Use existing"); // Debug message indicating the use of an existing timer
            }
        }

        // Event handler for the timer's Elapsed event
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Check if the timer is enabled
            if (aTimer.Enabled)
            {
                System.Diagnostics.Debug.WriteLine("Timer is Elepsed"); // Debug message indicating the timer has elapsed

                // Example code block for creating and adding a new event frame
                // MoveeEventFrame newEvent = new MoveeEventFrame
                // {
                //     RecvTime = 1234,
                //     MaxValue = 8,
                //     Status = 0,
                //     AckByt = "asdf",
                //     AckTime = 2323
                // };

                // SQLMoveeEventRepository moveeEventRepository = new SQLMoveeEventRepository();

                // moveeEventRepository.Add(newEvent);

                // _moveeEventRepository.Add(newEvent);

                // Disable the timer after the event
                aTimer.Enabled = false;
            }
        }
    }
}
