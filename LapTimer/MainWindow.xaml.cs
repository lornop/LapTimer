using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO.Ports;
using System.Timers;
using System.Globalization;

namespace LapTimer
{
    /// <summary>
    /// Loren Olsen
    /// ECET 230 
    /// Dec 14 2021
    /// Interaction logic for MainWindow.xaml
    /// Final project for ECET230 Class
    /// Im hoping to create a simple mx lap timer with RFID tages and a reader.
    /// This will be controlled by an arduino which will send data (tag ID) to my pc via a serial protocol
    /// Tag number will be parsed to a rider name/number and printed to screen with lap time and some other stuff maybe
    /// 
    /// 
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool bPortOpen = false;
        private int newPacketNumber, chkSumError, OverFlowNumber = 0;

        SerialPort serialPort = new SerialPort();

        DispatcherTimer dispatcherTimer =  new DispatcherTimer();

        String startTime = DateTime.Now.ToString("HH:mm:ss:fff");   //The time when the start button is pushed

        String currentTime = DateTime.Now.ToString("HH:mm:ss:fff"); //Time it is currently

        DateTime timerStartTime = new DateTime();       //Used to calculate the lap times

        public DateTime timerCurrentTime = new DateTime();  //Used to calculate lap times

        TimeSpan timerDebouncer = new TimeSpan(0, 0, 5);        //5 second debounce between RFID Tag reads for the same rider

        String TagUID;      //The Unique ID from the RFID Tag


        //Create two riders.
        Rider Cecilia = new Rider("0c682433", "K. Roczen", "94");   
        Rider Loren = new Rider("5cc5f032", "L. Olsen", "800");
        //I think if I named these after the UID instead of names it would have been easire to get the class more functional


        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            serialPort.BaudRate = 9600;
            serialPort.ReceivedBytesThreshold = 1;
            serialPort.DataReceived += SerialPort_DataReceived;
            setSerialPort();

        }

        private void setSerialPort()
        {
            string[] ports = SerialPort.GetPortNames();
            comboBox1.ItemsSource = ports;
            comboBox1.SelectedIndex = 0;

        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string text = serialPort.ReadLine();

            if (txtRecieved.Dispatcher.CheckAccess())
            {
                UpdateUI(text);
            }
            else
            {
                txtRecieved.Dispatcher.Invoke(() => { UpdateUI(text); });
            }

        }


        //Tags are
        //0c 68 24 33
        //5c c5 f0 32

        private void UpdateUI(string newPacket)
        {
            txtRecieved.Text = newPacket;
            //Function that calculates the current index for reading Chars from the data stream
            int i = 0;
            int nextIndex(int a, int l) //l is the length of the current word being read. 
            {
                i += l;     //Increment the index for the next time this is called. 
                return a;   //Return the index we recieved from the previous time this was called. 
            }
            //Returns the index i after the previous index is added to the word length.

            int calChkSum = 0;
            if (newPacket.Length > 19)
            {

                i = 0;  //start index reading at 0
                int l = 4;  //packet length is 4 chars
                if (newPacket.Substring((nextIndex(i, l)), l) == "####")
                {
                    l = 3;  //packet number is 3 digits
                    string packetnum = newPacket.Substring((nextIndex(i, l)), l);
                    int numberPackets =  Convert.ToInt32(packetnum);

                    if (numberPackets == 999)
                    {
                        OverFlowNumber++;

                    }

                    l = 8;  //RFID Tag UID are 8 chars long

                    TagUID = Convert.ToString(newPacket.Substring((nextIndex(i, l)), l));
                    txtTagUID.Text = TagUID;


                    l = 4;  //Checksum is the last 3 digits. Shouldnt reallly need this but just in case we add to the protocol ....

                    string checksum = newPacket.Substring((nextIndex(i, l)), l);

                    if (checksum == "UUUU")     //I had a hard time with the checksum in arduino. If my binary is correct UUUU should be a string of alternating 1s and 0s. Next best thing
                    {
                        //All is good, Check the lap time of the tag just in
                        txtChkSumError.Text = Convert.ToString(chkSumError);
                        checkLaptime();
                    }
                    else
                    {
                        chkSumError++;
                        txtChkSumError.Text = Convert.ToString(chkSumError);
                    }
                }

                else
                {
                    chkSumError++;
                    txtChkSumError.Text = Convert.ToString(chkSumError);
                }

            }
        }

        //Create a Timer to be used with a tick interval of 1ms when the start button is pressed 
        public void btnStart_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1);
            dispatcherTimer.Start();
            timerStartTime = DateTime.UtcNow;

        }

        //The main timer for the program
        public void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            timerCurrentTime = DateTime.UtcNow;
            TimeSpan timerTime = timerCurrentTime - timerStartTime;
            txtTimer.Text = timerTime.ToString(@"hh\:mm\:ss\.fff");

        }

        //Stop the timer when the stop button is pressed
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {

            dispatcherTimer.Stop();
        }


        public void checkLaptime()
        {

            var date1 = new DateTime(0);        //Get the current time
            if (txtTagUID.Text == "5cc5f032")
            {

                if (Cecilia.lastLapTimeIn == date1) //Fix some errors for the first lap
                {
                    Cecilia.lastLapTimeIn = timerStartTime;
                }

                TimeSpan newLapTime = timerCurrentTime - Cecilia.lastLapTimeIn;  //Get teh timespan between the current time and the last time the rider clocked in

                if (newLapTime > timerDebouncer)        //Debounce the timer
                {
                    Cecilia.lastLapTimeIn = timerCurrentTime;       //Get the new lap time and print it out
                    Cecilia.LapNumber++;
                    txtName.Text = Convert.ToString(Cecilia.RiderName);
                    txtBikeNumber.Text = Convert.ToString(Cecilia.RiderNumber);
                    txtLapNumber.Text = Convert.ToString(Cecilia.LapNumber);
                    Cecilia.lastLapTime = newLapTime;
                    txtLapTime.Text = Cecilia.lastLapTime.ToString(@"hh\:mm\:ss\.fff");     //Format the laptime for printing

                }
            }

            if (txtTagUID.Text == "0c682433")   //See comments above. 
            {
                if (Loren.lastLapTimeIn == date1)
                {
                    Loren.lastLapTimeIn = timerStartTime;
                }
                TimeSpan newLapTime = timerCurrentTime - Loren.lastLapTimeIn;

                if (newLapTime > timerDebouncer)
                {
                    Loren.lastLapTimeIn = timerCurrentTime;
                    Loren.LapNumber++;
                    txtName.Text = Convert.ToString(Loren.RiderName);
                    txtBikeNumber.Text = Convert.ToString(Loren.RiderNumber);
                    txtLapNumber.Text = Convert.ToString(Loren.LapNumber);
                    Loren.lastLapTime = newLapTime;
                    txtLapTime.Text = Loren.lastLapTime.ToString(@"hh\:mm\:ss\.fff");

                }
            }
        }


        private void btnOpenClose_Click(object sender, RoutedEventArgs e)
        {
            if (!bPortOpen)
            {
                serialPort.PortName = comboBox1.Text;
                serialPort.Open();
                btnOpenClose.Content = "Close";
                bPortOpen = true;

            }

            else
            {
                serialPort.Close();
                btnOpenClose.Content = "Open";
                bPortOpen = false;

            }
        }



        private void comboBox1_MouseEnter(object sender, MouseEventArgs e)
        {
            setSerialPort();

        }


        private void txtSend_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }





        private void txtSend_TextChanged(object sender, TextChangedEventArgs e)
        {

        }




    }
}
