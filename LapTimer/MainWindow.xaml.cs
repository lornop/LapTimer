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
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
