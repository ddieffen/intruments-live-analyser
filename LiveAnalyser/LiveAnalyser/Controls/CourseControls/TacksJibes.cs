using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LiveAnalyser.Data;
using LiveAnalyser.Model;
using LiveAnalyser.Controls.CourseControls;

namespace LiveAnalyser.Controls
{
    public partial class TacksJijbes : UserControl
    {
        long incrementSteps = 1;
        BackgroundWorker worker = new BackgroundWorker();
        double[] detectArray = new double[4] { 0, 0, 0, 0 };
        List<Manoeuver> mans = new List<Manoeuver>();

        public TacksJijbes()
        {
            InitializeComponent();

            this.dataGridView1.DataSource = null;
            this.dataGridView1.Columns.Clear();
            this.dataGridView1.Rows.Clear();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            DataGridViewColumn name = new DataGridViewColumn() { HeaderText = "Type", CellTemplate = new DataGridViewTextBoxCell() };
            name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            name.Name = "Name";
            name.ReadOnly = true;
            name.DataPropertyName = "Type"; //this practice is dangerous as renaming the attribute in the real class will make the datagrid showing empty cells, it also needs dotfuscator to not rename this attribute for the real object
            name.SortMode = DataGridViewColumnSortMode.Automatic;
            
        }

        internal void UpdateTable()
        {
            if(!worker.IsBusy)
                worker.RunWorkerAsync();
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.dataGridView1.DataSource = mans.ToArray();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            DataHolder database = Buisness.Database;
            
            if (database != null
                && database.TWA.Count > 0
                && database.TWS.Count > 0
                && database.SOW.Count > 0)
            {

                long startTime =
                    Math.Min(database.TWS.Keys.Min()
                    , Math.Min(database.TWA.Keys.Min()
                    , database.SOW.Keys.Min()));
                long endTime = Math.Max(database.TWS.Keys.Max()
                    , Math.Max(database.TWA.Keys.Max()
                    , database.SOW.Keys.Max()));
                mans = new List<Manoeuver>();

                detectArray = new double[4] { 0, 0, 0, 0 };
                for (long i = startTime; i < endTime; i = i + incrementSteps)
                {
                    foreach (KeyValuePair<long, double> pair in database.AWA.Where(item =>
                            item.Key >= i && item.Key < i + incrementSteps).ToList())
                    {
                        double apparentAngle = pair.Value % 180;
                        if (pair.Value > 180)
                            apparentAngle = apparentAngle - 180;

                        bool tack = true;
                        for (int j = 0; j <= detectArray.Count() - 2; j++)
                        {
                            detectArray[j] = detectArray[j+1];
                            tack &= Math.Abs(detectArray[j+1]) <= 90;
                        }
                        detectArray[detectArray.Count() - 1] = apparentAngle;
                        tack &= Math.Abs(apparentAngle) <= 90;

                        if(detectArray[0] > 0 && detectArray[1] > 0
                            && detectArray[2] < 0 && detectArray[3] < 0)
                        {
                            Manoeuver man = new Manoeuver();
                            man.Type = tack ? "Tack to Starboard" : "Jibe to Port";
                            mans.Add(man);
                        }
                        if (detectArray[0] < 0 && detectArray[1] < 0
                            && detectArray[2] > 0 && detectArray[3] > 0)
                        {
                            Manoeuver man = new Manoeuver();
                            man.Type = tack ? "Tack to Port" : "Jibe to Starboard";
                            mans.Add(man);
                        }

                    }
                }
            }
        }
    }
}
