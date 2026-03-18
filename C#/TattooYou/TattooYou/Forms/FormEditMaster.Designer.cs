using System;
using System.Drawing;
using System.Windows.Forms;

namespace TattooYou.Forms
{
    partial class FormEditMaster
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblFullName;
        private TextBox txtFullName;
        private Label lblBio;
        private TextBox txtBio;
        private Label lblPhotoUrl;
        private TextBox txtPhotoUrl;
        private Label lblIsActive;
        private CheckBox chkIsActive;
        private Label lblStyles;
        private CheckedListBox clbStyles;
        private Button btnSave;
        private Button btnCancel;

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
            this.lblFullName = new Label();
            this.txtFullName = new TextBox();
            this.lblBio = new Label();
            this.txtBio = new TextBox();
            this.lblPhotoUrl = new Label();
            this.txtPhotoUrl = new TextBox();
            this.lblIsActive = new Label();
            this.chkIsActive = new CheckBox();
            this.lblStyles = new Label();
            this.clbStyles = new CheckedListBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.SuspendLayout();
            //
            // lblFullName
            //
            this.lblFullName.AutoSize = true;
            this.lblFullName.Location = new Point(20, 20);
            this.lblFullName.Name = "lblFullName";
            this.lblFullName.Size = new Size(40, 15);
            this.lblFullName.TabIndex = 0;
            this.lblFullName.Text = "ФИО:";
            //
            // txtFullName
            //
            this.txtFullName.Location = new Point(20, 40);
            this.txtFullName.Name = "txtFullName";
            this.txtFullName.Size = new Size(300, 23);
            this.txtFullName.TabIndex = 1;
            //
            // lblBio
            //
            this.lblBio.AutoSize = true;
            this.lblBio.Location = new Point(20, 70);
            this.lblBio.Name = "lblBio";
            this.lblBio.Size = new Size(71, 15);
            this.lblBio.TabIndex = 2;
            this.lblBio.Text = "Биография:";
            //
            // txtBio
            //
            this.txtBio.Location = new Point(20, 90);
            this.txtBio.Multiline = true;
            this.txtBio.Name = "txtBio";
            this.txtBio.Size = new Size(300, 80);
            this.txtBio.TabIndex = 3;
            //
            // lblPhotoUrl
            //
            this.lblPhotoUrl.AutoSize = true;
            this.lblPhotoUrl.Location = new Point(20, 180);
            this.lblPhotoUrl.Name = "lblPhotoUrl";
            this.lblPhotoUrl.Size = new Size(71, 15);
            this.lblPhotoUrl.TabIndex = 4;
            this.lblPhotoUrl.Text = "Ссылка на фото:";
            //
            // txtPhotoUrl
            //
            this.txtPhotoUrl.Location = new Point(20, 200);
            this.txtPhotoUrl.Name = "txtPhotoUrl";
            this.txtPhotoUrl.Size = new Size(300, 23);
            this.txtPhotoUrl.TabIndex = 5;
            //
            // lblIsActive
            //
            this.lblIsActive.AutoSize = true;
            this.lblIsActive.Location = new Point(20, 240);
            this.lblIsActive.Name = "lblIsActive";
            this.lblIsActive.Size = new Size(62, 15);
            this.lblIsActive.TabIndex = 6;
            this.lblIsActive.Text = "Активен:";
            //
            // chkIsActive
            //
            this.chkIsActive.AutoSize = true;
            this.chkIsActive.Location = new Point(90, 240);
            this.chkIsActive.Name = "chkIsActive";
            this.chkIsActive.Size = new Size(15, 14);
            this.chkIsActive.TabIndex = 7;
            this.chkIsActive.UseVisualStyleBackColor = true;
            //
            // lblStyles
            //
            this.lblStyles.AutoSize = true;
            this.lblStyles.Location = new Point(20, 280);
            this.lblStyles.Name = "lblStyles";
            this.lblStyles.Size = new Size(43, 15);
            this.lblStyles.TabIndex = 8;
            this.lblStyles.Text = "Стили:";
            //
            // clbStyles
            //
            this.clbStyles.FormattingEnabled = true;
            this.clbStyles.Location = new Point(20, 300);
            this.clbStyles.Name = "clbStyles";
            this.clbStyles.Size = new Size(300, 130);
            this.clbStyles.TabIndex = 9;
            //
            // btnSave
            //
            this.btnSave.Location = new Point(20, 450);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new Size(100, 35);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new Point(140, 450);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(100, 35);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            //
            // FormEditMaster
            //
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(344, 511);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.clbStyles);
            this.Controls.Add(this.lblStyles);
            this.Controls.Add(this.chkIsActive);
            this.Controls.Add(this.lblIsActive);
            this.Controls.Add(this.txtPhotoUrl);
            this.Controls.Add(this.lblPhotoUrl);
            this.Controls.Add(this.txtBio);
            this.Controls.Add(this.lblBio);
            this.Controls.Add(this.txtFullName);
            this.Controls.Add(this.lblFullName);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEditMaster";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Редактирование мастера";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}