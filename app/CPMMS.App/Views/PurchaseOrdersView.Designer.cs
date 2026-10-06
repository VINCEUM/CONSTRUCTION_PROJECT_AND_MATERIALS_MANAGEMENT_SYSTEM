namespace CPMMS.App.Views
{
    partial class PurchaseOrdersView
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
            this.btnReceive = new System.Windows.Forms.Button();
            this.chkOpenOnly = new System.Windows.Forms.CheckBox();
            this.panelDetail = new System.Windows.Forms.Panel();
            this.detailHost = new System.Windows.Forms.Panel();
            this.gridItems = new System.Windows.Forms.DataGridView();
            this.lblDetailTitle = new System.Windows.Forms.Label();
            this.gridHost = new System.Windows.Forms.Panel();
            this.gridOrders = new System.Windows.Forms.DataGridView();
            this.panelBar.SuspendLayout();
            this.panelDetail.SuspendLayout();
            this.detailHost.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridItems)).BeginInit();
            this.gridHost.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridOrders)).BeginInit();
            this.SuspendLayout();
            //
            // panelBar
            //
            this.panelBar.BackColor = System.Drawing.Color.FromArgb(247, 249, 249);
            this.panelBar.Controls.Add(this.btnReceive);
            this.panelBar.Controls.Add(this.chkOpenOnly);
            this.panelBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBar.Location = new System.Drawing.Point(0, 0);
            this.panelBar.Name = "panelBar";
            this.panelBar.Size = new System.Drawing.Size(900, 44);
            this.panelBar.TabIndex = 0;
            //
            // chkOpenOnly
            //
            this.chkOpenOnly.AutoSize = true;
            this.chkOpenOnly.Checked = true;
            this.chkOpenOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOpenOnly.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.chkOpenOnly.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.chkOpenOnly.Location = new System.Drawing.Point(0, 11);
            this.chkOpenOnly.Name = "chkOpenOnly";
            this.chkOpenOnly.Size = new System.Drawing.Size(128, 21);
            this.chkOpenOnly.TabIndex = 0;
            this.chkOpenOnly.Text = "Open orders only";
            this.chkOpenOnly.CheckedChanged += new System.EventHandler(this.chkOpenOnly_CheckedChanged);
            //
            // btnReceive
            //
            this.btnReceive.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(225, 231, 233);
            this.btnReceive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReceive.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.btnReceive.BackColor = System.Drawing.Color.White;
            this.btnReceive.Location = new System.Drawing.Point(160, 7);
            this.btnReceive.Name = "btnReceive";
            this.btnReceive.Size = new System.Drawing.Size(140, 28);
            this.btnReceive.TabIndex = 1;
            this.btnReceive.Text = "Receive delivery...";
            this.btnReceive.UseVisualStyleBackColor = false;
            this.btnReceive.Click += new System.EventHandler(this.btnReceive_Click);
            //
            // panelDetail
            //
            this.panelDetail.BackColor = System.Drawing.Color.FromArgb(247, 249, 249);
            this.panelDetail.Controls.Add(this.detailHost);
            this.panelDetail.Controls.Add(this.lblDetailTitle);
            this.panelDetail.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelDetail.Location = new System.Drawing.Point(0, 290);
            this.panelDetail.Name = "panelDetail";
            this.panelDetail.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.panelDetail.Size = new System.Drawing.Size(900, 230);
            this.panelDetail.TabIndex = 1;
            //
            // lblDetailTitle
            //
            this.lblDetailTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDetailTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 11F);
            this.lblDetailTitle.ForeColor = System.Drawing.Color.FromArgb(18, 42, 51);
            this.lblDetailTitle.Location = new System.Drawing.Point(0, 8);
            this.lblDetailTitle.Name = "lblDetailTitle";
            this.lblDetailTitle.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.lblDetailTitle.Size = new System.Drawing.Size(900, 30);
            this.lblDetailTitle.TabIndex = 0;
            this.lblDetailTitle.Text = "Ordered items";
            //
            // detailHost
            //
            this.detailHost.BackColor = System.Drawing.Color.White;
            this.detailHost.Controls.Add(this.gridItems);
            this.detailHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailHost.Location = new System.Drawing.Point(0, 38);
            this.detailHost.Name = "detailHost";
            this.detailHost.Padding = new System.Windows.Forms.Padding(1);
            this.detailHost.Size = new System.Drawing.Size(900, 192);
            this.detailHost.TabIndex = 1;
            //
            // gridItems
            //
            this.gridItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridItems.Location = new System.Drawing.Point(1, 1);
            this.gridItems.Name = "gridItems";
            this.gridItems.Size = new System.Drawing.Size(898, 190);
            this.gridItems.TabIndex = 0;
            this.gridItems.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gridItems_CellFormatting);
            //
            // gridHost
            //
            this.gridHost.BackColor = System.Drawing.Color.White;
            this.gridHost.Controls.Add(this.gridOrders);
            this.gridHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridHost.Location = new System.Drawing.Point(0, 44);
            this.gridHost.Name = "gridHost";
            this.gridHost.Padding = new System.Windows.Forms.Padding(1);
            this.gridHost.Size = new System.Drawing.Size(900, 246);
            this.gridHost.TabIndex = 2;
            //
            // gridOrders
            //
            this.gridOrders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridOrders.Location = new System.Drawing.Point(1, 1);
            this.gridOrders.Name = "gridOrders";
            this.gridOrders.Size = new System.Drawing.Size(898, 244);
            this.gridOrders.TabIndex = 0;
            this.gridOrders.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridOrders_CellDoubleClick);
            this.gridOrders.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gridOrders_CellFormatting);
            this.gridOrders.SelectionChanged += new System.EventHandler(this.gridOrders_SelectionChanged);
            //
            // PurchaseOrdersView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(247, 249, 249);
            this.Controls.Add(this.gridHost);
            this.Controls.Add(this.panelDetail);
            this.Controls.Add(this.panelBar);
            this.Name = "PurchaseOrdersView";
            this.Size = new System.Drawing.Size(900, 520);
            this.panelBar.ResumeLayout(false);
            this.panelBar.PerformLayout();
            this.panelDetail.ResumeLayout(false);
            this.detailHost.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridItems)).EndInit();
            this.gridHost.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridOrders)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelBar;
        private System.Windows.Forms.CheckBox chkOpenOnly;
        private System.Windows.Forms.Button btnReceive;
        private System.Windows.Forms.Panel panelDetail;
        private System.Windows.Forms.Label lblDetailTitle;
        private System.Windows.Forms.Panel detailHost;
        private System.Windows.Forms.DataGridView gridItems;
        private System.Windows.Forms.Panel gridHost;
        private System.Windows.Forms.DataGridView gridOrders;
    }
}
