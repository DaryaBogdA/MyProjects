using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;
using TattooYou.Database;
using TattooYou.Helpers;

namespace TattooYou.Forms
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
            this.AcceptButton = btnLogin;
            ApplyColorScheme();
        }

        private void ApplyColorScheme()
        {
            this.BackColor = Color.FromArgb(44, 44, 44);
            lblTitle.ForeColor = Color.White;
            lblUsername.ForeColor = Color.White;
            lblPassword.ForeColor = Color.White;
            lblError.ForeColor = Color.FromArgb(255, 100, 100);
            btnLogin.BackColor = Color.FromArgb(155, 89, 182);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnRegister.BackColor = Color.FromArgb(155, 89, 182);
            btnRegister.ForeColor = Color.White;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.FlatAppearance.BorderSize = 0;
            btnExit.BackColor = Color.FromArgb(155, 89, 182);
            btnExit.ForeColor = Color.White;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.FlatAppearance.BorderSize = 0;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Заполните все поля";
                return;
            }

            string hash = PasswordHelper.HashPassword(password);

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT id, username, role FROM users WHERE username = @username AND password_hash = @hash";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@hash", hash);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Session.CurrentUserId = reader.GetInt32("id");
                        Session.CurrentUserName = reader.GetString("username");
                        Session.CurrentUserRole = reader.GetString("role");

                        MainForm mainForm = new MainForm();
                        mainForm.Show();
                        this.Hide();
                    }
                    else
                    {
                        lblError.Text = "Неверное имя пользователя или пароль";
                    }
                }
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            FormRegister registerForm = new FormRegister();
            registerForm.ShowDialog();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}