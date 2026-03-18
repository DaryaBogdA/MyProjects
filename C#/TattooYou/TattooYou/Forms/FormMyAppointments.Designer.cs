using System;
using System.Drawing;
using System.Windows.Forms;

namespace TattooYou.Forms
{
    partial class FormMyAppointments
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridView dgvAppointments;
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
            this.dgvAppointments = new DataGridView();
            this.btnCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAppointments)).BeginInit();
            this.SuspendLayout();
            //
            // dgvAppointments
            //
            this.dgvAppointments.AllowUserToAddRows = false;
            this.dgvAppointments.AllowUserToDeleteRows = false;
            this.dgvAppointments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAppointments.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAppointments.Location = new Point(12, 12);
            this.dgvAppointments.Name = "dgvAppointments";
            this.dgvAppointments.ReadOnly = true;
            this.dgvAppointments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvAppointments.Size = new Size(760, 350);
            this.dgvAppointments.TabIndex = 0;
            //
            // btnCancel
            //
            this.btnCancel.Location = new Point(12, 380);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(120, 35);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Отменить запись";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            //
            // FormMyAppointments
            //
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(784, 431);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.dgvAppointments);
            this.Name = "FormMyAppointments";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Мои записи";
            ((System.ComponentModel.ISupportInitialize)(this.dgvAppointments)).EndInit();
            this.ResumeLayout(false);
        }
    }
}