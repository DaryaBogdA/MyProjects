using System;
using System.Windows.Forms;
using shop.DataAccess;
using shop.Entities;
using shop.UI;
using shop.Utils;

namespace shop.Forms
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
            UiTheme.Apply(this);
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text;
            string confirm = txtConfirm.Text;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля.");
                return;
            }

            if (password != confirm)
            {
                MessageBox.Show("Пароли не совпадают.");
                return;
            }

            var repo = new UserRepository();
            if (repo.GetByLogin(login) != null)
            {
                MessageBox.Show("Пользователь с таким логином уже существует.");
                return;
            }

            var user = new User
            {
                Login = login,
                Password = PasswordHasher.Hash(password),
                Role = UserRole.user
            };

            try
            {
                repo.Add(user);
                MessageBox.Show("Регистрация прошла успешно! Теперь вы можете войти.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при регистрации: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}