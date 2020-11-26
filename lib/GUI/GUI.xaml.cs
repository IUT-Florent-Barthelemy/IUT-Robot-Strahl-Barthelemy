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
using EvArgsLibrary;
using Utilities;
using System.Windows.Threading;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class interfaceRobot : Window
    {


        Location robotLocation;
        public DispatcherTimer Updater = new DispatcherTimer();
        public float anglSpeed = 0;
        public float linSpeed = 0;
        public ulong robotTimestamp = 0;

        public interfaceRobot()
        {
            Updater.Interval = new TimeSpan(0, 0, 0, 0, 200);
            Updater.Tick += Updater_Tick;

            InitializeComponent();

            ScopeAngSpeed.AddOrUpdateLine(1, 100, "Angular speed");
            ScopeLinSpeed.AddOrUpdateLine(2, 100, "Linear speed");

            Updater.Start();
            
        }

        private void Updater_Tick(object sender, EventArgs e)
        {
            
            

        }

        public void UpdateLocation(object sender, PositionDataArgs e)
        {
            robotLocation = new Location(e.XposFromOdometry, e.YposFromOdometry, e.AngleRadFromOdometry, 0, 0, 0);
            WorldMap.UpdateRobotLocation(robotLocation);
            robotTimestamp = e.Timestamp;
            ScopeAngSpeed.AddPointToLine(1, new PointD(robotTimestamp, e.VitesseAngulaireFromOdometry));
            //ScopeLinSpeed.AddPointToLine(1, new PointD(robotTimestamp, e.VitesseLineaireFromOdometry));
        }

      
    }
}
