using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveAnalyser.Model;
using System.Windows.Forms;
using System.ComponentModel;

namespace LiveAnalyser.Controls.WaypointsControls
{
    /// <summary>
    /// stores position, name, and description of a point
    /// </summary>
    [Serializable]
    public class WayPoint
    {
        /// <summary>
        /// The lat and lon of the point
        /// </summary>
        private Position Pos = new Position();
        /// <summary>
        /// The name of the point 
        /// </summary>
        public string name;
        /// <summary>
        /// a text description of the point, can be set to be handly to select sets of WayPoints
        /// </summary>
        public string description = "";
        public WayPoint()
        {
            Pos = new Position();
            name = "";
            description = "";
        }
        public WayPoint(string latDeg, string latMin, string latDir, string lonDeg, string lonMin, string lonDir, string Name, string Descr)
        {
            Pos = new Position(latDeg, latMin, latDir, lonDeg, lonMin, lonDir);
            name = Name;
            description = Descr;
        }
        /// <summary>
        /// create new waypoint
        /// </summary>
        /// <param name="lat"></param> 47 32.333
        /// <param name="latDir"></param> N || S
        /// <param name="lon"></param> 134 32.333
        /// <param name="lonDir"></param> W || E
        public WayPoint(string lat, string latDir, string lon, string lonDir, string Name, string Descr)
        {
            Pos = new Position(lat, latDir, lon, lonDir);
            name = Name;
            description = Descr;
        }
        /// <summary>
        /// create new WayPoint
        /// </summary>
        /// <param name="lat"></param> 47 32.333 N
        /// <param name="lon"></param> 134.32.333 W

        public WayPoint(string lat, string lon, string Name, string Descr)
        {
            Pos = new Position(lat, lon );
            name = Name;
            description = Descr;
        }
        public WayPoint(Coordinate Lat, Coordinate Lon, string Name, string Descr)
        {
            Pos = new Position(Lat, Lon);
            name = Name;
            description = Descr;
        }
        public WayPoint(Position P, string Name, string Descr)
        {
            Pos = P;
            name = Name;
            description = Descr;
        }
            

 //           if (valid)
 //           {
 //               this.waypoints.Add(wp);
 //               this.UpdateDataGridOfWayPoints();
 //               this.UpdateInternalStorageOfWayPoints();
 //               MessageBox.Show("Added successfully");
 //           }
 



        
        public void setEqual(WayPoint X)
        {
            Pos.setEqual(X.getPos);
            name = X.Name;
            description = X.Description;
        }

 //       public void setDegMin(double Lat, double Lon, string name, string desc)
 //       {
 //           lat.fromDegMin(Lat, "N");
 //           lon.fromDegMin(Lon, "W");
 //           this.name = name;
 //           this.description = desc;
 //       }

        /// <summary>
        /// moves the current position by dist NM in direct degrees
        /// </summary>
        /// <param name="direct"> direction in degrees</param>
        /// <param name="dist"> distance in NM</param>
        public void Project(int direct, double dist)
        {

            while (direct > 359) { direct -= 360; }
            direct = 360 - direct;  // switch from clockwise to counter-clockwise
            direct = direct + 90;  // zero was up
            while (direct > 359) { direct -= 360; } // back to 0 to 359
            double x = Math.Cos(direct * Math.PI / 180) * dist / 60; // min to degrees
            double y = Math.Sin(direct * Math.PI / 180) * dist / 60; // min to degrees
            x = (Math.Round(x * 60, 3)) / 60;  // round to nearest 1/100 of minute
x = (Math.Round(x * 60, 3)) / 60;  // round to nearest 1/100 of minute
            // x must be increased to account for the arc of a deg being shorter as you move away for equalor
            // assuming that is distance is short compared the the radius of the earth
            //  x = x / cos( Lat ) 
            x = x / Math.Cos(Pos.lat.Radians());
            // Natical mile = one minute
            if (Pos.lat.dir == "S")
            {
                y = -y;
            }
            if (Pos.lon.dir == "W")
            {
                x = -x;
            }
            Pos.lat.Add(y);  
            Pos.lon.Add(x);  
        }
        /// <summary>
        /// returns a NMEA sentence for uploading this waypoint
        /// </summary>
        /// <returns></returns>
        public string NMEA()
        {
            string lonNMEA = Pos.lon.toNMEA();
            while(lonNMEA.Length < 10)
            {
                lonNMEA = "0" + lonNMEA;                
            }
            string sentence = "$GPWPL," + Pos.lat.toNMEA() + "," + Pos.lon.toNMEA() + "," + name + ",*";
            string checkSum = Buisness.getChecksum(sentence);
            return sentence + checkSum;
        }
        public void Upload()
        {
            string sentence = NMEA();
            //this.debug.Text = sentence;
            MessageBox.Show("NMEA sentence=\"" + sentence + "\"");
            Buisness.COMSendLine(sentence);

            //Buisness.COMSendLine("$GPWPL,5128.62,N,00027.58,W,EGLL*59"); //tested example which works
        }

        #region accessors for bingings

        public string Name
        {
            get { return this.name; }
        }
        
        [Browsable(false)]
        public Position getPos
        {
            get { return this.Pos; }
        }

        public string Description
        {
            get { return this.description; }
        }

        #endregion
    }
}
