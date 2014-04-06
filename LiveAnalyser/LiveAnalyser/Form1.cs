using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using LiveAnalyser.Model;
using LiveAnalyser.Controls;

namespace LiveAnalyser
{
    public partial class Form1 : Form
    {
        #region constructors

        /// <summary>
        /// Default constructor, initialise form controls and buisness class
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            Buisness.Initialize();

            //uncomment to load all previously recorded data. needs to uncomment Buisness.SaveData("data.bin"); in Form_Closing
            //Buisness.LoadData("data.bin");
            Buisness.DataReceivedEvent += new Buisness.DataReceived(Buisness_DataReceivedEvent);
            
            //Buisness.Say("Sorcerer analyser started, I'm sexy and I know it");
        }
    
        #endregion

        #region private methods

        /// <summary>
        /// Opens a file dialog from which an NMEA file can be loaded to simulate 
        /// data input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simulateWithNMEALogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (!String.IsNullOrEmpty(Properties.Settings.Default.LastOpenDialogFolder)
                && new DirectoryInfo(Properties.Settings.Default.LastOpenDialogFolder).Exists)
                openFileDialog1.InitialDirectory = Properties.Settings.Default.LastOpenDialogFolder;
            else
                openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            

            openFileDialog1.Filter = "All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        //Properties.Settings.Default.LastOpenDialogFolder = Path.GetDirectoryName(openFileDialog1.FileName);
                        using (myStream)
                            Buisness.UseFileStreamAsInput(openFileDialog1.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
        /// <summary>
        /// Delegate for avoiding cross thread operation errors
        /// </summary>
        /// <param name="feed"></param>
        private delegate void SetControlPropertyThreadSafeDelegate(string feed);
        /// <summary>
        /// Set the toolstrip bar labels to indicate that data has been added to the database
        /// This is mostly used to tell the user that the COM port connection is actually working
        /// </summary>
        /// <param name="feed"></param>
        private void Buisness_DataReceivedEvent(string feed)
        {
            if (this.statusStrip1.InvokeRequired)
            {
                this.statusStrip1.Invoke(new SetControlPropertyThreadSafeDelegate(Buisness_DataReceivedEvent), feed);
            }
            else
            {
                this.toolStripStatusLabel1.ForeColor = Color.Red;
                this.toolStripStatusLabel1.Text = "Receiving "+ feed +" data";
                this.timerReceivingDataShort.Enabled = false;
                this.timerReceivingDataLong.Enabled = false;
                this.timerReceivingDataShort.Enabled = true;
                this.timerReceivingDataLong.Enabled = true;
            }
        }
        /// <summary>
        /// Stops the short timer and set the tool strip status label back to black
        /// Used in parallel with Buisness_DataReceivedEvent and timerReceivingDataLong_Tick to show the user
        /// data has been received and added in the database, shows that the COM port communication works correctly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerReceivingData_Tick(object sender, EventArgs e)
        {
            this.timerReceivingDataShort.Enabled = false;
            this.toolStripStatusLabel1.ForeColor = Color.Black;
        }
        /// <summary>
        /// Stops the long timer and set the tool strip status label back to black and show Not connected
        /// Used to show to the user that no data has been received thought the COM port to the buisness class for the the past 5 seconds.
        /// Used in parallel with Buisness_DataReceivedEvent and timerReceivingData_Tick to show the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerReceivingDataLong_Tick(object sender, EventArgs e)
        {
            this.timerReceivingDataLong.Enabled = false;
            this.toolStripStatusLabel1.ForeColor = Color.Black;
            this.toolStripStatusLabel1.Text = "Not connected";
        }
        /// <summary>
        /// Tells the buisness class to stop any actions and saves the local user settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Buisness.Stopping();

            //uncomment to save all the data to a compressed xml string, uncomment Buisness.LoadData("data.bin"); in constructor to load data back
            //Buisness.SaveData("data.bin");

            Properties.Settings.Default.Save();

        }

        private void configureCOMPortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form configForm = new Form();
            SerialConfig control = new SerialConfig();
            configForm.Controls.Add(control);
            control.Dock = DockStyle.Fill;
            configForm.Size = new Size(
                control.MinimumSize.Width + 20
                , control.MinimumSize.Height + 40);
            configForm.ShowDialog();
        }
        
        #endregion

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Are you sure you want to close the application?\r\nData recording will be stopped.", "Warning", MessageBoxButtons.YesNo);
            if(res == System.Windows.Forms.DialogResult.Yes)
                Close();
        }

        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
       
    }
}
