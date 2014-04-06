using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using LiveAnalyser.Data;
using LiveAnalyser.Model;

namespace LiveAnalyser.Controls
{
    public partial class Course : UserControl
    {
        #region private members
        int countdown = 10;
        int refreshIn = 0;
        BackgroundWorker plotsUpdater = new BackgroundWorker();

        PointPairList averagedSOW = new PointPairList();
        PointPairList maxSOW = new PointPairList();
        PointPairList minSOW = new PointPairList();
        PointPairList averagedHDGTrue = new PointPairList();
        #endregion

        #region constructors
        public Course()
        {
            InitializeComponent();

            if (!DesignMode)
            {
                plotsUpdater.DoWork += new DoWorkEventHandler(plotsUpdater_DoWork);
                plotsUpdater.RunWorkerCompleted += new RunWorkerCompletedEventHandler(plotsUpdater_RunWorkerCompleted);
                this.timer1.Enabled = true;
                refreshIn = countdown;

                GraphPane paneTrueWindSpeed = this.zedGraphControl1.GraphPane;
                paneTrueWindSpeed.Title.Text = "Speed Over Water";
                paneTrueWindSpeed.XAxis.Title.Text = "Date";
                paneTrueWindSpeed.XAxis.Type = AxisType.Date;
                paneTrueWindSpeed.XAxis.Scale.Format = "HH:mm:ss";
                paneTrueWindSpeed.YAxis.Title.Text = "Speed (kn)";
                this.zedGraphControl1.ZoomEvent += new ZedGraphControl.ZoomEventHandler(zedGraphControl1_ZoomEvent);

                GraphPane paneTrueWindDirection = this.zedGraphControl2.GraphPane;
                paneTrueWindDirection.Title.Text = "True Heading (T)";
                paneTrueWindDirection.XAxis.Title.Text = "Date";
                paneTrueWindDirection.XAxis.Type = AxisType.Date;
                paneTrueWindDirection.XAxis.Scale.Format = "HH:mm:ss";
                paneTrueWindDirection.YAxis.Title.Text = "Heading";
                this.zedGraphControl2.ZoomEvent += new ZedGraphControl.ZoomEventHandler(zedGraphControl1_ZoomEvent); 
            }
        
        }

       

        void zedGraphControl1_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            (sender as ZedGraphControl).AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            if (sender != this.zedGraphControl1)
            {
                this.zedGraphControl1.ZoomEvent -= new ZedGraphControl.ZoomEventHandler(zedGraphControl1_ZoomEvent);
                this.zedGraphControl1.GraphPane.XAxis.Scale.Min = (sender as ZedGraphControl).GraphPane.XAxis.Scale.Min;
                this.zedGraphControl1.GraphPane.XAxis.Scale.Max = (sender as ZedGraphControl).GraphPane.XAxis.Scale.Max;
                this.zedGraphControl1.Invalidate();
                this.zedGraphControl1.ZoomEvent += new ZedGraphControl.ZoomEventHandler(zedGraphControl1_ZoomEvent);
            }
            if (sender != this.zedGraphControl2)
            {
                this.zedGraphControl2.ZoomEvent -= new ZedGraphControl.ZoomEventHandler(zedGraphControl1_ZoomEvent);
                this.zedGraphControl2.GraphPane.XAxis.Scale.Min = (sender as ZedGraphControl).GraphPane.XAxis.Scale.Min;
                this.zedGraphControl2.GraphPane.XAxis.Scale.Max = (sender as ZedGraphControl).GraphPane.XAxis.Scale.Max;
                this.zedGraphControl2.Invalidate();
                this.zedGraphControl2.ZoomEvent += new ZedGraphControl.ZoomEventHandler(zedGraphControl1_ZoomEvent);
            }
        }

        void zedGraphControl_RegionChanged(object sender, EventArgs e)
        {
            if (sender != this.zedGraphControl1)
            {
                this.zedGraphControl1.ZoomEvent -= new ZedGraphControl.ZoomEventHandler(zedGraphControl1_ZoomEvent);
                this.zedGraphControl1.GraphPane.XAxis.Scale.Min = (sender as ZedGraphControl).GraphPane.XAxis.Scale.Min;
                this.zedGraphControl1.GraphPane.XAxis.Scale.Max = (sender as ZedGraphControl).GraphPane.XAxis.Scale.Max;
                this.zedGraphControl1.Invalidate();
                this.zedGraphControl1.ZoomEvent += new ZedGraphControl.ZoomEventHandler(zedGraphControl1_ZoomEvent);
            }
            if (sender != this.zedGraphControl2)
            {
                this.zedGraphControl2.ZoomEvent -= new ZedGraphControl.ZoomEventHandler(zedGraphControl1_ZoomEvent);
                this.zedGraphControl2.GraphPane.XAxis.Scale.Min = (sender as ZedGraphControl).GraphPane.XAxis.Scale.Min;
                this.zedGraphControl2.GraphPane.XAxis.Scale.Max = (sender as ZedGraphControl).GraphPane.XAxis.Scale.Max;
                this.zedGraphControl2.Invalidate();
                this.zedGraphControl2.ZoomEvent += new ZedGraphControl.ZoomEventHandler(zedGraphControl1_ZoomEvent);
            }

        }

