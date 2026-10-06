namespace CPMMS.App
{
    partial class NewRequestForm
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
            this.panelRemarks = new System.Windows.Forms.Panel();
            this.txtRemarks = new System.Windows.Forms.TextBox();
            this.lblRemarks = new System.Windows.Forms.Label();
            this.panelFields = new System.Windows.Forms.Panel();
            this.dtNeeded = new System.Windows.Forms.DateTimePicker();
            this.lblNeeded = new System.Windows.Forms.Label();
            this.cmbProject = new System.Windows.Forms.ComboBox();
            this.lblProject = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelAddBar = new System.Windows.Forms.Panel();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.numQty = new System.Windows.Forms.NumericUpDown();
            this.cmbMaterial = new System.Windows.Forms.ComboBox();
            this.panelHost = new System.Windows.Forms.Panel();
            this.grid = new System.Windows.Forms.DataGridView();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.flowButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnSaveDraft = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblError = new System.Windows.Forms.Label();
            this.panelHeader.SuspendLayout();
            this.panelRemarks.SuspendLayout();
            this.panelFields.SuspendLayout();
            this.panelAddBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQty)).BeginInit();
            this.panelHost.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.panelFooter.SuspendLayout();
            this.flowButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // panelHeader
            //
            this.panelHeader.BackColor = System.Drawing.Color.White;
            this.panelHeader.Controls.Add(this.panelRemarks);
            this.panelHeader.Controls.Add(this.panelFields);
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Padding = new System.Windows.Forms.Padding(18, 12, 18, 8);
            this.panelHeader.Size = new System.Drawing.Size(820, 128);
            this.panelHeader.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 16F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(18, 42, 51);
            this.lblTitle.Location = new System.Drawing.Point(18, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(784, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "New material request";
            //
            // panelFields
            //
            this.panelFields.Controls.Add(this.dtNeeded);
            this.panelFields.Controls.Add(this.lblNeeded);
            this.panelFields.Controls.Add(this.cmbProject);
            this.panelFields.Controls.Add(this.lblProject);
            this.panelFields.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFields.Location = new System.Drawing.Point(18, 42);
            this.panelFields.Name = "panelFields";
            this.panelFields.Size = new System.Drawing.Size(784, 34);
            this.panelFields.TabIndex = 1;
            //
            // lblProject
            //
            this.lblProject.AutoSize = true;
            this.lblProject.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblProject.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.lblProject.Location = new System.Drawing.Point(0, 8);
            this.lblProject.Name = "lblProject";
            this.lblProject.Size = new System.Drawing.Size(48, 17);
            this.lblProject.TabIndex = 0;
            this.lblProject.Text = "Project";
            //
            // cmbProject
            //
            this.cmbProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProject.Location = new System.Drawing.Point(60, 4);
            this.cmbProject.Name = "cmbProject";
            this.cmbProject.Size = new System.Drawing.Size(320, 26);
            this.cmbProject.TabIndex = 1;
            //
            // lblNeeded
            //
            this.lblNeeded.AutoSize = true;
            this.lblNeeded.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblNeeded.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.lblNeeded.Location = new System.Drawing.Point(404, 8);
            this.lblNeeded.Name = "lblNeeded";
            this.lblNeeded.Size = new System.Drawing.Size(66, 17);
            this.lblNeeded.TabIndex = 2;
            this.lblNeeded.Text = "Needed by";
            //
            // dtNeeded
            //
            this.dtNeeded.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtNeeded.Location = new System.Drawing.Point(482, 4);
            this.dtNeeded.Name = "dtNeeded";
            this.dtNeeded.Size = new System.Drawing.Size(160, 25);
            this.dtNeeded.TabIndex = 3;
            //
            // panelRemarks
            //
            this.panelRemarks.Controls.Add(this.txtRemarks);
            this.panelRemarks.Controls.Add(this.lblRemarks);
            this.panelRemarks.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelRemarks.Location = new System.Drawing.Point(18, 76);
            this.panelRemarks.Name = "panelRemarks";
            this.panelRemarks.Size = new System.Drawing.Size(784, 34);
            this.panelRemarks.TabIndex = 2;
            //
            // lblRemarks
            //
            this.lblRemarks.AutoSize = true;
            this.lblRemarks.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblRemarks.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.lblRemarks.Location = new System.Drawing.Point(0, 8);
            this.lblRemarks.Name = "lblRemarks";
            this.lblRemarks.Size = new System.Drawing.Size(58, 17);
            this.lblRemarks.TabIndex = 0;
            this.lblRemarks.Text = "Remarks";
            //
            // txtRemarks
            //
            this.txtRemarks.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRemarks.Location = new System.Drawing.Point(70, 4);
            this.txtRemarks.Name = "txtRemarks";
            this.txtRemarks.PlaceholderText = "optional — what this request is for";
            this.txtRemarks.Size = new System.Drawing.Size(538, 25);
            this.txtRemarks.TabIndex = 1;
            //
            // panelAddBar
            //
            this.panelAddBar.BackColor = System.Drawing.Color.FromArgb(247, 249, 249);
            this.panelAddBar.Controls.Add(this.btnRemove);
            this.panelAddBar.Controls.Add(this.btnAdd);
            this.panelAddBar.Controls.Add(this.numQty);
            this.panelAddBar.Controls.Add(this.cmbMaterial);
            this.panelAddBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelAddBar.Location = new System.Drawing.Point(0, 128);
            this.panelAddBar.Name = "panelAddBar";
            this.panelAddBar.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.panelAddBar.Size = new System.Drawing.Size(820, 44);
            this.panelAddBar.TabIndex = 1;
            //
            // cmbMaterial
            //
            this.cmbMaterial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMaterial.Location = new System.Drawing.Point(0, 8);
            this.cmbMaterial.Name = "cmbMaterial";
            this.cmbMaterial.Size = new System.Drawing.Size(420, 26);
            this.cmbMaterial.TabIndex = 0;
            //
            // numQty
            //
            this.numQty.DecimalPlaces = 2;
            this.numQty.Location = new System.Drawing.Point(430, 8);
            this.numQty.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            this.numQty.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            this.numQty.Name = "numQty";
            this.numQty.Size = new System.Drawing.Size(90, 25);
            this.numQty.TabIndex = 1;
            this.numQty.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            // btnAdd
            //
            this.btnAdd.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(225, 231, 233);
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.btnAdd.BackColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(528, 7);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(80, 28);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add line";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            //
            // btnRemove
            //
            this.btnRemove.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(192, 82, 72);
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.ForeColor = System.Drawing.Color.FromArgb(192, 82, 72);
            this.btnRemove.BackColor = System.Drawing.Color.White;
            this.btnRemove.Location = new System.Drawing.Point(616, 7);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(130, 28);
            this.btnRemove.TabIndex = 3;
            this.btnRemove.Text = "Remove selected";
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            //
            // panelHost
            //
            this.panelHost.BackColor = System.Drawing.Color.White;
            this.panelHost.Controls.Add(this.grid);
            this.panelHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHost.Location = new System.Drawing.Point(0, 172);
            this.panelHost.Name = "panelHost";
            this.panelHost.Padding = new System.Windows.Forms.Padding(1);
            this.panelHost.Size = new System.Drawing.Size(820, 312);
            this.panelHost.TabIndex = 2;
            //
            // grid
            //
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Location = new System.Drawing.Point(1, 1);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(818, 310);
            this.grid.TabIndex = 0;
            //
            // panelFooter
            //
            this.panelFooter.BackColor = System.Drawing.Color.White;
            this.panelFooter.Controls.Add(this.flowButtons);
            this.panelFooter.Controls.Add(this.lblError);
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFooter.Location = new System.Drawing.Point(0, 484);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Padding = new System.Windows.Forms.Padding(18, 10, 18, 10);
            this.panelFooter.Size = new System.Drawing.Size(820, 76);
            this.panelFooter.TabIndex = 3;
            //
            // lblError
            //
            this.lblError.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblError.ForeColor = System.Drawing.Color.FromArgb(192, 82, 72);
            this.lblError.Location = new System.Drawing.Point(18, 10);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(784, 20);
            this.lblError.TabIndex = 0;
            //
            // flowButtons
            //
            this.flowButtons.Controls.Add(this.btnSubmit);
            this.flowButtons.Controls.Add(this.btnSaveDraft);
            this.flowButtons.Controls.Add(this.btnCancel);
            this.flowButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowButtons.Location = new System.Drawing.Point(18, 30);
            this.flowButtons.Name = "flowButtons";
            this.flowButtons.Size = new System.Drawing.Size(784, 36);
            this.flowButtons.TabIndex = 1;
            //
            // btnSubmit
            //
            this.btnSubmit.BackColor = System.Drawing.Color.FromArgb(31, 122, 107);
            this.btnSubmit.FlatAppearance.BorderSize = 0;
            this.btnSubmit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSubmit.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.btnSubmit.ForeColor = System.Drawing.Color.White;
            this.btnSubmit.Location = new System.Drawing.Point(604, 3);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(180, 32);
            this.btnSubmit.TabIndex = 0;
            this.btnSubmit.Text = "Submit for approval";
            this.btnSubmit.UseVisualStyleBackColor = false;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            //
            // btnSaveDraft
            //
            this.btnSaveDraft.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(225, 231, 233);
            this.btnSaveDraft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveDraft.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.btnSaveDraft.BackColor = System.Drawing.Color.White;
            this.btnSaveDraft.Location = new System.Drawing.Point(506, 3);
            this.btnSaveDraft.Name = "btnSaveDraft";
            this.btnSaveDraft.Size = new System.Drawing.Size(92, 30);
            this.btnSaveDraft.TabIndex = 1;
            this.btnSaveDraft.Text = "Save as draft";
            this.btnSaveDraft.UseVisualStyleBackColor = false;
            this.btnSaveDraft.Click += new System.EventHandler(this.btnSaveDraft_Click);
            //
            // btnCancel
            //
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(225, 231, 233);
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(420, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 30);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // NewRequestForm
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(247, 249, 249);
            this.ClientSize = new System.Drawing.Size(820, 560);
            this.Controls.Add(this.panelHost);
            this.Controls.Add(this.panelAddBar);
            this.Controls.Add(this.panelFooter);
            this.Controls.Add(this.panelHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.Name = "NewRequestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New material request";
            this.panelHeader.ResumeLayout(false);
            this.panelRemarks.ResumeLayout(false);
            this.panelRemarks.PerformLayout();
            this.panelFields.ResumeLayout(false);
            this.panelFields.PerformLayout();
            this.panelAddBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numQty)).EndInit();
            this.panelHost.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.panelFooter.ResumeLayout(false);
            this.flowButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelFields;
        private System.Windows.Forms.Label lblProject;
        private System.Windows.Forms.ComboBox cmbProject;
        private System.Windows.Forms.Label lblNeeded;
        private System.Windows.Forms.DateTimePicker dtNeeded;
        private System.Windows.Forms.Panel panelRemarks;
        private System.Windows.Forms.Label lblRemarks;
        private System.Windows.Forms.TextBox txtRemarks;
        private System.Windows.Forms.Panel panelAddBar;
        private System.Windows.Forms.ComboBox cmbMaterial;
        private System.Windows.Forms.NumericUpDown numQty;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Panel panelHost;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.Panel panelFooter;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.FlowLayoutPanel flowButtons;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnSaveDraft;
        private System.Windows.Forms.Button btnCancel;
    }
}
