using System.Drawing;
using System.Windows.Forms;

namespace SanatoriumStaro
{
    partial class RegisterForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtLogin;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private System.Windows.Forms.TextBox txtFullName;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Label lblLogin;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblConfirm;
        private System.Windows.Forms.Label lblFullName;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.Label lblEmail;
        //private System.Windows.Forms.Button btnCancel;
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
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.txtFullName = new System.Windows.Forms.TextBox();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.btnRegister = new System.Windows.Forms.Button();
            this.lblLogin = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblConfirm = new System.Windows.Forms.Label();
            this.lblFullName = new System.Windows.Forms.Label();
            this.lblPhone = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.SuspendLayout();

            this.BackColor = Color.FromArgb(255, 255, 224);
            this.Font = new Font("Segoe UI", 9F);
            this.StartPosition = FormStartPosition.CenterScreen; 

            int y = 30;       
            int labelX = 40;  
            int textX = 150;    
            int width = 250;    
            int lineHeight = 35;
            //
            // lblLogin
            //
            this.lblLogin.AutoSize = true;
            this.lblLogin.Location = new System.Drawing.Point(labelX, y + 5);
            this.lblLogin.Text = "Логин*";
            this.txtLogin.Location = new System.Drawing.Point(textX, y);
            this.txtLogin.Size = new System.Drawing.Size(width, 22);
            y += lineHeight;
            ////
            //// btnCancel
            ////
            //this.btnCancel = new System.Windows.Forms.Button();
            //this.btnCancel.Location = new System.Drawing.Point(textX, y + 50); 
            //this.btnCancel.Size = new System.Drawing.Size(100, 35);
            //this.btnCancel.Text = "Отмена";
            //this.btnCancel.FlatStyle = FlatStyle.Flat;
            //this.btnCancel.BackColor = Color.DimGray;
            //this.btnCancel.ForeColor = Color.White;
            //this.btnCancel.FlatAppearance.BorderSize = 0;
            //this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // lblPassword
            //
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(labelX, y + 5);
            this.lblPassword.Text = "Пароль*";
            this.txtPassword.Location = new System.Drawing.Point(textX, y);
            this.txtPassword.Size = new System.Drawing.Size(width, 22);
            this.txtPassword.UseSystemPasswordChar = true;
            y += lineHeight;
            //
            // lblConfirm
            //
            this.lblConfirm.AutoSize = true;
            this.lblConfirm.Location = new System.Drawing.Point(labelX, y + 5);
            this.lblConfirm.Text = "Повторите*";
            this.txtConfirmPassword.Location = new System.Drawing.Point(textX, y);
            this.txtConfirmPassword.Size = new System.Drawing.Size(width, 22);
            this.txtConfirmPassword.UseSystemPasswordChar = true;
            y += lineHeight;
            //
            // lblFullName
            //
            this.lblFullName.AutoSize = true;
            this.lblFullName.Location = new System.Drawing.Point(labelX, y + 5);
            this.lblFullName.Text = "ФИО*";
            this.txtFullName.Location = new System.Drawing.Point(textX, y);
            this.txtFullName.Size = new System.Drawing.Size(width, 22);
            y += lineHeight;
            //
            // lblPhone
            //
            this.lblPhone.AutoSize = true;
            this.lblPhone.Location = new System.Drawing.Point(labelX, y + 5);
            this.lblPhone.Text = "Телефон";
            this.txtPhone.Location = new System.Drawing.Point(textX, y);
            this.txtPhone.Size = new System.Drawing.Size(width, 22);
            y += lineHeight;
            //
            // lblEmail
            //
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(labelX, y + 5);
            this.lblEmail.Text = "Email";
            this.txtEmail.Location = new System.Drawing.Point(textX, y);
            this.txtEmail.Size = new System.Drawing.Size(width, 22);
            y += 40;
            //
            // btnRegister
            //
            this.btnRegister.Location = new System.Drawing.Point(textX, y);
            this.btnRegister.Size = new System.Drawing.Size(180, 35);
            this.btnRegister.Text = "Зарегистрироваться";
            this.btnRegister.FlatStyle = FlatStyle.Flat;
            this.btnRegister.BackColor = Color.ForestGreen;
            this.btnRegister.ForeColor = Color.White;
            this.btnRegister.FlatAppearance.BorderSize = 0;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
 
            this.ClientSize = new System.Drawing.Size(500, y + 60);
            this.Controls.AddRange(new Control[] {
                this.lblLogin, this.txtLogin,
                this.lblPassword, this.txtPassword,
                this.lblConfirm, this.txtConfirmPassword,
                this.lblFullName, this.txtFullName,
                this.lblPhone, this.txtPhone,
                this.lblEmail, this.txtEmail,
                //this.btnCancel,
                this.btnRegister
            });
            this.Text = "Регистрация";

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}