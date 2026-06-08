using System;
using System.Windows.Forms;
using SanatoriumStaro.Models;

namespace SanatoriumStaro
{
    public partial class MainForm : Form
    {
        private User currentUser;

        public MainForm(User user)
        {
            currentUser = user;
            InitializeComponent();
            CustomizeForRole();

            // Настройка горячих клавиш
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
        }

        private void CustomizeForRole()
        {
            lblWelcome.Text = $"Добро пожаловать, {currentUser.FullName}!";
            if (currentUser.Role == "admin")
            {
                btnManageServices.Visible = true;
                btnAllAppointments.Visible = true;
            }
            else
            {
                btnManageServices.Visible = false;
                btnAllAppointments.Visible = false;
            }
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            ProfileForm profileForm = new ProfileForm(currentUser);
            profileForm.ProfileUpdated += ProfileForm_ProfileUpdated;
            profileForm.ShowDialog();
        }

        private void ProfileForm_ProfileUpdated(object sender, EventArgs e)
        {
            // Обновляем данные после изменения профиля
            currentUser = DatabaseHelper.GetUserById(currentUser.Id);
            lblWelcome.Text = $"Добро пожаловать, {currentUser.FullName}!";
        }

        private void btnServices_Click(object sender, EventArgs e)
        {
            ServicesForm servicesForm = new ServicesForm(currentUser);
            servicesForm.ShowDialog();
        }

        private void btnMyAppointments_Click(object sender, EventArgs e)
        {
            AppointmentsForm appointmentsForm = new AppointmentsForm(currentUser, false);
            appointmentsForm.ShowDialog();
        }

        private void btnManageServices_Click(object sender, EventArgs e)
        {
            ManageServicesForm manageForm = new ManageServicesForm();
            manageForm.ShowDialog();
        }

        private void btnAllAppointments_Click(object sender, EventArgs e)
        {
            AppointmentsForm appointmentsForm = new AppointmentsForm(currentUser, true);
            appointmentsForm.ShowDialog();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.OpenForms["LoginForm"]?.Show();
        }
        private void btnStatistics_Click(object sender, EventArgs e)
        {
            StatisticsForm statisticsForm = new StatisticsForm(currentUser);
            statisticsForm.ShowDialog();
        }
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                btnServices.PerformClick();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.M)
            {
                btnMyAppointments.PerformClick();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.P) 
            {
                btnProfile.PerformClick();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.Q) 
            {
                btnLogout.PerformClick();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.U && btnManageServices.Visible) 
            {
                btnManageServices.PerformClick();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.A && btnAllAppointments.Visible)
            {
                btnAllAppointments.PerformClick();
                e.SuppressKeyPress = true;
            }
        }
    }
}