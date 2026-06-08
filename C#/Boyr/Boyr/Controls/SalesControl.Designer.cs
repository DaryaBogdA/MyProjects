using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Boyr.Controls
{
    partial class SalesControl
    {
        private System.ComponentModel.IContainer components = null;
        private TabControl tabControl;
        private TabPage tabChart;
        private TabPage tabTable;
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
            this.tabControl = new TabControl();
            this.tabChart = new TabPage();
            this.tabTable = new TabPage();
            this.dataGridViewSales = new DataGridView();

            this.tabControl.SuspendLayout();
            this.tabTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSales)).BeginInit();
            this.SuspendLayout();

            // tabControl
            this.tabControl.Controls.Add(this.tabChart);
            this.tabControl.Controls.Add(this.tabTable);
            this.tabControl.Dock = DockStyle.Fill;
            this.tabControl.Location = new Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new Size(800, 550);
            this.tabControl.TabIndex = 0;

            // tabChart
            this.tabChart.BackColor = Color.White;
            this.tabChart.Location = new Point(4, 24);
            this.tabChart.Name = "tabChart";
            this.tabChart.Padding = new Padding(3);
            this.tabChart.Size = new Size(792, 522);
            this.tabChart.TabIndex = 0;
            this.tabChart.Text = "📊 График продаж";

            // tabTable
            this.tabTable.BackColor = Color.White;
            this.tabTable.Controls.Add(this.dataGridViewSales);
            this.tabTable.Location = new Point(4, 24);
            this.tabTable.Name = "tabTable";
            this.tabTable.Padding = new Padding(3);
            this.tabTable.Size = new Size(792, 522);
            this.tabTable.TabIndex = 1;
            this.tabTable.Text = "📋 Список продаж";

            // dataGridViewSales
            this.dataGridViewSales.AllowUserToAddRows = false;
            this.dataGridViewSales.AllowUserToDeleteRows = false;
            this.dataGridViewSales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewSales.BackgroundColor = Color.White;
            this.dataGridViewSales.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSales.Dock = DockStyle.Fill;
            this.dataGridViewSales.Location = new Point(3, 3);
            this.dataGridViewSales.Name = "dataGridViewSales";
            this.dataGridViewSales.ReadOnly = true;
            this.dataGridViewSales.RowHeadersVisible = false;
            this.dataGridViewSales.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewSales.Size = new Size(786, 516);
            this.dataGridViewSales.TabIndex = 0;

            // SalesControl
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.tabControl);
            this.Name = "SalesControl";
            this.Size = new Size(800, 550);

            this.tabControl.ResumeLayout(false);
            this.tabTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSales)).EndInit();
            this.ResumeLayout(false);
        }
    }
}