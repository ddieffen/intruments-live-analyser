using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace LiveAnalyser.Controls.CourseControls
{
    public partial class Map : UserControl
    {
        #region private members
        List<Contour> currentContour = new List<Contour>();
        #endregion

        #region public internals
        public Map()
        {
            InitializeComponent();

            GraphPane myPane = this.zedGraphControl1.GraphPane;

            // Set the titles
            myPane.Title.IsVisible = false;// = "Teams positions";
            myPane.XAxis.Title.IsVisible = false; //.Text = "Longitude";
            myPane.YAxis.Title.IsVisible = false; //.Text = "Latitude";
            myPane.Legend.IsVisible = false;

            this.zedGraphControl1.IsShowPointValues = true;
            this.zedGraphControl1.ZoomEvent += new ZedGraphControl.ZoomEventHandler(zedGraphControl1_ZoomEvent);
        }

        public void LoadContour(string filename)
        {
            string line;

            currentContour = new List<Contour>();

            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader(filename);

            Contour currContour = new Contour();
            while ((line = file.ReadLine()) != null)
            {
                if (String.IsNullOrEmpty(line) == false && line[0] != '#')
                {
                    if (line[0] != '>')
                    {
                        string[] split = line.Split('\t');
                        ContourPoint cp = new ContourPoint(Convert.ToDouble(split[1]), Convert.ToDouble(split[0]));
                        currContour.points.Add(cp);
                    }
                    else if (line[0] == '>')
                    {
                        if (currContour.points.Count > 0)
                        {
                            currentContour.Add(currContour);
                            currContour = new Contour();
                        }
                    }
                }

            }

            file.Close();
        }
        #endregion

        #region private methods

        private void zedGraphControl1_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            this.SyncAxis();
        }

        private void TraceContours(GraphPane myPane)
        {
            if (currentContour != null)
            {
                foreach (Contour cnt in currentContour)
                {
                    PointPairList ppl = new PointPairList();

                    foreach (ContourPoint cp in cnt.points)
                    {
                        ppl.Add(cp.lonE, cp.latN, 0, "");
                    }
                    LineItem myCurve = myPane.AddCurve("Contour", ppl, Color.Black, SymbolType.None);
                    myCurve.Symbol.IsVisible = false;
                    myCurve.Line.IsVisible = true;
                }
            }
        }

        private void SyncAxis()
        {
            GraphPane pane = this.zedGraphControl1.GraphPane;
            double centerY = (pane.YAxis.Scale.Max - pane.YAxis.Scale.Min) / 2 + pane.YAxis.Scale.Min;
            double centerX = (pane.XAxis.Scale.Max - pane.XAxis.Scale.Min) / 2 + pane.XAxis.Scale.Min;

            double yPixPerUnit = pane.Chart.Rect.Height / (pane.YAxis.Scale.Max - pane.YAxis.Scale.Min);
            double newUnitSpanForX = pane.Chart.Rect.Width / yPixPerUnit;
            pane.XAxis.Scale.Min = centerX - newUnitSpanForX / 2;
            pane.XAxis.Scale.Max = centerX + newUnitSpanForX / 2;
            pane.XAxis.Scale.MinAuto = false;
            pane.XAxis.Scale.MaxAuto = false;

            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Refresh();
        }

        private void CenterOnPosition(double lat, double lon)
        {
            this.zedGraphControl1.ZoomEvent -= new ZedGraphControl.ZoomEventHandler(zedGraphControl1_ZoomEvent);
            GraphPane pane = this.zedGraphControl1.GraphPane;
            double centerY = lat;
            double centerX = lon;
            double yPixPerUnit = pane.Chart.Rect.Height / (pane.YAxis.Scale.Max - pane.YAxis.Scale.Min);
            double newUnitSpanForX = pane.Chart.Rect.Width / yPixPerUnit;
            double newUnitsSpanForY = pane.Chart.Rect.Height / yPixPerUnit;
            pane.XAxis.Scale.Min = centerX - newUnitSpanForX / 2;
            pane.XAxis.Scale.Max = centerX + newUnitSpanForX / 2;
            pane.XAxis.Scale.MinAuto = false;
            pane.XAxis.Scale.MaxAuto = false;
            pane.YAxis.Scale.Min = centerY - newUnitsSpanForY / 2;
            pane.YAxis.Scale.Max = centerY + newUnitsSpanForY / 2;
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Refresh();
            this.zedGraphControl1.ZoomEvent += new ZedGraphControl.ZoomEventHandler(zedGraphControl1_ZoomEvent);
        }

        #endregion

        #region container classes

        public class Contour
        {
            public List<ContourPoint> points = new List<ContourPoint>();
        }

        public struct ContourPoint
        {
            public double latN;
            public double lonE;

            public ContourPoint(double lat, double lon)
            {
                this.latN = lat;
                this.lonE = lon;
            }
        }

        #endregion
    }
}
