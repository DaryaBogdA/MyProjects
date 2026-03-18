using System;
using System.Drawing;
using System.Windows.Forms;

namespace TattooYou.Forms
{
    partial class FormEditService
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblName;
        private TextBox txtName;
        private Label lblDescription;
        private TextBox txtDescription;
        private Label lblPrice;
        private TextBox txtPrice;
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
            this.lblPrice = new Label();
            this.txtPrice = new TextBox();
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
            // lblPrice
            //
            this.lblPrice.AutoSize = true;
            this.lblPrice.Location = new Point(20, 180);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new Size(38, 15);
            this.lblPrice.TabIndex = 4;
            this.lblPrice.Text = "Цена:";
            //
            // txtPrice
            //
            this.txtPrice.Location = new Point(20, 200);
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.Size = new Size(120, 23);
            this.txtPrice.TabIndex = 5;
            //
            // btnSave
            //
            this.btnSave.Location = new Point(20, 250);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new Size(100, 35);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new Point(140, 250);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(100, 35);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            //
            // FormEditService
            //
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(304, 311);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtPrice);
            this.Controls.Add(this.lblPrice);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEditService";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Редактирование услуги";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}