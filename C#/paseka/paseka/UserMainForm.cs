using paseka.Helpers;
using System;
using System.Windows.Forms;

namespace paseka
{
    public partial class UserMainForm : Form
    {
        public UserMainForm()
        {
            InitializeComponent();
            ThemeHelper.ApplyDarkTheme(this);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnViewServices_Click(object sender, EventArgs e)
        {
            ServicesForm form = new ServicesForm(isAdmin: false);
            form.ShowDialog();
        }

        private void btnMyBookings_Click(object sender, EventArgs e)
        {
            MyBookingsForm form = new MyBookingsForm();
            form.ShowDialog();
        }

        private void btnBookService_Click(object sender, EventArgs e)
        {
            BookingForm form = new BookingForm();
            form.ShowDialog();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
            foreach (Form f in Application.OpenForms)
                if (f is LoginForm)
                {
                    f.Show();
                    break;
                }
        }
    }
}