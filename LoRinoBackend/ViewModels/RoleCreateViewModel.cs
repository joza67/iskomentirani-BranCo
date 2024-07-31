using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoRinoBackend.ViewModels
{
    public class RoleCreateViewModel
    {
        [Required(ErrorMessage = "Naziv uloge je potreban.")]
        [Display(Name = "Naziv uloge")]
        public string RoleName { get; set; }
    }
}
