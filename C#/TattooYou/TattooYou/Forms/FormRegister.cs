using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;
using TattooYou.Database;
using TattooYou.Helpers;

namespace TattooYou.Forms
{
    public partial class FormRegister : Form
    {
        public FormRegister()
        {
            InitializeComponent();
            ApplyColorScheme();
        }

        private void ApplyColorScheme()
        {
            this.BackColor = Color.FromArgb(44, 44, 44);
            lblTitle.ForeColor = Color.White;
            lblUsername.ForeColor = Color.White;
            lblEmail.ForeColor = Color.White;
            lblPassword.ForeColor = Color.White;
            lblConfirmPassword.ForeColor = Color.White;
            lblError.ForeColor = Color.FromArgb(255, 100, 100);
            btnRegister.BackColor = Color.FromArgb(155, 89, 182);
            btnRegister.ForeColor = Color.White;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.FlatAppearance.BorderSize = 0;
            btnCancel.BackColor = Color.FromArgb(155, 89, 182);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string confirm = txtConfirmPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirm))
            {
                lblError.Text = "Заполните все поля";
                return;
            }

            if (!IsValidEmail(email))
            {
                lblError.Text = "Введите корректный email (например, name@domain.com)";
                return;
            }

            if (password.Length < 8)
            {
                lblError.Text = "Пароль должен содержать не менее 8 символов";
                return;
            }

            if (password != confirm)
            {
                lblError.Text = "Пароли не совпадают";
                return;
            }

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@username", username);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (count > 0)
                {
                    lblError.Text = "Имя пользователя уже занято";
                    return;
                }

                string hash = PasswordHelper.HashPassword(password);
                string insertQuery = "INSERT INTO users (username, password_hash, email, role) VALUES (@username, @hash, @email, 'user')";
                MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@username", username);
                insertCmd.Parameters.AddWithValue("@hash", hash);
                insertCmd.Parameters.AddWithValue("@email", email);

                try
                {
                    insertCmd.ExecuteNonQuery();
                    MessageBox.Show("Регистрация прошла успешно! Теперь вы можете войти.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    lblError.Text = "Ошибка при регистрации: " + ex.Message;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}