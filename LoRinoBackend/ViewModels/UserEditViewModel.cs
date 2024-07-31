using LoRinoBackend.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoRinoBackend.ViewModels
{
    public class UserEditViewModel
    {
        public UserEditViewModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }
        [Display(Name = "ID korisnika")]
        public string Id { get; set; }

        [Display(Name = "Ime")]
        public string FirstName { get; set; }
        [Display(Name = "Prezime")]
        public string LastName { get; set; }

        [Display(Name = "Broj telefona")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Korisničko ime")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Adresa e-pošte")]
        public string Email { get; set; }

        public List<string> Claims { get; set; }

        public IList<string> Roles { get; set; }

        public List<LocationUser> UserLocations { get; set; }

        [Required]
        public int? CompanyId { get; set; }
        [Display(Name = "Tvrtka")]
        public List<SelectListItem> CompanyList { get; set; }
        [Display(Name = "Ulica")]
        public string Street { get; set; }
        [Display(Name = "Grad")]
        public string City { get; set; }
        [Display(Name = "Država")]
        public string Country { get; set; }
    }
}
