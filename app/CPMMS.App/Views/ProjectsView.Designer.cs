namespace CPMMS.App.Views
{
    partial class ProjectsView
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
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
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
            this.panelBar.Controls.Add(this.btnEdit);
            this.panelBar.Controls.Add(this.btnAdd);
            this.panelBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBar.Location = new System.Drawing.Point(0, 0);
            this.panelBar.Name = "panelBar";
            this.panelBar.Size = new System.Drawing.Size(900, 46);
            this.panelBar.TabIndex = 0;
            //
            // btnAdd
            //
            this.btnAdd.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(225, 231, 233);
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.btnAdd.BackColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(0, 8);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(116, 28);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "Add project...";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            //
            // btnEdit
            //
            this.btnEdit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(225, 231, 233);
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.btnEdit.BackColor = System.Drawing.Color.White;
            this.btnEdit.Location = new System.Drawing.Point(126, 8);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(60, 28);
            this.btnEdit.TabIndex = 1;
            this.btnEdit.Text = "Edit...";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            //
            // lblTitle
            //
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 11F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(18, 42, 51);
            this.lblTitle.Location = new System.Drawing.Point(0, 46);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.lblTitle.Size = new System.Drawing.Size(900, 30);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Budget against actual material cost. Progress is the weighted roll-up of each phase.";
            //
            // gridHost
            //
            this.gridHost.BackColor = System.Drawing.Color.White;
            this.gridHost.Controls.Add(this.grid);
            this.gridHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridHost.Location = new System.Drawing.Point(0, 76);
            this.gridHost.Name = "gridHost";
            this.gridHost.Padding = new System.Windows.Forms.Padding(1);
            this.gridHost.Size = new System.Drawing.Size(900, 444);
            this.gridHost.TabIndex = 2;
            //
            // grid
            //
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Location = new System.Drawing.Point(1, 1);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(898, 442);
            this.grid.TabIndex = 0;
            this.grid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellDoubleClick);
            this.grid.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.grid_CellFormatting);
            //
            // ProjectsView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(247, 249, 249);
            this.Controls.Add(this.gridHost);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.panelBar);
            this.Name = "ProjectsView";
            this.Size = new System.Drawing.Size(900, 520);
            this.panelBar.ResumeLayout(false);
            this.gridHost.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelBar;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel gridHost;
        private System.Windows.Forms.DataGridView grid;
    }
}
