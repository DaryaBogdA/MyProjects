using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Avto.Data;
using Avto.Models;

namespace Avto.Forms
{
    public partial class UserManagementForm : Form
    {
        private List<User> _users;
        private int? _selectedUserId = null;

        public UserManagementForm()
        {
            InitializeComponent();
        }

        private void UserManagementForm_Load(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                _users = DatabaseHelper.GetUsers();
                dgvUsers.DataSource = null;
                dgvUsers.DataSource = _users;

                dgvUsers.Columns["Id"].HeaderText = "ID";
                dgvUsers.Columns["Username"].HeaderText = "Логин";
                dgvUsers.Columns["FullName"].HeaderText = "Полное имя";
                dgvUsers.Columns["Role"].HeaderText = "Роль";
                dgvUsers.Columns["IsActive"].HeaderText = "Активен";
                dgvUsers.Columns["PasswordHash"].Visible = false;

                dgvUsers.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователей: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                var selected = dgvUsers.SelectedRows[0].DataBoundItem as User;
                _selectedUserId = selected?.Id;
            }
            else
            {
                _selectedUserId = null;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            UserEditForm form = new UserEditForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadUsers();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (_selectedUserId == null)
            {
                MessageBox.Show("Выберите пользователя для редактирования.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var user = _users.Find(u => u.Id == _selectedUserId);
            if (user != null)
            {
                UserEditForm form = new UserEditForm(user);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadUsers();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedUserId == null)
            {
                MessageBox.Show("Выберите пользователя для удаления.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Удалить выбранного пользователя?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.DeleteUser(_selectedUserId.Value);
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}