namespace LiveAnalyser
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.softwareParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comPortsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.simulateWithNMEALogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelSimulationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configureCOMPortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerReceivingDataShort = new System.Windows.Forms.Timer(this.components);
            this.timerReceivingDataLong = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.weather1 = new LiveAnalyser.Controls.Weather();
            this.course1 = new LiveAnalyser.Controls.Course();
            this.speedControl1 = new LiveAnalyser.Controls.SpeedControl();
            this.waypoints1 = new LiveAnalyser.Controls.Waypoints();
            this.timing1 = new LiveAnalyser.Controls.Timing();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.comPortsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(889, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.softwareParametersToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // softwareParametersToolStripMenuItem
            // 
            this.softwareParametersToolStripMenuItem.Name = "softwareParametersToolStripMenuItem";
            this.softwareParametersToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.softwareParametersToolStripMenuItem.Text = "Software Parameters";
            // 
            // comPortsToolStripMenuItem
            // 
            this.comPortsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.simulateWithNMEALogFileToolStripMenuItem,
            this.cancelSimulationToolStripMenuItem,
            this.configureCOMPortToolStripMenuItem});
            this.comPortsToolStripMenuItem.Name = "comPortsToolStripMenuItem";
            this.comPortsToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.comPortsToolStripMenuItem.Text = "COM Ports";
            // 
            // simulateWithNMEALogFileToolStripMenuItem
            // 
            this.simulateWithNMEALogFileToolStripMenuItem.Name = "simulateWithNMEALogFileToolStripMenuItem";
            this.simulateWithNMEALogFileToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.simulateWithNMEALogFileToolStripMenuItem.Text = "Simulate with NMEA log file";
            this.simulateWithNMEALogFileToolStripMenuItem.Click += new System.EventHandler(this.simulateWithNMEALogFileToolStripMenuItem_Click);
            // 
            // cancelSimulationToolStripMenuItem
            // 
            this.cancelSimulationToolStripMenuItem.Enabled = false;
            this.cancelSimulationToolStripMenuItem.Name = "cancelSimulationToolStripMenuItem";
            this.cancelSimulationToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.cancelSimulationToolStripMenuItem.Text = "Cancel simulation";
            // 
            // configureCOMPortToolStripMenuItem
            // 
            this.configureCOMPortToolStripMenuItem.Name = "configureCOMPortToolStripMenuItem";
            this.configureCOMPortToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.configureCOMPortToolStripMenuItem.Text = "Configure COM Port";
            this.configureCOMPortToolStripMenuItem.Click += new System.EventHandler(this.configureCOMPortToolStripMenuItem_Click);
            // 
            // timerReceivingDataShort
            // 
            this.timerReceivingDataShort.Interval = 500;
            this.timerReceivingDataShort.Tick += new System.EventHandler(this.timerReceivingData_Tick);
            // 
            // timerReceivingDataLong
            // 
            this.timerReceivingDataLong.Interval = 10000;
            this.timerReceivingDataLong.Tick += new System.EventHandler(this.timerReceivingDataLong_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 542);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(889, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(77, 17);
            this.toolStripStatusLabel1.Text = "Not connected";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.waypoints1);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(881, 492);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Waypoints";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.speedControl1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(881, 492);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Speed";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.course1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(881, 492);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Course";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.weather1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(881, 492);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Weather";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(889, 518);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.timing1);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(881, 492);
            this.tabPage4.TabIndex = 5;
            this.tabPage4.Text = "Timing";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // weather1
            // 
            this.weather1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.weather1.Location = new System.Drawing.Point(3, 3);
            this.weather1.Name = "weather1";
            this.weather1.Size = new System.Drawing.Size(875, 486);
            this.weather1.TabIndex = 0;
            // 
            // course1
            // 
            this.course1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.course1.Location = new System.Drawing.Point(3, 3);
            this.course1.Name = "course1";
            this.course1.Size = new System.Drawing.Size(875, 486);
            this.course1.TabIndex = 0;
            // 
            // speedControl1
            // 
            this.speedControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.speedControl1.Location = new System.Drawing.Point(3, 3);
            this.speedControl1.Name = "speedControl1";
            this.speedControl1.Size = new System.Drawing.Size(875, 486);
            this.speedControl1.TabIndex = 0;
            // 
            // waypoints1
            // 
            this.waypoints1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.waypoints1.Location = new System.Drawing.Point(3, 3);
            this.waypoints1.Name = "waypoints1";
            this.waypoints1.Size = new System.Drawing.Size(875, 486);
            this.waypoints1.TabIndex = 0;
            // 
            // timing1
            // 
            this.timing1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timing1.Location = new System.Drawing.Point(3, 3);
            this.timing1.Name = "timing1";
            this.timing1.Size = new System.Drawing.Size(875, 486);
            this.timing1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 564);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Live Analyser";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem comPortsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem simulateWithNMEALogFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cancelSimulationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configureCOMPortToolStripMenuItem;
        private System.Windows.Forms.Timer timerReceivingDataShort;
        private System.Windows.Forms.Timer timerReceivingDataLong;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.TabPage tabPage5;
        private Controls.Waypoints waypoints1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage2;
        private Controls.Course course1;
        private System.Windows.Forms.TabPage tabPage1;
        private Controls.Weather weather1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem softwareParametersToolStripMenuItem;
        private Controls.SpeedControl speedControl1;
        private System.Windows.Forms.TabPage tabPage4;
        private Controls.Timing timing1;

    }
}

