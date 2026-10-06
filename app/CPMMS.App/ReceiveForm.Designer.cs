namespace CPMMS.App
{
    partial class ReceiveForm
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
            this.panelFields = new System.Windows.Forms.Panel();
            this.lblDr = new System.Windows.Forms.Label();
            this.txtDrNo = new System.Windows.Forms.TextBox();
            this.lblDate = new System.Windows.Forms.Label();
            this.dtDate = new System.Windows.Forms.DateTimePicker();
            this.lblSub = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelHost = new System.Windows.Forms.Panel();
            this.gridLines = new System.Windows.Forms.DataGridView();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.flowButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnPost = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblError = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.panelHeader.SuspendLayout();
            this.panelFields.SuspendLayout();
            this.panelHost.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridLines)).BeginInit();
            this.panelFooter.SuspendLayout();
            this.flowButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // panelHeader
            //
            this.panelHeader.BackColor = System.Drawing.Color.White;
            this.panelHeader.Controls.Add(this.panelFields);
            this.panelHeader.Controls.Add(this.lblSub);
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Padding = new System.Windows.Forms.Padding(18, 12, 18, 8);
            this.panelHeader.Size = new System.Drawing.Size(1020, 96);
            this.panelHeader.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 16F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(18, 42, 51);
            this.lblTitle.Location = new System.Drawing.Point(18, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(984, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "PO no.  ·  Supplier";
            //
            // lblSub
            //
            this.lblSub.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSub.ForeColor = System.Drawing.Color.FromArgb(126, 144, 153);
            this.lblSub.Location = new System.Drawing.Point(18, 42);
            this.lblSub.Name = "lblSub";
            this.lblSub.Size = new System.Drawing.Size(984, 20);
            this.lblSub.TabIndex = 1;
            this.lblSub.Text = "Ordered";
            //
            // panelFields
            //
            this.panelFields.Controls.Add(this.dtDate);
            this.panelFields.Controls.Add(this.lblDate);
            this.panelFields.Controls.Add(this.txtDrNo);
            this.panelFields.Controls.Add(this.lblDr);
            this.panelFields.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFields.Location = new System.Drawing.Point(18, 62);
            this.panelFields.Name = "panelFields";
            this.panelFields.Size = new System.Drawing.Size(984, 34);
            this.panelFields.TabIndex = 2;
            //
            // lblDr
            //
            this.lblDr.AutoSize = true;
            this.lblDr.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblDr.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.lblDr.Location = new System.Drawing.Point(0, 8);
            this.lblDr.Name = "lblDr";
            this.lblDr.Size = new System.Drawing.Size(66, 17);
            this.lblDr.TabIndex = 0;
            this.lblDr.Text = "DR number";
            //
            // txtDrNo
            //
            this.txtDrNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDrNo.Location = new System.Drawing.Point(78, 4);
            this.txtDrNo.Name = "txtDrNo";
            this.txtDrNo.PlaceholderText = "Supplier's receipt no.";
            this.txtDrNo.Size = new System.Drawing.Size(220, 25);
            this.txtDrNo.TabIndex = 1;
            //
            // lblDate
            //
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblDate.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.lblDate.Location = new System.Drawing.Point(322, 8);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(80, 17);
            this.lblDate.TabIndex = 2;
            this.lblDate.Text = "Delivery date";
            //
            // dtDate
            //
            this.dtDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtDate.Location = new System.Drawing.Point(414, 4);
            this.dtDate.Name = "dtDate";
            this.dtDate.Size = new System.Drawing.Size(160, 25);
            this.dtDate.TabIndex = 3;
            //
            // panelHost
            //
            this.panelHost.BackColor = System.Drawing.Color.White;
            this.panelHost.Controls.Add(this.gridLines);
            this.panelHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHost.Location = new System.Drawing.Point(0, 96);
            this.panelHost.Name = "panelHost";
            this.panelHost.Padding = new System.Windows.Forms.Padding(1);
            this.panelHost.Size = new System.Drawing.Size(1020, 352);
            this.panelHost.TabIndex = 1;
            //
            // gridLines
            //
            this.gridLines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridLines.Location = new System.Drawing.Point(1, 1);
            this.gridLines.Name = "gridLines";
            this.gridLines.Size = new System.Drawing.Size(1018, 350);
            this.gridLines.TabIndex = 0;
            //
            // panelFooter
            //
            this.panelFooter.BackColor = System.Drawing.Color.White;
            this.panelFooter.Controls.Add(this.flowButtons);
            this.panelFooter.Controls.Add(this.lblError);
            this.panelFooter.Controls.Add(this.lblTotal);
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFooter.Location = new System.Drawing.Point(0, 448);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Padding = new System.Windows.Forms.Padding(18, 10, 18, 10);
            this.panelFooter.Size = new System.Drawing.Size(1020, 92);
            this.panelFooter.TabIndex = 2;
            //
            // lblTotal
            //
            this.lblTotal.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI Semibold", 11F);
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(18, 42, 51);
            this.lblTotal.Location = new System.Drawing.Point(18, 10);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(984, 26);
            this.lblTotal.TabIndex = 0;
            //
            // lblError
            //
            this.lblError.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblError.ForeColor = System.Drawing.Color.FromArgb(192, 82, 72);
            this.lblError.Location = new System.Drawing.Point(18, 36);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(984, 20);
            this.lblError.TabIndex = 1;
            //
            // flowButtons
            //
            this.flowButtons.Controls.Add(this.btnPost);
            this.flowButtons.Controls.Add(this.btnCancel);
            this.flowButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowButtons.Location = new System.Drawing.Point(18, 46);
            this.flowButtons.Name = "flowButtons";
            this.flowButtons.Size = new System.Drawing.Size(984, 36);
            this.flowButtons.TabIndex = 2;
            //
            // btnPost
            //
            this.btnPost.BackColor = System.Drawing.Color.FromArgb(31, 122, 107);
            this.btnPost.FlatAppearance.BorderSize = 0;
            this.btnPost.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPost.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.btnPost.ForeColor = System.Drawing.Color.White;
            this.btnPost.Location = new System.Drawing.Point(851, 3);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(130, 32);
            this.btnPost.TabIndex = 0;
            this.btnPost.Text = "Post delivery";
            this.btnPost.UseVisualStyleBackColor = false;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            //
            // btnCancel
            //
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(225, 231, 233);
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(765, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // ReceiveForm
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(247, 249, 249);
            this.ClientSize = new System.Drawing.Size(1020, 540);
            this.Controls.Add(this.panelHost);
            this.Controls.Add(this.panelFooter);
            this.Controls.Add(this.panelHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.Name = "ReceiveForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Receive delivery";
            this.panelHeader.ResumeLayout(false);
            this.panelFields.ResumeLayout(false);
            this.panelFields.PerformLayout();
            this.panelHost.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridLines)).EndInit();
            this.panelFooter.ResumeLayout(false);
            this.flowButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSub;
        private System.Windows.Forms.Panel panelFields;
        private System.Windows.Forms.Label lblDr;
        private System.Windows.Forms.TextBox txtDrNo;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.DateTimePicker dtDate;
        private System.Windows.Forms.Panel panelHost;
        private System.Windows.Forms.DataGridView gridLines;
        private System.Windows.Forms.Panel panelFooter;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.FlowLayoutPanel flowButtons;
        private System.Windows.Forms.Button btnPost;
        private System.Windows.Forms.Button btnCancel;
    }
}
