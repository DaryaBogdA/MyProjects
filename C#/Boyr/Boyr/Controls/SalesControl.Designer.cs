using System.Drawing;
using System.Windows.Forms;

namespace Boyr.Controls
{
    partial class SalesControl
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridView dataGridViewSales;

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
            this.dataGridViewSales = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSales)).BeginInit();
            this.SuspendLayout();

            // dataGridViewSales
            this.dataGridViewSales.AllowUserToAddRows = false;
            this.dataGridViewSales.AllowUserToDeleteRows = false;
            this.dataGridViewSales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewSales.BackgroundColor = Color.White;
            this.dataGridViewSales.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSales.Dock = DockStyle.Fill;
            this.dataGridViewSales.Location = new Point(0, 0);
            this.dataGridViewSales.Name = "dataGridViewSales";
            this.dataGridViewSales.ReadOnly = true;
            this.dataGridViewSales.RowHeadersVisible = false;
            this.dataGridViewSales.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewSales.Size = new Size(800, 550);
            this.dataGridViewSales.TabIndex = 0;

            // SalesControl
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewSales);
            this.Name = "SalesControl";
            this.Size = new Size(800, 550);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSales)).EndInit();
            this.ResumeLayout(false);
        }
    }
}