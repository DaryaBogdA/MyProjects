using MySql.Data.MySqlClient;
using paseka.DataAccess;
using paseka.Helpers;
using paseka.Models;
using System;
using System.Data;
using System.Windows.Forms;

namespace paseka
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            ThemeHelper.ApplyDarkTheme(this);
            this.StartPosition = FormStartPosition.CenterScreen;

            TextBoxWatermark.Set(txtLogin, "Логин");
            TextBoxWatermark.Set(txtPassword, "Пароль");
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "SELECT id, login, role, is_active FROM users WHERE login = @login AND password = @password"; MySqlParameter[] parameters = {
                    new MySqlParameter("@login", login),
                    new MySqlParameter("@password", password)
                };

                DataTable result = DatabaseHelper.ExecuteQuery(query, parameters);

                if (result.Rows.Count > 0)
                {
                    DataRow row = result.Rows[0];
                    bool isActive = Convert.ToBoolean(row["is_active"]);
                    if (!isActive)
                    {
                        MessageBox.Show("Ваша учётная запись заблокирована. Обратитесь к администратору.", "Доступ запрещён",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    CurrentUser.Id = Convert.ToInt32(row["id"]);
                    CurrentUser.Login = row["login"].ToString();
                    CurrentUser.Role = row["role"].ToString();

                    Form mainForm = (CurrentUser.Role == "admin")
                        ? new AdminMainForm()
                        : (Form)new UserMainForm();

                    mainForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            RegisterForm regForm = new RegisterForm();
            regForm.ShowDialog();
        }
    }
}