        #endregion

        #region private methods
        void plotsUpdater_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GraphPane paneTrueWindSpeed = this.zedGraphControl1.GraphPane;
            paneTrueWindSpeed.CurveList.Clear();
            paneTrueWindSpeed.GraphObjList.Clear();
            this.zedGraphControl1.AxisChange();
            paneTrueWindSpeed.AddCurve("Speed", this.averagedSOW, Color.Green, SymbolType.XCross);
            paneTrueWindSpeed.AddCurve("Max", this.maxSOW, Color.Red, SymbolType.Plus);
            paneTrueWindSpeed.AddCurve("Min", this.minSOW, Color.Blue, SymbolType.Plus);
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Refresh();

            GraphPane paneTrueWindDirection = this.zedGraphControl2.GraphPane;
            paneTrueWindDirection.CurveList.Clear();
            paneTrueWindDirection.GraphObjList.Clear();
            paneTrueWindDirection.YAxis.Scale.Min = -10;
            paneTrueWindDirection.YAxis.Scale.Max = 370;
            this.zedGraphControl2.AxisChange();
            paneTrueWindDirection.AddCurve("Direction", this.averagedHDGTrue, Color.Green, SymbolType.XCross);
            this.zedGraphControl2.AxisChange();
            this.zedGraphControl2.Refresh();

        }
        void plotsUpdater_DoWork(object sender, DoWorkEventArgs e)
        {
            DataHolder database = Buisness.Database;
            double averagingPeriod = countdown;
            averagedSOW = new PointPairList();
            maxSOW = new PointPairList();
            minSOW = new PointPairList();
            averagedHDGTrue = new PointPairList();

            if (database != null)
            {
                //generate two sets of points: true wind speed averaged over a minute
                //and true wind speed gust as the max value over that minute
                long dataStart = -1;
                int measurePoints = 0;
                double avr = 0;
                double max = double.MinValue;
                double min = double.MaxValue;
                List<KeyValuePair<long, double>> dataSOW = database.SOW.Where(item => item.Key > 0).ToList();
                foreach (KeyValuePair<long, double> pair in dataSOW.OrderBy(i => i.Key))
                {
                    if (pair.Key > dataStart + averagingPeriod)
                    {
                        dataStart = pair.Key;
                        if (measurePoints > 0)
                        {
                            double plotDate = (double)new XDate(Tools.FromPOSIX(dataStart));
                            PointPair avrS = new PointPair(plotDate, avr / (double)measurePoints);
                            PointPair maxS = new PointPair(plotDate, max);
                            PointPair minS = new PointPair(plotDate, min);
                            avr = 0;
                            max = double.MinValue;
                            min = double.MaxValue;
                            measurePoints = 0;
                            averagedSOW.Add(avrS);
                            maxSOW.Add(maxS);
                            minSOW.Add(minS);
                        }
                    }
                    else
                    {
                        measurePoints++;
                        avr += (pair.Value / 0.514444);
                        if ((pair.Value / 0.514444) > max)
                            max = (pair.Value / 0.514444);
                        if ((pair.Value / 0.514444) < min)
                            min = (pair.Value / 0.514444);
                    }
                }

                //generate two sets of points: true wind direction averaged over a minute
                //and true wind direction gust as the max value over that minute
                //need to http://en.wikipedia.org/wiki/Mean_of_circular_quantities
                dataStart = -1;
                avr = 0;
                double avrsin = 0;
                double avrcos = 0;
                measurePoints = 0;
                List<KeyValuePair<long, double>> dataHDGT = database.HDGT.Where(item => item.Key > 0).ToList();
                foreach (KeyValuePair<long, double> pair in dataHDGT.OrderBy(i => i.Key))
                {
                    if (pair.Key > dataStart + averagingPeriod)
                    {
                        dataStart = pair.Key;
                        if (measurePoints > 0)
                        {
                            double plotDate = (double)new XDate(Tools.FromPOSIX(dataStart));
                            PointPair avrD = new PointPair(plotDate, (360 + Math.Atan2(avrsin / measurePoints, avrcos / measurePoints) * 180 / Math.PI) % 360);
                            avr = 0;
                            avrsin = 0;
                            avrcos = 0;
                            measurePoints = 0;
                            this.averagedHDGTrue.Add(avrD);
                        }
                    }
                    else
                    {
                        measurePoints++;
                        avrsin += Math.Sin(pair.Value * Math.PI / 180);
                        avrcos += Math.Cos(pair.Value * Math.PI / 180);
                    }
                }
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            countdown--;

            this.label2.Text = countdown.ToString() + " seconds";
            if (countdown <= 0 && plotsUpdater.IsBusy == false)
            {
                countdown = refreshIn;
                plotsUpdater.RunWorkerAsync();
                //this.performance1.UpdateTable();
                this.tacksJijbes1.UpdateTable();
            }
        }
        #endregion

    }
}
