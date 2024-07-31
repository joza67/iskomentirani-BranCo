using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoRinoBackend.Models
{
    // Represents a mailing list associated with a company and its users
    public class MailList
    {
        // Unique identifier for the mailing list
        public int Id { get; set; }

        // Company associated with the mailing list
        public Company Company { get; set; }

        // Collection of users who are part of this mailing list
        public ICollection<ApplicationUser> Users { get; set; }
    }
}
