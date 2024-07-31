using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoRinoBackend.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ime je potrebno")]
        [Display(Name = "Ime")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Prezime je potrebno")]
        [Display(Name = "Prezime")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Adresa e-pošte je potrebna")]
        [EmailAddress(ErrorMessage = "Nevažeća adresa e-pošte")]
        [Remote(action: "IsEmailInUse", controller: "Account")]
        [Display(Name = "E-pošta")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lozinka je potrebna")]
        [DataType(DataType.Password)]
        [Display(Name = "Lozinka")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potvrdite lozinku")]
        [Compare("Password",
            ErrorMessage = "Lozinke se ne podudaraju")]
        public string ConfirmPassword { get; set; }

        [Required (ErrorMessage = "Naziv tvrtke je potreban")]
        public int? CompanyId { get; set; }
        [Display(Name = "Naziv tvrtke")]
        public List<SelectListItem> CompanyList { get; set; }
    }
}
