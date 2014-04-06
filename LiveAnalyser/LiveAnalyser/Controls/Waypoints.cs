using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LiveAnalyser.Controls
{
    public partial class Waypoints : UserControl
    {
        public Waypoints()
        {
            InitializeComponent();
            this.projection1.WaypointToSaveEvent += new Projection.WaypointToSave(projection1_WaypointToSaveEvent);
        }

        void projection1_WaypointToSaveEvent(WaypointsControls.WayPoint wp)
        {
            if (wp != null)
            {
                if (!String.IsNullOrEmpty(wp.name))
                {
                    bool result = this.waypointsManagement1.PushWaypoint(wp);

                    if (result)
                        MessageBox.Show("Added successfully");
                }
                else
                    MessageBox.Show("Waypoint must have a name");
            }
        }
    }
}
