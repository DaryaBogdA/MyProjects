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
            profileForm.ShowDialog();
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
    }
}