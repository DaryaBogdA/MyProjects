using Boyr.Controls;
using Boyr.DataAccess;
using Boyr.Services;
using Boyr.UI;
using System;
using System.Windows.Forms;

namespace Boyr.Forms
{
    public partial class MainForm : Form
    {
        private ProductRepository productRepo = new ProductRepository();

        public MainForm()
        {
            InitializeComponent();
            UiTheme.Apply(this);
            ConfigureAdminButton();
            LoadProducts();
            SetupShortcuts();
        }

        private void SetupShortcuts()
        {
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.K)
            {
                btnCatalog_Click(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                btnCart_Click(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.O)
            {
                btnOrders_Click(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.A && AuthService.Instance.IsAdmin)
            {
                btnAdmin_Click(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.Q)
            {
                btnExit_Click(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.F5)
            {
                LoadProducts();
                e.SuppressKeyPress = true;
            }
        }

        private void ConfigureAdminButton()
        {
            btnAdmin.Visible = AuthService.Instance.IsAdmin;
        }

        public void LoadProducts()
        {
            flowLayoutPanel.Controls.Clear();
            var products = productRepo.GetAll();
            foreach (var product in products)
            {
                var card = new ProductCardControl(product);
                flowLayoutPanel.Controls.Add(card);
            }
        }

        private void btnCatalog_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void btnCart_Click(object sender, EventArgs e)
        {
            CartForm cart = new CartForm();
            cart.ShowDialog();
            LoadProducts();
        }

        private void btnOrders_Click(object sender, EventArgs e)
        {
            if (!AuthService.Instance.IsAuthenticated)
            {
                MessageBox.Show("Необходимо войти в систему.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            OrderHistoryForm history = new OrderHistoryForm();
            history.ShowDialog();
        }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            if (!AuthService.Instance.IsAdmin)
            {
                MessageBox.Show("Доступ запрещён.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            AdminForm admin = new AdminForm();
            admin.ShowDialog();
            LoadProducts();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            AuthService.Instance.Logout();
            Application.Restart();
        }
    }
}