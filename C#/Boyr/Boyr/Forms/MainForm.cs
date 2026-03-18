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
        }

        private void ConfigureAdminButton()
        {
            btnAdmin.Visible = AuthService.IsAdmin;
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
            // Уже на каталоге, но можно обновить список
            LoadProducts();
        }

        private void btnCart_Click(object sender, EventArgs e)
        {
            CartForm cart = new CartForm();
            cart.ShowDialog();
            LoadProducts(); // обновить количество после возможных покупок
        }

        private void btnOrders_Click(object sender, EventArgs e)
        {
            if (!AuthService.IsAuthenticated)
            {
                MessageBox.Show("Необходимо войти в систему.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            OrderHistoryForm history = new OrderHistoryForm();
            history.ShowDialog();
        }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            if (!AuthService.IsAdmin)
            {
                MessageBox.Show("Доступ запрещён.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            AdminForm admin = new AdminForm();
            admin.ShowDialog();
            LoadProducts(); // возможно товары изменились
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            AuthService.Logout();
            Application.Restart();
        }
    }
}