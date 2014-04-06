using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LiveAnalyser.Controls.PerformanceControls
{
    public partial class PerformanceLine : UserControl
    {
        public PerformanceLine()
        {
            InitializeComponent();
        }

        public PerformanceLine(string TWS, string SOW, string TWA)
        {
            InitializeComponent();
            this.label1.Text = TWS;
            this.label2.Text = SOW;
            this.label3.Text = TWA;
        }
    }
}
