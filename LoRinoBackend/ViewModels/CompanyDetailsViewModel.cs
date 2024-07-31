using LoRinoBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoRinoBackend.ViewModels
{
    public class CompanyDetailsViewModel
    {
        public Company Company { get; set; }

        public string PageTitle { get; set; }
    }
}
