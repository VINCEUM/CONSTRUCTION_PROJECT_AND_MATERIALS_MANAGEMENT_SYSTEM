namespace CPMMS.App
{
    partial class LedgerForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSub = new System.Windows.Forms.Label();
            this.panelHost = new System.Windows.Forms.Panel();
            this.gridLedger = new System.Windows.Forms.DataGridView();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.lblFooter = new System.Windows.Forms.Label();
            this.panelHeader.SuspendLayout();
            this.panelHost.SuspendLayout();
            this.panelFooter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridLedger)).BeginInit();
            this.SuspendLayout();
            //
            // panelHeader
            //
            this.panelHeader.BackColor = System.Drawing.Color.White;
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Controls.Add(this.lblSub);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Padding = new System.Windows.Forms.Padding(18, 14, 18, 8);
            this.panelHeader.Size = new System.Drawing.Size(940, 84);
            this.panelHeader.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 16F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(18, 42, 51);
            this.lblTitle.Location = new System.Drawing.Point(18, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(904, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Material code  ·  Material name";
            //
            // lblSub
            //
            this.lblSub.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblSub.ForeColor = System.Drawing.Color.FromArgb(126, 144, 153);
            this.lblSub.Location = new System.Drawing.Point(18, 54);
            this.lblSub.Name = "lblSub";
            this.lblSub.Size = new System.Drawing.Size(904, 22);
            this.lblSub.TabIndex = 1;
            this.lblSub.Text = "On hand";
            //
            // panelHost
            //
            this.panelHost.BackColor = System.Drawing.Color.White;
            this.panelHost.Controls.Add(this.gridLedger);
            this.panelHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHost.Location = new System.Drawing.Point(0, 84);
            this.panelHost.Name = "panelHost";
            this.panelHost.Padding = new System.Windows.Forms.Padding(1);
            this.panelHost.Size = new System.Drawing.Size(940, 436);
            this.panelHost.TabIndex = 1;
            //
            // gridLedger
            //
            this.gridLedger.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridLedger.Location = new System.Drawing.Point(1, 1);
            this.gridLedger.Name = "gridLedger";
            this.gridLedger.Size = new System.Drawing.Size(938, 434);
            this.gridLedger.TabIndex = 0;
            //
            // panelFooter
            //
            this.panelFooter.Controls.Add(this.lblFooter);
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFooter.Location = new System.Drawing.Point(0, 520);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Padding = new System.Windows.Forms.Padding(12, 0, 12, 0);
            this.panelFooter.Size = new System.Drawing.Size(940, 40);
            this.panelFooter.TabIndex = 2;
            //
            // lblFooter
            //
            this.lblFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFooter.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblFooter.Location = new System.Drawing.Point(12, 0);
            this.lblFooter.Name = "lblFooter";
            this.lblFooter.Size = new System.Drawing.Size(916, 40);
            this.lblFooter.TabIndex = 0;
            this.lblFooter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // LedgerForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(247, 249, 249);
            this.ClientSize = new System.Drawing.Size(940, 560);
            this.Controls.Add(this.panelHost);
            this.Controls.Add(this.panelFooter);
            this.Controls.Add(this.panelHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.Name = "LedgerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Stock card";
            this.panelHeader.ResumeLayout(false);
            this.panelHost.ResumeLayout(false);
            this.panelFooter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridLedger)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSub;
        private System.Windows.Forms.Panel panelHost;
        private System.Windows.Forms.DataGridView gridLedger;
        private System.Windows.Forms.Panel panelFooter;
        private System.Windows.Forms.Label lblFooter;
    }
}
