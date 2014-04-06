using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using LiveAnalyser.Model;

namespace LiveAnalyser.Controls
{
    public partial class SpeedControl : UserControl
    {
        int refreshIn = 10;
        int countdown = 0;

        long lastTimeUpdated = 0;
        BackgroundWorker worker = new BackgroundWorker();
        PointPairList ppl1 = new PointPairList();
        PointPairList ppl2 = new PointPairList();
        PointPairList ppl3 = new PointPairList();
        PointPairList ppl4 = new PointPairList();
        long timeAnalysed = 900; //15 min equivalent 

        public SpeedControl()
        {
            InitializeComponent();

            if (!DesignMode)
            {
                countdown = refreshIn;

                GraphPane paneTrueWindSpeed = this.zedGraphControl1.GraphPane;
                paneTrueWindSpeed.Title.Text = "On starboard tack upwind";
                paneTrueWindSpeed.XAxis.Title.Text = "SOW (kn)";
                paneTrueWindSpeed.YAxis.Title.Text = "SOG (kn)";
                paneTrueWindSpeed.YAxis.MajorGrid.IsVisible = true;
                paneTrueWindSpeed.YAxis.MinorGrid.IsVisible = true;
                paneTrueWindSpeed.XAxis.MajorGrid.IsVisible = true;
                paneTrueWindSpeed.XAxis.MinorGrid.IsVisible = true;
                this.zedGraphControl1.ZoomEvent += new ZedGraphControl.ZoomEventHandler(zedGraphControl_ZoomEvent);

                GraphPane paneTrueWindDirection = this.zedGraphControl2.GraphPane;
                paneTrueWindDirection.Title.Text = "On starboard tack downwind";
                paneTrueWindDirection.XAxis.Title.Text = "SOW (kn)";
                paneTrueWindDirection.YAxis.Title.Text = "SOG (kn)";
                paneTrueWindDirection.YAxis.MajorGrid.IsVisible = true;
                paneTrueWindDirection.YAxis.MinorGrid.IsVisible = true;
                paneTrueWindDirection.XAxis.MajorGrid.IsVisible = true;
                paneTrueWindDirection.XAxis.MinorGrid.IsVisible = true;
                this.zedGraphControl2.ZoomEvent += new ZedGraphControl.ZoomEventHandler(zedGraphControl_ZoomEvent);

                GraphPane paneApparentWindSpeed = this.zedGraphControl3.GraphPane;
                paneApparentWindSpeed.Title.Text = "On port tack downwind";
                paneApparentWindSpeed.XAxis.Title.Text = "SOW (kn)";
                paneApparentWindSpeed.YAxis.Title.Text = "SOG (kn)";
                paneApparentWindSpeed.YAxis.MajorGrid.IsVisible = true;
                paneApparentWindSpeed.YAxis.MinorGrid.IsVisible = true;
                paneApparentWindSpeed.XAxis.MajorGrid.IsVisible = true;
                paneApparentWindSpeed.XAxis.MinorGrid.IsVisible = true;
                this.zedGraphControl3.ZoomEvent += new ZedGraphControl.ZoomEventHandler(zedGraphControl_ZoomEvent);

                GraphPane paneApparentWindDirection = this.zedGraphControl4.GraphPane;
                paneApparentWindDirection.Title.Text = "On port tack upwind";
                paneApparentWindDirection.XAxis.Title.Text = "SOW (kn)";
                paneApparentWindDirection.YAxis.Title.Text = "SOG (kn)";
                paneApparentWindDirection.YAxis.MajorGrid.IsVisible = true;
                paneApparentWindDirection.YAxis.MinorGrid.IsVisible = true;
                paneApparentWindDirection.XAxis.MajorGrid.IsVisible = true;
                paneApparentWindDirection.XAxis.MinorGrid.IsVisible = true;
                this.zedGraphControl4.ZoomEvent += new ZedGraphControl.ZoomEventHandler(zedGraphControl_ZoomEvent);

                worker.WorkerSupportsCancellation = true;
                worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

                timerPlotsUpdate.Enabled = true;
                this.lastTimeUpdated = Tools.ToPOSIX(DateTime.Now);
            }
        }

        

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            GraphPane pane1 = this.zedGraphControl1.GraphPane;
            pane1.CurveList.Clear();
            pane1.GraphObjList.Clear();
            this.zedGraphControl1.AxisChange();
            double[] ab1 = Tools.LeastSquares(ppl1);
            pane1.AddCurve(ab1[1] + "*X+" + ab1[0], Tools.CreateLinear(ab1, ppl1), Color.Red);
            LineItem myCurve1 = pane1.AddCurve("Title", ppl1, Color.Black, SymbolType.Diamond);
            myCurve1.Line.IsVisible = false;

            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Refresh();

