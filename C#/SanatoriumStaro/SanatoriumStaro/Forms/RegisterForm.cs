using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SanatoriumStaro
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text;
            string confirm = txtConfirmPassword.Text;
            string fullName = txtFullName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullName))
            {
                MessageBox.Show("Заполните все обязательные поля (*).");
                return;
            }

            if (password != confirm)
            {
                MessageBox.Show("Пароли не совпадают.");
                return;
            }

            if (password.Length < 8)
            {
                MessageBox.Show("Пароль должен содержать не менее 8 символов.");
                return;
            }

            if (!string.IsNullOrEmpty(email) && !IsValidEmail(email))
            {
                MessageBox.Show("Введите корректный email адрес");
                return;
            }

            if (!string.IsNullOrEmpty(phone) && !IsValidBelarusPhone(phone))
            {
                MessageBox.Show("Введите корректный номер телефона в формате +37529XXXXXXX");
                return;
            }

            bool result = DatabaseHelper.RegisterUser(login, password, fullName, phone, email, "user");
            if (result)
            {
                MessageBox.Show("Регистрация успешна! Теперь вы можете войти.");
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка регистрации. Возможно, логин уже занят.");
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidBelarusPhone(string phone)
        {
            string cleaned = phone.Replace(" ", "").Replace("-", "");
            return Regex.IsMatch(cleaned, @"^\+375(29|33|44|25)\d{7}$");
        }
    }
}