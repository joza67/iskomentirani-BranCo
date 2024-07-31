using LoRinoBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoRinoBackend.ViewModels
{
    public class DeviceDetailsViewModel
    {
        public Device Device { get; set; }

        public int CurrentId { get; set; }

        public string PageTitle { get; set; }
    }
}
