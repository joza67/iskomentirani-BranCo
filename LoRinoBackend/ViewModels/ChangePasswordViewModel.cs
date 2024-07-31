using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoRinoBackend.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Trenutna lozinka je potrebna.")]
        [DataType(DataType.Password)]
        [Display(Name = "Trenutna lozinka")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Nova lozinka je potrebna.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nova lozinka")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potvrdite lozinku")]
        [Compare("NewPassword", ErrorMessage =
            "Lozinke se ne podudaraju.")]
        public string ConfirmPassword { get; set; }
    }
}
