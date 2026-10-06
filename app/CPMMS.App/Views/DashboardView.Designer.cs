namespace CPMMS.App.Views
{
    partial class DashboardView
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.flowCards = new System.Windows.Forms.FlowLayoutPanel();
            this.card1 = new System.Windows.Forms.Panel();
            this.lblCard1Sub = new System.Windows.Forms.Label();
            this.lblCard1Lbl = new System.Windows.Forms.Label();
            this.lblCard1Val = new System.Windows.Forms.Label();
            this.card2 = new System.Windows.Forms.Panel();
            this.lblCard2Sub = new System.Windows.Forms.Label();
            this.lblCard2Lbl = new System.Windows.Forms.Label();
            this.lblCard2Val = new System.Windows.Forms.Label();
            this.card3 = new System.Windows.Forms.Panel();
            this.lblCard3Sub = new System.Windows.Forms.Label();
            this.lblCard3Lbl = new System.Windows.Forms.Label();
            this.lblCard3Val = new System.Windows.Forms.Label();
            this.card4 = new System.Windows.Forms.Panel();
            this.lblCard4Sub = new System.Windows.Forms.Label();
            this.lblCard4Lbl = new System.Windows.Forms.Label();
            this.lblCard4Val = new System.Windows.Forms.Label();
            this.card5 = new System.Windows.Forms.Panel();
            this.lblCard5Sub = new System.Windows.Forms.Label();
            this.lblCard5Lbl = new System.Windows.Forms.Label();
            this.lblCard5Val = new System.Windows.Forms.Label();
            this.panelBanner = new System.Windows.Forms.Panel();
            this.lblBanner = new System.Windows.Forms.Label();
            this.lblLowTitle = new System.Windows.Forms.Label();
            this.gridHost = new System.Windows.Forms.Panel();
            this.gridLow = new System.Windows.Forms.DataGridView();
            this.flowCards.SuspendLayout();
            this.card1.SuspendLayout();
            this.card2.SuspendLayout();
            this.card3.SuspendLayout();
            this.card4.SuspendLayout();
            this.card5.SuspendLayout();
            this.panelBanner.SuspendLayout();
            this.gridHost.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridLow)).BeginInit();
            this.SuspendLayout();
            //
            // flowCards
            //
            this.flowCards.BackColor = System.Drawing.Color.FromArgb(247, 249, 249);
            this.flowCards.Controls.Add(this.card1);
            this.flowCards.Controls.Add(this.card2);
            this.flowCards.Controls.Add(this.card3);
            this.flowCards.Controls.Add(this.card4);
            this.flowCards.Controls.Add(this.card5);
            this.flowCards.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowCards.Location = new System.Drawing.Point(0, 0);
            this.flowCards.Name = "flowCards";
            this.flowCards.Size = new System.Drawing.Size(900, 108);
            this.flowCards.TabIndex = 0;
            this.flowCards.WrapContents = false;
            //
            // card1
            //
            this.card1.BackColor = System.Drawing.Color.White;
            this.card1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.card1.Controls.Add(this.lblCard1Sub);
            this.card1.Controls.Add(this.lblCard1Lbl);
            this.card1.Controls.Add(this.lblCard1Val);
            this.card1.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.card1.Name = "card1";
            this.card1.Padding = new System.Windows.Forms.Padding(16, 12, 16, 12);
            this.card1.Size = new System.Drawing.Size(220, 92);
            this.card1.TabIndex = 0;
            //
            // lblCard1Val
            //
            this.lblCard1Val.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCard1Val.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.lblCard1Val.ForeColor = System.Drawing.Color.FromArgb(18, 42, 51);
            this.lblCard1Val.Location = new System.Drawing.Point(16, 12);
            this.lblCard1Val.Name = "lblCard1Val";
            this.lblCard1Val.Size = new System.Drawing.Size(186, 38);
            this.lblCard1Val.TabIndex = 2;
            this.lblCard1Val.Text = "0";
            //
            // lblCard1Lbl
            //
            this.lblCard1Lbl.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCard1Lbl.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblCard1Lbl.ForeColor = System.Drawing.Color.FromArgb(126, 144, 153);
            this.lblCard1Lbl.Location = new System.Drawing.Point(16, 50);
            this.lblCard1Lbl.Name = "lblCard1Lbl";
            this.lblCard1Lbl.Size = new System.Drawing.Size(186, 20);
            this.lblCard1Lbl.TabIndex = 1;
            this.lblCard1Lbl.Text = "Ongoing projects";
            //
            // lblCard1Sub
            //
            this.lblCard1Sub.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCard1Sub.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblCard1Sub.ForeColor = System.Drawing.Color.FromArgb(126, 144, 153);
            this.lblCard1Sub.Location = new System.Drawing.Point(16, 70);
            this.lblCard1Sub.Name = "lblCard1Sub";
            this.lblCard1Sub.Size = new System.Drawing.Size(186, 18);
            this.lblCard1Sub.TabIndex = 0;
            this.lblCard1Sub.Text = "in total";
            //
            // card2
            //
            this.card2.BackColor = System.Drawing.Color.White;
            this.card2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.card2.Controls.Add(this.lblCard2Sub);
            this.card2.Controls.Add(this.lblCard2Lbl);
            this.card2.Controls.Add(this.lblCard2Val);
            this.card2.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.card2.Name = "card2";
            this.card2.Padding = new System.Windows.Forms.Padding(16, 12, 16, 12);
            this.card2.Size = new System.Drawing.Size(220, 92);
            this.card2.TabIndex = 1;
            //
            // lblCard2Val
            //
            this.lblCard2Val.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCard2Val.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.lblCard2Val.ForeColor = System.Drawing.Color.FromArgb(18, 42, 51);
            this.lblCard2Val.Location = new System.Drawing.Point(16, 12);
            this.lblCard2Val.Name = "lblCard2Val";
            this.lblCard2Val.Size = new System.Drawing.Size(186, 38);
            this.lblCard2Val.TabIndex = 2;
            this.lblCard2Val.Text = "₱0";
            //
            // lblCard2Lbl
            //
            this.lblCard2Lbl.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCard2Lbl.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblCard2Lbl.ForeColor = System.Drawing.Color.FromArgb(126, 144, 153);
            this.lblCard2Lbl.Location = new System.Drawing.Point(16, 50);
            this.lblCard2Lbl.Name = "lblCard2Lbl";
            this.lblCard2Lbl.Size = new System.Drawing.Size(186, 20);
            this.lblCard2Lbl.TabIndex = 1;
            this.lblCard2Lbl.Text = "Contract value";
            //
            // lblCard2Sub
            //
            this.lblCard2Sub.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCard2Sub.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblCard2Sub.ForeColor = System.Drawing.Color.FromArgb(126, 144, 153);
            this.lblCard2Sub.Location = new System.Drawing.Point(16, 70);
            this.lblCard2Sub.Name = "lblCard2Sub";
            this.lblCard2Sub.Size = new System.Drawing.Size(186, 18);
            this.lblCard2Sub.TabIndex = 0;
            this.lblCard2Sub.Text = "ongoing and planned";
            //
            // card3
            //
            this.card3.BackColor = System.Drawing.Color.White;
            this.card3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.card3.Controls.Add(this.lblCard3Sub);
            this.card3.Controls.Add(this.lblCard3Lbl);
            this.card3.Controls.Add(this.lblCard3Val);
            this.card3.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.card3.Name = "card3";
            this.card3.Padding = new System.Windows.Forms.Padding(16, 12, 16, 12);
            this.card3.Size = new System.Drawing.Size(220, 92);
            this.card3.TabIndex = 2;
            //
            // lblCard3Val
            //
            this.lblCard3Val.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCard3Val.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.lblCard3Val.ForeColor = System.Drawing.Color.FromArgb(18, 42, 51);
            this.lblCard3Val.Location = new System.Drawing.Point(16, 12);
            this.lblCard3Val.Name = "lblCard3Val";
            this.lblCard3Val.Size = new System.Drawing.Size(186, 38);
            this.lblCard3Val.TabIndex = 2;
            this.lblCard3Val.Text = "₱0";
            //
            // lblCard3Lbl
            //
            this.lblCard3Lbl.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCard3Lbl.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblCard3Lbl.ForeColor = System.Drawing.Color.FromArgb(126, 144, 153);
            this.lblCard3Lbl.Location = new System.Drawing.Point(16, 50);
            this.lblCard3Lbl.Name = "lblCard3Lbl";
            this.lblCard3Lbl.Size = new System.Drawing.Size(186, 20);
            this.lblCard3Lbl.TabIndex = 1;
            this.lblCard3Lbl.Text = "Stock value";
            //
            // lblCard3Sub
            //
            this.lblCard3Sub.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCard3Sub.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblCard3Sub.ForeColor = System.Drawing.Color.FromArgb(126, 144, 153);
            this.lblCard3Sub.Location = new System.Drawing.Point(16, 70);
            this.lblCard3Sub.Name = "lblCard3Sub";
            this.lblCard3Sub.Size = new System.Drawing.Size(186, 18);
            this.lblCard3Sub.TabIndex = 0;
            this.lblCard3Sub.Text = "materials";
            //
            // card4
            //
            this.card4.BackColor = System.Drawing.Color.White;
            this.card4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.card4.Controls.Add(this.lblCard4Sub);
            this.card4.Controls.Add(this.lblCard4Lbl);
            this.card4.Controls.Add(this.lblCard4Val);
            this.card4.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.card4.Name = "card4";
            this.card4.Padding = new System.Windows.Forms.Padding(16, 12, 16, 12);
            this.card4.Size = new System.Drawing.Size(220, 92);
            this.card4.TabIndex = 3;
            //
            // lblCard4Val
            //
            this.lblCard4Val.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCard4Val.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.lblCard4Val.ForeColor = System.Drawing.Color.FromArgb(18, 42, 51);
            this.lblCard4Val.Location = new System.Drawing.Point(16, 12);
            this.lblCard4Val.Name = "lblCard4Val";
            this.lblCard4Val.Size = new System.Drawing.Size(186, 38);
            this.lblCard4Val.TabIndex = 2;
            this.lblCard4Val.Text = "0";
            //
            // lblCard4Lbl
            //
            this.lblCard4Lbl.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCard4Lbl.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblCard4Lbl.ForeColor = System.Drawing.Color.FromArgb(126, 144, 153);
            this.lblCard4Lbl.Location = new System.Drawing.Point(16, 50);
            this.lblCard4Lbl.Name = "lblCard4Lbl";
            this.lblCard4Lbl.Size = new System.Drawing.Size(186, 20);
            this.lblCard4Lbl.TabIndex = 1;
            this.lblCard4Lbl.Text = "Low stock";
            //
            // lblCard4Sub
            //
            this.lblCard4Sub.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCard4Sub.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblCard4Sub.ForeColor = System.Drawing.Color.FromArgb(126, 144, 153);
            this.lblCard4Sub.Location = new System.Drawing.Point(16, 70);
            this.lblCard4Sub.Name = "lblCard4Sub";
            this.lblCard4Sub.Size = new System.Drawing.Size(186, 18);
            this.lblCard4Sub.TabIndex = 0;
            this.lblCard4Sub.Text = "at or below reorder point";
            //
            // card5
            //
            this.card5.BackColor = System.Drawing.Color.White;
            this.card5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.card5.Controls.Add(this.lblCard5Sub);
            this.card5.Controls.Add(this.lblCard5Lbl);
            this.card5.Controls.Add(this.lblCard5Val);
            this.card5.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.card5.Name = "card5";
            this.card5.Padding = new System.Windows.Forms.Padding(16, 12, 16, 12);
            this.card5.Size = new System.Drawing.Size(220, 92);
            this.card5.TabIndex = 4;
            //
            // lblCard5Val
            //
            this.lblCard5Val.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCard5Val.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.lblCard5Val.ForeColor = System.Drawing.Color.FromArgb(18, 42, 51);
            this.lblCard5Val.Location = new System.Drawing.Point(16, 12);
            this.lblCard5Val.Name = "lblCard5Val";
            this.lblCard5Val.Size = new System.Drawing.Size(186, 38);
            this.lblCard5Val.TabIndex = 2;
            this.lblCard5Val.Text = "0";
            //
            // lblCard5Lbl
            //
            this.lblCard5Lbl.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCard5Lbl.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblCard5Lbl.ForeColor = System.Drawing.Color.FromArgb(126, 144, 153);
            this.lblCard5Lbl.Location = new System.Drawing.Point(16, 50);
            this.lblCard5Lbl.Name = "lblCard5Lbl";
            this.lblCard5Lbl.Size = new System.Drawing.Size(186, 20);
            this.lblCard5Lbl.TabIndex = 1;
            this.lblCard5Lbl.Text = "Pending requests";
            //
            // lblCard5Sub
            //
            this.lblCard5Sub.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCard5Sub.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblCard5Sub.ForeColor = System.Drawing.Color.FromArgb(126, 144, 153);
            this.lblCard5Sub.Location = new System.Drawing.Point(16, 70);
            this.lblCard5Sub.Name = "lblCard5Sub";
            this.lblCard5Sub.Size = new System.Drawing.Size(186, 18);
            this.lblCard5Sub.TabIndex = 0;
            this.lblCard5Sub.Text = "waiting for approval";
            //
            // panelBanner
            //
            this.panelBanner.BackColor = System.Drawing.Color.FromArgb(231, 244, 239);
            this.panelBanner.Controls.Add(this.lblBanner);
            this.panelBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBanner.Location = new System.Drawing.Point(0, 108);
            this.panelBanner.Name = "panelBanner";
            this.panelBanner.Padding = new System.Windows.Forms.Padding(12, 0, 12, 0);
            this.panelBanner.Size = new System.Drawing.Size(900, 40);
            this.panelBanner.TabIndex = 1;
            //
            // lblBanner
            //
            this.lblBanner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBanner.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(42, 123, 80);
            this.lblBanner.Location = new System.Drawing.Point(12, 0);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Size = new System.Drawing.Size(876, 40);
            this.lblBanner.TabIndex = 0;
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBanner.Text = "Ledger check";
            //
            // lblLowTitle
            //
            this.lblLowTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLowTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 11F);
            this.lblLowTitle.ForeColor = System.Drawing.Color.FromArgb(18, 42, 51);
            this.lblLowTitle.Location = new System.Drawing.Point(0, 148);
            this.lblLowTitle.Name = "lblLowTitle";
            this.lblLowTitle.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.lblLowTitle.Size = new System.Drawing.Size(900, 30);
            this.lblLowTitle.TabIndex = 2;
            this.lblLowTitle.Text = "Low stock";
            //
            // gridHost
            //
            this.gridHost.BackColor = System.Drawing.Color.White;
            this.gridHost.Controls.Add(this.gridLow);
            this.gridHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridHost.Location = new System.Drawing.Point(0, 178);
            this.gridHost.Name = "gridHost";
            this.gridHost.Padding = new System.Windows.Forms.Padding(1);
            this.gridHost.Size = new System.Drawing.Size(900, 342);
            this.gridHost.TabIndex = 3;
            //
            // gridLow
            //
            this.gridLow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridLow.Location = new System.Drawing.Point(1, 1);
            this.gridLow.Name = "gridLow";
            this.gridLow.Size = new System.Drawing.Size(898, 340);
            this.gridLow.TabIndex = 0;
            //
            // DashboardView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(247, 249, 249);
            this.Controls.Add(this.gridHost);
            this.Controls.Add(this.lblLowTitle);
            this.Controls.Add(this.panelBanner);
            this.Controls.Add(this.flowCards);
            this.Name = "DashboardView";
            this.Size = new System.Drawing.Size(900, 520);
            this.flowCards.ResumeLayout(false);
            this.card1.ResumeLayout(false);
            this.card2.ResumeLayout(false);
            this.card3.ResumeLayout(false);
            this.card4.ResumeLayout(false);
            this.card5.ResumeLayout(false);
            this.panelBanner.ResumeLayout(false);
            this.gridHost.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridLow)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.FlowLayoutPanel flowCards;
        private System.Windows.Forms.Panel card1;
        private System.Windows.Forms.Label lblCard1Val;
        private System.Windows.Forms.Label lblCard1Lbl;
        private System.Windows.Forms.Label lblCard1Sub;
        private System.Windows.Forms.Panel card2;
        private System.Windows.Forms.Label lblCard2Val;
        private System.Windows.Forms.Label lblCard2Lbl;
        private System.Windows.Forms.Label lblCard2Sub;
        private System.Windows.Forms.Panel card3;
        private System.Windows.Forms.Label lblCard3Val;
        private System.Windows.Forms.Label lblCard3Lbl;
        private System.Windows.Forms.Label lblCard3Sub;
        private System.Windows.Forms.Panel card4;
        private System.Windows.Forms.Label lblCard4Val;
        private System.Windows.Forms.Label lblCard4Lbl;
        private System.Windows.Forms.Label lblCard4Sub;
        private System.Windows.Forms.Panel card5;
        private System.Windows.Forms.Label lblCard5Val;
        private System.Windows.Forms.Label lblCard5Lbl;
        private System.Windows.Forms.Label lblCard5Sub;
        private System.Windows.Forms.Panel panelBanner;
        private System.Windows.Forms.Label lblBanner;
        private System.Windows.Forms.Label lblLowTitle;
        private System.Windows.Forms.Panel gridHost;
        private System.Windows.Forms.DataGridView gridLow;
    }
}
