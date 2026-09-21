namespace CPMMS.App
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelNav = new System.Windows.Forms.Panel();
            this.panelNavItems = new System.Windows.Forms.Panel();
            this.lblNavBrand = new System.Windows.Forms.Label();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.btnSignOut = new System.Windows.Forms.Button();
            this.lblUser = new System.Windows.Forms.Label();
            this.lblHeading = new System.Windows.Forms.Label();
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelNav.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.SuspendLayout();
            //
            // panelNav
            //
            this.panelNav.BackColor = System.Drawing.Color.FromArgb(22, 38, 46);
            this.panelNav.Controls.Add(this.panelNavItems);
            this.panelNav.Controls.Add(this.lblNavBrand);
            this.panelNav.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelNav.Location = new System.Drawing.Point(0, 0);
            this.panelNav.Name = "panelNav";
            this.panelNav.Size = new System.Drawing.Size(224, 720);
            this.panelNav.TabIndex = 0;
            //
            // lblNavBrand
            //
            this.lblNavBrand.AutoSize = false;
            this.lblNavBrand.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblNavBrand.Font = new System.Drawing.Font("Segoe UI Semibold", 11F);
            this.lblNavBrand.ForeColor = System.Drawing.Color.White;
            this.lblNavBrand.Location = new System.Drawing.Point(0, 0);
            this.lblNavBrand.Name = "lblNavBrand";
            this.lblNavBrand.Padding = new System.Windows.Forms.Padding(18, 0, 12, 0);
            this.lblNavBrand.Size = new System.Drawing.Size(224, 78);
            this.lblNavBrand.TabIndex = 0;
            this.lblNavBrand.Text = "CPMMS";
            this.lblNavBrand.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // panelNavItems
            //
            this.panelNavItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelNavItems.Location = new System.Drawing.Point(0, 78);
            this.panelNavItems.Name = "panelNavItems";
            this.panelNavItems.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.panelNavItems.Size = new System.Drawing.Size(224, 642);
            this.panelNavItems.TabIndex = 1;
            //
            // panelHeader
            //
            this.panelHeader.BackColor = System.Drawing.Color.White;
            this.panelHeader.Controls.Add(this.btnSignOut);
            this.panelHeader.Controls.Add(this.lblUser);
            this.panelHeader.Controls.Add(this.lblHeading);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(224, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(976, 64);
            this.panelHeader.TabIndex = 1;
            //
            // lblHeading
            //
            this.lblHeading.AutoSize = true;
            this.lblHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 14F);
            this.lblHeading.ForeColor = System.Drawing.Color.FromArgb(18, 42, 51);
            this.lblHeading.Location = new System.Drawing.Point(22, 18);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(110, 25);
            this.lblHeading.TabIndex = 0;
            this.lblHeading.Text = "Dashboard";
            //
            // lblUser
            //
            this.lblUser.AutoSize = false;
            this.lblUser.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.lblUser.ForeColor = System.Drawing.Color.FromArgb(126, 144, 153);
            this.lblUser.Location = new System.Drawing.Point(626, 22);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(240, 20);
            this.lblUser.TabIndex = 1;
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // btnSignOut
            //
            this.btnSignOut.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnSignOut.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(225, 231, 233);
            this.btnSignOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSignOut.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.btnSignOut.Location = new System.Drawing.Point(876, 18);
            this.btnSignOut.Name = "btnSignOut";
            this.btnSignOut.Size = new System.Drawing.Size(80, 28);
            this.btnSignOut.TabIndex = 2;
            this.btnSignOut.Text = "Sign out";
            this.btnSignOut.UseVisualStyleBackColor = true;
            this.btnSignOut.Click += new System.EventHandler(this.btnSignOut_Click);
            //
            // panelContent
            //
            this.panelContent.BackColor = System.Drawing.Color.FromArgb(247, 249, 249);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(224, 64);
            this.panelContent.Name = "panelContent";
            this.panelContent.Padding = new System.Windows.Forms.Padding(18);
            this.panelContent.Size = new System.Drawing.Size(976, 656);
            this.panelContent.TabIndex = 2;
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1200, 720);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panelNav);
            this.MinimumSize = new System.Drawing.Size(1080, 640);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Construction Project and Materials Management System";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panelNav.ResumeLayout(false);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelNav;
        private System.Windows.Forms.Label lblNavBrand;
        private System.Windows.Forms.Panel panelNavItems;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Button btnSignOut;
        private System.Windows.Forms.Panel panelContent;
    }
}
