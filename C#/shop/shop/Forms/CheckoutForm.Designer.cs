using System;
using System.Drawing;
using System.Windows.Forms;

namespace shop.Forms
{
    partial class CheckoutForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblCustomer;
        private TextBox txtCustomerName;
        private Button btnConfirm;
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
            this.lblCustomer = new Label();
            this.txtCustomerName = new TextBox();
            this.btnConfirm = new Button();
            this.btnCancel = new Button();
            this.SuspendLayout();

            // lblCustomer
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Location = new Point(30, 30);
            this.lblCustomer.Text = "Имя покупателя:";

            // txtCustomerName
            this.txtCustomerName.Location = new Point(150, 27);
            this.txtCustomerName.Size = new Size(200, 22);

            // btnConfirm
            this.btnConfirm.BackColor = Color.Black;
            this.btnConfirm.FlatStyle = FlatStyle.Flat;
            this.btnConfirm.ForeColor = Color.White;
            this.btnConfirm.Location = new Point(150, 70);
            this.btnConfirm.Size = new Size(100, 30);
            this.btnConfirm.Text = "Подтвердить";
            this.btnConfirm.UseVisualStyleBackColor = false;
            this.btnConfirm.Click += new EventHandler(this.btnConfirm_Click);

            // btnCancel
            this.btnCancel.BackColor = Color.Black;
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.ForeColor = Color.White;
            this.btnCancel.Location = new Point(260, 70);
            this.btnCancel.Size = new Size(100, 30);
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // CheckoutForm
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(400, 130);
            this.Controls.Add(this.lblCustomer);
            this.Controls.Add(this.txtCustomerName);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Оформление заказа";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}