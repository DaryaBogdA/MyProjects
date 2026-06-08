using System;
using System.Windows.Forms;
using SanatoriumStaro.Models;

namespace SanatoriumStaro
{
    // Объявление делегата для события обновления профиля
    public delegate void ProfileUpdatedEventHandler(object sender, EventArgs e);

    public partial class ProfileForm : Form
    {
        private User currentUser;

        public event ProfileUpdatedEventHandler ProfileUpdated;

        public ProfileForm(User user)
        {
            currentUser = user;
            InitializeComponent();
            LoadData();

        }

        private void LoadData()
        {
            txtFullName.Text = currentUser.FullName;
            txtPhone.Text = currentUser.Phone;
            txtEmail.Text = currentUser.Email;
            lblLoginValue.Text = currentUser.Login;
            lblRoleValue.Text = currentUser.Role == "admin" ? "Администратор" : "Пользователь";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            currentUser.FullName = txtFullName.Text.Trim();
            currentUser.Phone = txtPhone.Text.Trim();
            currentUser.Email = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(currentUser.FullName))
            {
                MessageBox.Show("ФИО не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DatabaseHelper.UpdateUser(currentUser))
            {
                MessageBox.Show("Данные обновлены.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ProfileUpdated?.Invoke(this, EventArgs.Empty);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка при обновлении.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}