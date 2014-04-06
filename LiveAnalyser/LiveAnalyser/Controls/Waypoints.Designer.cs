namespace LiveAnalyser.Controls
{
    partial class Waypoints
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.projection1 = new LiveAnalyser.Controls.Projection();
            this.waypointsManagement1 = new LiveAnalyser.Controls.WaypointsControls.WaypointsManagement();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.projection1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.waypointsManagement1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(777, 454);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // projection1
            // 
            this.projection1.AutoSize = true;
            this.projection1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.projection1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projection1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.projection1.Location = new System.Drawing.Point(3, 3);
            this.projection1.MinimumSize = new System.Drawing.Size(346, 286);
            this.projection1.Name = "projection1";
            this.projection1.Size = new System.Drawing.Size(382, 448);
            this.projection1.TabIndex = 0;
            // 
            // waypointsManagement1
            // 
            this.waypointsManagement1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.waypointsManagement1.Location = new System.Drawing.Point(391, 3);
            this.waypointsManagement1.Name = "waypointsManagement1";
            this.waypointsManagement1.Size = new System.Drawing.Size(383, 448);
            this.waypointsManagement1.TabIndex = 1;
            // 
            // Waypoints
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Waypoints";
            this.Size = new System.Drawing.Size(777, 454);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Projection projection1;
        private WaypointsControls.WaypointsManagement waypointsManagement1;
    }
}
