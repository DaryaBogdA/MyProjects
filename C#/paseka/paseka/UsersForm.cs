using paseka.DataAccess;
using paseka.Helpers;
using paseka.Models;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace paseka
{
    public partial class UsersForm : Form
    {
        public UsersForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            ThemeHelper.ApplyDarkTheme(this);
            LoadUsers();
        }

        private void LoadUsers()
        {
            string query = "SELECT id, login, role, is_active FROM users ORDER BY id";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            dgvUsers.DataSource = dt;

            dgvUsers.Columns["id"].HeaderText = "ID";
            dgvUsers.Columns["id"].ReadOnly = true;

            dgvUsers.Columns["login"].HeaderText = "Логин";
            dgvUsers.Columns["login"].ReadOnly = true;

            dgvUsers.Columns["role"].HeaderText = "Роль";
            dgvUsers.Columns["role"].ReadOnly = false;

            dgvUsers.Columns["is_active"].HeaderText = "Активен";
            dgvUsers.Columns["is_active"].ReadOnly = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvUsers.Rows)
            {
                if (row.IsNewRow) continue;

                int id = Convert.ToInt32(row.Cells["id"].Value);
                if (id == CurrentUser.Id)
                {
                    continue;
                }

                string role = row.Cells["role"].Value?.ToString() ?? "user";
                bool isActive = Convert.ToBoolean(row.Cells["is_active"].Value);

                string updateQuery = "UPDATE users SET role = @role, is_active = @is_active WHERE id = @id";
                MySqlParameter[] parameters = {
                    new MySqlParameter("@role", role),
                    new MySqlParameter("@is_active", isActive),
                    new MySqlParameter("@id", id)
                };
                DatabaseHelper.ExecuteNonQuery(updateQuery, parameters);
            }
            MessageBox.Show("Изменения сохранены.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}