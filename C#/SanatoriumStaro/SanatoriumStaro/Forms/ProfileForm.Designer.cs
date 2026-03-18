using System.Windows.Forms;

namespace SanatoriumStaro
{
    partial class ProfileForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblLogin;
        private System.Windows.Forms.Label lblLoginValue;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.Label lblRoleValue;
        private System.Windows.Forms.Label lblFullName;
        private System.Windows.Forms.TextBox txtFullName;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;

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
            this.lblLogin = new System.Windows.Forms.Label();
            this.lblLoginValue = new System.Windows.Forms.Label();
            this.lblRole = new System.Windows.Forms.Label();
            this.lblRoleValue = new System.Windows.Forms.Label();
            this.lblFullName = new System.Windows.Forms.Label();
            this.txtFullName = new System.Windows.Forms.TextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            this.StartPosition = FormStartPosition.CenterScreen;
            int y = 20;
            int labelX = 30;
            int textX = 120;
            int width = 200;
            //
            // lblLogin
            //
            this.lblLogin.AutoSize = true;
            this.lblLogin.Location = new System.Drawing.Point(labelX, y + 3);
            this.lblLogin.Text = "Логин:";
            this.lblLoginValue.AutoSize = true;
            this.lblLoginValue.Location = new System.Drawing.Point(textX, y + 3);
            this.lblLoginValue.Text = "";
            y += 30;
            //
            // lblRole
            //
            this.lblRole.AutoSize = true;
            this.lblRole.Location = new System.Drawing.Point(labelX, y + 3);
            this.lblRole.Text = "Роль:";
            this.lblRoleValue.AutoSize = true;
            this.lblRoleValue.Location = new System.Drawing.Point(textX, y + 3);
            this.lblRoleValue.Text = "";
            y += 30;
            //
            // lblFullName
            //
            this.lblFullName.AutoSize = true;
            this.lblFullName.Location = new System.Drawing.Point(labelX, y + 3);
            this.lblFullName.Text = "ФИО:";
            this.txtFullName.Location = new System.Drawing.Point(textX, y);
            this.txtFullName.Size = new System.Drawing.Size(width, 20);
            y += 30;
            //
            // lblPhone
            //
            this.lblPhone.AutoSize = true;
            this.lblPhone.Location = new System.Drawing.Point(labelX, y + 3);
            this.lblPhone.Text = "Телефон:";
            this.txtPhone.Location = new System.Drawing.Point(textX, y);
            this.txtPhone.Size = new System.Drawing.Size(width, 20);
            y += 30;
            //
            // lblEmail
            //
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(labelX, y + 3);
            this.lblEmail.Text = "Email:";
            this.txtEmail.Location = new System.Drawing.Point(textX, y);
            this.txtEmail.Size = new System.Drawing.Size(width, 20);
            y += 40;
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(textX, y);
            this.btnSave.Size = new System.Drawing.Size(90, 30);
            this.btnSave.Text = "Сохранить";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(textX + 100, y);
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.Text = "Отмена";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // ProfileForm
            //
            this.ClientSize = new System.Drawing.Size(400, y + 50);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblLogin, this.lblLoginValue,
                this.lblRole, this.lblRoleValue,
                this.lblFullName, this.txtFullName,
                this.lblPhone, this.txtPhone,
                this.lblEmail, this.txtEmail,
                this.btnSave, this.btnCancel
            });
            this.Text = "Личный кабинет";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}