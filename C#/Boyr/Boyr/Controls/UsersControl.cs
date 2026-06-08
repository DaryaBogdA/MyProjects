using Boyr.DataAccess;
using Boyr.Entities;
using Boyr.Services;
using Boyr.UI;
using System;
using System.Windows.Forms;

namespace Boyr.Controls
{
    public partial class UsersControl : UserControl
    {
        private UserRepository userRepo = new UserRepository();

        public UsersControl()
        {
            InitializeComponent();
            UiTheme.ApplyToControlTree(this);
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                var users = userRepo.GetAll();

                dataGridViewUsers.AutoGenerateColumns = false;
                dataGridViewUsers.Columns.Clear();

                dataGridViewUsers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Name = "Id", Visible = false });
                dataGridViewUsers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Login", HeaderText = "Логин", Name = "Login" });
                dataGridViewUsers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Role", HeaderText = "Роль", Name = "Role" });

                dataGridViewUsers.DataSource = users;
                dataGridViewUsers.Refresh();
                dataGridViewUsers.ClearSelection();
                cmbUserRole.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки пользователей: " + ex.Message);
            }
        }

        private void dataGridViewUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewUsers.CurrentRow == null || dataGridViewUsers.Rows.Count == 0)
                return;

            try
            {
                var row = dataGridViewUsers.CurrentRow;
                string role = row.Cells["Role"].Value?.ToString();
                if (role == "admin")
                    cmbUserRole.SelectedItem = "admin";
                else
                    cmbUserRole.SelectedItem = "user";
            }
            catch { }
        }

        private void btnChangeRole_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsers.CurrentRow == null)
            {
                MessageBox.Show("Выберите пользователя!");
                return;
            }

            if (cmbUserRole.SelectedItem == null)
            {
                MessageBox.Show("Выберите новую роль!");
                return;
            }

            int userId = (int)dataGridViewUsers.CurrentRow.Cells["Id"].Value;
            var user = userRepo.GetById(userId);
            if (user == null)
            {
                MessageBox.Show("Пользователь не найден!");
                return;
            }

            string selectedRole = cmbUserRole.SelectedItem.ToString();
            user.Role = selectedRole == "admin" ? UserRole.admin : UserRole.user;

            userRepo.Update(user);
            LoadUsers();
            MessageBox.Show("Роль пользователя изменена!");
        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsers.CurrentRow == null)
            {
                MessageBox.Show("Выберите пользователя!");
                return;
            }

            int userId = (int)dataGridViewUsers.CurrentRow.Cells["Id"].Value;

            if (AuthService.Instance.CurrentUser != null && AuthService.Instance.CurrentUser.Id == userId)
            {
                MessageBox.Show("Вы не можете удалить себя!");
                return;
            }

            if (MessageBox.Show("Удалить пользователя?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                userRepo.Delete(userId);
                LoadUsers();
                MessageBox.Show("Пользователь удалён!");
            }
        }
    }
}