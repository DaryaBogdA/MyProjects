using System;
using System.Drawing;
using System.Windows.Forms;

namespace TattooYou.Forms
{
    partial class FormEditStyle
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblName;
        private TextBox txtName;
        private Label lblDescription;
        private TextBox txtDescription;
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
            this.lblName = new Label();
            this.txtName = new TextBox();
            this.lblDescription = new Label();
            this.txtDescription = new TextBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.SuspendLayout();
            //
            // lblName
            //
            this.lblName.AutoSize = true;
            this.lblName.Location = new Point(20, 20);
            this.lblName.Name = "lblName";
            this.lblName.Size = new Size(62, 15);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Название:";
            //
            // txtName
            //
            this.txtName.Location = new Point(20, 40);
            this.txtName.Name = "txtName";
            this.txtName.Size = new Size(260, 23);
            this.txtName.TabIndex = 1;
            //
            // lblDescription
            //
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new Point(20, 70);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new Size(65, 15);
            this.lblDescription.TabIndex = 2;
            this.lblDescription.Text = "Описание:";
            //
            // txtDescription
            //
            this.txtDescription.Location = new Point(20, 90);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new Size(260, 80);
            this.txtDescription.TabIndex = 3;
            //
            // btnSave
            //
            this.btnSave.Location = new Point(20, 190);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new Size(100, 35);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new Point(140, 190);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(100, 35);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            //
            // FormEditStyle
            //
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(304, 241);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEditStyle";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Редактирование стиля";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}