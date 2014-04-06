using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LiveAnalyser.Model;
using LiveAnalyser.Data;
using ZedGraph;

namespace LiveAnalyser.Controls
{
    public partial class Weather : UserControl
    {
        #region private members
       
        int refreshIn = 15;
        int countdown = 0;

        BackgroundWorker plotsUpdater = new BackgroundWorker();

        PointPairList averagedTrueWindSpeed = new PointPairList();
        PointPairList maxTrueWindSpeed = new PointPairList();
        PointPairList minTrueWindSpeed = new PointPairList();
        PointPairList averagedTrueWindDirection = new PointPairList();
        PointPairList averagedAparentWindDirection = new PointPairList();
        PointPairList averagedAparentWindSpeed = new PointPairList();

        long timeAnalysed = 900;//15 minutees;

        #endregion

        #region constructor
        
        public Weather()
        {
            InitializeComponent();

            if (!DesignMode)
            {
                GraphPane paneTrueWindSpeed = this.zedGraphControl1.GraphPane;
                paneTrueWindSpeed.Title.Text = "True Wind Speed";
                paneTrueWindSpeed.XAxis.Title.Text = "Date";
                paneTrueWindSpeed.XAxis.Type = AxisType.Date;
                paneTrueWindSpeed.XAxis.Scale.MajorUnit = DateUnit.Hour;
                paneTrueWindSpeed.YAxis.Title.Text = "Speed (kn)";
                //this.zedGraphControl1.ZoomEvent += new ZedGraphControl.ZoomEventHandler(zedGraphControl_ZoomEvent);

                GraphPane paneTrueWindDirection = this.zedGraphControl2.GraphPane;
                paneTrueWindDirection.Title.Text = "True Wind Direction (T)";
                paneTrueWindDirection.XAxis.Title.Text = "Date";
                paneTrueWindDirection.XAxis.Type = AxisType.Date;
                paneTrueWindDirection.XAxis.Scale.MajorUnit = DateUnit.Hour;
                paneTrueWindDirection.YAxis.Title.Text = "Direction";
                //this.zedGraphControl2.ZoomEvent += new ZedGraphControl.ZoomEventHandler(zedGraphControl_ZoomEvent);

                GraphPane paneApparentWindSpeed = this.zedGraphControl3.GraphPane;
                paneApparentWindSpeed.Title.Text = "Apparent Wind Speed";
                paneApparentWindSpeed.XAxis.Title.Text = "Date";
                paneApparentWindSpeed.XAxis.Type = AxisType.Date;
                paneApparentWindSpeed.XAxis.Scale.MajorUnit = DateUnit.Hour;
                paneApparentWindSpeed.YAxis.Title.Text = "Speed (kn)";
                //this.zedGraphControl3.ZoomEvent += new ZedGraphControl.ZoomEventHandler(zedGraphControl_ZoomEvent);

                GraphPane paneApparentWindDirection = this.zedGraphControl4.GraphPane;
                paneApparentWindDirection.Title.Text = "Apparent Wind Angle (Positive to Starboard)";
                paneApparentWindDirection.XAxis.Title.Text = "Date";
                paneApparentWindDirection.XAxis.Type = AxisType.Date;
                paneApparentWindDirection.XAxis.Scale.MajorUnit = DateUnit.Hour;
                paneApparentWindDirection.YAxis.Title.Text = "Angle";
                //this.zedGraphControl4.ZoomEvent += new ZedGraphControl.ZoomEventHandler(zedGraphControl_ZoomEvent);

                plotsUpdater.DoWork += new DoWorkEventHandler(plotsUpdater_DoWork);
                plotsUpdater.RunWorkerCompleted += new RunWorkerCompletedEventHandler(plotsUpdater_RunWorkerCompleted);
                this.timerUpdateGraphs.Enabled = true;
                this.timerUpdateLatest.Enabled = true;
                countdown = refreshIn;
            }

          
        }

        #endregion

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

