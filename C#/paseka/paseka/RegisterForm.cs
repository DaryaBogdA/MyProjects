using MySql.Data.MySqlClient;
using paseka.DataAccess;
using paseka.Helpers;
using System;
using System.Data;
using System.Windows.Forms;

namespace paseka
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
            ThemeHelper.ApplyDarkTheme(this);
            this.StartPosition = FormStartPosition.CenterScreen;

            TextBoxWatermark.Set(txtLogin, "Логин");
            TextBoxWatermark.Set(txtPassword, "Пароль");
            TextBoxWatermark.Set(txtConfirmPassword, "Подтвердите пароль");
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text.Trim();
            string confirm = txtConfirmPassword.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password.Length < 8)
            {
                MessageBox.Show("Пароль должен содержать не менее 8 символов!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password != confirm)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string checkQuery = "SELECT COUNT(*) FROM users WHERE login = @login";
                MySqlParameter[] checkParams = { new MySqlParameter("@login", login) };
                DataTable dt = DatabaseHelper.ExecuteQuery(checkQuery, checkParams);
                if (Convert.ToInt32(dt.Rows[0][0]) > 0)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string insertQuery = "INSERT INTO users (login, password, role) VALUES (@login, @password, 'user')";
                MySqlParameter[] insertParams = {
                    new MySqlParameter("@login", login),
                    new MySqlParameter("@password", password)
                };
                DatabaseHelper.ExecuteNonQuery(insertQuery, insertParams);

                MessageBox.Show("Регистрация успешна! Теперь вы можете войти.",
                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}