namespace LoRinoBackend.Models
{
    // Class representing the SMTP settings required for sending emails
    public class SmtpSettings
    {
        // The SMTP server host (e.g., "smtp.example.com")
        public string Host { get; set; }

        // The port number used by the SMTP server (e.g., 587 for TLS, 465 for SSL)
        public int Port { get; set; }

        // Indicates whether to use SSL for the SMTP connection (true or false)
        public bool EnableSsl { get; set; }

        // The username for authenticating with the SMTP server
        public string Username { get; set; }

        // The password for authenticating with the SMTP server
        public string Password { get; set; }
    }
}
