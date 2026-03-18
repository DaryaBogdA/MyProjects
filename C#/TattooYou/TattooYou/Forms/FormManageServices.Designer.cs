using System;
using System.Drawing;
using System.Windows.Forms;

namespace TattooYou.Forms
{
    partial class FormManageServices
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridView dgvServices;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnExport;
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
            this.dgvServices = new DataGridView();
            this.btnAdd = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServices)).BeginInit();
            this.SuspendLayout();
            //
            // dgvServices
            //
            this.dgvServices.AllowUserToAddRows = false;
            this.dgvServices.AllowUserToDeleteRows = false;
            this.dgvServices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvServices.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvServices.Location = new Point(12, 12);
            this.dgvServices.Name = "dgvServices";
            this.dgvServices.ReadOnly = true;
            this.dgvServices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvServices.Size = new Size(760, 350);
            this.dgvServices.TabIndex = 0;
            //
            // btnAdd
            //
            this.btnAdd.Location = new Point(12, 380);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new Size(100, 35);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Добавить";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new EventHandler(this.btnAdd_Click);
            //
            // btnEdit
            //
            this.btnEdit.Location = new Point(130, 380);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new Size(100, 35);
            this.btnEdit.TabIndex = 2;
            this.btnEdit.Text = "Изменить";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new EventHandler(this.btnEdit_Click);
            //
            // btnDelete
            //
            this.btnDelete.Location = new Point(248, 380);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new Size(100, 35);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "Удалить";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
            //
            // btnExport
            //
            this.btnExport = new Button();
            this.btnExport.Location = new Point(368, 380);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new Size(100, 35);
            this.btnExport.TabIndex = 4;
            this.btnExport.Text = "Экспорт";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new EventHandler(this.btnExport_Click);
            this.Controls.Add(this.btnExport);
            //
            // FormManageServices
            //
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(784, 431);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.dgvServices);
            this.Name = "FormManageServices";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Управление услугами";
            ((System.ComponentModel.ISupportInitialize)(this.dgvServices)).EndInit();
            this.ResumeLayout(false);
        }
    }
}