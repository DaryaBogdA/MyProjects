using System;
using System.Windows.Forms;
using shop.Services;
using shop.UI;

namespace shop.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            UiTheme.Apply(this);
            SetupShortcuts();
        }

        private void SetupShortcuts()
        {
            this.KeyPreview = true;
            this.KeyDown += LoginForm_KeyDown;
        }

        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.R)
            {
                // Ctrl+R - открыть регистрацию
                btnRegister_Click(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                // Enter - выполнить вход
                btnLogin_Click(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                // Esc - выход из приложения
                btnExit_Click(null, null);
                e.SuppressKeyPress = true;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль.");
                return;
            }

            if (AuthService.Login(login, password))
            {
                MessageBox.Show($"Добро пожаловать, {AuthService.CurrentUser.Login}!");
                MainForm main = new MainForm();
                main.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.");
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            RegisterForm reg = new RegisterForm();
            reg.ShowDialog();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}