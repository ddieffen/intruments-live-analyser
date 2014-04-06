using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace LiveAnalyser.Controls.WaypointsControls
{
    [Serializable]
    public class Position
    {
        public Coordinate lat = new Coordinate();
        public Coordinate lon = new Coordinate();
        //public double time = 0;
        public Position()
        {
        }
        public Position(Coordinate Lat, Coordinate Lon)
        {
            lat = Lat;
            lon = Lon;
            CheckDir(Lat.dir, Lon.dir); 
        }
        public Position( string Lat, string LatDir, string Lon, string LonDir )
        {            
            lat = new Coordinate( Lat, LatDir );
            lon = new Coordinate( Lon, LonDir );
            CheckDir(LatDir, LonDir); 
        }
        public Position(string LatDeg, string LatMin, string LatDir, string LonDeg, string LonMin, string LonDir)
        {
            lat = new Coordinate(LatDeg, LatMin, LatDir);
            lon = new Coordinate(LonDeg, LonMin, LonDir);
            CheckDir(LatDir, LonDir);
        }
        public Position( string Lat, string Lon)
        {            

            lat = new Coordinate( Lat );
            lon = new Coordinate( Lon );
            CheckDir(lat.dir, lon.dir);
  
        }
        private bool CheckDir(string LatDir, string LonDir)
        {
            if (LatDir != "N" && LatDir != "S")
            {
                MessageBox.Show("Direction of \"" + LatDir + " is invalid for Lat, must be N or S");
                return false;
            }
            else if (LonDir != "W" && LonDir != "E")
            {
                MessageBox.Show("Direction of \"" + LonDir + " is invalid for Lon, must be W or E");
                return false;
            }
            else
            {
                return true;
            }
        }
        public void setEqual(Position X)
        {
            lat.setEqual(X.lat);
            lon.setEqual(X.lon);
        }
    }
}
