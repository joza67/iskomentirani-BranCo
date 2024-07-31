using LoRinoBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoRinoBackend.ViewModels
{
    public class LocationCreateViewModel
    {
        [Required(ErrorMessage = "Unos naziva je obavezan"), MaxLength(50, ErrorMessage = "Naziv lokacije ne smije biti dulji od 50 karaktera")]
        [Display(Name = "Naziv")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Unos geografske širine je obavezan")]
        [Display(Name = "Geo. širina")]
        [DisplayFormat(DataFormatString = "{0:N06}", ApplyFormatInEditMode = true)]
        public double Latitude { get; set; }
        [Required(ErrorMessage = "Unos geografske dužine je obavezan")]
        [Display(Name = "Geo. dužina")]
        [DisplayFormat(DataFormatString = "{0:N06}", ApplyFormatInEditMode = true)]
        public double Longitude { get; set; }
        [Required(ErrorMessage = "Unos zooma na karti je obavezan")]
        [Display(Name = "Zoom na karti")]
        [DisplayFormat(DataFormatString = "{0:N06}", ApplyFormatInEditMode = true)]
        public double MapZoom { get; set; }
        [Required(ErrorMessage = "Unos vremena sakupljanja je obavezan")]
        [Display(Name = "Vrijeme sakupljanja alarma")]
        [Range(1, 60, ErrorMessage = "Trajanje vremena sakupljanja događaja je od 1 do 60 sekundi")]
        public int TimerLenght { get; set; }
        [Required(ErrorMessage = "Unos prometnice je obavezan"), MaxLength(50, ErrorMessage = "Naziv prometnice ne smije biti dulji od 50 karaktera")]
        [Display(Name = "Prometnica")]
        public string Road { get; set; }
        [Required(ErrorMessage = "Unos dionice je obavezan"), MaxLength(50, ErrorMessage = "Naziv dionice ne smije biti dulji od 50 karaktera")]
        [Display(Name = "Dionica")]
        public string RoadSection { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public List<SelectListItem> CompaniesDDL { get; set; }


    }
}
