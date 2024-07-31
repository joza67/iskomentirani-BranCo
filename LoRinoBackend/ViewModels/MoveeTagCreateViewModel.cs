using System.ComponentModel.DataAnnotations;

namespace LoRinoBackend.ViewModels
{
    public class MoveeTagCreateViewModel
    {

        [Required(ErrorMessage = "Naziv je potreban")]
        [Display(Name = "Naziv")]
        public string Name { get; set; }
        public bool Active { get; set; }
        public int CompanyId { get; set; }

    }
}
