using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveAnalyser.Controls.CourseControls
{
    public class Manoeuver
    {
        public string Type { get; set; }
        public long StartTime { get; set; }
        public long StartSOW { get; set; }
        public long EndTime { get; set; }
        public long EndSOW { get; set; }
    }
}
