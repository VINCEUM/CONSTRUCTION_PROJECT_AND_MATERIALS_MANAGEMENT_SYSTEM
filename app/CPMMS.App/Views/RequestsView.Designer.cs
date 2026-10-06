namespace CPMMS.App.Views
{
    partial class RequestsView
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnReject = new System.Windows.Forms.Button();
            this.btnApprove = new System.Windows.Forms.Button();
            this.btnIssue = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.panelDetail = new System.Windows.Forms.Panel();
            this.detailHost = new System.Windows.Forms.Panel();
            this.gridItems = new System.Windows.Forms.DataGridView();
            this.lblDetailTitle = new System.Windows.Forms.Label();
            this.gridHost = new System.Windows.Forms.Panel();
            this.gridRequests = new System.Windows.Forms.DataGridView();
            this.panelBar.SuspendLayout();
            this.panelDetail.SuspendLayout();
            this.detailHost.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridItems)).BeginInit();
            this.gridHost.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridRequests)).BeginInit();
            this.SuspendLayout();
            //
            // panelBar
            //
            this.panelBar.BackColor = System.Drawing.Color.FromArgb(247, 249, 249);
            this.panelBar.Controls.Add(this.btnCancel);
            this.panelBar.Controls.Add(this.btnSubmit);
            this.panelBar.Controls.Add(this.btnReject);
            this.panelBar.Controls.Add(this.btnApprove);
            this.panelBar.Controls.Add(this.btnIssue);
            this.panelBar.Controls.Add(this.btnNew);
            this.panelBar.Controls.Add(this.cmbStatus);
            this.panelBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBar.Location = new System.Drawing.Point(0, 0);
            this.panelBar.Name = "panelBar";
            this.panelBar.Size = new System.Drawing.Size(900, 44);
            this.panelBar.TabIndex = 0;
            //
            // cmbStatus
            //
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.cmbStatus.Items.AddRange(new object[] {
            "All statuses", "draft", "submitted", "approved", "partially_issued", "issued",
            "rejected", "cancelled", "voided"});
            this.cmbStatus.Location = new System.Drawing.Point(0, 8);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(200, 26);
            this.cmbStatus.TabIndex = 0;
            this.cmbStatus.SelectedIndexChanged += new System.EventHandler(this.cmbStatus_SelectedIndexChanged);
            //
            // btnNew
            //
            this.btnNew.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(225, 231, 233);
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.btnNew.BackColor = System.Drawing.Color.White;
            this.btnNew.Location = new System.Drawing.Point(212, 7);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(122, 28);
            this.btnNew.TabIndex = 1;
            this.btnNew.Text = "New request...";
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            //
            // btnIssue
            //
            this.btnIssue.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(225, 231, 233);
            this.btnIssue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIssue.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.btnIssue.BackColor = System.Drawing.Color.White;
            this.btnIssue.Location = new System.Drawing.Point(342, 7);
            this.btnIssue.Name = "btnIssue";
            this.btnIssue.Size = new System.Drawing.Size(130, 28);
            this.btnIssue.TabIndex = 2;
            this.btnIssue.Text = "Issue materials...";
            this.btnIssue.UseVisualStyleBackColor = false;
            this.btnIssue.Click += new System.EventHandler(this.btnIssue_Click);
            //
            // btnApprove
            //
            this.btnApprove.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(31, 122, 107);
            this.btnApprove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApprove.ForeColor = System.Drawing.Color.FromArgb(31, 122, 107);
            this.btnApprove.BackColor = System.Drawing.Color.White;
            this.btnApprove.Enabled = false;
            this.btnApprove.Location = new System.Drawing.Point(480, 7);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(80, 28);
            this.btnApprove.TabIndex = 3;
            this.btnApprove.Text = "Approve";
            this.btnApprove.UseVisualStyleBackColor = false;
            this.btnApprove.Click += new System.EventHandler(this.btnApprove_Click);
            //
            // btnReject
            //
            this.btnReject.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(192, 82, 72);
            this.btnReject.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReject.ForeColor = System.Drawing.Color.FromArgb(192, 82, 72);
            this.btnReject.BackColor = System.Drawing.Color.White;
            this.btnReject.Enabled = false;
            this.btnReject.Location = new System.Drawing.Point(568, 7);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new System.Drawing.Size(80, 28);
            this.btnReject.TabIndex = 4;
            this.btnReject.Text = "Reject...";
            this.btnReject.UseVisualStyleBackColor = false;
            this.btnReject.Click += new System.EventHandler(this.btnReject_Click);
            //
            // btnSubmit
            //
            this.btnSubmit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(225, 231, 233);
            this.btnSubmit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSubmit.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.btnSubmit.BackColor = System.Drawing.Color.White;
            this.btnSubmit.Enabled = false;
            this.btnSubmit.Location = new System.Drawing.Point(656, 7);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(80, 28);
            this.btnSubmit.TabIndex = 5;
            this.btnSubmit.Text = "Submit...";
            this.btnSubmit.UseVisualStyleBackColor = false;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            //
            // btnCancel
            //
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(192, 82, 72);
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(192, 82, 72);
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(744, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 28);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel...";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // panelDetail
            //
            this.panelDetail.BackColor = System.Drawing.Color.FromArgb(247, 249, 249);
            this.panelDetail.Controls.Add(this.detailHost);
            this.panelDetail.Controls.Add(this.lblDetailTitle);
            this.panelDetail.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelDetail.Location = new System.Drawing.Point(0, 280);
            this.panelDetail.Name = "panelDetail";
            this.panelDetail.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.panelDetail.Size = new System.Drawing.Size(900, 240);
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
            this.lblDetailTitle.Text = "Requested items";
            //
            // detailHost
            //
            this.detailHost.BackColor = System.Drawing.Color.White;
            this.detailHost.Controls.Add(this.gridItems);
            this.detailHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailHost.Location = new System.Drawing.Point(0, 38);
            this.detailHost.Name = "detailHost";
            this.detailHost.Padding = new System.Windows.Forms.Padding(1);
            this.detailHost.Size = new System.Drawing.Size(900, 202);
            this.detailHost.TabIndex = 1;
            //
            // gridItems
            //
            this.gridItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridItems.Location = new System.Drawing.Point(1, 1);
            this.gridItems.Name = "gridItems";
            this.gridItems.Size = new System.Drawing.Size(898, 200);
            this.gridItems.TabIndex = 0;
            this.gridItems.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gridItems_CellFormatting);
            //
            // gridHost
            //
            this.gridHost.BackColor = System.Drawing.Color.White;
            this.gridHost.Controls.Add(this.gridRequests);
            this.gridHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridHost.Location = new System.Drawing.Point(0, 44);
            this.gridHost.Name = "gridHost";
            this.gridHost.Padding = new System.Windows.Forms.Padding(1);
            this.gridHost.Size = new System.Drawing.Size(900, 236);
            this.gridHost.TabIndex = 2;
            //
            // gridRequests
            //
            this.gridRequests.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridRequests.Location = new System.Drawing.Point(1, 1);
            this.gridRequests.Name = "gridRequests";
            this.gridRequests.Size = new System.Drawing.Size(898, 234);
            this.gridRequests.TabIndex = 0;
            this.gridRequests.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridRequests_CellDoubleClick);
            this.gridRequests.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gridRequests_CellFormatting);
            this.gridRequests.SelectionChanged += new System.EventHandler(this.gridRequests_SelectionChanged);
            //
            // RequestsView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(247, 249, 249);
            this.Controls.Add(this.gridHost);
            this.Controls.Add(this.panelDetail);
            this.Controls.Add(this.panelBar);
            this.Name = "RequestsView";
            this.Size = new System.Drawing.Size(900, 520);
            this.panelBar.ResumeLayout(false);
            this.panelDetail.ResumeLayout(false);
            this.detailHost.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridItems)).EndInit();
            this.gridHost.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridRequests)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelBar;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnIssue;
        private System.Windows.Forms.Button btnApprove;
        private System.Windows.Forms.Button btnReject;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panelDetail;
        private System.Windows.Forms.Label lblDetailTitle;
        private System.Windows.Forms.Panel detailHost;
        private System.Windows.Forms.DataGridView gridItems;
        private System.Windows.Forms.Panel gridHost;
        private System.Windows.Forms.DataGridView gridRequests;
    }
}
