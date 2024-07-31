// Importing necessary namespaces for functionalities related to email, logging, and settings
using System; // Provides fundamental types and functions
using System.IO; // Provides types for file and stream handling
using System.Net; // Provides network-related classes, such as SmtpClient and NetworkCredential
using System.Net.Mail; // Provides classes for email operations
using System.Threading.Tasks; // Provides support for asynchronous programming
using Microsoft.Extensions.Logging; // Provides logging functionality
using Microsoft.Extensions.Options; // Provides support for options pattern
using System.Security.Cryptography.X509Certificates; // Provides types for certificate handling
using System.Net.Security; // Provides types for SSL/TLS operations

namespace LoRinoBackend.Models
{
    // Class responsible for sending emails via SMTP
    // Implements the IEmailSender interface for sending emails
    public class SmtpEmailSender : IEmailSender
    {
        // Private fields for storing SMTP settings and logger
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<SmtpEmailSender> logger;

        // Constructor to initialize SMTP settings and logger
        public SmtpEmailSender(IOptions<SmtpSettings> smtpSettings, ILogger<SmtpEmailSender> logger)
        {
            _smtpSettings = smtpSettings.Value; // Assigns SMTP settings from options
            this.logger = logger; // Assigns logger
        }

        // Method to send an email asynchronously
        public async Task SendEmailAsync(string email, string subject, string message, bool isBodyHtml)
        {
            try
            {
                // Create and configure an SmtpClient instance
                using (var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
                {
                    client.UseDefaultCredentials = false; // Disable default credentials
                    client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password); // Set credentials
                    client.EnableSsl = _smtpSettings.EnableSsl; // Enable SSL if configured

                    // Bypass SSL certificate validation (Note: Be cautious with this approach in production)
                    ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, SslPolicyErrors) => true;

                    // Create a MailMessage instance with specified details
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpSettings.Username), // Sender's email address
                        Subject = subject, // Subject of the email
                        Body = message, // Body of the email
                        IsBodyHtml = isBodyHtml, // Specifies if the body is HTML
                    };
                    mailMessage.To.Add(email); // Add recipient email address

                    // Create a unique user state for tracking the email send completion
                    string userState = Convert.ToBase64String(Guid.NewGuid().ToByteArray())[..8];

                    // Send the email asynchronously
                    await client.SendMailAsync(mailMessage);

                    // Optionally, handle completion, cancellation, or errors using events
                    client.SendCompleted += (s, e) =>
                    {
                        if (e.UserState.ToString() == userState)
                        {
                            if (e.Cancelled)
                            {
                                logger.LogInformation("Send canceled."); // Log cancellation
                            }
                            if (e.Error != null)
                            {
                                logger.LogInformation($"Error: {e.Error.ToString()}"); // Log error
                            }
                            else
                            {
                                logger.LogInformation("E-mail sent to {0}.", mailMessage.To); // Log success
                            }
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                // Log any exception that occurs during email sending
                logger.LogError(ex, "Error sending email to {Email}: {Message}", email, ex.Message);
                throw; // Optionally re-throw the exception to propagate it upwards
            }
        }

        /// <summary>
        /// Sends an alarm email to a user with details about the alarm
        /// </summary>
        /// <param name="uName">Username of the recipient</param>
        /// <param name="alarmCount">Number of alarms</param>
        /// <param name="locationName">Location related to the alarm</param>
        /// <param name="email">Recipient email address</param>
        /// <returns>A Task representing the asynchronous operation</returns>
        public async Task SendAlarmMailAsync(string uName, string alarmCount, string locationName, string email)
        {
            string body = string.Empty;
            // Read HTML template from file for the alarm email body
            using (StreamReader reader = new("./wwwroot/mailTemplateAlarm.html"))
            {
                body = await reader.ReadToEndAsync(); // Asynchronously read the template content
            }
            // Replace placeholders in the template with actual values
            body = body.Replace("{UserName}", uName);
            body = body.Replace("{AlarmCount}", alarmCount);
            body = body.Replace("{LocationName}", locationName);

            // Send the email with the prepared body
            await SendEmailAsync(email, "New alarm", body, true);
        }
    }
}
