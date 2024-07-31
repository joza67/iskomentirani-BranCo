using LoRinoBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoRinoBackend.ViewModels
{
    public class HomeDetailsViewModel
    {
        public LoRaData CurrentLoRaData { get; set; }
        public string PageTitle { get; set; }
    }
}
