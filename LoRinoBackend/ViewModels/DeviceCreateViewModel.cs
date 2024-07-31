using LoRinoBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoRinoBackend.ViewModels
{
    public class DeviceCreateViewModel
    {
        [Display(Name = "Naziv uređaja")]
        [Required(ErrorMessage = "Naziv uređaja je potrebno")]
        public string Name { get; set; }
        [Display(Name = "DevEUI")]
        [Required(ErrorMessage = "DevEUI je potreban.")]
        [RegularExpression("^([A-Fa-f0-9]{16})$", ErrorMessage = "DevEUI je valjani 16 znamenkasti heksadekatski zapis.")]
        public string DevEui { get; set; }
        [Required(ErrorMessage = "Tip uređaja je potreban")]
        public int DeviceTypeId { get; set; }
        [Display(Name = "Tip uređaja")]
        public List<SelectListItem> DeviceTypeList { get; set; }
        [Display(Name = "Tvrtka")]
        //[Required(ErrorMessage = "Odabrati tvrtku je potrebno.")]
        public int CompanyId { get; set; }
        [Display(Name = "Tvrtka")]
        public List<SelectListItem> CompanyList { get; set; }
        [Display(Name = "Lokacija")]
        //[Required(ErrorMessage = "Odabrati lokacija je potrebno.")]
        public int LocationId { get; set; }
        [Display(Name = "Lokacija")]
        public List<SelectListItem> LocationList { get; set; }

        public Location Location { get; set; }
        public Company Company { get; set; }
        public IEnumerable<Device> Devices { get; set; }

        [Display(Name = "Geo. Širina")]
        [DisplayFormat(DataFormatString = "{0:N06}", ApplyFormatInEditMode = true)]
        //[RegularExpression("^[,][0-9]+$|[0-9]*[,]*[0-9]+$", ErrorMessage = "The device Latitude field is not a valid decimal digits value")]
        public double Latitude { get; set; }
        //[RegularExpression("^[,][0-9]+$|[0-9]*[,]*[0-9]+$", ErrorMessage = "The device Longitude field is not a valid decimal digits value")]

        [Display(Name = "Geo. dužina")]
        [DisplayFormat(DataFormatString = "{0:N06}", ApplyFormatInEditMode = true)]
        public double Longitude { get; set; }
        [BindProperty, DataType(DataType.Date)]
        [Display(Name = "Ističe datuma")]
        public DateTime Expires { get; set; }
    }
}
