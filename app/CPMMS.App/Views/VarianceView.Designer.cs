namespace CPMMS.App.Views
{
    partial class VarianceView
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelBar = new System.Windows.Forms.Panel();
            this.lblSummary = new System.Windows.Forms.Label();
            this.chkOverruns = new System.Windows.Forms.CheckBox();
            this.cmbProject = new System.Windows.Forms.ComboBox();
            this.gridHost = new System.Windows.Forms.Panel();
            this.grid = new System.Windows.Forms.DataGridView();
            this.panelBar.SuspendLayout();
            this.gridHost.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.SuspendLayout();
            //
            // panelBar
            //
            this.panelBar.BackColor = System.Drawing.Color.FromArgb(247, 249, 249);
            this.panelBar.Controls.Add(this.lblSummary);
            this.panelBar.Controls.Add(this.chkOverruns);
            this.panelBar.Controls.Add(this.cmbProject);
            this.panelBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBar.Location = new System.Drawing.Point(0, 0);
            this.panelBar.Name = "panelBar";
            this.panelBar.Size = new System.Drawing.Size(900, 46);
            this.panelBar.TabIndex = 0;
            //
            // cmbProject
            //
            this.cmbProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProject.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.cmbProject.Location = new System.Drawing.Point(0, 8);
            this.cmbProject.Name = "cmbProject";
            this.cmbProject.Size = new System.Drawing.Size(300, 26);
            this.cmbProject.TabIndex = 0;
            this.cmbProject.SelectedIndexChanged += new System.EventHandler(this.cmbProject_SelectedIndexChanged);
            //
            // chkOverruns
            //
            this.chkOverruns.AutoSize = true;
            this.chkOverruns.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.chkOverruns.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.chkOverruns.Location = new System.Drawing.Point(316, 11);
            this.chkOverruns.Name = "chkOverruns";
            this.chkOverruns.Size = new System.Drawing.Size(180, 21);
            this.chkOverruns.TabIndex = 1;
            this.chkOverruns.Text = "Overruns only (over 10%)";
            this.chkOverruns.CheckedChanged += new System.EventHandler(this.chkOverruns_CheckedChanged);
            //
            // lblSummary
            //
            this.lblSummary.AutoSize = false;
            this.lblSummary.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblSummary.ForeColor = System.Drawing.Color.FromArgb(126, 144, 153);
            this.lblSummary.Location = new System.Drawing.Point(530, 14);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(420, 20);
            this.lblSummary.TabIndex = 2;
            //
            // gridHost
            //
            this.gridHost.BackColor = System.Drawing.Color.White;
            this.gridHost.Controls.Add(this.grid);
            this.gridHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridHost.Location = new System.Drawing.Point(0, 46);
            this.gridHost.Name = "gridHost";
            this.gridHost.Padding = new System.Windows.Forms.Padding(1);
            this.gridHost.Size = new System.Drawing.Size(900, 474);
            this.gridHost.TabIndex = 1;
            //
            // grid
            //
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Location = new System.Drawing.Point(1, 1);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(898, 472);
            this.grid.TabIndex = 0;
            this.grid.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.grid_CellFormatting);
            //
            // VarianceView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(247, 249, 249);
            this.Controls.Add(this.gridHost);
            this.Controls.Add(this.panelBar);
            this.Name = "VarianceView";
            this.Size = new System.Drawing.Size(900, 520);
            this.panelBar.ResumeLayout(false);
            this.panelBar.PerformLayout();
            this.gridHost.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelBar;
        private System.Windows.Forms.ComboBox cmbProject;
        private System.Windows.Forms.CheckBox chkOverruns;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.Panel gridHost;
        private System.Windows.Forms.DataGridView grid;
    }
}
