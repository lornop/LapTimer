using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapTimer
{
    

    class Rider
    {
        public string TagID;
        public string RiderName;
        public string RiderNumber;
        public int LapTime;
        public int LapNumber;


        int lapcount1, lapcount2;

        DateTime lastLapTimeIn = new DateTime();
        TimeSpan lastLapTime = new TimeSpan();
        TimeSpan fastLapTime = new TimeSpan();


        public void RiderTimer()
        {
            
        }





    }





}
