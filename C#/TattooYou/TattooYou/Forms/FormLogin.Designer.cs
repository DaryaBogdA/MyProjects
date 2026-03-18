using System;
using System.Drawing;
using System.Windows.Forms;

namespace TattooYou.Forms
{
    partial class FormLogin
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblPassword;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;
        private Button btnExit;
        private Label lblError;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblUsername = new Label();
            this.txtUsername = new TextBox();
            this.lblPassword = new Label();
            this.txtPassword = new TextBox();
            this.btnLogin = new Button();
            this.btnRegister = new Button();
            this.btnExit = new Button();
            this.lblError = new Label();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblTitle.Location = new Point(120, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(160, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "TattooYou";
            this.lblTitle.ForeColor = Color.White;
            //
            // lblUsername
            //
            this.lblUsername.AutoSize = true;
            this.lblUsername.Font = new Font("Segoe UI", 10F);
            this.lblUsername.Location = new Point(50, 100);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new Size(41, 19);
            this.lblUsername.TabIndex = 1;
            this.lblUsername.Text = "Логин";
            this.lblUsername.ForeColor = Color.White;
            //
            // txtUsername
            //
            this.txtUsername.Location = new Point(50, 125);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new Size(300, 23);
            this.txtUsername.TabIndex = 2;
            this.txtUsername.BackColor = Color.FromArgb(64, 64, 64);
            this.txtUsername.ForeColor = Color.White;
            this.txtUsername.BorderStyle = BorderStyle.FixedSingle;
            //
            // lblPassword
            //
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new Font("Segoe UI", 10F);
            this.lblPassword.Location = new Point(50, 170);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new Size(49, 19);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "Пароль";
            this.lblPassword.ForeColor = Color.White;
            //
            // txtPassword
            //
            this.txtPassword.Location = new Point(50, 195);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new Size(300, 23);
            this.txtPassword.TabIndex = 4;
            this.txtPassword.BackColor = Color.FromArgb(64, 64, 64);
            this.txtPassword.ForeColor = Color.White;
            this.txtPassword.BorderStyle = BorderStyle.FixedSingle;
            //
            // btnLogin
            //
            this.btnLogin.Location = new Point(50, 240);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new Size(140, 35);
            this.btnLogin.TabIndex = 5;
            this.btnLogin.Text = "Войти";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new EventHandler(this.btnLogin_Click);
            //
            // btnRegister
            //
            this.btnRegister.Location = new Point(210, 240);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new Size(140, 35);
            this.btnRegister.TabIndex = 6;
            this.btnRegister.Text = "Регистрация";
            this.btnRegister.UseVisualStyleBackColor = false;
            this.btnRegister.Click += new EventHandler(this.btnRegister_Click);
            //
            // btnExit
            //
            this.btnExit.Location = new Point(50, 290);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new Size(300, 35);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "Выход";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new EventHandler(this.btnExit_Click);
            //
            // lblError
            //
            this.lblError.AutoSize = true;
            this.lblError.Font = new Font("Segoe UI", 9F);
            this.lblError.Location = new Point(50, 340);
            this.lblError.Name = "lblError";
            this.lblError.Size = new Size(0, 15);
            this.lblError.TabIndex = 8;
            this.lblError.ForeColor = Color.Red;
            //
            // FormLogin
            //
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(400, 390);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.lblTitle);
            this.Name = "FormLogin";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Вход в систему";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}