using System;
using System.Drawing;
using System.Windows.Forms;

namespace TattooYou.Forms
{
    partial class FormManageUsers
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridView dgvUsers;
        private Button btnChangeRole;
        private Button btnDelete;

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
            this.dgvUsers = new DataGridView();
            this.btnChangeRole = new Button();
            this.btnDelete = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).BeginInit();
            this.SuspendLayout();
            //
            // dgvUsers
            //
            this.dgvUsers.AllowUserToAddRows = false;
            this.dgvUsers.AllowUserToDeleteRows = false;
            this.dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsers.Location = new Point(12, 12);
            this.dgvUsers.Name = "dgvUsers";
            this.dgvUsers.ReadOnly = true;
            this.dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvUsers.Size = new Size(760, 350);
            this.dgvUsers.TabIndex = 0;
            //
            // btnChangeRole
            //
            this.btnChangeRole.Location = new Point(12, 380);
            this.btnChangeRole.Name = "btnChangeRole";
            this.btnChangeRole.Size = new Size(150, 35);
            this.btnChangeRole.TabIndex = 1;
            this.btnChangeRole.Text = "Изменить роль";
            this.btnChangeRole.UseVisualStyleBackColor = false;
            this.btnChangeRole.Click += new EventHandler(this.btnChangeRole_Click);
            //
            // btnDelete
            //
            this.btnDelete.Location = new Point(180, 380);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new Size(100, 35);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Удалить";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
            //
            // FormManageUsers
            //
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(784, 431);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnChangeRole);
            this.Controls.Add(this.dgvUsers);
            this.Name = "FormManageUsers";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Управление пользователями";
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).EndInit();
            this.ResumeLayout(false);
        }
    }
}