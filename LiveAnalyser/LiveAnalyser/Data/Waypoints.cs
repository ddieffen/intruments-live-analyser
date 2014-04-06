using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LiveAnalyser.Controls.WaypointsControls
{
    /// <summary>
    /// Stores a list of WayPoints by name
    /// </summary>
    public partial class WayPoints
    {
        public Data.SerializableConcuentDictionary<string, WayPoint> Dic = new Data.SerializableConcuentDictionary<string, WayPoint>();
        /// <summary>
        /// Add or change this WayPoint
        /// </summary>
        /// <param name="A"></param>
        public void Add(WayPoint A) 
        {
            string name = A.Name;
            if ( ! this.Dic.TryAdd(name, A) ) 
            {
                this.Dic[name] = A;
            }
        }
        public void Add( string lat, string latDir, string lon, string lonDir, string name, string descr)
        {
            WayPoint New = new WayPoint(lat, latDir, lon, lonDir, name, descr);
            this.Add(New);
        }
        /// <summary>
        /// add new WayPoint to list
        /// </summary>
        /// <param name="lat"></param>  47 32.33 N
        /// <param name="lon"></param> 130 32,33 W
        public void Add(string lat, string lon, string name, string descr)
        {
            WayPoint New = new WayPoint(lat, lon, name, descr);
            this.Add(New);
        }

    }
}
