using System;
using System.Drawing;
using System.Windows.Forms;

namespace TattooYou.Forms
{
    partial class FormAllAppointments
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridView dgvAppointments;
        private Button btnRefresh;

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
            this.btnRefresh = new Button();
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
            // btnRefresh
            //
            this.btnRefresh.Location = new Point(12, 380);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new Size(100, 35);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Обновить";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);
            //
            // FormAllAppointments
            //
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(784, 431);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.dgvAppointments);
            this.Name = "FormAllAppointments";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Все записи";
            ((System.ComponentModel.ISupportInitialize)(this.dgvAppointments)).EndInit();
            this.ResumeLayout(false);
        }
    }
}