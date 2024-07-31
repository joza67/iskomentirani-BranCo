using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoRinoBackend.Models;

namespace LoRinoBackend.Models
{
    public class MoveeDataFrameViewModel
    {
        public MoveeDataFrame moveeDataFrame { get; set; }
        public int cntAlarm { get; set; }
        List<int> LocationIds { get; set; }
    }
}
