namespace CPMMS.App
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelBrand = new System.Windows.Forms.Panel();
            this.lblTagline = new System.Windows.Forms.Label();
            this.lblBrand = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSub = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.lblError = new System.Windows.Forms.Label();
            this.lblHint = new System.Windows.Forms.Label();
            this.btnDemo = new System.Windows.Forms.Button();
            this.panelBrand.SuspendLayout();
            this.SuspendLayout();
            //
            // panelBrand
            //
            this.panelBrand.BackColor = System.Drawing.Color.FromArgb(22, 38, 46);
            this.panelBrand.Controls.Add(this.lblTagline);
            this.panelBrand.Controls.Add(this.lblBrand);
            this.panelBrand.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelBrand.Location = new System.Drawing.Point(0, 0);
            this.panelBrand.Name = "panelBrand";
            this.panelBrand.Size = new System.Drawing.Size(300, 500);
            this.panelBrand.TabIndex = 0;
            //
            // lblBrand
            //
            this.lblBrand.AutoSize = false;
            this.lblBrand.ForeColor = System.Drawing.Color.White;
            this.lblBrand.Font = new System.Drawing.Font("Segoe UI Semibold", 17F);
            this.lblBrand.Location = new System.Drawing.Point(32, 110);
            this.lblBrand.Name = "lblBrand";
            this.lblBrand.Size = new System.Drawing.Size(244, 160);
            this.lblBrand.TabIndex = 0;
            this.lblBrand.Text = "Construction Project and Materials Management";
            //
            // lblTagline
            //
            this.lblTagline.AutoSize = false;
            this.lblTagline.ForeColor = System.Drawing.Color.FromArgb(126, 144, 153);
            this.lblTagline.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTagline.Location = new System.Drawing.Point(34, 284);
            this.lblTagline.Name = "lblTagline";
            this.lblTagline.Size = new System.Drawing.Size(236, 80);
            this.lblTagline.TabIndex = 1;
            this.lblTagline.Text = "Plan it, buy it, receive it, release it — and see where the difference went.";
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 16F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(18, 42, 51);
            this.lblTitle.Location = new System.Drawing.Point(344, 96);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(120, 30);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Sign in";
            //
            // lblSub
            //
            this.lblSub.AutoSize = true;
            this.lblSub.ForeColor = System.Drawing.Color.FromArgb(126, 144, 153);
            this.lblSub.Location = new System.Drawing.Point(346, 132);
            this.lblSub.Name = "lblSub";
            this.lblSub.Size = new System.Drawing.Size(200, 15);
            this.lblSub.TabIndex = 2;
            this.lblSub.Text = "Use your company account.";
            //
            // lblEmail
            //
            this.lblEmail.AutoSize = true;
            this.lblEmail.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.lblEmail.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.lblEmail.Location = new System.Drawing.Point(346, 176);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(40, 15);
            this.lblEmail.TabIndex = 3;
            this.lblEmail.Text = "Email";
            //
            // txtEmail
            //
            this.txtEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmail.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtEmail.Location = new System.Drawing.Point(348, 196);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(320, 25);
            this.txtEmail.TabIndex = 0;
            //
            // lblPassword
            //
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.lblPassword.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.lblPassword.Location = new System.Drawing.Point(346, 236);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(60, 15);
            this.lblPassword.TabIndex = 5;
            this.lblPassword.Text = "Password";
            //
            // txtPassword
            //
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPassword.Location = new System.Drawing.Point(348, 256);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(320, 25);
            this.txtPassword.TabIndex = 1;
            this.txtPassword.UseSystemPasswordChar = true;
            //
            // btnLogin
            //
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(31, 122, 107);
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Location = new System.Drawing.Point(348, 304);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(320, 40);
            this.btnLogin.TabIndex = 2;
            this.btnLogin.Text = "Sign in";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            //
            // btnDemo
            //
            this.btnDemo.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(220, 227, 229);
            this.btnDemo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDemo.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.btnDemo.ForeColor = System.Drawing.Color.FromArgb(62, 85, 94);
            this.btnDemo.Location = new System.Drawing.Point(348, 354);
            this.btnDemo.Name = "btnDemo";
            this.btnDemo.Size = new System.Drawing.Size(320, 36);
            this.btnDemo.TabIndex = 3;
            this.btnDemo.Text = "Continue without a database (demo data)";
            this.btnDemo.UseVisualStyleBackColor = true;
            this.btnDemo.Visible = false;
            this.btnDemo.Click += new System.EventHandler(this.btnDemo_Click);
            //
            // lblError
            //
            this.lblError.AutoSize = false;
            this.lblError.ForeColor = System.Drawing.Color.FromArgb(192, 82, 72);
            this.lblError.Location = new System.Drawing.Point(346, 404);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(324, 40);
            this.lblError.TabIndex = 8;
            this.lblError.Text = "";
            //
            // lblHint
            //
            this.lblHint.AutoSize = false;
            this.lblHint.ForeColor = System.Drawing.Color.FromArgb(126, 144, 153);
            this.lblHint.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblHint.Location = new System.Drawing.Point(346, 448);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(324, 34);
            this.lblHint.TabIndex = 9;
            this.lblHint.Text = "Demo: admin@buildcorp.test / password";
            //
            // LoginForm
            //
            this.AcceptButton = this.btnLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(700, 500);
            this.Controls.Add(this.btnDemo);
            this.Controls.Add(this.lblHint);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.lblSub);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.panelBrand);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CPMMS — Sign in";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.panelBrand.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Panel panelBrand;
        private System.Windows.Forms.Label lblBrand;
        private System.Windows.Forms.Label lblTagline;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSub;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Label lblHint;
        private System.Windows.Forms.Button btnDemo;
    }
}
