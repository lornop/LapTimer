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

        StringBuilder stringBuilderSend = new StringBuilder("###1111196");

        DispatcherTimer dispatcherTimer =  new DispatcherTimer();

        String startTime = DateTime.Now.ToString("HH:mm:ss:fff");

        String currentTime = DateTime.Now.ToString("HH:mm:ss:fff");

        DateTime timerStartTime = new DateTime();

        DateTime timerCurrentTime = new DateTime();



        Rider Cecilia = new Rider("0c682433", "K. Roczen", "94");
        Rider Loren = new Rider("5cc5f032", "J. Barcia", "51");



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
                        //txtOverFlow.Text = Convert.ToString(OverFlowNumber);
                    }


                    l = 8;  //RFID Tag UID are 8 chars long

                    txtTagUID.Text = newPacket.Substring((nextIndex(i, l)), l);



                    l = 4;  //Checksum is the last 3 digits. Shouldnt reallly need this but just in case we add to the protocol ....
                            //txtRXChkSum.Text = newPacket.Substring((nextIndex(i, l)), l);

                    //    for (i = 3; i < 34; i++)
                    //    {
                    //        calChkSum += (byte)newPacket[i];
                    //    }
                    //    calChkSum %= 1000;  //To get the last threee digits like in the recieved protocol

                    //    //txtCalChkSum.Text = Convert.ToString(calChkSum);
                    //    int recChkSum = Convert.ToInt32(newPacket.Substring(34, 3));
                    //    if (recChkSum == calChkSum)
                    //    {
                    //        //DisplaySolarData(newPacket);
                    //    }

                    string checksum = newPacket.Substring((nextIndex(i, l)), l);
                    if (checksum == "UUUU")
                    {
                        //good to go
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

        public void checkLaptime()
        {


            if (txtTagUID.Text == "0c682433")
            {
                txtName.Text = Convert.ToString(Cecilia.RiderName);
                txtBikeNumber.Text = Convert.ToString(Cecilia.RiderNumber);
                timerCurrentTime = DateTime.UtcNow;
                //TimeSpan timerTime = timerCurrentTime - lastLapTime1;

                TimeSpan timerDebouncer = new TimeSpan(0, 0, 5);

                //int TimerDebouncer = Convert.ToInt32(timerTime);

                //if (timerTime > timerDebouncer)
                //{

                //}

                txtLapNumber.Text = Convert.ToString(Cecilia.LapNumber);
            }

            if (txtTagUID.Text == "0c682433")
            {

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

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            sendPacket();
        }

        private void sendPacket()
        {
            try
            {
                int txChkSum = 0;
                for (int i = 3; i < 7; i++)
                {
                    txChkSum += (byte)stringBuilderSend[i];
                }
                txChkSum %= 1000;
                stringBuilderSend.Remove(7, 3);
                stringBuilderSend.Insert(7, txChkSum.ToString("D3"));
                //txtSend.Text = stringBuilderSend.ToString();


                string messageOut = stringBuilderSend.ToString();

                messageOut += "\r\n";                   //add CR and LF
                byte[] messageBytes = Encoding.UTF8.GetBytes(messageOut);   //Convert the textbox to an array of bytes(UTF8)
                serialPort.Write(messageBytes, 0, messageBytes.Length);     //Write to the serial port the bytes, 0 offset, how many bytes are in the array
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void txtSend_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }


        public void btnStart_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1);
            dispatcherTimer.Start();

            timerStartTime = DateTime.UtcNow;


            


        }

        public void dispatcherTimer_Tick(object sender, EventArgs e)
        {


            timerCurrentTime = DateTime.UtcNow;
            //date2 = new DateTime();
            TimeSpan timerTime = timerCurrentTime - timerStartTime;

            txtTimer.Text = timerTime.ToString();






            //Tags are
            //0c 68 24 33
            //5c c5 f0 32
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            //txtRecieved.Clear();
        }

        //private void btnBit3_Click(object sender, RoutedEventArgs e)
        //{
        //    ButtonClicked(3, 3);
        //}

        //private void btnBit2_Click(object sender, RoutedEventArgs e)
        //{
        //    ButtonClicked(2, 3);
        //}

        //private void btnBit1_Click(object sender, RoutedEventArgs e)
        //{
        //    ButtonClicked(1, 3);
        //}

        //private void btnBit0_Click(object sender, RoutedEventArgs e)
        //{
        //    ButtonClicked(0, 3);
        //}

        //private void ButtonClicked(int v, int state)                //States are 0 = off, 1 = On and 3 = toggle
        //{
        //    Button[] btnBit = new Button[] { btnBit0, btnBit1, btnBit2, btnBit3 };

        //    if (state == 0)
        //    {
        //        btnBit[v].Content = "0";
        //        stringBuilderSend[v + 3] = '0';
        //    }

        //    if (state == 1)
        //    {
        //        btnBit[v].Content = "1";
        //        stringBuilderSend[v + 3] = '1';
        //    }

        //    if (state == 3)
        //    {
        //        if (btnBit[v].Content.ToString() == "0")
        //        {
        //            btnBit[v].Content = "1";
        //            stringBuilderSend[v + 3] = '1';

        //        }
        //        else
        //        {
        //            btnBit[v].Content = "0";
        //            stringBuilderSend[v + 3] = '0';
        //        }
        //    }

        //    sendPacket();
        //}

        private void txtSend_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


    }
}
