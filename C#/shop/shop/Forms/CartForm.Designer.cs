using System;
using System.Drawing;
using System.Windows.Forms;

namespace shop.Forms
{
    partial class CartForm
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridView dataGridView;
        private Button btnRemove;
        private Button btnClear;
        private Button btnCheckout;
        private Label lblTotal;
        private Panel panelBottom;

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
            this.dataGridView = new DataGridView();
            this.btnRemove = new Button();
            this.btnClear = new Button();
            this.btnCheckout = new Button();
            this.lblTotal = new Label();
            this.panelBottom = new Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();

            // dataGridView
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = Color.White;
            this.dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new DataGridViewColumn[] {
                new DataGridViewTextBoxColumn {
                    Name = "ProductId",
                    HeaderText = "ID",
                    Visible = false
                },
                new DataGridViewTextBoxColumn {
                    Name = "Name",
                    HeaderText = "Товар",
                    ReadOnly = true      // ← добавить
                },
                new DataGridViewTextBoxColumn {
                    Name = "Price",
                    HeaderText = "Цена",
                    ReadOnly = true      // ← добавить
                },
                new DataGridViewTextBoxColumn {
                    Name = "Quantity",
                    HeaderText = "Количество" 
                    // ReadOnly по умолчанию false – редактируемая
                },
                new DataGridViewTextBoxColumn {
                    Name = "Total",
                    HeaderText = "Сумма",
                    ReadOnly = true      // ← добавить
                }
            });
            this.dataGridView.Dock = DockStyle.Fill;
            this.dataGridView.Location = new Point(0, 0);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.ReadOnly = false;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new Size(700, 350);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.CellEndEdit += new DataGridViewCellEventHandler(this.dataGridView_CellEndEdit);

            // panelBottom
            this.panelBottom.BackColor = Color.LightGray;
            this.panelBottom.Controls.Add(this.lblTotal);
            this.panelBottom.Controls.Add(this.btnCheckout);
            this.panelBottom.Controls.Add(this.btnClear);
            this.panelBottom.Controls.Add(this.btnRemove);
            this.panelBottom.Dock = DockStyle.Bottom;
            this.panelBottom.Location = new Point(0, 350);
            this.panelBottom.Size = new Size(700, 60);
            this.panelBottom.TabIndex = 1;

            // btnRemove
            this.btnRemove.BackColor = Color.Black;
            this.btnRemove.FlatStyle = FlatStyle.Flat;
            this.btnRemove.ForeColor = Color.White;
            this.btnRemove.Location = new Point(10, 15);
            this.btnRemove.Size = new Size(100, 30);
            this.btnRemove.Text = "Удалить";
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new EventHandler(this.btnRemove_Click);

            // btnClear
            this.btnClear.BackColor = Color.Black;
            this.btnClear.FlatStyle = FlatStyle.Flat;
            this.btnClear.ForeColor = Color.White;
            this.btnClear.Location = new Point(120, 15);
            this.btnClear.Size = new Size(100, 30);
            this.btnClear.Text = "Очистить";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new EventHandler(this.btnClear_Click);

            // btnCheckout
            this.btnCheckout.BackColor = Color.Black;
            this.btnCheckout.FlatStyle = FlatStyle.Flat;
            this.btnCheckout.ForeColor = Color.White;
            this.btnCheckout.Location = new Point(230, 15);
            this.btnCheckout.Size = new Size(150, 30);
            this.btnCheckout.Text = "Оформить заказ";
            this.btnCheckout.UseVisualStyleBackColor = false;
            this.btnCheckout.Click += new EventHandler(this.btnCheckout_Click);

            // lblTotal
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblTotal.ForeColor = Color.Black;
            this.lblTotal.Location = new Point(500, 18);
            this.lblTotal.Size = new Size(100, 21);
            this.lblTotal.Text = "Итого: 0 ₽";

            // CartForm
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(700, 410);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.panelBottom);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Корзина";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}