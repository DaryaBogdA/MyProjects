using paseka.Helpers;
using System;
using System.Windows.Forms;

namespace paseka
{
    public partial class AdminMainForm : Form
    {
        public AdminMainForm()
        {
            InitializeComponent();
            ThemeHelper.ApplyDarkTheme(this);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnServices_Click(object sender, EventArgs e)
        {
            ServicesForm form = new ServicesForm(isAdmin: true);
            form.ShowDialog();
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            UsersForm form = new UsersForm();
            form.ShowDialog();
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            InventoryForm form = new InventoryForm();
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