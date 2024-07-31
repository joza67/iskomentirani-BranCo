using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoRinoBackend.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email adresa je potrebna.")]
        [EmailAddress(ErrorMessage = "Nevažeća email adresa")]
        public string Email { get; set; }
    }
}
