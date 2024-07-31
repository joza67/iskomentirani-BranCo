using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoRinoBackend.Models;

namespace LoRinoBackend.Models
{
    public class HomeDataFrameViewModel
    {
        public MoveeDataFrame moveeDataFrame { get; set; }

        public int cntAlarm { get; set; }
        public int totalMsg { get; set; }
        public int msgToday{ get; set; }
        public int msgYesterDay { get; set; }
        public HomeDataFrameViewModel()
        {
            moveeDataFrame = new MoveeDataFrame();
            cntAlarm = 0;
            totalMsg = 0;
            msgToday = 0;
            msgYesterDay = 0;
        }

    }
}
