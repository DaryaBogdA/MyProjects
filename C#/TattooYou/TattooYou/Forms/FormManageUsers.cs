using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using TattooYou.Database;

namespace TattooYou.Forms
{
    public partial class FormManageUsers : Form
    {
        public FormManageUsers()
        {
            InitializeComponent();
            ApplyColorScheme();
            LoadUsers();
        }

        private void ApplyColorScheme()
        {
            this.BackColor = Color.White;
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Label lbl)
                    lbl.ForeColor = Color.Black;
                if (ctrl is Button btn)
                {
                    btn.BackColor = Color.FromArgb(155, 89, 182);
                    btn.ForeColor = Color.White;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                }
            }
            dgvUsers.BackgroundColor = Color.White;
            dgvUsers.DefaultCellStyle.BackColor = Color.White;
            dgvUsers.DefaultCellStyle.ForeColor = Color.Black;
            dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvUsers.EnableHeadersVisualStyles = false;
        }

        private void LoadUsers()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT id, username, email, role FROM users ORDER BY username";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvUsers.DataSource = dt;

                if (dgvUsers.Columns.Count > 0)
                {
                    dgvUsers.Columns["id"].HeaderText = "ID";
                    dgvUsers.Columns["username"].HeaderText = "Логин";
                    dgvUsers.Columns["email"].HeaderText = "Email";
                    dgvUsers.Columns["role"].HeaderText = "Роль";
                }
            }
        }

        private void btnChangeRole_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пользователя");
                return;
            }
            int id = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["id"].Value);
            string currentRole = dgvUsers.SelectedRows[0].Cells["role"].Value.ToString();
            string newRole = currentRole == "admin" ? "user" : "admin";
            DialogResult dr = MessageBox.Show($"Изменить роль на '{newRole}'?", "Подтверждение", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string update = "UPDATE users SET role = @role WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(update, conn);
                    cmd.Parameters.AddWithValue("@role", newRole);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                LoadUsers();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пользователя");
                return;
            }
            int id = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["id"].Value);
            string username = dgvUsers.SelectedRows[0].Cells["username"].Value.ToString();
            DialogResult dr = MessageBox.Show($"Удалить пользователя '{username}'?", "Подтверждение", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string delete = "DELETE FROM users WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(delete, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                LoadUsers();
            }
        }
    }
}