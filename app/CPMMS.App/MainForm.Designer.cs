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
            this.btnDashboard = new System.Windows.Forms.Button();
            this.btnProjects = new System.Windows.Forms.Button();
            this.btnMaterials = new System.Windows.Forms.Button();
            this.btnRequests = new System.Windows.Forms.Button();
            this.btnPurchaseOrders = new System.Windows.Forms.Button();
            this.btnVariance = new System.Windows.Forms.Button();
            this.btnSuppliers = new System.Windows.Forms.Button();
            this.btnUsers = new System.Windows.Forms.Button();
            this.lblNavBrand = new System.Windows.Forms.Label();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.btnSignOut = new System.Windows.Forms.Button();
            this.lblUser = new System.Windows.Forms.Label();
            this.lblHeading = new System.Windows.Forms.Label();
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelNav.SuspendLayout();
            this.panelNavItems.SuspendLayout();
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
            this.panelNavItems.Controls.Add(this.btnUsers);
            this.panelNavItems.Controls.Add(this.btnSuppliers);
            this.panelNavItems.Controls.Add(this.btnVariance);
            this.panelNavItems.Controls.Add(this.btnPurchaseOrders);
            this.panelNavItems.Controls.Add(this.btnRequests);
            this.panelNavItems.Controls.Add(this.btnMaterials);
            this.panelNavItems.Controls.Add(this.btnProjects);
            this.panelNavItems.Controls.Add(this.btnDashboard);
            this.panelNavItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelNavItems.Location = new System.Drawing.Point(0, 78);
            this.panelNavItems.Name = "panelNavItems";
            this.panelNavItems.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.panelNavItems.Size = new System.Drawing.Size(224, 642);
            this.panelNavItems.TabIndex = 1;
            //
            // btnDashboard
            //
            this.btnDashboard.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDashboard.FlatAppearance.BorderSize = 0;
            this.btnDashboard.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(34, 58, 68);
            this.btnDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDashboard.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.btnDashboard.ForeColor = System.Drawing.Color.FromArgb(203, 218, 222);
            this.btnDashboard.BackColor = System.Drawing.Color.FromArgb(22, 38, 46);
            this.btnDashboard.Location = new System.Drawing.Point(10, 300);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(204, 42);
            this.btnDashboard.TabIndex = 0;
            this.btnDashboard.Text = "   Dashboard";
            this.btnDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDashboard.UseVisualStyleBackColor = false;
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            //
            // btnProjects
            //
            this.btnProjects.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnProjects.FlatAppearance.BorderSize = 0;
            this.btnProjects.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(34, 58, 68);
            this.btnProjects.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProjects.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.btnProjects.ForeColor = System.Drawing.Color.FromArgb(203, 218, 222);
            this.btnProjects.BackColor = System.Drawing.Color.FromArgb(22, 38, 46);
            this.btnProjects.Location = new System.Drawing.Point(10, 258);
            this.btnProjects.Name = "btnProjects";
            this.btnProjects.Size = new System.Drawing.Size(204, 42);
            this.btnProjects.TabIndex = 1;
            this.btnProjects.Text = "   Projects";
            this.btnProjects.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnProjects.UseVisualStyleBackColor = false;
            this.btnProjects.Click += new System.EventHandler(this.btnProjects_Click);
            //
            // btnMaterials
            //
            this.btnMaterials.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnMaterials.FlatAppearance.BorderSize = 0;
            this.btnMaterials.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(34, 58, 68);
            this.btnMaterials.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMaterials.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.btnMaterials.ForeColor = System.Drawing.Color.FromArgb(203, 218, 222);
            this.btnMaterials.BackColor = System.Drawing.Color.FromArgb(22, 38, 46);
            this.btnMaterials.Location = new System.Drawing.Point(10, 216);
            this.btnMaterials.Name = "btnMaterials";
            this.btnMaterials.Size = new System.Drawing.Size(204, 42);
            this.btnMaterials.TabIndex = 2;
            this.btnMaterials.Text = "   Materials";
            this.btnMaterials.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMaterials.UseVisualStyleBackColor = false;
            this.btnMaterials.Click += new System.EventHandler(this.btnMaterials_Click);
            //
            // btnRequests
            //
            this.btnRequests.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRequests.FlatAppearance.BorderSize = 0;
            this.btnRequests.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(34, 58, 68);
            this.btnRequests.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRequests.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.btnRequests.ForeColor = System.Drawing.Color.FromArgb(203, 218, 222);
            this.btnRequests.BackColor = System.Drawing.Color.FromArgb(22, 38, 46);
            this.btnRequests.Location = new System.Drawing.Point(10, 174);
            this.btnRequests.Name = "btnRequests";
            this.btnRequests.Size = new System.Drawing.Size(204, 42);
            this.btnRequests.TabIndex = 3;
            this.btnRequests.Text = "   Material Requests";
            this.btnRequests.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRequests.UseVisualStyleBackColor = false;
            this.btnRequests.Click += new System.EventHandler(this.btnRequests_Click);
            //
            // btnPurchaseOrders
            //
            this.btnPurchaseOrders.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPurchaseOrders.FlatAppearance.BorderSize = 0;
            this.btnPurchaseOrders.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(34, 58, 68);
            this.btnPurchaseOrders.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPurchaseOrders.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.btnPurchaseOrders.ForeColor = System.Drawing.Color.FromArgb(203, 218, 222);
            this.btnPurchaseOrders.BackColor = System.Drawing.Color.FromArgb(22, 38, 46);
            this.btnPurchaseOrders.Location = new System.Drawing.Point(10, 132);
            this.btnPurchaseOrders.Name = "btnPurchaseOrders";
            this.btnPurchaseOrders.Size = new System.Drawing.Size(204, 42);
            this.btnPurchaseOrders.TabIndex = 4;
            this.btnPurchaseOrders.Text = "   Purchase Orders";
            this.btnPurchaseOrders.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPurchaseOrders.UseVisualStyleBackColor = false;
            this.btnPurchaseOrders.Click += new System.EventHandler(this.btnPurchaseOrders_Click);
            //
            // btnVariance
            //
            this.btnVariance.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnVariance.FlatAppearance.BorderSize = 0;
            this.btnVariance.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(34, 58, 68);
            this.btnVariance.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVariance.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.btnVariance.ForeColor = System.Drawing.Color.FromArgb(203, 218, 222);
            this.btnVariance.BackColor = System.Drawing.Color.FromArgb(22, 38, 46);
            this.btnVariance.Location = new System.Drawing.Point(10, 90);
            this.btnVariance.Name = "btnVariance";
            this.btnVariance.Size = new System.Drawing.Size(204, 42);
            this.btnVariance.TabIndex = 5;
            this.btnVariance.Text = "   Variance Report";
            this.btnVariance.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVariance.UseVisualStyleBackColor = false;
            this.btnVariance.Click += new System.EventHandler(this.btnVariance_Click);
            //
            // btnSuppliers
            //
            this.btnSuppliers.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSuppliers.FlatAppearance.BorderSize = 0;
            this.btnSuppliers.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(34, 58, 68);
            this.btnSuppliers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSuppliers.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.btnSuppliers.ForeColor = System.Drawing.Color.FromArgb(203, 218, 222);
            this.btnSuppliers.BackColor = System.Drawing.Color.FromArgb(22, 38, 46);
            this.btnSuppliers.Location = new System.Drawing.Point(10, 48);
            this.btnSuppliers.Name = "btnSuppliers";
            this.btnSuppliers.Size = new System.Drawing.Size(204, 42);
            this.btnSuppliers.TabIndex = 6;
            this.btnSuppliers.Text = "   Suppliers";
            this.btnSuppliers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSuppliers.UseVisualStyleBackColor = false;
            this.btnSuppliers.Click += new System.EventHandler(this.btnSuppliers_Click);
            //
            // btnUsers
            //
            this.btnUsers.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnUsers.FlatAppearance.BorderSize = 0;
            this.btnUsers.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(34, 58, 68);
            this.btnUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUsers.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.btnUsers.ForeColor = System.Drawing.Color.FromArgb(203, 218, 222);
            this.btnUsers.BackColor = System.Drawing.Color.FromArgb(22, 38, 46);
            this.btnUsers.Location = new System.Drawing.Point(10, 6);
            this.btnUsers.Name = "btnUsers";
            this.btnUsers.Size = new System.Drawing.Size(204, 42);
            this.btnUsers.TabIndex = 7;
            this.btnUsers.Text = "   Users";
            this.btnUsers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUsers.UseVisualStyleBackColor = false;
            this.btnUsers.Click += new System.EventHandler(this.btnUsers_Click);
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
            this.panelNavItems.ResumeLayout(false);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelNav;
        private System.Windows.Forms.Label lblNavBrand;
        private System.Windows.Forms.Panel panelNavItems;
        private System.Windows.Forms.Button btnDashboard;
        private System.Windows.Forms.Button btnProjects;
        private System.Windows.Forms.Button btnMaterials;
        private System.Windows.Forms.Button btnRequests;
        private System.Windows.Forms.Button btnPurchaseOrders;
        private System.Windows.Forms.Button btnVariance;
        private System.Windows.Forms.Button btnSuppliers;
        private System.Windows.Forms.Button btnUsers;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Button btnSignOut;
        private System.Windows.Forms.Panel panelContent;
    }
}
