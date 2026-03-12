using System.Windows.Forms;

namespace paseka
{
    partial class BookingForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblService;
        private System.Windows.Forms.ComboBox cmbService;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.DateTimePicker dtpTime;
        private System.Windows.Forms.Label lblEndTime;
        private System.Windows.Forms.Button btnBook;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridView dgvFood;
        private System.Windows.Forms.Label lblFood;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuantity;
        private System.Windows.Forms.Label lblTotalPrice;

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
            this.lblService = new System.Windows.Forms.Label();
            this.cmbService = new System.Windows.Forms.ComboBox();
            this.lblDate = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.lblTime = new System.Windows.Forms.Label();
            this.dtpTime = new System.Windows.Forms.DateTimePicker();
            this.lblEndTime = new System.Windows.Forms.Label();
            this.btnBook = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblService
            // 
            this.lblService.AutoSize = true;
            this.lblService.Location = new System.Drawing.Point(20, 20);
            this.lblService.Name = "lblService";
            this.lblService.Size = new System.Drawing.Size(46, 13);
            this.lblService.TabIndex = 0;
            this.lblService.Text = "Услуга:";
            // 
            // cmbService
            // 
            this.cmbService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbService.FormattingEnabled = true;
            this.cmbService.Location = new System.Drawing.Point(80, 17);
            this.cmbService.Name = "cmbService";
            this.cmbService.Size = new System.Drawing.Size(200, 21);
            this.cmbService.TabIndex = 1;
            this.cmbService.SelectedIndexChanged += new System.EventHandler(this.cmbService_SelectedIndexChanged);
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(20, 50);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(36, 13);
            this.lblDate.TabIndex = 2;
            this.lblDate.Text = "Дата:";
            // 
            // dtpDate
            // 
            this.dtpDate.Location = new System.Drawing.Point(80, 47);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(140, 20);
            this.dtpDate.TabIndex = 3;
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(20, 80);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(43, 13);
            this.lblTime.TabIndex = 4;
            this.lblTime.Text = "Время:";
            // 
            // dtpTime
            // 
            this.dtpTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpTime.Location = new System.Drawing.Point(80, 77);
            this.dtpTime.Name = "dtpTime";
            this.dtpTime.ShowUpDown = true;
            this.dtpTime.Size = new System.Drawing.Size(90, 20);
            this.dtpTime.TabIndex = 5;
            this.dtpTime.ValueChanged += new System.EventHandler(this.dtpTime_ValueChanged);
            // 
            // lblEndTime
            // 
            this.lblEndTime.AutoSize = true;
            this.lblEndTime.Location = new System.Drawing.Point(20, 110);
            this.lblEndTime.Name = "lblEndTime";
            this.lblEndTime.Size = new System.Drawing.Size(77, 13);
            this.lblEndTime.TabIndex = 6;
            this.lblEndTime.Text = "Окончание: --";
            // 
            // btnBook
            // 
            this.btnBook.Location = new System.Drawing.Point(80, 140);
            this.btnBook.Name = "btnBook";
            this.btnBook.Size = new System.Drawing.Size(100, 30);
            this.btnBook.TabIndex = 7;
            this.btnBook.Text = "Забронировать";
            this.btnBook.UseVisualStyleBackColor = true;
            this.btnBook.Click += new System.EventHandler(this.btnBook_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(190, 140);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // BookingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 200);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnBook);
            this.Controls.Add(this.lblEndTime);
            this.Controls.Add(this.dtpTime);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.dtpDate);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.cmbService);
            this.Controls.Add(this.lblService);
            this.Name = "BookingForm";
            this.Text = "Бронирование";
            this.ResumeLayout(false);
            this.PerformLayout();
            //
            // lblFood
            //
            this.lblFood = new System.Windows.Forms.Label();
            this.lblFood.AutoSize = true;
            this.lblFood.Location = new System.Drawing.Point(20, 180);
            this.lblFood.Name = "lblFood";
            this.lblFood.Size = new System.Drawing.Size(105, 13);
            this.lblFood.TabIndex = 9;
            this.lblFood.Text = "Добавить еду к заказу:";
            //
            // lblTotalPrice
            //
            this.lblTotalPrice = new System.Windows.Forms.Label();
            this.lblTotalPrice.AutoSize = true;
            this.lblTotalPrice.Location = new System.Drawing.Point(20, 360);
            this.lblTotalPrice.Name = "lblTotalPrice";
            this.lblTotalPrice.Size = new System.Drawing.Size(90, 13);
            this.lblTotalPrice.TabIndex = 11;
            this.lblTotalPrice.Text = "Итого: 0 руб.";
            this.Controls.Add(this.lblTotalPrice);

            this.ClientSize = new System.Drawing.Size(600, 450);
            //
            // dgvFood
            //
            this.dgvFood = new System.Windows.Forms.DataGridView();
            this.dgvFood.AllowUserToAddRows = false; // <- добавить
            this.dgvFood.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFood.Location = new System.Drawing.Point(20, 200);
            this.dgvFood.Name = "dgvFood";
            this.dgvFood.Size = new System.Drawing.Size(550, 150);
            this.dgvFood.TabIndex = 10;
            this.dgvFood.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

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

            this.dgvFood.Columns.AddRange(new DataGridViewColumn[] {
            this.colSelect,
            this.colName,
            this.colPrice,
            this.colQuantity
        });

            this.Controls.Add(this.lblFood);
            this.Controls.Add(this.dgvFood);

            this.ClientSize = new System.Drawing.Size(600, 400);
        }

        #endregion
    }
}