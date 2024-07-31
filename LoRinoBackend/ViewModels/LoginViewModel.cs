using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoRinoBackend.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email adresa je potrebna")]
        [EmailAddress(ErrorMessage = "Nevažeća email adresa.")]
        [Display(Name = "Email adresa")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lozinka je potrebna.")]
        [DataType(DataType.Password)]
        [Display(Name = "Lozinka")]
        public string Password { get; set; }

        [Display(Name = "Zapamti prijavu")]
        public bool RememberMe { get; set; }
    }
}
