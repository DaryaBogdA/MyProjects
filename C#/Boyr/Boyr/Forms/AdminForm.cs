using Boyr.Controls;
using Boyr.UI;
using System;
using System.Windows.Forms;

namespace Boyr.Forms
{
    public partial class AdminForm : Form
    {
        private ProductsControl productsControl;
        private UsersControl usersControl;
        private SalesControl salesControl;

        public AdminForm()
        {
            InitializeComponent();
            UiTheme.Apply(this);
            InitializeControls();
            ShowProducts();
        }

        private void InitializeControls()
        {
            productsControl = new ProductsControl();
            usersControl = new UsersControl();
            salesControl = new SalesControl();

            productsControl.Dock = DockStyle.Fill;
            usersControl.Dock = DockStyle.Fill;
            salesControl.Dock = DockStyle.Fill;
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            ShowProducts();
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            ShowUsers();
        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            ShowSales();
        }

        private void ShowProducts()
        {
            panelRight.Controls.Clear();
            panelRight.Controls.Add(productsControl);
        }

        private void ShowUsers()
        {
            panelRight.Controls.Clear();
            panelRight.Controls.Add(usersControl);
        }

        private void ShowSales()
        {
            panelRight.Controls.Clear();
            panelRight.Controls.Add(salesControl);
        }
    }
}