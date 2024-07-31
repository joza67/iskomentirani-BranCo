using LoRinoBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoRinoBackend.ViewModels
{
    public class DeviceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DevEui { get; set; }
        public string DecryptKeys { get; set; }
        public string Description { get; set; }
        public int deviceTypeId { get; set; }
        public int CompanyId { get; set; }
    }
}
