using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using LiveAnalyser.Model;

namespace LiveAnalyser.Controls
{
    public partial class SerialConfig : UserControl
    {
        public SerialConfig()
        {
            InitializeComponent();

            string[] sPorts = SerialPort.GetPortNames();
            foreach (string s in sPorts)
                this.comboBoxPort.Items.Add(s);
            this.comboBoxPort.SelectedItem = Properties.Settings.Default.SerialPortName;
            this.comboBoxPort.SelectedValueChanged += new EventHandler(ParamSelectionChanged);

            this.comboBoxBaud.Items.Add(4800);
            this.comboBoxBaud.Items.Add(38400);
            this.comboBoxBaud.SelectedItem = Properties.Settings.Default.SerialPortBaud;
            this.comboBoxBaud.SelectedValueChanged += new EventHandler(ParamSelectionChanged);

            this.comboBoxParity.Items.Add(Parity.None);
            this.comboBoxParity.SelectedItem = Parity.None;
            this.comboBoxParity.Enabled = false;
            this.comboBoxParity.SelectedValueChanged += new EventHandler(ParamSelectionChanged);

            this.comboBoxDataBits.Items.Add(8);
            this.comboBoxDataBits.SelectedItem = 8;
            this.comboBoxDataBits.Enabled = false;
            this.comboBoxDataBits.SelectedValueChanged += new EventHandler(ParamSelectionChanged);

            this.comboBoxStopBits.Items.Add(StopBits.One);
            this.comboBoxStopBits.SelectedItem = StopBits.One;
            this.comboBoxStopBits.Enabled = false;
            this.comboBoxStopBits.SelectedValueChanged += new EventHandler(ParamSelectionChanged);
            
        }

        private void ParamSelectionChanged(object sender, EventArgs e)
        {
           try
            {
                if(this.comboBoxPort.SelectedItem != null)
                    Properties.Settings.Default.SerialPortName = this.comboBoxPort.SelectedItem.ToString();
                if(this.comboBoxBaud.SelectedItem != null)
                    Properties.Settings.Default.SerialPortBaud = Convert.ToInt32(this.comboBoxBaud.SelectedItem);
                if(this.comboBoxParity.SelectedItem != null)
                    Properties.Settings.Default.SerialPortParity = (Parity)this.comboBoxParity.SelectedItem;
                if(this.comboBoxDataBits.SelectedItem != null)
                    Properties.Settings.Default.SerialPortDataBits = Convert.ToInt32(this.comboBoxDataBits.SelectedItem);
                if(this.comboBoxStopBits.SelectedItem != null)
                    Properties.Settings.Default.SerialPortStopBits = (StopBits)this.comboBoxStopBits.SelectedItem;

                Properties.Settings.Default.Save();

                Buisness.StopComPort();

                Buisness.OpenComPort(Properties.Settings.Default.SerialPortName,
                           Properties.Settings.Default.SerialPortBaud,
                           Properties.Settings.Default.SerialPortParity,
                           Properties.Settings.Default.SerialPortDataBits,
                           Properties.Settings.Default.SerialPortStopBits);

                if (Buisness.IsCOMPortActive)
                    MessageBox.Show("Configuration changed and port open");
            }
            catch (Exception er) 
            {
                MessageBox.Show("Trying to open COM port with new parameters:\r\n" + er.Message);
            }
        }
    }
}
