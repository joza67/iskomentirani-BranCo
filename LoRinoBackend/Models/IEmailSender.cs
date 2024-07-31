// Import necessary namespaces
using System.Threading.Tasks; // Provides asynchronous programming functionalities

namespace LoRinoBackend.Models
{
    // Interface defining the contract for email sending operations
    public interface IEmailSender
    {
        // Asynchronous method to send an email
        // Parameters:
        // - email: The recipient's email address
        // - subject: The subject of the email
        // - message: The body of the email
        // - isBodyHtml: A boolean indicating whether the email body is in HTML format
        Task SendEmailAsync(string email, string subject, string message, bool isBodyHtml);

        // Asynchronous method to send an alarm notification email
        // Parameters:
        // - uName: The name of the user
        // - alarmCount: The count of alarms
        // - locationName: The name of the location
        // - email: The recipient's email address
        Task SendAlarmMailAsync(string uName, string alarmCount, string locationName, string email);
    }
}
