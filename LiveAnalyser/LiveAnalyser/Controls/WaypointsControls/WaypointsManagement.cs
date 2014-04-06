using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Be.Timvw.Framework.ComponentModel;
using LiveAnalyser.Model;
using System.Threading;
using LiveAnalyser.Controls.WaypointsControls.WaypointsManagementClasses;
using System.Text.RegularExpressions;

namespace LiveAnalyser.Controls.WaypointsControls
{
    public partial class WaypointsManagement : UserControl
    {
        #region private members

        List<WayPoint> waypoints = new List<WayPoint>();
        string nameFilter = "";
        string descFilter = "";

        #endregion

        #region public internal
        public WaypointsManagement()
        {
            InitializeComponent();

            this.waypoints = this.LoadTuples();
            this.UpdateDataGridOfWayPoints();
        }

        #endregion

        #region private methods

        List<WayPoint> LoadTuples()
        {
            if (!String.IsNullOrEmpty(Properties.Settings.Default.Waypoints))
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Properties.Settings.Default.Waypoints)))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        return (List<WayPoint>)bf.Deserialize(ms);
                    }
                }
                catch 
                {
                    return new List<WayPoint>();
                }
            }
            return new List<WayPoint>();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\" ;
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*" ;
            openFileDialog1.FilterIndex = 2 ;
            openFileDialog1.RestoreDirectory = true ;

            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    gpxType gpxObject = gpxType.LoadFromFile(openFileDialog1.FileName);
                    if (gpxObject != null)
                    {
                        foreach (wptType wpt in gpxObject.wpt)
                        {
                            WayPoint wp = new WayPoint();
                            wp.getPos.lat.degrees = (double)wpt.lat;
                            wp.getPos.lat.dir = "N";
                            wp.getPos.lon.degrees = -(double)wpt.lon;
                            wp.getPos.lon.dir = "W";
                            wp.name = String.IsNullOrEmpty(wpt.name) == false ? wpt.name : "";
                            wp.description = String.IsNullOrEmpty(wpt.desc) == false ? wpt.desc : "" ;
                            this.waypoints.Add(wp);
                        }
                    }
                }
                catch (Exception ex) 
                {//error while loading the file
                    MessageBox.Show(ex.Message);
                }
            }

            this.UpdateInternalStorageOfWayPoints();
            this.UpdateDataGridOfWayPoints();
            
        }

        /// <summary>
        /// This methods tries to parse the text boxes for entering a new Waypoint to the dataset, adding it if successful, otherwise show message box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            WayPoint wp = new WayPoint();
            bool valid = true;
            string message = "";

            Regex decimalLatDegrees = new Regex(@"^(\-?\d+\.\d*)([\'\s]*[NSns])$");
            Regex decimalLatMinutes = new Regex(@"^(\-?\d+)[°,\s]*(\d+\.\d*)([\'\s]*[NSns])$");
            Regex decimalLatSeconds = new Regex(@"^(\-?\d+)[°,\s]*(\-?\d+)[',\s]*(\d+\.\d*)([\'\s]*[NSns])$");

            if (decimalLatDegrees.IsMatch(this.textBoxLat.Text))
            {
                Match m = decimalLatDegrees.Match(this.textBoxLat.Text);
                double degrees = Convert.ToDouble(m.Groups[1].ToString());
                string orientation = m.Groups[3].ToString().Trim().Trim('\'').ToUpper();

                wp.getPos.lat.degrees = degrees;
                wp.getPos.lat.dir = orientation;
            }
            else if (decimalLatMinutes.IsMatch(this.textBoxLat.Text))
            {
                Match m = decimalLatMinutes.Match(this.textBoxLat.Text);
                double degrees = Convert.ToDouble(m.Groups[1].ToString());
                double minutes = Convert.ToDouble(m.Groups[2].ToString());
                string orientation = m.Groups[3].ToString().Trim().Trim('\'').ToUpper();

                degrees += minutes / 60;

                wp.getPos.lat.degrees = degrees;
                wp.getPos.lat.dir = orientation;
            }
            else if (decimalLatSeconds.IsMatch(this.textBoxLat.Text))
            {
                Match m = decimalLatSeconds.Match(this.textBoxLat.Text);
                double degrees = Convert.ToDouble(m.Groups[1].ToString());
                double minutes = Convert.ToDouble(m.Groups[2].ToString());
                double seconds = Convert.ToDouble(m.Groups[3].ToString());
                string orientation = m.Groups[4].ToString().Trim().Trim('\'');

                degrees += minutes / 60 + seconds / 3600;

                wp.getPos.lat.degrees = degrees;
                wp.getPos.lat.dir = orientation;
            }
            else
            {
                message += "Can't parse this latitude. Formats accepted:\r\n"
                   + "Decimal degrees: 41.2563 N or S\r\n"
                   + "Degrees decimal minutes: 40°26.7717 N or S\r\n"
                   + "Degress minutes decimal seconds: 41°15'22.68'' N or S\r\n"
                   + "Single quote or spaces can be used as separators, don't forget to add N or S at the end of your coordinate\r\n\r\n";
                valid = false;
            }

            Regex decimalLonDegrees = new Regex(@"^(\-?\d+\.\d*)[°\s]*([EWew]})$");
            Regex decimalLonMinutes = new Regex(@"^(\-?\d+)[°,\s]*(\d+\.\d*)[\'\s]*([EWew])$");
            Regex decimalLonSeconds = new Regex(@"^(\-?\d+)[°,\s]*(\-?\d+)[',\s]*(\d+\.\d*)[\'\s]*([EWew])$");

            if (decimalLonDegrees.IsMatch(this.textBoxLon.Text))
            {
                Match m = decimalLonDegrees.Match(this.textBoxLon.Text);
                double degrees = Convert.ToDouble(m.Groups[1].ToString());
                string orientation = m.Groups[3].ToString().Trim().Trim('\'').ToUpper();

                wp.getPos.lat.degrees = degrees;
                wp.getPos.lat.dir = orientation;
            }
            else if (decimalLonMinutes.IsMatch(this.textBoxLon.Text))
            {
                Match m = decimalLonMinutes.Match(this.textBoxLon.Text);
                double degrees = Convert.ToDouble(m.Groups[1].ToString());
                double minutes = Convert.ToDouble(m.Groups[2].ToString());
                string orientation = m.Groups[3].ToString().Trim().Trim('\'').ToUpper();

                degrees += minutes / 60;

                wp.getPos.lat.degrees = degrees;
                wp.getPos.lat.dir = orientation;
            }
            else if (decimalLonSeconds.IsMatch(this.textBoxLon.Text))
            {
                Match m = decimalLonSeconds.Match(this.textBoxLon.Text);
                double degrees = Convert.ToDouble(m.Groups[1].ToString());
                double minutes = Convert.ToDouble(m.Groups[2].ToString());
                double seconds = Convert.ToDouble(m.Groups[3].ToString());
                string orientation = m.Groups[4].ToString().Trim().Trim('\'');

                degrees += minutes / 60 + seconds / 3600;

                wp.getPos.lat.degrees = degrees;
                wp.getPos.lat.dir = orientation;
            }
            else
            {
                message += "Can't parse this longitude. Formats accepted:\r\n"
                      + "Decimal degrees: 41.2563 E or W\r\n"
                      + "Degrees decimal minutes: 40°26.7717 E or W\r\n"
                      + "Degress minutes decimal seconds: 41°15'22.68'' E or W\r\n"
                      + "Single quote or spaces can be used as separators, don't forget to add E or W at the end of your coordinate\r\n\r\n";
                valid = false;
            }

            if (this.textBoxName.Text.Length == 0
               && this.textBoxName.Text.Length <= 10)
            {
                message += "The waypoint needs a name\r\n\r\n";
                valid = false;
            }
            else if (this.waypoints.Any(item => item.name == this.textBoxName.Text))
            {
                message += "This waypoint name is already used";
                valid = false;
            }
            else
            {
                wp.name = this.textBoxName.Text;
            }

            wp.description = this.textBoxDesc.Text;

            if (valid)
            {
                this.PushWaypoint(wp);
                MessageBox.Show("Added successfully");
            }
            else 
            {
                MessageBox.Show(message);
            }
        }

        /// <summary>
        /// Adds a waypoint to the local member of this control, serializes the list of </br>
        /// waypoints and saves them to the user properties. Update the list displayed</br>
        /// </summary>
        /// <param name="wp">The waypoint to be added</param>
        /// <returns>True if added correctly</returns>
        public bool PushWaypoint(WayPoint wp)
        {
            bool result = true;
            try
            {
                this.waypoints.Add(wp);
                this.UpdateDataGridOfWayPoints();
                this.UpdateInternalStorageOfWayPoints();
            }
            catch 
            {
                result = false;
            }
            return result;
        }

        private void UpdateInternalStorageOfWayPoints()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamReader sr = new StreamReader(ms))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(ms, this.waypoints);
                    ms.Position = 0;
                    byte[] buffer = new byte[(int)ms.Length];
                    ms.Read(buffer, 0, buffer.Length);
                    Properties.Settings.Default.Waypoints = Convert.ToBase64String(buffer);
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void UpdateDataGridOfWayPoints()
        {
            IEnumerable<WayPoint> filtered = this.waypoints;

            if (!String.IsNullOrEmpty(this.nameFilter))
            {
                IEnumerable<WayPoint> byName = filtered.Where(item => item.name.ToLower().StartsWith(this.nameFilter));
                int count = byName.Count();
                filtered = byName;
            }
            if (!String.IsNullOrEmpty(this.descFilter))
            {
                IEnumerable<WayPoint> byDesc = filtered.Where(item => item.description.ToLower().Contains(this.descFilter));
                int count = byDesc.Count();
                filtered = byDesc;
            }

            SortableBindingList<WayPoint> source = new SortableBindingList<WayPoint>(filtered);
            source.OrderBy(item => item.Name);
            this.dataGridView1.DataSource = source;
            this.dataGridView1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            gpxType gpxObject = new gpxType();

            foreach (WayPoint wpt in this.waypoints)
            {
                wptType wp = new wptType();
                wp.lat = wpt.getPos.lat.dir.ToLower() == "n" ? (decimal)wpt.getPos.lat.degrees : (decimal)-wpt.getPos.lat.degrees;
                wp.lon = wpt.getPos.lon.dir.ToLower() == "n" ? (decimal)wpt.getPos.lon.degrees : (decimal)-wpt.getPos.lon.degrees;
                wp.name = wpt.name;
                wp.desc = wpt.description;
                gpxObject.wpt.Add(wp);
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "GPX files (*.gpx)|*.gpx|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    gpxObject.SaveToFile(saveFileDialog1.FileName);
                }
                catch (Exception ex) 
                {//error while saving the file
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (WayPoint wp in (this.dataGridView1.DataSource as SortableBindingList<WayPoint>))
            {
                Buisness.COMSendLine(wp.NMEA());
                Thread.Sleep(250); //assuming that transmission takes 250ms
            }
        }

        private void textBoxNameFilter_TextChanged(object sender, EventArgs e)
        {
            this.nameFilter = this.textBoxNameFilter.Text.ToLower();
            this.UpdateDataGridOfWayPoints();
        }

        private void textBoxDescriptionFilter_TextChanged(object sender, EventArgs e)
        {
            this.descFilter = this.textBoxDescriptionFilter.Text.ToLower();
            this.UpdateDataGridOfWayPoints();
        }

        #endregion

        /// <summary>
        /// Shows the context menu for the data grid view containing the list of available options
        /// for waypoints represented into the data grid view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu m = new ContextMenu();

                int r = dataGridView1.HitTest(e.X, e.Y).RowIndex;
                if (r >= 0 && r < dataGridView1.Rows.Count)
                {
                    DataGridViewRow row = dataGridView1.Rows[r];
                    WayPoint waypoint = (WayPoint)row.DataBoundItem;

                    MenuItem edit = m.MenuItems.Add("Edit Waypoint");
                    edit.Click += new EventHandler((sx, ex) => edit_Click(sx, ex, waypoint));
                    MenuItem delete = m.MenuItems.Add("Delete Waypoint");
                    delete.Click += new EventHandler((sx, ex) => delete_Click(sx, ex, waypoint));

                    m.Show(dataGridView1, new Point(e.X, e.Y));
                }
            }
        }

        /// <summary>
        /// Deleltes the Waypoint passed as an argument from the class member that lists all waypoints and refreshes the datagrid view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="wp"></param>
        void delete_Click(object sender, EventArgs e, WayPoint wp)
        {
            DialogResult res = MessageBox.Show("Do you want to delete this waypoint?\r\nThis is not reversible", "Warning", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                this.waypoints.Remove(wp);

                this.UpdateDataGridOfWayPoints();
                this.UpdateInternalStorageOfWayPoints();
            }
            else
            {
                MessageBox.Show("Not deleted");
            }
        }


        /// <summary>
        /// Opens the waypoint data editor and refreshes the datagrid when the editor is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="wp"></param>
        void edit_Click(object sender, EventArgs e, WayPoint wp)
        {
            WaypointEditor wpe = new WaypointEditor(wp);
            wpe.EditorClosingEvent += (updatedWp) =>
            {
                this.UpdateDataGridOfWayPoints();
                this.UpdateInternalStorageOfWayPoints();
            };
            wpe.ShowDialog();
        }

       
    }
}
