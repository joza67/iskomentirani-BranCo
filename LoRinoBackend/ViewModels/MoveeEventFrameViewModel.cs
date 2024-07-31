using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoRinoBackend.Models;
using LoRinoBackend.ViewModels;

namespace LoRinoBackend.Models
{
    public class MoveeEventFrameViewModel
    {
        public MoveeEventFrame moveeEventFrame { get; set; }
        public List<MoveeDataFrame> moveeDataFrames { get; set; }
        public List<MoveeEventTag> MoveeEventTag { get; set; }
        public List<MoveeTag> MoveeTag { get; set; }
        public List<MoveeEventComment> MoveeEventComments { get; set; }
        public MoveeEventComment MoveeEventComment { get; set; }
        public IEnumerable<Device> Devices { get; set; }
        public List<LogViewModel> LogViewModel { get; set; }
        public int cntAlarm { get; set; }
        List<int> LocationIds { get; set; }
    }
}
