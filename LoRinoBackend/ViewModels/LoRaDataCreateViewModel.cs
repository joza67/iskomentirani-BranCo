using LoRinoBackend.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoRinoBackend.ViewModels
{
    public class LoRaDataCreateViewModel
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        [Display(Name = "DevEUI")]
        [Required(ErrorMessage = "The device EUI field is required")]
        [RegularExpression("^([A-Fa-f0-9]{16})$", ErrorMessage = "The device EUI field is not a valid 16 digits hexadecimal value")]
        public string DevEui { get; set; }
        [Required]
        public int FPort { get; set; }
        [Required]
        public string RawData { get; set; }
    }
}
