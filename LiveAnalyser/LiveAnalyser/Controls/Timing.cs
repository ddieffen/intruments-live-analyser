using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LiveAnalyser.Model;

namespace LiveAnalyser.Controls
{
    public partial class Timing : UserControl
    {
        long countdownStartTime = 0;
        long countdownInitialValue = 300;
        long countdown = 0;


        public Timing()
        {
            InitializeComponent();

            if (!DesignMode)
            {
                countdown = countdownInitialValue;
                countdownStartTime = Tools.ToPOSIX(System.DateTime.Now);
                timer1.Start();
            }
        }

        
        private void timer1_Tick(object sender, EventArgs e)
        {
            long now = Tools.ToPOSIX(System.DateTime.Now); //to be replace with GPS time
            countdown = countdown - (countdownStartTime - now);
            labelGpsTime.Text = TimeZone.CurrentTimeZone.ToLocalTime(Tools.FromPOSIX(Buisness.LatestGPSTimeUTC) ).ToString("hh:mm:ss");

        }
    }
}
