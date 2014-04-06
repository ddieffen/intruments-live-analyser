using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace LiveAnalyser.Controls.WaypointsControls.WaypointsManagementClasses
{
    public partial class WaypointEditor : Form
    {
        WayPoint wp;
        WayPoint wpGoldCopy;

        #region public event
        public delegate void EditorClosing(WayPoint wp);
        [Category("Action")]
        [Description("Fires when the value is changed")]
        public event EditorClosing EditorClosingEvent;
        #endregion

        public WaypointEditor()
        {
            InitializeComponent();
        }

        public WaypointEditor(WayPoint wp)
        {
            InitializeComponent();

            this.wp = wp;
            this.wpGoldCopy = new WayPoint();
            wpGoldCopy.setEqual(wp);

            this.textBoxName.Text = this.wp.Name;
            this.textBoxLat.Text = this.wp.getPos.lat.displayDegMinDir();
            this.textBoxLon.Text = this.wp.getPos.lon.displayDegMinDir();
            this.textBoxDesc.Text = this.wp.Description;
        }

        /// <summary>
        /// Prevents from entering non legal chars for the waypoint.
        /// The authorized chars are A-Z a-z 0-1 and -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.Match(e.KeyChar.ToString(), @"[a-zA-Z0-9_\-]*").Length == 0
                && e.KeyChar != (char)Keys.Delete && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }
        /// <summary>
        /// Validates the length of the name before leaving the text box (lost focus)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxName_Validating(object sender, CancelEventArgs e)
        {
            if (this.textBoxName.Text.Length == 0
                && this.textBoxName.Text.Length <= 10)
            {
                MessageBox.Show("The waypoint needs a name");
                e.Cancel = true;
            }
        }
        /// <summary>
        /// If the name is validated we assign the new name to the object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxName_Validated(object sender, EventArgs e)
        {
            this.wp.name = this.textBoxName.Text;
        }

        private void textBoxLat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.Match(e.KeyChar.ToString(), @"[0-9\. \'NSns]*").Length == 0
                && e.KeyChar != (char)Keys.Delete && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        /// <summary>
        /// Validates the format for coordinate modification, formats accepted
        /// Decimal degrees: 41.2563 N or S
        /// Decimal minutes: 40°26.7717 N or S
        /// Degress minutes seconds: 41°15'22.68" N or S
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxLat_Validating(object sender, CancelEventArgs e)
        {
            Regex decimalDegrees = new Regex(@"^(\-?\d+\.\d*)[°\s]*([NSns])$");
            Regex decimalMinutes = new Regex(@"^(\-?\d+)[°,\s]*(\d+\.\d*)[\'\s]*([NSns])$");
            Regex decimalSeconds = new Regex(@"^(\-?\d+)[°,\s]*(\-?\d+)[',\s]*(\d+\.\d*)[\'\s]*([NSns])$");

            if (decimalDegrees.IsMatch((sender as TextBox).Text)
                || decimalMinutes.IsMatch((sender as TextBox).Text)
                || decimalSeconds.IsMatch((sender as TextBox).Text))
            {
               
            }
            else
            {
                MessageBox.Show("Can't parse this latitude. Formats accepted:\r\n"
                    + "Decimal degrees: 41.2563 N or S\r\n"
                    + "Degrees decimal minutes: 40°26.7717 N or S\r\n"
                    + "Degress minutes decimal seconds: 41°15'22.68'' N or S\r\n"
                    + "Single quote or spaces can be used as separators, don't forget to add N or S at the end of your coordinate");
                e.Cancel = true;
            }
        }
        private void textBoxLat_Validated(object sender, EventArgs e)
        {
            Regex decimalDegrees = new Regex(@"^(\-?\d+\.\d*)([\'\s]*[NSns]{1:1})$");
            Regex decimalMinutes = new Regex(@"^(\-?\d+)[°,\s]*(\d+\.\d*)([\'\s]*[NSns]{1:1})$");
            Regex decimalSeconds = new Regex(@"^(\-?\d+)[°,\s]*(\-?\d+)[',\s]*(\d+\.\d*)([\'\s]*[NSns]{1:1})$");

            if (decimalDegrees.IsMatch((sender as TextBox).Text))
            {
                Match m = decimalDegrees.Match((sender as TextBox).Text);
                double degrees = Convert.ToDouble(m.Groups[1].ToString());
                string orientation = m.Groups[3].ToString().Trim().Trim('\'').ToUpper();

                this.wp.getPos.lat.degrees = degrees;
                this.wp.getPos.lat.dir = orientation;
            }
            else if (decimalMinutes.IsMatch((sender as TextBox).Text))
            {
                Match m = decimalMinutes.Match((sender as TextBox).Text);
                double degrees = Convert.ToDouble(m.Groups[1].ToString());
                double minutes = Convert.ToDouble(m.Groups[2].ToString());
                string orientation = m.Groups[3].ToString().Trim().Trim('\'').ToUpper();

                degrees += minutes / 60;
                
                this.wp.getPos.lat.degrees = degrees;
                this.wp.getPos.lat.dir = orientation;
            }
            else if (decimalSeconds.IsMatch((sender as TextBox).Text))
            {
                Match m = decimalSeconds.Match((sender as TextBox).Text);
                double degrees = Convert.ToDouble(m.Groups[1].ToString());
                double minutes = Convert.ToDouble(m.Groups[2].ToString());
                double seconds = Convert.ToDouble(m.Groups[3].ToString());
                string orientation = m.Groups[4].ToString().Trim().Trim('\'');

                degrees += minutes / 60 + seconds / 3600;

                this.wp.getPos.lat.degrees = degrees;
                this.wp.getPos.lat.dir = orientation;
            }
        }

        private void textBoxLon_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.Match(e.KeyChar.ToString(), @"[0-9\. \'EWew]*").Length == 0
                && e.KeyChar != (char)Keys.Delete && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }
        private void textBoxLon_Validating(object sender, CancelEventArgs e)
        {
            Regex decimalDegrees = new Regex(@"^(\-?\d+\.\d*)[°\s]*([EWew]})$");
            Regex decimalMinutes = new Regex(@"^(\-?\d+)[°,\s]*(\d+\.\d*)[\'\s]*([EWew])$");
            Regex decimalSeconds = new Regex(@"^(\-?\d+)[°,\s]*(\-?\d+)[',\s]*(\d+\.\d*)[\'\s]*([EWew])$");

            if (decimalDegrees.IsMatch((sender as TextBox).Text)
                   || decimalMinutes.IsMatch((sender as TextBox).Text)
                   || decimalSeconds.IsMatch((sender as TextBox).Text))
            {

            }
            else
            {
                MessageBox.Show("Can't parse this longitude. Formats accepted:\r\n"
                   + "Decimal degrees: 41.2563 E or W\r\n"
                   + "Degrees decimal minutes: 40°26.7717 E or W\r\n"
                   + "Degress minutes decimal seconds: 41°15'22.68'' E or W\r\n"
                   + "Single quote or spaces can be used as separators, don't forget to add E or W at the end of your coordinate");
                e.Cancel = true;
            }
        }
        private void textBoxLon_Validated(object sender, EventArgs e)
        {
            Regex decimalDegrees = new Regex(@"^(\-?\d+\.\d*)[°\s]*([EWew]})$");
            Regex decimalMinutes = new Regex(@"^(\-?\d+)[°,\s]*(\d+\.\d*)[\'\s]*([EWew])$");
            Regex decimalSeconds = new Regex(@"^(\-?\d+)[°,\s]*(\-?\d+)[',\s]*(\d+\.\d*)[\'\s]*([EWew])$");

            if (decimalDegrees.IsMatch((sender as TextBox).Text))
            {
                Match m = decimalDegrees.Match((sender as TextBox).Text);
                double degrees = Convert.ToDouble(m.Groups[1].ToString());
                string orientation = m.Groups[3].ToString().Trim().Trim('\'').ToUpper();

                this.wp.getPos.lat.degrees = degrees;
                this.wp.getPos.lat.dir = orientation;
            }
            else if (decimalMinutes.IsMatch((sender as TextBox).Text))
            {
                Match m = decimalMinutes.Match((sender as TextBox).Text);
                double degrees = Convert.ToDouble(m.Groups[1].ToString());
                double minutes = Convert.ToDouble(m.Groups[2].ToString());
                string orientation = m.Groups[3].ToString().Trim().Trim('\'').ToUpper();

                degrees += minutes / 60;

                this.wp.getPos.lat.degrees = degrees;
                this.wp.getPos.lat.dir = orientation;
            }
            else if (decimalSeconds.IsMatch((sender as TextBox).Text))
            {
                Match m = decimalSeconds.Match((sender as TextBox).Text);
                double degrees = Convert.ToDouble(m.Groups[1].ToString());
                double minutes = Convert.ToDouble(m.Groups[2].ToString());
                double seconds = Convert.ToDouble(m.Groups[3].ToString());
                string orientation = m.Groups[4].ToString().Trim().Trim('\'');

                degrees += minutes / 60 + seconds / 3600;

                this.wp.getPos.lat.degrees = degrees;
                this.wp.getPos.lat.dir = orientation;
            }
        }

        private void textBoxDesc_Validated(object sender, EventArgs e)
        {
            this.wp.description = this.textBoxDesc.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.EditorClosingEvent != null)
                this.EditorClosingEvent(this.wp);
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.wp.setEqual(wpGoldCopy);
            if (this.EditorClosingEvent != null)
                this.EditorClosingEvent(this.wp);
            this.Close();
        }

        

        
    }
}
