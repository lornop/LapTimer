using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapTimer
{
    

    class Rider
    {
        public string TagID { get; set; }
        public string RiderName { get; set; }
        public string RiderNumber { get; set; }
        public int LapNumber { get; set; }

        public DateTime lastLapTimeIn = new DateTime();
        public TimeSpan fastLapTime = new TimeSpan();
        public TimeSpan lastLapTime = new TimeSpan(0,0,0,0,0);
        public TimeSpan totalTime = new TimeSpan();


        public void RiderTimer()
        {
            
        }


        public Rider(string TagNum, string Name, string bikeNumber)
        {
            TagID = TagNum;
            RiderName = Name;
            RiderNumber = bikeNumber;
           
        }



    }





}
