using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapTimer
{


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


        //public void newcheckLaptime(String UID)
        //{
        //    //timerCurrentTime = DateTime.UtcNow;

        //    var date1 = new DateTime(0);
        //    if (UID == "5cc5f032")
        //    {


        //        if (this.lastLapTimeIn == date1)
        //        {
        //            this.lastLapTimeIn = timerStartTime;
        //        }


        //        TimeSpan newLapTime = timerCurrentTime - Cecilia.lastLapTimeIn;

        //        if (newLapTime > timerDebouncer)
        //        {
        //            Cecilia.lastLapTimeIn = timerCurrentTime;
        //            Cecilia.LapNumber++;
        //            txtName.Text = Convert.ToString(Cecilia.RiderName);
        //            txtBikeNumber.Text = Convert.ToString(Cecilia.RiderNumber);
        //            txtLapNumber.Text = Convert.ToString(Cecilia.LapNumber);
        //            Cecilia.lastLapTime = newLapTime;
        //            txtLapTime.Text = Cecilia.lastLapTime.ToString(@"hh\:mm\:ss\.fff");

        //        }

        //    }

        //    if (txtTagUID.Text == "0c682433")
        //    {
        //        if (Loren.lastLapTimeIn == date1)
        //        {
        //            Loren.lastLapTimeIn = timerStartTime;
        //        }
        //        TimeSpan newLapTime = timerCurrentTime - Loren.lastLapTimeIn;

        //        if (newLapTime > timerDebouncer)
        //        {
        //            Loren.lastLapTimeIn = timerCurrentTime;
        //            Loren.LapNumber++;
        //            txtName.Text = Convert.ToString(Loren.RiderName);
        //            txtBikeNumber.Text = Convert.ToString(Loren.RiderNumber);
        //            txtLapNumber.Text = Convert.ToString(Loren.LapNumber);
        //            Loren.lastLapTime = newLapTime;
        //            txtLapTime.Text = Loren.lastLapTime.ToString(@"hh\:mm\:ss\.fff");

        //        }
        //    }
        //}

    }
}
