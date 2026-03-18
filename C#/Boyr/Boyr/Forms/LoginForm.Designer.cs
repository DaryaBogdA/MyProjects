using System.Drawing;
using System.Windows.Forms;

namespace Boyr.Forms
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
        private Panel panelHeader;
        private Label lblIcon;
        private Label lblTitle;

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
            this.panelHeader = new Panel();
            this.lblIcon = new Label();
            this.lblTitle = new Label();
            this.txtLogin = new TextBox();
            this.txtPassword = new TextBox();
            this.lblLogin = new Label();
            this.lblPassword = new Label();
            this.btnLogin = new Button();
            this.btnRegister = new Button();
            this.btnExit = new Button();
            this.panelHeader.SuspendLayout();
            this.SuspendLayout();

            // panelHeader
            this.panelHeader.BackColor = Color.FromArgb(0, 128, 128);
            this.panelHeader.Controls.Add(this.lblIcon);
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = DockStyle.Top;
            this.panelHeader.Location = new Point(0, 0);
            this.panelHeader.Size = new Size(450, 80);

            // lblIcon
            this.lblIcon.AutoSize = true;
            this.lblIcon.Font = new Font("Segoe UI", 24F, FontStyle.Regular);
            this.lblIcon.ForeColor = Color.White;
            this.lblIcon.Location = new Point(20, 20);
            this.lblIcon.Text = "💍";

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.Location = new Point(80, 25);
            this.lblTitle.Text = "Boyr";

            // lblLogin
            this.lblLogin.AutoSize = true;
            this.lblLogin.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblLogin.ForeColor = Color.Teal;
            this.lblLogin.Location = new Point(50, 110);
            this.lblLogin.Text = "Логин:";

            // txtLogin
            this.txtLogin.Location = new Point(50, 135);
            this.txtLogin.Size = new Size(350, 25);

            // lblPassword
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblPassword.ForeColor = Color.Teal;
            this.lblPassword.Location = new Point(50, 175);
            this.lblPassword.Text = "Пароль:";

            // txtPassword
            this.txtPassword.Location = new Point(50, 200);
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new Size(350, 25);

            // btnLogin
            this.btnLogin.Location = new Point(50, 250);
            this.btnLogin.Size = new Size(110, 35);
            this.btnLogin.Text = "Войти";
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click); // ДОБАВЛЕНО

            // btnRegister
            this.btnRegister.Location = new Point(170, 250);
            this.btnRegister.Size = new Size(110, 35);
            this.btnRegister.Text = "Регистрация";
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click); // ДОБАВЛЕНО

            // btnExit
            this.btnExit.Location = new Point(290, 250);
            this.btnExit.Size = new Size(110, 35);
            this.btnExit.Text = "Выход";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click); // ДОБАВЛЕНО

            // LoginForm
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(450, 320);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtLogin);
            this.Controls.Add(this.lblLogin);
            this.Controls.Add(this.panelHeader);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Вход в Boyr";
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}