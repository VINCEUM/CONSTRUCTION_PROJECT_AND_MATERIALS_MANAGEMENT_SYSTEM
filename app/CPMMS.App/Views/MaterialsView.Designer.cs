namespace CPMMS.App.Views
{
    partial class MaterialsView
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
            this.lblCount = new System.Windows.Forms.Label();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnStockCard = new System.Windows.Forms.Button();
            this.chkLowOnly = new System.Windows.Forms.CheckBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
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
            this.panelBar.Controls.Add(this.lblCount);
            this.panelBar.Controls.Add(this.btnEdit);
            this.panelBar.Controls.Add(this.btnAdd);
            this.panelBar.Controls.Add(this.btnStockCard);
            this.panelBar.Controls.Add(this.chkLowOnly);
            this.panelBar.Controls.Add(this.txtSearch);
            this.panelBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBar.Location = new System.Drawing.Point(0, 0);
            this.panelBar.Name = "panelBar";
            this.panelBar.Size = new System.Drawing.Size(900, 46);
            this.panelBar.TabIndex = 0;
            //
            // txtSearch
            //
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtSearch.Location = new System.Drawing.Point(0, 8);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.PlaceholderText = "Search by name, code or category…";
            this.txtSearch.Size = new System.Drawing.Size(320, 25);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            //
            // chkLowOnly
            //
            this.chkLowOnly.AutoSize = true;
            this.chkLowOnly.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.chkLowOnly.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.chkLowOnly.Location = new System.Drawing.Point(336, 11);
            this.chkLowOnly.Name = "chkLowOnly";
            this.chkLowOnly.Size = new System.Drawing.Size(104, 21);
            this.chkLowOnly.TabIndex = 1;
            this.chkLowOnly.Text = "Low stock only";
            this.chkLowOnly.CheckedChanged += new System.EventHandler(this.chkLowOnly_CheckedChanged);
            //
            // btnStockCard
            //
            this.btnStockCard.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(225, 231, 233);
            this.btnStockCard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStockCard.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.btnStockCard.BackColor = System.Drawing.Color.White;
            this.btnStockCard.Location = new System.Drawing.Point(470, 8);
            this.btnStockCard.Name = "btnStockCard";
            this.btnStockCard.Size = new System.Drawing.Size(80, 28);
            this.btnStockCard.TabIndex = 2;
            this.btnStockCard.Text = "Stock card";
            this.btnStockCard.UseVisualStyleBackColor = false;
            this.btnStockCard.Click += new System.EventHandler(this.btnStockCard_Click);
            //
            // btnAdd
            //
            this.btnAdd.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(225, 231, 233);
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.btnAdd.BackColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(556, 8);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(56, 28);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add...";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            //
            // btnEdit
            //
            this.btnEdit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(225, 231, 233);
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.btnEdit.BackColor = System.Drawing.Color.White;
            this.btnEdit.Location = new System.Drawing.Point(618, 8);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(60, 28);
            this.btnEdit.TabIndex = 4;
            this.btnEdit.Text = "Edit...";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            //
            // lblCount
            //
            this.lblCount.AutoSize = false;
            this.lblCount.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblCount.ForeColor = System.Drawing.Color.FromArgb(126, 144, 153);
            this.lblCount.Location = new System.Drawing.Point(688, 14);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(300, 20);
            this.lblCount.TabIndex = 5;
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
            this.grid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellDoubleClick);
            this.grid.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.grid_CellFormatting);
            //
            // MaterialsView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(247, 249, 249);
            this.Controls.Add(this.gridHost);
            this.Controls.Add(this.panelBar);
            this.Name = "MaterialsView";
            this.Size = new System.Drawing.Size(900, 520);
            this.panelBar.ResumeLayout(false);
            this.panelBar.PerformLayout();
            this.gridHost.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelBar;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.CheckBox chkLowOnly;
        private System.Windows.Forms.Button btnStockCard;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Panel gridHost;
        private System.Windows.Forms.DataGridView grid;
    }
}