            GraphPane pane2 = this.zedGraphControl2.GraphPane;
            pane2.CurveList.Clear();
            pane2.GraphObjList.Clear();
            this.zedGraphControl2.AxisChange();
            double[] ab2 = Tools.LeastSquares(ppl2);
            pane2.AddCurve(ab2[1] + "*X+" + ab2[0], Tools.CreateLinear(ab2, ppl2), Color.Red);
            LineItem myCurve2 = pane2.AddCurve("Title", ppl2, Color.Black, SymbolType.Diamond);
            myCurve2.Line.IsVisible = false;
            this.zedGraphControl2.AxisChange();
            this.zedGraphControl2.Refresh();

            GraphPane pane3 = this.zedGraphControl3.GraphPane;
            pane3.CurveList.Clear();
            pane3.GraphObjList.Clear();
            this.zedGraphControl3.AxisChange();
            double[] ab3 = Tools.LeastSquares(ppl3);
            pane3.AddCurve(ab3[1] + "*X+" + ab3[0], Tools.CreateLinear(ab3, ppl3), Color.Red);
            LineItem myCurve3 = pane3.AddCurve("Title", ppl3, Color.Black, SymbolType.Diamond);
            myCurve3.Line.IsVisible = false;
            this.zedGraphControl3.AxisChange();
            this.zedGraphControl3.Refresh();

            GraphPane pane4 = this.zedGraphControl4.GraphPane;
            pane4.CurveList.Clear();
            pane4.GraphObjList.Clear();
            this.zedGraphControl4.AxisChange();
            double[] ab4 = Tools.LeastSquares(ppl4);
            pane4.AddCurve(ab4[1] + "*X+" + ab4[0], Tools.CreateLinear(ab4, ppl4), Color.Red);
            LineItem myCurve4 = pane4.AddCurve("Title", ppl4, Color.Black, SymbolType.Diamond);
            myCurve4.Line.IsVisible = false;
            this.zedGraphControl4.AxisChange();
            this.zedGraphControl4.Refresh();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            long startTime = Tools.ToPOSIX(DateTime.Now);
            if (this.timeAnalysed != -1)
                startTime = Tools.ToPOSIX(DateTime.Now) - this.timeAnalysed;
            else
                startTime = Math.Min(Buisness.Database.AWA.Keys.Min(),
                    Math.Min(Buisness.Database.SOG.Keys.Min(), Buisness.Database.SOW.Keys.Min()));

            long endTime = Tools.ToPOSIX(DateTime.Now);

            long interval = refreshIn;
            for (long i = startTime; i < endTime; i = i + interval)
            {
                List<KeyValuePair<long, double>> AWA = Buisness.Database.AWA.Where(pair => pair.Key >= i && pair.Key < i + interval).OrderBy(item => item.Key).ToList();
                List<KeyValuePair<long, double>> SOW = Buisness.Database.SOW.Where(pair => pair.Key >= i && pair.Key < i + interval).OrderBy(item => item.Key).ToList();
                List<KeyValuePair<long, double>> SOG = Buisness.Database.SOG.Where(pair => pair.Key >= i && pair.Key < i + interval).OrderBy(item => item.Key).ToList();

                //generate two sets of points: true wind direction averaged over a minute
                //and true wind direction gust as the max value over that minute
                //need to http://en.wikipedia.org/wiki/Mean_of_circular_quantities
                double avrAWA = 0;
                double avrsin = 0;
                double avrcos = 0;
                int measurePoints = 0;
                foreach (KeyValuePair<long, double> pair in AWA)
                {
                    avrsin += Math.Sin(pair.Value * Math.PI / 180);
                    avrcos += Math.Cos(pair.Value * Math.PI / 180);
                    measurePoints++;
                }
                avrAWA = (360 + Math.Atan2(avrsin / measurePoints, avrcos / measurePoints) * 180 / Math.PI) % 360;

                double avrSOG = 0;
                measurePoints = 0;
                foreach (KeyValuePair<long, double> pair in SOG)
                {
                    avrSOG += pair.Value;
                    measurePoints++;
                }
                avrSOG = avrSOG / measurePoints / 0.514444;

                double avrSOW = 0;
                measurePoints = 0;
                foreach (KeyValuePair<long, double> pair in SOW)
                {
                    avrSOW += pair.Value;
                    measurePoints++;
                }
                avrSOW = avrSOW / measurePoints / 0.514444;

                if (avrAWA > 0 && avrAWA <= 90)
                    this.ppl1.Add(avrSOW, avrSOG);
                else if (avrAWA > 90 && avrAWA <= 180)
                    this.ppl2.Add(avrSOW, avrSOG);
                else if (avrAWA > 180 && avrAWA <= 270)
                    this.ppl3.Add(avrSOW, avrSOG);
                else if (avrAWA > 270)
                    this.ppl4.Add(avrSOW, avrSOG);
            }
        }

