using LoRinoBackend.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoRinoBackend.ViewModels
{
    public class CompanyCreateViewModel
    {
        [Required(ErrorMessage = "Ime je potrebno"), MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        [Display(Name = "Ime")]
        public string Name { get; set; }
        [Display(Name = "Adresa e-pošte tvrtke")]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Nevažeća adresa e-pošte")]
        [Required(ErrorMessage = "Adresa e-pošte je potrebno")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Ulica je potrebna")]
        [Display(Name = "Ulica")]
        public string Street { get; set; }
        [Required(ErrorMessage = "Grad je potreban")]
        [Display(Name = "Grad")]
        public string City { get; set; }
        [Required(ErrorMessage = "Država je potrebna")]
        [Display(Name = "Država")]
        public string Country { get; set; }
        public IFormFile Photo { get; set; }
    }
}
