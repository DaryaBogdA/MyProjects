using System;
using System.Drawing;
using System.Windows.Forms;

namespace TattooYou.Forms
{
    partial class FormBookAppointment
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblService;
        private ComboBox cmbService;
        private Label lblStyle;
        private ComboBox cmbStyle;
        private Label lblMaster;
        private ComboBox cmbMaster;
        private Label lblSize;
        private ComboBox cmbSize;
        private Label lblDate;
        private MonthCalendar monthCalendar;
        private Label lblTime;
        private ComboBox cmbTime;
        private Button btnBook;
        private Button btnCancel;
        private Label lblPrice;

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
            this.lblService = new System.Windows.Forms.Label();
            this.cmbService = new System.Windows.Forms.ComboBox();
            this.lblStyle = new System.Windows.Forms.Label();
            this.cmbStyle = new System.Windows.Forms.ComboBox();
            this.lblMaster = new System.Windows.Forms.Label();
            this.cmbMaster = new System.Windows.Forms.ComboBox();
            this.lblSize = new System.Windows.Forms.Label();
            this.cmbSize = new System.Windows.Forms.ComboBox();
            this.lblDate = new System.Windows.Forms.Label();
            this.monthCalendar = new System.Windows.Forms.MonthCalendar();
            this.lblTime = new System.Windows.Forms.Label();
            this.cmbTime = new System.Windows.Forms.ComboBox();
            this.btnBook = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblPrice = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // lblService
            //
            this.lblService.AutoSize = true;
            this.lblService.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblService.Location = new System.Drawing.Point(23, 21);
            this.lblService.Name = "lblService";
            this.lblService.Size = new System.Drawing.Size(65, 23);
            this.lblService.TabIndex = 0;
            this.lblService.Text = "Услуга:";
            //
            // cmbService
            //
            this.cmbService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbService.FormattingEnabled = true;
            this.cmbService.Location = new System.Drawing.Point(23, 48);
            this.cmbService.Name = "cmbService";
            this.cmbService.Size = new System.Drawing.Size(285, 24);
            this.cmbService.TabIndex = 1;
            this.cmbService.SelectedIndexChanged += new System.EventHandler(this.cmbService_SelectedIndexChanged);
            //
            // lblStyle
            //
            this.lblStyle.AutoSize = true;
            this.lblStyle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStyle.Location = new System.Drawing.Point(23, 85);
            this.lblStyle.Name = "lblStyle";
            this.lblStyle.Size = new System.Drawing.Size(60, 23);
            this.lblStyle.TabIndex = 2;
            this.lblStyle.Text = "Стиль:";
            //
            // cmbStyle
            //
            this.cmbStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStyle.FormattingEnabled = true;
            this.cmbStyle.Location = new System.Drawing.Point(23, 112);
            this.cmbStyle.Name = "cmbStyle";
            this.cmbStyle.Size = new System.Drawing.Size(285, 24);
            this.cmbStyle.TabIndex = 3;
            this.cmbStyle.SelectedIndexChanged += new System.EventHandler(this.cmbStyle_SelectedIndexChanged);
            //
            // lblMaster
            //
            this.lblMaster.AutoSize = true;
            this.lblMaster.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblMaster.Location = new System.Drawing.Point(23, 149);
            this.lblMaster.Name = "lblMaster";
            this.lblMaster.Size = new System.Drawing.Size(72, 23);
            this.lblMaster.TabIndex = 4;
            this.lblMaster.Text = "Мастер:";
            //
            // cmbMaster
            //
            this.cmbMaster.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMaster.FormattingEnabled = true;
            this.cmbMaster.Location = new System.Drawing.Point(23, 176);
            this.cmbMaster.Name = "cmbMaster";
            this.cmbMaster.Size = new System.Drawing.Size(285, 24);
            this.cmbMaster.TabIndex = 5;
            this.cmbMaster.SelectedIndexChanged += new System.EventHandler(this.cmbMaster_SelectedIndexChanged);
            //
            // lblSize
            //
            this.lblSize.AutoSize = true;
            this.lblSize.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSize.Location = new System.Drawing.Point(23, 213);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(72, 23);
            this.lblSize.TabIndex = 6;
            this.lblSize.Text = "Размер:";
            //
            // cmbSize
            //
            this.cmbSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSize.FormattingEnabled = true;
            this.cmbSize.Items.AddRange(new object[] {
            "small",
            "medium",
            "large"});
            this.cmbSize.Location = new System.Drawing.Point(23, 240);
            this.cmbSize.Name = "cmbSize";
            this.cmbSize.Size = new System.Drawing.Size(285, 24);
            this.cmbSize.TabIndex = 7;
            this.cmbSize.SelectedIndexChanged += new System.EventHandler(this.cmbSize_SelectedIndexChanged);
            //
            // lblPrice
            //
            this.lblPrice.AutoSize = true;
            this.lblPrice.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPrice.Location = new System.Drawing.Point(23, 265);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(58, 23);
            this.lblPrice.TabIndex = 14;
            this.lblPrice.Text = "Цена:";
            //
            // lblDate
            //
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDate.Location = new System.Drawing.Point(23, 277);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(51, 23);
            this.lblDate.TabIndex = 8;
            this.lblDate.Text = "Дата:";
            //
            // monthCalendar
            //
            this.monthCalendar.Location = new System.Drawing.Point(23, 304);
            this.monthCalendar.Margin = new System.Windows.Forms.Padding(10);
            this.monthCalendar.Name = "monthCalendar";
            this.monthCalendar.TabIndex = 9;
            //
            // lblTime
            //
            this.lblTime.AutoSize = true;
            this.lblTime.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTime.Location = new System.Drawing.Point(23, 522);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(64, 23);
            this.lblTime.TabIndex = 10;
            this.lblTime.Text = "Время:";
            //
            // cmbTime
            //
            this.cmbTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTime.FormattingEnabled = true;
            this.cmbTime.Items.AddRange(new object[] {
            "10:00",
            "13:00",
            "16:00",
            "19:00"});
            this.cmbTime.Location = new System.Drawing.Point(23, 549);
            this.cmbTime.Name = "cmbTime";
            this.cmbTime.Size = new System.Drawing.Size(285, 24);
            this.cmbTime.TabIndex = 11;
            //
            // btnBook
            //
            this.btnBook.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnBook.Location = new System.Drawing.Point(23, 597);
            this.btnBook.Name = "btnBook";
            this.btnBook.Size = new System.Drawing.Size(137, 37);
            this.btnBook.TabIndex = 12;
            this.btnBook.Text = "Записаться";
            this.btnBook.UseVisualStyleBackColor = false;
            this.btnBook.Click += new System.EventHandler(this.btnBook_Click);
            //
            // btnCancel
            //
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCancel.Location = new System.Drawing.Point(171, 597);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(137, 37);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // FormBookAppointment
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 648);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnBook);
            this.Controls.Add(this.cmbTime);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.monthCalendar);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.cmbSize);
            this.Controls.Add(this.lblSize);
            this.Controls.Add(this.cmbMaster);
            this.Controls.Add(this.lblMaster);
            this.Controls.Add(this.cmbStyle);
            this.Controls.Add(this.lblStyle);
            this.Controls.Add(this.cmbService);
            this.Controls.Add(this.lblService);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormBookAppointment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Запись на тату";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}