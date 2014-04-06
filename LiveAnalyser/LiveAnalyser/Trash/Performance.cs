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
using LiveAnalyser.Controls.PerformanceControls;

namespace LiveAnalyser.Controls
{
    public partial class Performance : UserControl
    {
        #region private memners
        long averagingPeriod = 30;
        BackgroundWorker worker = new BackgroundWorker();

        Dictionary<long, double> avrSOW = new Dictionary<long, double>();
        Dictionary<long, double> avrTWA = new Dictionary<long, double>();
        Dictionary<long, double> avrTWS = new Dictionary<long, double>();

        Dictionary<int, int> ranges = new Dictionary<int, int>();
        Dictionary<string, Dictionary<string, KeyValuePair<double, double>>> records = new Dictionary<string, Dictionary<string, KeyValuePair<double, double>>>();

        #endregion

        #region constructors
        public Performance()
        {
            InitializeComponent();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            ranges.Add(0, 4);
            ranges.Add(4, 6);
            ranges.Add(6, 8);
            ranges.Add(8, 10);
            ranges.Add(10,12);
            ranges.Add(12,14);
            ranges.Add(14,16);
            ranges.Add(16,20);
            ranges.Add(20,25);
            ranges.Add(15,30);
        }
        #endregion

        #region internal methods

        internal void UpdateTable()
        {
            if(!worker.IsBusy && !worker.CancellationPending)
                worker.RunWorkerAsync();
        }

        #endregion

        #region private methods
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.flowLayoutPanel1.Controls.Clear();
            this.flowLayoutPanel2.Controls.Clear();
            foreach (KeyValuePair<string, Dictionary<string, KeyValuePair<double, double>>> updown in records)
            { 
                foreach(KeyValuePair<string, KeyValuePair<double, double>> windrange in updown.Value)
                {
                    PerformanceLine pl = new PerformanceLine(windrange.Key, windrange.Value.Key.ToString("F1"), windrange.Value.Value.ToString("F1"));
                    if (updown.Key == "Up")
                        this.flowLayoutPanel1.Controls.Add(pl);
                    else
                        this.flowLayoutPanel2.Controls.Add(pl);
                }
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            DataHolder database = Buisness.Database;
            avrSOW = new Dictionary<long, double>();
            avrTWA = new Dictionary<long, double>();
            avrTWS = new Dictionary<long, double>();

            if (database != null
                && database.TWA.Count > 0
                && database.TWS.Count > 0
                && database.SOW.Count > 0)
            {

                //generate two sets of points: true wind speed averaged over a minute
                //and true wind speed gust as the max value over that minute
                int measurePoints = 0;
                double avr = 0;
                double avrsin = 0;
                double avrcos = 0;

                long startTime =
                    Math.Min(database.TWS.Keys.Min()
                    , Math.Min(database.TWA.Keys.Min()
                    , database.SOW.Keys.Min()));
                long endTime = Math.Max(database.TWS.Keys.Max()
                    , Math.Max(database.TWA.Keys.Max()
                    , database.SOW.Keys.Max()));

                for (long i = startTime; i < endTime; i = i + averagingPeriod)
                {
                    foreach (KeyValuePair<long, double> pair in database.TWS.Where(item =>
                        item.Key >= i && item.Key < i + averagingPeriod).ToList())
                    {
                        measurePoints++;
                        avr += (pair.Value / 0.514444);
                    }
                    avrTWS.Add(i, avr / (double)measurePoints);

                    avrsin = 0;
                    avrcos = 0;
                    measurePoints = 0;
                    foreach (KeyValuePair<long, double> pair in database.TWA.Where(item =>
                        item.Key >= i && item.Key < i + averagingPeriod).ToList())
                    {
                        measurePoints++;
                        avrsin += Math.Sin(pair.Value * Math.PI / 180);
                        avrcos += Math.Cos(pair.Value * Math.PI / 180);
                    }
                    double apparentAngle = (Math.Atan2(avrsin / measurePoints, avrcos / measurePoints) * 180 / Math.PI) % 180;
                    if ((Math.Atan2(avrsin / measurePoints, avrcos / measurePoints) * 180 / Math.PI) > 180)
                        apparentAngle = apparentAngle - 180;
                    avrTWA.Add(i, apparentAngle);

                    avr = 0;
                    measurePoints = 0;
                    List<KeyValuePair<long, double>> dataSOW = database.SOW.Where(item =>
                        item.Key >= i && item.Key < i + averagingPeriod).ToList();
                    foreach (KeyValuePair<long, double> pair in dataSOW)
                    {
                        measurePoints++;
                        avr += (pair.Value / 0.514444);
                    }
                    avrSOW.Add(i, avr / (double)measurePoints);
                }


                //foreach range of wind
                records = new Dictionary<string, Dictionary<string, KeyValuePair<double, double>>>();
                records.Add("Up", new Dictionary<string, KeyValuePair<double, double>>());
                records.Add("Down", new Dictionary<string, KeyValuePair<double, double>>());
                foreach (KeyValuePair<int, int> rangeInterval in ranges)
                {
                    IEnumerable<long> upwindTWA = from n in avrTWA where Math.Abs(n.Value) > 20 && Math.Abs(n.Value) < 90 select n.Key;
                    IEnumerable<long> downwindTWA = from n in avrTWA where Math.Abs(n.Value) > 110 && Math.Abs(n.Value) < 180 select n.Key;
                    IEnumerable<long> TWSInRange = from n in avrTWS where (n.Value >= rangeInterval.Key && n.Value < rangeInterval.Value) select n.Key;
                    IEnumerable<KeyValuePair<long, double>> speedInRangeUp = from n in avrSOW
                                                               where upwindTWA.Contains(n.Key) && TWSInRange.Contains(n.Key)
                                                               select n;
                    IEnumerable<KeyValuePair<long, double>> speedInRangeDown = from n in avrSOW
                                                                               where downwindTWA.Contains(n.Key) && TWSInRange.Contains(n.Key)
                                                                             select n;

                    double maxPerf = 0;
                    double SOW_maxPerf = 0;
                    double TWA_maxPerf = 0;
                    double TWS_maxPerf = 0;
                    foreach (KeyValuePair<long,double> item in speedInRangeUp)
                    {
                        double perf = Math.Cos(Math.Abs(avrTWA[item.Key]) * Math.PI / 180) * item.Value;
                        if (perf > maxPerf)
                        {
                            maxPerf = perf;
                            SOW_maxPerf = item.Value;
                            TWA_maxPerf = avrTWA[item.Key];
                            TWS_maxPerf = avrTWS[item.Key];
                        }
                    }
                    KeyValuePair<double, double> pair = new KeyValuePair<double, double>(SOW_maxPerf, TWA_maxPerf);
                    records["Up"].Add(rangeInterval.Value.ToString(), pair);

                    maxPerf = 0;
                    SOW_maxPerf = 0;
                    TWA_maxPerf = 0;
                    TWS_maxPerf = 0;
                    foreach (KeyValuePair<long, double> item in speedInRangeDown)
                    {
                        double perf = Math.Cos(Math.Abs(avrTWA[item.Key]) * Math.PI / 180) * item.Value;
                        if (perf > maxPerf)
                        {
                            maxPerf = perf;
                            SOW_maxPerf = item.Value;
                            TWA_maxPerf = avrTWA[item.Key];
                            TWS_maxPerf = avrTWS[item.Key];
                        }
                    }
                    pair = new KeyValuePair<double, double>(SOW_maxPerf, TWA_maxPerf);
                    records["Down"].Add(rangeInterval.Value.ToString(), pair);
                }
            }
        }
        #endregion
    }
}

