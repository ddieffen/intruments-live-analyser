using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LiveAnalyser.Controls.WaypointsControls
{
    [Serializable]
    public class Coordinate
    {
        /// <summary>
        /// Coordinate in decimal degrees
        /// </summary>
        public double degrees = 0;
        /// <summary>
        /// Direction for that coordinate
        /// N = North
        /// E = East
        /// S = South
        /// W = West
        /// </summary>
        public string dir = "";
        private void Set(double Deg, string Dir)
        {
            dir = Dir.ToUpper();
            degrees = Deg;

            if (dir == "E" || dir == "W")
            {
                if (degrees > 180 || degrees < 0)
                {
                    MessageBox.Show("degrees = " + degrees.ToString() + " is invalid for lon");
                }
            }
            else if (dir == "N" || dir == "S")
            {
                if (degrees > 90 || degrees < 0)
                {
                    MessageBox.Show("degrees = " + degrees.ToString() + " is invalid for lat");
                }
            }
            else
            {
                MessageBox.Show("direction of " + dir + " is invalid");
            }
        }
        public Coordinate()
        {
            degrees = 0;
            dir = "";
        }
        /// <summary>
        /// set based on inputs of 47.345, "N"
        /// </summary>
        public Coordinate(double Deg, string Dir)
        {
            this.Set(Deg, Dir);
        }
        /// <summary>
        /// set based on inputs of 4725.345, "N"
        /// Should remove this by parsing MNEA into string, not double
        /// </summary>
        public void fromNMEA(double DegMin, string Dir)
        {
            double d = Math.Floor(DegMin / 100);
            double min = (DegMin - (d * 100));
            this.Set ( d + min / 60, Dir);
        }
 
        /// <summary>
        /// set based on inputs of "47° 25.345" or "47° 5.345" or 47 25.345 or 45.787
        /// </summary>
        public Coordinate(string DegMin, string Dir)
        {
            this.Set(parseDegMin(DegMin), Dir);
        }
        public Coordinate(string Deg, string Min, string Dir)
        {
            double DegD = 0;
            double MinD = 0;
            if (!Double.TryParse(Deg, out DegD))
            {
                MessageBox.Show("degrees = " + Deg + " is invalid");
            }
            if ( !Double.TryParse(Min, out MinD) )
            {
                MessageBox.Show("Minutes = " + Min + " is invalid");
            }
            DegD = DegD + MinD / 60;
            this.Set(DegD, Dir);
        }
        private double parseDegMin(string DegMin)
        {
            string Deg = Regex.Replace(DegMin, "^[^0-9.]", ""); //remove leading blanks, etc
            Deg = Regex.Replace(DegMin, "[^0-9.]", "x"); //47xx25.345xx
            if ( Deg.IndexOf("x") < 0 ) 
            {
                // assume ddmm.mmm where dd is deg and mm.mmm is mintues
                //int dot = Deg.IndexOf(".");
                //string a = Deg.Substring(0, (Deg.IndexOf(".") - 2));
                //string b = Deg.Substring((Deg.IndexOf(".") - 2));
                Deg = Deg.Substring(0,(Deg.IndexOf(".")-2)) + "x" + Deg.Substring((Deg.IndexOf(".")-2)); // ddxmm.mmm 
            }

            string D = Deg.Substring(0, Deg.IndexOf("x"));      //47
            string Min = Deg.Substring(Deg.IndexOf("x"));       //  xx25.345xx
            Min = Regex.Replace(Min, "[^0-9.]", ""); //25.345
            double DegD;
            Double.TryParse(D, out DegD);
            double DegM;
            Double.TryParse(Min, out DegM);
            DegD = DegD + DegM / 60;
            return DegD;
        }  
        /// <summary>
        /// set based on inputs of "47° 25.345 N" or "47° 5.345 N" or 47 25.345N
        /// </summary>
        public Coordinate(string DegMin)
        {
            string Dir = Regex.Replace(DegMin.ToUpper(), "[^NESW.]", "");
            this.Set(parseDegMin(DegMin), Dir);
        }

        /// <summary>
        /// return part of the NMEA sentence for this cooridate: 4725.345,N
        /// </summary>
        /// <returns></returns>
        public string toNMEA()
        {
            double d = Math.Floor(degrees);
            double min = (degrees - d) * 60;
            string strMin = min.ToString("F3");
            if (min < 10) { strMin = "0" + strMin; }
            string strDeg = d.ToString("F0");
            if ((dir == "E" || dir == "W") && d < 100) { strDeg = "0" + strDeg; }
            return  strDeg + strMin + "," + dir.ToUpper();
        }

      
        /// <summary>
        /// returns the angle of the cooridate in Radians
        /// </summary>
        /// <returns></returns>
        public double Radians()
        {
            return degrees * Math.PI / 180;
        }
        private void compDir()
        {              
            if (dir == "N")
            {
                dir = "S";
            }
            else if (dir == "S")
            {
                dir = "N";
            }
            else if (dir == "W")
            {
                dir = "E";
            }
            else if (dir == "E")
            {
                dir = "W";
            }
        }
        /// <summary>
        /// Adds degrees to Coordinate
        /// </summary>
        /// <param name="D">degrees to add</param>
        public void Add(double D)
        {
            degrees += D;
            if (degrees < 0)
            {
                // crossed equ or w/e med
                degrees = -degrees;
                compDir();
            }
            if ((dir == "N" || dir == "S") && degrees > 90)
            {
                // crossed over one of the poles
                degrees = 180 - degrees;
            }
            if ((dir == "E" || dir == "W") && degrees > 180)
            {
                // crossed over e/w med + 180
                degrees = 360 - degrees;
                compDir();
            }
            if (dir == "E" || dir == "W")
            {
                if (degrees > 180 || degrees < 0)
                {
                    MessageBox.Show("degrees = " + degrees.ToString() + " is invalid for lon");
                }
            }
            else if (dir == "N" || dir == "S")
            {
                if (degrees > 90 || degrees < 0)
                {
                    MessageBox.Show("degrees = " + degrees.ToString() + " is invalid for lat");
                }
            }
        }
        public string displayDeg()
        {
            double d = Math.Floor(degrees);
            string strDeg = d.ToString("F0");
            if (d < 10) { strDeg = "0" + strDeg; }
            if ((dir == "E" || dir == "W") && d < 100) { strDeg = "0" + strDeg; }
            return strDeg;
        }
        public string displayMin()
        {
            double d = Math.Floor(degrees);
            double min = (degrees - d) * 60;
            string strMin = min.ToString("F3");
            if (min < 10) { strMin = "0" + strMin; }
            return strMin;
        }
        public string displayDegMin()
        {
            double d = Math.Floor(degrees);
            double min = (degrees - d) * 60;
            string strMin = min.ToString("F3"); 
            string strDeg = d.ToString("F0");
            if (min < 10) { strMin = "0" + strMin; } 
            if (d < 10) { strDeg = "0" + strDeg; }
            if ((dir == "E" || dir == "W") && d < 100) { strDeg = "0" + strDeg; }
            return strDeg + "° " + strMin + "\'\' ";
        }
        ///<summary>
        /// Return "47° 25.345 N"
        ///</summary>
        public string displayDegMinDir()
        {
            double d = Math.Floor(degrees);
            double min = (degrees - d) * 60;
            string strMin = min.ToString("F3");
            string strDeg = d.ToString("F0");
            if (min < 10) { strMin = "0" + strMin; } 
            if (d < 10) { strDeg = "0" + strDeg; }
            if ((dir == "E" || dir == "W") && d < 100 ) { strDeg = "0" + strDeg; }
            return strDeg + "° " + strMin + "\'\' " + dir;
        }

        public void setEqual(Coordinate X)
        {
            degrees = X.degrees;
            dir = X.dir;
        }
    }
}
