using System.Drawing;
using System.Windows.Forms;

namespace Boyr.Controls
{
    partial class UsersControl
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridView dataGridViewUsers;
        private Panel panelUserButtons;
        private Button btnDeleteUser;
        private Button btnChangeRole;
        private ComboBox cmbUserRole;
        private Label lblUserRole;

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
            this.dataGridViewUsers = new DataGridView();
            this.panelUserButtons = new Panel();
            this.btnDeleteUser = new Button();
            this.btnChangeRole = new Button();
            this.cmbUserRole = new ComboBox();
            this.lblUserRole = new Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUsers)).BeginInit();
            this.panelUserButtons.SuspendLayout();
            this.SuspendLayout();

            // dataGridViewUsers
            this.dataGridViewUsers.AllowUserToAddRows = false;
            this.dataGridViewUsers.AllowUserToDeleteRows = false;
            this.dataGridViewUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewUsers.BackgroundColor = Color.White;
            this.dataGridViewUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewUsers.Dock = DockStyle.Fill;
            this.dataGridViewUsers.Location = new Point(0, 50);
            this.dataGridViewUsers.MultiSelect = false;
            this.dataGridViewUsers.Name = "dataGridViewUsers";
            this.dataGridViewUsers.ReadOnly = true;
            this.dataGridViewUsers.RowHeadersVisible = false;
            this.dataGridViewUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewUsers.Size = new Size(800, 500);
            this.dataGridViewUsers.TabIndex = 0;
            this.dataGridViewUsers.SelectionChanged += new System.EventHandler(this.dataGridViewUsers_SelectionChanged);

            // panelUserButtons
            this.panelUserButtons.BackColor = Color.LightGray;
            this.panelUserButtons.Controls.Add(this.btnDeleteUser);
            this.panelUserButtons.Controls.Add(this.btnChangeRole);
            this.panelUserButtons.Controls.Add(this.cmbUserRole);
            this.panelUserButtons.Controls.Add(this.lblUserRole);
            this.panelUserButtons.Dock = DockStyle.Top;
            this.panelUserButtons.Location = new Point(0, 0);
            this.panelUserButtons.Name = "panelUserButtons";
            this.panelUserButtons.Size = new Size(800, 50);
            this.panelUserButtons.TabIndex = 1;

            // btnDeleteUser
            this.btnDeleteUser.BackColor = Color.Teal;
            this.btnDeleteUser.FlatStyle = FlatStyle.Flat;
            this.btnDeleteUser.ForeColor = Color.White;
            this.btnDeleteUser.Location = new Point(300, 10);
            this.btnDeleteUser.Size = new Size(100, 30);
            this.btnDeleteUser.TabIndex = 3;
            this.btnDeleteUser.Text = "Удалить";
            this.btnDeleteUser.UseVisualStyleBackColor = false;
            this.btnDeleteUser.Click += new System.EventHandler(this.btnDeleteUser_Click);

            // btnChangeRole
            this.btnChangeRole.BackColor = Color.Teal;
            this.btnChangeRole.FlatStyle = FlatStyle.Flat;
            this.btnChangeRole.ForeColor = Color.White;
            this.btnChangeRole.Location = new Point(200, 10);
            this.btnChangeRole.Size = new Size(94, 30);
            this.btnChangeRole.TabIndex = 2;
            this.btnChangeRole.Text = "Изменить роль";
            this.btnChangeRole.UseVisualStyleBackColor = false;
            this.btnChangeRole.Click += new System.EventHandler(this.btnChangeRole_Click);

            // cmbUserRole
            this.cmbUserRole.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbUserRole.Items.AddRange(new object[] { "user", "admin" });
            this.cmbUserRole.Location = new Point(80, 12);
            this.cmbUserRole.Name = "cmbUserRole";
            this.cmbUserRole.Size = new Size(100, 24);
            this.cmbUserRole.TabIndex = 1;

            // lblUserRole
            this.lblUserRole.AutoSize = true;
            this.lblUserRole.Location = new Point(12, 17);
            this.lblUserRole.Name = "lblUserRole";
            this.lblUserRole.Size = new Size(69, 15);
            this.lblUserRole.TabIndex = 0;
            this.lblUserRole.Text = "Новая роль:";

            // UsersControl
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewUsers);
            this.Controls.Add(this.panelUserButtons);
            this.Name = "UsersControl";
            this.Size = new Size(800, 550);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUsers)).EndInit();
            this.panelUserButtons.ResumeLayout(false);
            this.panelUserButtons.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}