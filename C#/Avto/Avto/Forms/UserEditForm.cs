using System;
using System.Windows.Forms;
using Avto.Data;
using Avto.Models;

namespace Avto.Forms
{
    public partial class UserEditForm : Form
    {
        private User _user;
        private bool _isNew;

        public UserEditForm()
        {
            InitializeComponent();
            _isNew = true;
            _user = new User { IsActive = true };
        }

        public UserEditForm(User user)
        {
            InitializeComponent();
            _isNew = false;
            _user = user;
        }

        private void UserEditForm_Load(object sender, EventArgs e)
        {
            if (!_isNew)
            {
                txtUsername.Text = _user.Username;
                txtFullName.Text = _user.FullName;
                cmbRole.SelectedItem = _user.Role.ToString();
                chkIsActive.Checked = _user.IsActive;
                txtPassword.Text = "";
            }
            else
            {
                cmbRole.SelectedIndex = 1;
                chkIsActive.Checked = true;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Введите логин.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_isNew && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Введите пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmbRole.SelectedItem == null)
            {
                MessageBox.Show("Выберите роль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _user.Username = txtUsername.Text.Trim();
            _user.FullName = txtFullName.Text.Trim();
            _user.Role = (UserRole)Enum.Parse(typeof(UserRole), cmbRole.SelectedItem.ToString());
            _user.IsActive = chkIsActive.Checked;

            bool result;
            if (_isNew)
            {
                string hash = AuthHelper.ComputeSha256Hash(txtPassword.Text);
                _user.PasswordHash = hash;
                result = DatabaseHelper.AddUser(_user);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    _user.PasswordHash = AuthHelper.ComputeSha256Hash(txtPassword.Text);
                }
                result = DatabaseHelper.UpdateUser(_user);
            }

            if (result)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Не удалось сохранить пользователя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}