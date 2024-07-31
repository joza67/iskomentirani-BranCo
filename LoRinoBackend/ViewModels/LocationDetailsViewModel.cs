using LoRinoBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoRinoBackend.ViewModels
{
    public class LocationDetailsViewModel
    {
        public Location Location { get; set; }
        public IEnumerable<MoveeDataFrame> moveeDataFrames { get; set; }
        public List<MoveeEventFrame> moveeEventFrames { get; set; }
        public List<Device> Devices { get; set; }
        public MoveeEventFrame moveeEventFrame { get; set; }
        public int cntAlarm { get; set; }
        List<int> LocationIds { get; set; }
        public string PageTitle { get; set; }
    }
}
