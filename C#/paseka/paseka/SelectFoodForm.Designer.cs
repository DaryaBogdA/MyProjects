namespace paseka
{
    partial class SelectFoodForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvFood;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuantity;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.dgvFood = new System.Windows.Forms.DataGridView();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFood)).BeginInit();
            this.SuspendLayout();

            // dgvFood
            this.dgvFood = new System.Windows.Forms.DataGridView();
            this.dgvFood.AllowUserToAddRows = false; // <- добавлено
            this.dgvFood.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFood.Location = new System.Drawing.Point(12, 12);
            this.dgvFood.Name = "dgvFood";
            this.dgvFood.Size = new System.Drawing.Size(560, 250);
            this.dgvFood.TabIndex = 0;
            this.dgvFood.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

            this.colSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colSelect.HeaderText = "Выбрать";
            this.colSelect.Name = "colSelect";
            this.colSelect.Width = 50;

            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName.HeaderText = "Название";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;

            this.colPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrice.HeaderText = "Цена";
            this.colPrice.Name = "colPrice";
            this.colPrice.ReadOnly = true;

            this.colQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQuantity.HeaderText = "Количество";
            this.colQuantity.Name = "colQuantity";

            this.dgvFood.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colSelect,
                this.colName,
                this.colPrice,
                this.colQuantity
            });

            // btnOk
            this.btnOk.Location = new System.Drawing.Point(12, 280);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(90, 30);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "Добавить";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(120, 280);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // SelectFoodForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 331);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.dgvFood);
            this.Name = "SelectFoodForm";
            this.Text = "Выбор еды";
            ((System.ComponentModel.ISupportInitialize)(this.dgvFood)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion
    }
}