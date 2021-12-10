using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapTimer
{

    //Create a new class for the riders
    public class Rider
    {
        public DateTime timerCurrentTime = new DateTime();
        public string TagID { get; set; }
        public string RiderName { get; set; }
        public string RiderNumber { get; set; }
        public int LapNumber { get; set; }

        public DateTime lastLapTimeIn = new DateTime();
        public TimeSpan fastLapTime = new TimeSpan();
        public TimeSpan lastLapTime = new TimeSpan(0, 0, 0, 0, 0);
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
        //I tried to get more functionality out of the class, but couldnt figure it out.
        //Im unsure of how to create more interaction between objects and access them from the main terminal
        //For example calcualting the new lap time when the UID of the riders tag is read via serial?
        //Independently of the other rider? 

        //checklaptime(UID_From_Serial)
        //    {
        //        calculatelaptime(Rider.UID_From_Serial);
        //    }

    }
}
