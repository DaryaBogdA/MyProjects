using System;
using System.Drawing;
using System.Windows.Forms;

namespace TattooYou.Forms
{
    partial class FormMasters
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridView dgvMasters;
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
            this.dgvMasters = new DataGridView();
            this.btnAdd = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMasters)).BeginInit();
            this.SuspendLayout();
            //
            // dgvMasters
            this.dgvMasters.AllowUserToAddRows = false;
            this.dgvMasters.AllowUserToDeleteRows = false;
            this.dgvMasters.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMasters.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMasters.Location = new Point(12, 12);
            this.dgvMasters.Name = "dgvMasters";
            this.dgvMasters.ReadOnly = true;
            this.dgvMasters.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvMasters.Size = new Size(760, 350);
            this.dgvMasters.TabIndex = 0;
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
            // FormMasters
            //
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(784, 431);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.dgvMasters);
            this.Name = "FormMasters";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Управление мастерами";
            ((System.ComponentModel.ISupportInitialize)(this.dgvMasters)).EndInit();
            this.ResumeLayout(false);
        }
    }
}