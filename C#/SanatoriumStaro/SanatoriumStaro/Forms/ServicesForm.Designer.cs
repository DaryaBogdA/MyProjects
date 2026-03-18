using System.Drawing;
using System.Windows.Forms;

namespace SanatoriumStaro
{
    partial class ServicesForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvServices;
        private System.Windows.Forms.Button btnBook;
        private System.Windows.Forms.Button btnClose;

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
            this.dgvServices = new System.Windows.Forms.DataGridView();
            this.btnBook = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServices)).BeginInit();
            this.SuspendLayout();
            this.BackColor = Color.FromArgb(255, 255, 224);
            this.Font = new Font("Segoe UI", 9F);
            this.StartPosition = FormStartPosition.CenterScreen;
            //
            // dgvServices
            //
            this.dgvServices.BackgroundColor = Color.White;
            this.dgvServices.BorderStyle = BorderStyle.None;
            this.dgvServices.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 250); // светло-фиолетовый для чередования
            this.dgvServices.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            this.dgvServices.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.dgvServices.EnableHeadersVisualStyles = false;
            this.dgvServices.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            this.dgvServices.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            //
            // btnBook
            //
            this.btnBook.FlatStyle = FlatStyle.Flat;
            this.btnBook.BackColor = Color.ForestGreen;
            this.btnBook.ForeColor = Color.White;
            this.btnBook.FlatAppearance.BorderSize = 0;
            //
            // btnClose
            //
            this.btnClose.FlatStyle = FlatStyle.Flat;
            this.btnClose.BackColor = Color.DimGray;
            this.btnClose.ForeColor = Color.White;
            this.btnClose.FlatAppearance.BorderSize = 0;
            //
            // dgvServices
            //
            this.dgvServices.AllowUserToAddRows = false;
            this.dgvServices.AllowUserToDeleteRows = false;
            this.dgvServices.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvServices.Location = new System.Drawing.Point(12, 12);
            this.dgvServices.MultiSelect = false;
            this.dgvServices.ReadOnly = true;
            this.dgvServices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvServices.Size = new System.Drawing.Size(660, 300);
            this.dgvServices.TabIndex = 0;
            //
            // btnBook
            //
            this.btnBook.Location = new System.Drawing.Point(12, 320);
            this.btnBook.Size = new System.Drawing.Size(150, 30);
            this.btnBook.Text = "Записаться";
            this.btnBook.Click += new System.EventHandler(this.btnBook_Click);
            //
            // btnClose
            //
            this.btnClose.Location = new System.Drawing.Point(180, 320);
            this.btnClose.Size = new System.Drawing.Size(100, 30);
            this.btnClose.Text = "Закрыть";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // ServicesForm
            //
            this.ClientSize = new System.Drawing.Size(684, 361);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnBook);
            this.Controls.Add(this.dgvServices);
            this.Text = "Услуги санатория";
            ((System.ComponentModel.ISupportInitialize)(this.dgvServices)).EndInit();
            this.ResumeLayout(false);
        }
    }
}