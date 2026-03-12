using System;
using System.Drawing;
using System.Windows.Forms;

namespace shop.Forms
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtLogin;
        private TextBox txtPassword;
        private Label lblLogin;
        private Label lblPassword;
        private Button btnLogin;
        private Button btnRegister;
        private Button btnExit;
        private Panel panel;

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
            this.txtLogin = new TextBox();
            this.txtPassword = new TextBox();
            this.lblLogin = new Label();
            this.lblPassword = new Label();
            this.btnLogin = new Button();
            this.btnRegister = new Button();
            this.btnExit = new Button();
            this.panel = new Panel();
            this.panel.SuspendLayout();
            this.SuspendLayout();

            // panel
            this.panel.BackColor = Color.FromArgb(64, 64, 64); // тёмно-серый
            this.panel.Controls.Add(this.lblLogin);
            this.panel.Controls.Add(this.txtLogin);
            this.panel.Controls.Add(this.lblPassword);
            this.panel.Controls.Add(this.txtPassword);
            this.panel.Controls.Add(this.btnLogin);
            this.panel.Controls.Add(this.btnRegister);
            this.panel.Controls.Add(this.btnExit);
            this.panel.Dock = DockStyle.Fill;
            this.panel.Location = new Point(0, 0);
            this.panel.Size = new Size(400, 250);

            // lblLogin
            this.lblLogin.AutoSize = true;
            this.lblLogin.ForeColor = Color.White;
            this.lblLogin.Location = new Point(50, 40);
            this.lblLogin.Text = "Логин:";

            // txtLogin
            this.txtLogin.Location = new Point(120, 37);
            this.txtLogin.Size = new Size(200, 22);

            // lblPassword
            this.lblPassword.AutoSize = true;
            this.lblPassword.ForeColor = Color.White;
            this.lblPassword.Location = new Point(50, 80);
            this.lblPassword.Text = "Пароль:";

            // txtPassword
            this.txtPassword.Location = new Point(120, 77);
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new Size(200, 22);

            // btnLogin
            this.btnLogin.BackColor = Color.Black;
            this.btnLogin.FlatStyle = FlatStyle.Flat;
            this.btnLogin.ForeColor = Color.White;
            this.btnLogin.Location = new Point(120, 120);
            this.btnLogin.Size = new Size(100, 30);
            this.btnLogin.Text = "Войти";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new EventHandler(this.btnLogin_Click);

            // btnRegister
            this.btnRegister.BackColor = Color.Black;
            this.btnRegister.FlatStyle = FlatStyle.Flat;
            this.btnRegister.ForeColor = Color.White;
            this.btnRegister.Location = new Point(230, 120);
            this.btnRegister.Size = new Size(100, 30);
            this.btnRegister.Text = "Регистрация";
            this.btnRegister.UseVisualStyleBackColor = false;
            this.btnRegister.Click += new EventHandler(this.btnRegister_Click);

            // btnExit
            this.btnExit.BackColor = Color.Black;
            this.btnExit.FlatStyle = FlatStyle.Flat;
            this.btnExit.ForeColor = Color.White;
            this.btnExit.Location = new Point(120, 170);
            this.btnExit.Size = new Size(210, 30);
            this.btnExit.Text = "Выход";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new EventHandler(this.btnExit_Click);

            // LoginForm
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(400, 250);
            this.Controls.Add(this.panel);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Вход в ILNI Shop";
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}