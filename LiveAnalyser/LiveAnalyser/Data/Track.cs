using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LiveAnalyser.Controls.WaypointsControls;

namespace LiveAnalyser.Data
{
    public partial class Track 
    {
        /// <summary>
        /// stores a series of positions by time
        /// </summary>
        
     
        public Data.SerializableConcuentDictionary<double, Position> Dic = new Data.SerializableConcuentDictionary<double, Position>();
        /// <summary>
        /// Add or change this time - position 
        /// </summary>
        /// <param name="A"></param>
        public void Set(double T, Position P) 
        {
            if ( ! this.Dic.TryAdd(T, P) ) 
            {
                this.Dic[T] = P;
            }
        }

    }
}