        void plotsUpdater_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GraphPane paneTrueWindSpeed = this.zedGraphControl1.GraphPane;
            paneTrueWindSpeed.CurveList.Clear();
            paneTrueWindSpeed.GraphObjList.Clear();
            this.zedGraphControl1.AxisChange();
            paneTrueWindSpeed.AddCurve("Speed", averagedTrueWindSpeed, Color.Green, SymbolType.XCross);
            paneTrueWindSpeed.AddCurve("Gust", maxTrueWindSpeed, Color.Red, SymbolType.Plus);
            paneTrueWindSpeed.AddCurve("Low", minTrueWindSpeed, Color.Blue, SymbolType.Plus);
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Refresh();

            GraphPane paneTrueWindDirection = this.zedGraphControl2.GraphPane;
            paneTrueWindDirection.CurveList.Clear();
            paneTrueWindDirection.GraphObjList.Clear();
            //paneTrueWindDirection.YAxis.Scale.Min = -10;
            //paneTrueWindDirection.YAxis.Scale.Max = 370;
            this.zedGraphControl2.AxisChange();
            paneTrueWindDirection.AddCurve("Direction", averagedTrueWindDirection, Color.Green, SymbolType.XCross);
            this.zedGraphControl2.AxisChange();
            this.zedGraphControl2.Refresh();

            GraphPane paneApparentWindSpeed = this.zedGraphControl3.GraphPane;
            paneApparentWindSpeed.CurveList.Clear();
            paneApparentWindSpeed.GraphObjList.Clear();
            this.zedGraphControl3.AxisChange();
            paneApparentWindSpeed.AddCurve("Speed", averagedAparentWindSpeed, Color.Green, SymbolType.XCross);
            //paneApparentWindSpeed.AddCurve("Gust", maxTrueWindSpeed, Color.Red, SymbolType.Plus);
            //paneApparentWindSpeed.AddCurve("Low", minTrueWindSpeed, Color.Blue, SymbolType.Plus);
            this.zedGraphControl3.AxisChange();
            this.zedGraphControl3.Refresh();

            GraphPane paneApparentWindDirection = this.zedGraphControl4.GraphPane;
            paneApparentWindDirection.CurveList.Clear();
            paneApparentWindDirection.GraphObjList.Clear();
            //paneApparentWindDirection.YAxis.Scale.Min = -190;
            //paneApparentWindDirection.YAxis.Scale.Max = 190;
            this.zedGraphControl4.AxisChange();
            paneApparentWindDirection.AddCurve("Direction", averagedAparentWindDirection, Color.Green, SymbolType.XCross);
            this.zedGraphControl4.AxisChange();
            this.zedGraphControl4.Refresh();

        }

