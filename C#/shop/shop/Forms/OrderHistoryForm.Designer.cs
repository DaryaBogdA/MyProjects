using System;
using System.Drawing;
using System.Windows.Forms;

namespace shop.Forms
{
    partial class OrderHistoryForm
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridView dataGridView;
        private DataGridView dataGridViewItems;
        private SplitContainer splitContainer;

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
            this.splitContainer = new SplitContainer();
            this.dataGridView = new DataGridView();
            this.dataGridViewItems = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewItems)).BeginInit();
            this.SuspendLayout();

            // splitContainer
            this.splitContainer.Dock = DockStyle.Fill;
            this.splitContainer.Location = new Point(0, 0);
            this.splitContainer.Orientation = Orientation.Horizontal;
            this.splitContainer.Size = new Size(600, 400);
            this.splitContainer.SplitterDistance = 200;
            this.splitContainer.TabIndex = 0;

            // dataGridView (верхняя)
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = Color.White;
            this.dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new DataGridViewColumn[] {
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "№ заказа" },
                new DataGridViewTextBoxColumn { Name = "Date", HeaderText = "Дата" },
                new DataGridViewTextBoxColumn { Name = "Customer", HeaderText = "Покупатель" },
                new DataGridViewTextBoxColumn { Name = "Total", HeaderText = "Сумма" }
            });
            this.dataGridView.Dock = DockStyle.Fill;
            this.dataGridView.Location = new Point(0, 0);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new Size(600, 200);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.SelectionChanged += new EventHandler(this.dataGridView_SelectionChanged);

            // dataGridViewItems (нижняя)
            this.dataGridViewItems.AllowUserToAddRows = false;
            this.dataGridViewItems.AllowUserToDeleteRows = false;
            this.dataGridViewItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewItems.BackgroundColor = Color.White;
            this.dataGridViewItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewItems.Columns.AddRange(new DataGridViewColumn[] {
                new DataGridViewTextBoxColumn { Name = "Product", HeaderText = "Товар" },
                new DataGridViewTextBoxColumn { Name = "Quantity", HeaderText = "Кол-во" },
                new DataGridViewTextBoxColumn { Name = "Price", HeaderText = "Цена" }
            });
            this.dataGridViewItems.Dock = DockStyle.Fill;
            this.dataGridViewItems.Location = new Point(0, 0);
            this.dataGridViewItems.ReadOnly = true;
            this.dataGridViewItems.RowHeadersVisible = false;
            this.dataGridViewItems.Size = new Size(600, 196);
            this.dataGridViewItems.TabIndex = 0;

            // splitContainer.Panel1
            this.splitContainer.Panel1.Controls.Add(this.dataGridView);
            // splitContainer.Panel2
            this.splitContainer.Panel2.Controls.Add(this.dataGridViewItems);

            // OrderHistoryForm
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(600, 400);
            this.Controls.Add(this.splitContainer);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "История заказов";
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewItems)).EndInit();
            this.ResumeLayout(false);
        }
    }
}