        #region private methods

        void zedGraphControl_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            (sender as ZedGraphControl).AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            if (sender != this.zedGraphControl1)
            {
                this.zedGraphControl1.ZoomEvent -= new ZedGraphControl.ZoomEventHandler(zedGraphControl_ZoomEvent);
                this.zedGraphControl1.GraphPane.XAxis.Scale.Min = (sender as ZedGraphControl).GraphPane.XAxis.Scale.Min;
                this.zedGraphControl1.GraphPane.XAxis.Scale.Max = (sender as ZedGraphControl).GraphPane.XAxis.Scale.Max;
                this.zedGraphControl1.Invalidate();
                this.zedGraphControl1.ZoomEvent += new ZedGraphControl.ZoomEventHandler(zedGraphControl_ZoomEvent);
            }
            if (sender != this.zedGraphControl2)
            {
                this.zedGraphControl2.ZoomEvent -= new ZedGraphControl.ZoomEventHandler(zedGraphControl_ZoomEvent);
                this.zedGraphControl2.GraphPane.XAxis.Scale.Min = (sender as ZedGraphControl).GraphPane.XAxis.Scale.Min;
                this.zedGraphControl2.GraphPane.XAxis.Scale.Max = (sender as ZedGraphControl).GraphPane.XAxis.Scale.Max;
                this.zedGraphControl2.Invalidate();
                this.zedGraphControl2.ZoomEvent += new ZedGraphControl.ZoomEventHandler(zedGraphControl_ZoomEvent);
            }
            if (sender != this.zedGraphControl3)
            {
                this.zedGraphControl3.ZoomEvent -= new ZedGraphControl.ZoomEventHandler(zedGraphControl_ZoomEvent);
                this.zedGraphControl3.GraphPane.XAxis.Scale.Min = (sender as ZedGraphControl).GraphPane.XAxis.Scale.Min;
                this.zedGraphControl3.GraphPane.XAxis.Scale.Max = (sender as ZedGraphControl).GraphPane.XAxis.Scale.Max;
                this.zedGraphControl3.Invalidate();
                this.zedGraphControl3.ZoomEvent += new ZedGraphControl.ZoomEventHandler(zedGraphControl_ZoomEvent);
            }
            if (sender != this.zedGraphControl4)
            {
                this.zedGraphControl4.ZoomEvent -= new ZedGraphControl.ZoomEventHandler(zedGraphControl_ZoomEvent);
                this.zedGraphControl4.GraphPane.XAxis.Scale.Min = (sender as ZedGraphControl).GraphPane.XAxis.Scale.Min;
                this.zedGraphControl4.GraphPane.XAxis.Scale.Max = (sender as ZedGraphControl).GraphPane.XAxis.Scale.Max;
                this.zedGraphControl4.Invalidate();
                this.zedGraphControl4.ZoomEvent += new ZedGraphControl.ZoomEventHandler(zedGraphControl_ZoomEvent);
            }
        }

        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            countdown--;

            this.labelCountdown.Text = countdown.ToString() + " seconds";
            if (countdown <= 0 && worker.IsBusy == false)
            {
                countdown = refreshIn;
                UpdateCharts();
            }
        }

        private void UpdateCharts()
        {
            long now = Tools.ToPOSIX(DateTime.Now);
            if(!worker.IsBusy)
                worker.RunWorkerAsync();

            lastTimeUpdated = Tools.ToPOSIX(DateTime.Now);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            this.labelSOW.Text = Buisness.LatestSOW.ToString("F1") + "kn";
            this.labelSOG.Text = Buisness.LatestSOG.ToString("F1") + "kn";
            this.labelAWA.Text = Buisness.LatestAWA.ToString("F1") + "deg";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedItem != null)
            {
                switch (((ComboBox)sender).SelectedItem.ToString())
                {
                    case "Everything":
                        timeAnalysed = -1;
                        break;
                    case "24 hours":
                        timeAnalysed = 60 * 60 * 24;
                        break;
                    case "12 hours":
                        timeAnalysed = 60 * 60 * 12;
                        break;
                    case "6 hours":
                        timeAnalysed = 60 * 60 * 6;
                        break;
                    case "1 hour":
                        timeAnalysed = 60 * 60;
                        break;
                    case "30 min":
                        timeAnalysed = 60 * 30;
                        break;
                    case "15 min":
                        timeAnalysed = 60 * 15;
                        break;
                }

                this.UpdateCharts();
            }
        }
    }
}