        void plotsUpdater_DoWork(object sender, DoWorkEventArgs e)
        {
            double averagingPeriod = refreshIn;

            //set the time max
            long maxTime = Tools.ToPOSIX(DateTime.Now);
            //set the time min according to the desired option, or set 0 to get all the data since the beginning
            long minTimeSelected = timeAnalysed != -1 ? maxTime - timeAnalysed : 0;

            //generate two sets of points: true wind speed averaged over a minute
            //and true wind speed gust as the max value over that minute
            long dataStart = -1;
            int measurePoints = 0;
            double avr = 0;
            double max = double.MinValue;
            double min = double.MaxValue;

            long minTWSTime = minTimeSelected;
            if (this.averagedTrueWindSpeed.Count > 0)
                minTWSTime = (long)this.averagedTrueWindSpeed.Max(item => item.Z);

            IEnumerable<KeyValuePair<long, double>> dataTWS = Buisness.Database.TWS.Where(
                item => item.Key >= minTWSTime && item.Key < maxTime);
            foreach (KeyValuePair<long, double> pair in dataTWS.OrderBy(i => i.Key))
            {
                if (pair.Key > dataStart + averagingPeriod)
                {
                    dataStart = pair.Key;
                    if (measurePoints > 0)
                    {
                        double plotDate = (double)new XDate(Tools.FromPOSIX(dataStart));
                        PointPair avrS = new PointPair(plotDate, avr / (double)measurePoints, pair.Key);
                        PointPair maxS = new PointPair(plotDate, max, pair.Key);
                        PointPair minS = new PointPair(plotDate, min, pair.Key);
                        avr = 0;
                        max = double.MinValue;
                        min = double.MaxValue;
                        measurePoints = 0;
                        averagedTrueWindSpeed.Add(avrS);
                        maxTrueWindSpeed.Add(maxS);
                        minTrueWindSpeed.Add(minS);
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

            long minTWDTime = minTimeSelected;
            if (this.averagedTrueWindDirection.Count > 0)
                minTWDTime = (long)this.averagedTrueWindDirection.Max(item => item.Z);

            IEnumerable<KeyValuePair<long, double>> dataTWD = Buisness.Database.TWDT.Where(
            item => item.Key >= minTWDTime && item.Key < maxTime);
            foreach (KeyValuePair<long, double> pair in dataTWD.OrderBy(i => i.Key))
            {
                if (pair.Key > dataStart + averagingPeriod)
                {
                    dataStart = pair.Key;
                    if (measurePoints > 0)
                    {
                        double plotDate = (double)new XDate(Tools.FromPOSIX(dataStart));
                        PointPair avrD = new PointPair(plotDate, (360 + Math.Atan2(avrsin / measurePoints, avrcos / measurePoints) * 180 / Math.PI) % 360, pair.Key);
                        avr = 0;
                        avrsin = 0;
                        avrcos = 0;
                        measurePoints = 0;

                        #region Don's obscure code
                        int wrapLimit = 20;
                        int PrevI = this.averagedTrueWindDirection.Count - 1;
                        if (PrevI >= 1)  /// skip first point
                        {
                            double PrevV = this.averagedTrueWindDirection[PrevI].Y;
                            if (avrD.Y < wrapLimit)
                            {
                                if (PrevV > 360 - wrapLimit)
                                {  /// wind moving from north to south, avoid spikes back and fore
                                    avrD.Y += 360;
                                }
                            }
                            else if (avrD.Y > 360 - wrapLimit)
                            {
                                if (PrevV < wrapLimit)
                                {  /// wind moving from south to north, avoid spikes back and fore
                                    avrD.Y -= 360;
                                }
                            }
                            else if (PrevV > 360)
                            {
                                if (avrD.Y < 180)
                                {  /// really moved to the south, back out > 360 values
                                    for (int i = this.averagedTrueWindDirection.Count - 1; i >= 0; i--)
                                    {
                                        double V = this.averagedTrueWindDirection[i].Y;
                                        if (V < 360) { break; }
                                        this.averagedTrueWindDirection[i].Y -= 360;
                                    }
                                }
                            }
                            else if (PrevV < 0)
                            {
                                if (avrD.Y > 180)
                                {  /// really moved to the north, back out < 0 values
                                    for (int i = this.averagedTrueWindDirection.Count - 1; i >= 0; i--)
                                    {
                                        double V = this.averagedTrueWindDirection[i].Y;
                                        if (V >= 0) { break; }
                                        this.averagedTrueWindDirection[i].Y += 360;
                                    }
                                }
                            }
                        }
                        #endregion

                        this.averagedTrueWindDirection.Add(avrD);
                    }
                }
                else
                {
                    measurePoints++;
                    avrsin += Math.Sin(pair.Value * Math.PI / 180);
                    avrcos += Math.Cos(pair.Value * Math.PI / 180);
                }
            }

            //generate two sets of points: true wind direction averaged over a minute
            //and true wind direction gust as the max value over that minute
            dataStart = -1;
            avr = 0;
            measurePoints = 0;
            max = double.MinValue;
            min = double.MaxValue;

            long minAWSTime = minTimeSelected;
            if (this.averagedAparentWindSpeed.Count > 0)
                minAWSTime = (long)this.averagedAparentWindSpeed.Max(item => item.Z);

            IEnumerable<KeyValuePair<long, double>> dataAWS = Buisness.Database.AWS.Where(
            item => item.Key >= minAWSTime && item.Key < maxTime);
            foreach (KeyValuePair<long, double> pair in dataAWS.OrderBy(i => i.Key))
            {
                if (pair.Key > dataStart + averagingPeriod)
                {
                    dataStart = pair.Key;
                    if (measurePoints > 0)
                    {
                        double plotDate = (double)new XDate(Tools.FromPOSIX(dataStart));
                        PointPair avrS = new PointPair(plotDate, avr / (double)measurePoints, pair.Key);
                        PointPair maxS = new PointPair(plotDate, max, pair.Key);
                        PointPair minS = new PointPair(plotDate, min, pair.Key);
                        avr = 0;
                        max = double.MinValue;
                        min = double.MaxValue;
                        measurePoints = 0;
                        this.averagedAparentWindSpeed.Add(avrS);
                    }
                }
                else
                {
                    measurePoints++;
                    avr += (pair.Value / 0.514444);
                    if ((pair.Value) > max)
                        max = (pair.Value / 0.514444);
                    if ((pair.Value) < min)
                        min = (pair.Value / 0.514444);
                }
            }

            //generate two sets of points: true wind direction averaged over a minute
            //and true wind direction gust as the max value over that minute
            dataStart = -1;
            avr = 0;
            measurePoints = 0;
            avrsin = 0;
            avrcos = 0;

            long minAWATime = minTimeSelected;
            if (this.averagedAparentWindDirection.Count > 0)
                minAWATime = (long)this.averagedAparentWindDirection.Max(item => item.Z);

            IEnumerable<KeyValuePair<long, double>> dataAWA = Buisness.Database.AWA.Where(
            item => item.Key >= minAWATime && item.Key < maxTime);
            foreach (KeyValuePair<long, double> pair in dataAWA.OrderBy(i => i.Key))
            {
                if (pair.Key > dataStart + averagingPeriod)
                {
                    dataStart = pair.Key;
                    if (measurePoints > 0)
                    {
                        double plotDate = (double)new XDate(Tools.FromPOSIX(dataStart));
                        double apparentAngle = (Math.Atan2(avrsin / measurePoints, avrcos / measurePoints) * 180 / Math.PI) % 180;
                        if ((Math.Atan2(avrsin / measurePoints, avrcos / measurePoints) * 180 / Math.PI) > 180)
                            apparentAngle = apparentAngle - 180;
                        PointPair avrAWA = new PointPair(plotDate, apparentAngle, pair.Key);
                        avr = 0;
                        avrsin = 0;
                        avrcos = 0;
                        measurePoints = 0;
                        this.averagedAparentWindDirection.Add(avrAWA);
                    }
                }
                else
                {
                    measurePoints++;
                    avrsin += Math.Sin(pair.Value * Math.PI / 180);
                    avrcos += Math.Cos(pair.Value * Math.PI / 180);
                }
            }

            ////remove the non desired points
            this.averagedAparentWindDirection.RemoveAll(item => item.Z < minTimeSelected);
            this.averagedAparentWindSpeed.RemoveAll(item => item.Z < minTimeSelected);
            this.averagedTrueWindDirection.RemoveAll(item => item.Z < minTimeSelected);
            this.averagedTrueWindSpeed.RemoveAll(item => item.Z < minTimeSelected);
            this.maxTrueWindSpeed.RemoveAll(item => item.Z < minTimeSelected);
            this.minTrueWindSpeed.RemoveAll(item => item.Z < minTimeSelected);
        }

        private void timerUpdateCountdown_Tick(object sender, EventArgs e)
        {
            countdown--;

            this.label2.Text = countdown.ToString() + " seconds";
            if (countdown <= 0 && plotsUpdater.IsBusy == false)
            {
                countdown = refreshIn;
                plotsUpdater.RunWorkerAsync();
            }
        }

        private void timerUpdateLatest_Tick(object sender, EventArgs e)
        {
            this.labelTWS.Text = Buisness.LatestTWS.ToString("F1") + "kn";
            this.labelTWD.Text = Buisness.LatestTWD.ToString("F1") + "deg";
            this.labelAWD.Text = Buisness.LatestAWA.ToString("F1") + "kn";
            this.labelAWS.Text = Buisness.LatestAWS.ToString("F1") + "deg";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
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

            this.averagedAparentWindDirection.Clear();
            this.averagedAparentWindSpeed.Clear();
            this.averagedTrueWindDirection.Clear();
            this.averagedTrueWindSpeed.Clear();
            this.maxTrueWindSpeed.Clear();
            this.minTrueWindSpeed.Clear();
        
            if(plotsUpdater.IsBusy == false)
                plotsUpdater.RunWorkerAsync();
        }
        #endregion

       
        
    }
}
