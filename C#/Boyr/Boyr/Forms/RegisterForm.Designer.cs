using System.Drawing;
using System.Windows.Forms;

namespace Boyr.Forms
{
    partial class RegisterForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtLogin;
        private TextBox txtPassword;
        private TextBox txtConfirm;
        private Label lblLogin;
        private Label lblPassword;
        private Label lblConfirm;
        private Button btnRegister;
        private Button btnCancel;
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
            this.txtConfirm = new TextBox();
            this.lblLogin = new Label();
            this.lblPassword = new Label();
            this.lblConfirm = new Label();
            this.btnRegister = new Button();
            this.btnCancel = new Button();
            this.panelHeader.SuspendLayout();
            this.SuspendLayout();

            // panelHeader
            this.panelHeader.BackColor = Color.FromArgb(0, 128, 128);
            this.panelHeader.Controls.Add(this.lblIcon);
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = DockStyle.Top;
            this.panelHeader.Location = new Point(0, 0);
            this.panelHeader.Size = new Size(450, 70);

            // lblIcon
            this.lblIcon.AutoSize = true;
            this.lblIcon.Font = new Font("Segoe UI", 20F, FontStyle.Regular);
            this.lblIcon.ForeColor = Color.White;
            this.lblIcon.Location = new Point(20, 15);
            this.lblIcon.Text = "💍";

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.Location = new Point(70, 20);
            this.lblTitle.Text = "Регистрация";

            // lblLogin
            this.lblLogin.AutoSize = true;
            this.lblLogin.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblLogin.ForeColor = Color.Teal;
            this.lblLogin.Location = new Point(50, 100);
            this.lblLogin.Text = "Логин:";

            // txtLogin
            this.txtLogin.Location = new Point(50, 125);
            this.txtLogin.Size = new Size(350, 25);

            // lblPassword
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblPassword.ForeColor = Color.Teal;
            this.lblPassword.Location = new Point(50, 165);
            this.lblPassword.Text = "Пароль:";

            // txtPassword
            this.txtPassword.Location = new Point(50, 190);
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new Size(350, 25);

            // lblConfirm
            this.lblConfirm.AutoSize = true;
            this.lblConfirm.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblConfirm.ForeColor = Color.Teal;
            this.lblConfirm.Location = new Point(50, 230);
            this.lblConfirm.Text = "Подтверждение:";

            // txtConfirm
            this.txtConfirm.Location = new Point(50, 255);
            this.txtConfirm.PasswordChar = '*';
            this.txtConfirm.Size = new Size(350, 25);

            // btnRegister
            this.btnRegister.Location = new Point(140, 310);
            this.btnRegister.Size = new Size(130, 35);
            this.btnRegister.Text = "Зарегистрироваться";
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click); // ДОБАВЛЕНО

            // btnCancel
            this.btnCancel.Location = new Point(280, 310);
            this.btnCancel.Size = new Size(90, 35);
            this.btnCancel.Text = "Отмена";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click); // ДОБАВЛЕНО

            // RegisterForm
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(450, 380);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.txtConfirm);
            this.Controls.Add(this.lblConfirm);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtLogin);
            this.Controls.Add(this.lblLogin);
            this.Controls.Add(this.panelHeader);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Регистрация";
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}