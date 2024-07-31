using LoRinoBackend.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoRinoBackend.ViewModels
{
    public class DeviceCreateTypeViewModel
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "The device Name field is required")]
        public string Name { get; set; }
        public IFormFile Photo { get; set; }
    }
}
