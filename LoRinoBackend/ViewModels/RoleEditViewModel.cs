using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoRinoBackend.ViewModels
{
    public class RoleEditViewModel
    {
        [Display(Name = "ID uloge")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Ime uloge je potrebno")]
        [Display(Name = " Naziv uloge")]
        public string RoleName { get; set; }

        public List<string> Users { get; set; } = new List<string>();
    }
}
