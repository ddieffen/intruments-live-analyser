using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LiveAnalyser.Model;
using System.Text.RegularExpressions;
using LiveAnalyser.Data;
using LiveAnalyser.Controls.WaypointsControls;



namespace LiveAnalyser.Controls
{
    public partial class Projection : UserControl
    {
        #region delegates

        public delegate void WaypointToSave(WayPoint wp);
        public event WaypointToSave WaypointToSaveEvent;

        #endregion


        #region private members
        WayPoint Base = new WayPoint();
        WayPoint Proj = new WayPoint();
        bool BaseDef = false;
        bool ProjDef = false;

        //int UploadCnt = 0;
        #endregion

        #region public and internals
        public Projection()
        {
            InitializeComponent();
        }
        #endregion

        #region private methods
       
        private void SetToCurrent_Click(object sender, EventArgs e)
        {
            DataHolder database = Buisness.Database;
            if (database != null && database.Pos.Count > 0)
            {
                //database.POS  
                int last = database.Pos.Count - 1;
                long lastTime = database.Pos.Keys.Max();
                Position Pos = database.Pos[lastTime];
                Base = new WayPoint(database.Pos[lastTime], "Cur", "current position");
                BaseDef = true;
                DisplayBase(sender, e); 
            }
            else
            {
                this.BaseLatDeg.Text = "??";
                this.BaseLonDeg.Text = "???";
                this.BaseLatMin.Text = "???";
                this.BaseLonMin.Text = "???";
            }
            
        }
        private void DisplayBase(object sender, EventArgs e)
        {
            if (BaseDef)
            {
                this.BaseLatDeg.TextChanged -= new System.EventHandler(this.SetBase);
                this.BaseLatMin.TextChanged -= new System.EventHandler(this.SetBase);
                this.BaseLonDeg.TextChanged -= new System.EventHandler(this.SetBase); 
                this.BaseLonMin.TextChanged -= new System.EventHandler(this.SetBase);
                this.BaseLatDir.TextChanged -= new System.EventHandler(this.SetBase);
                this.BaseLonDir.TextChanged -= new System.EventHandler(this.SetBase);
                this.BaseName.TextChanged -= new System.EventHandler(this.SetBase);

                this.BaseLatDeg.Text = Base.getPos.lat.displayDeg();
                this.BaseLatMin.Text = Base.getPos.lat.displayMin();
                this.BaseLonDeg.Text = Base.getPos.lon.displayDeg();
                this.BaseLonMin.Text = Base.getPos.lon.displayMin();
                this.BaseLatDir.Text = Base.getPos.lat.dir;
                this.BaseLonDir.Text = Base.getPos.lon.dir;

                this.BaseName.Text = Base.Name;

                this.BaseLatDeg.TextChanged += new System.EventHandler(this.SetBase);
                this.BaseLatMin.TextChanged += new System.EventHandler(this.SetBase);
                this.BaseLonDeg.TextChanged += new System.EventHandler(this.SetBase);
                this.BaseLonMin.TextChanged += new System.EventHandler(this.SetBase);
                this.BaseLatDir.TextChanged += new System.EventHandler(this.SetBase);
                this.BaseLonDir.TextChanged += new System.EventHandler(this.SetBase);
                this.BaseName.TextChanged += new System.EventHandler(this.SetBase);


                Project(sender, e);
            }
        }

        private void Project(object sender, EventArgs e)
        {
            int dir;
            double dist;
            bool valuesOK = int.TryParse(this.dir.Text, out dir);
            valuesOK &= Double.TryParse(this.dist.Text, out dist);
            if (valuesOK && BaseDef )
            {
                Proj.setEqual(Base);
                Proj.name += "P";
                Proj.Project(dir, dist);
                ProjLat.Text = Proj.getPos.lat.displayDegMinDir();
                ProjLon.Text = Proj.getPos.lon.displayDegMinDir();
                ProjName.Text = Proj.name;
                ProjDesc.Text = "Proj of " + Base.Name + " by " + dist.ToString() + "nm at " + dir.ToString() + "°";
                ProjDef = true;
                return;
            }
            else if (!BaseDef)
            {
                MessageBox.Show("must set Base waypoint first");
            }
            ProjLat.Text = "??";
            ProjLon.Text = "???";
            ProjName.Text = "";
            ProjDesc.Text = "invalid";
            ProjDef = false;
        }

  

        /// <summary>
        /// Tests if a key pressed in a text box is a numerical value or a dot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Numeric_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.KeyChar) || e.KeyChar == 8 || e.KeyChar == '.');
        }
        #endregion


        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }


        private void tableLayoutPanel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void BaseUpload(object sender, EventArgs e)
        {
            if (BaseDef)
            {
                Base.Upload();
            }
            else
            {
                MessageBox.Show("must set Base before uploading");
            }

        }

        private void ProjUpload(object sender, EventArgs e)
        {           
            if (ProjDef)
            {
                Proj.Upload();
            }
            else
            {
                MessageBox.Show("must set Projection before uploading");
            }
        }

        private void CurAddList(object sender, EventArgs e)
        {
            if (this.WaypointToSaveEvent != null)
            {
                if (this.Base != null)
                {
                    this.WaypointToSaveEvent(this.Base);
                }
                else
                {
                    MessageBox.Show("Please define a current position first");
                }
            }
        }

        private void SetBase(object sender, EventArgs e)
        {
            if (BaseLatDeg.Text.IndexOf("?") < 0 &&
                 BaseLatMin.Text.IndexOf("?") < 0 &&
                 BaseLonDeg.Text.IndexOf("?") < 0 &&
                 BaseLonMin.Text.IndexOf("?") < 0)
            {
                Base = new WayPoint(BaseLatDeg.Text, BaseLatMin.Text, BaseLatDir.Text, BaseLonDeg.Text, BaseLonMin.Text, BaseLonDir.Text, BaseName.Text, BaseDesc.Text);
                BaseDef = true;
                Project(sender, e);
            }
        }

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void SetProjName(object sender, EventArgs e)
        {
            if (ProjDef)
            {
                Proj.name = ProjName.Text;
            }
        }

        private void AddProj_Click(object sender, EventArgs e)
        {
            if (this.WaypointToSaveEvent != null)
            {
                if (this.Proj != null)
                {
                    this.WaypointToSaveEvent(this.Proj);
                }
                else
                {
                    MessageBox.Show("Please define a projection first");
                }
            }
        }
 

    }
}
