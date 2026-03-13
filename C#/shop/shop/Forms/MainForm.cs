using System;
using System.Drawing;
using System.Windows.Forms;
using shop.Controls;
using shop.DataAccess;
using shop.Entities;
using shop.Services;
using shop.UI;

namespace shop.Forms
{
    public partial class MainForm : Form
    {
        private ProductRepository productRepo = new ProductRepository();

        public MainForm()
        {
            InitializeComponent();
            UiTheme.Apply(this);
            ConfigureMenu();
            LoadProducts();
        }

        private void ConfigureMenu()
        {
            menuStrip.BackColor = Color.FromArgb(64, 64, 64);
            menuStrip.ForeColor = Color.White;
            foreach (ToolStripMenuItem item in menuStrip.Items)
            {
                item.ForeColor = Color.White;
            }

            if (AuthService.IsAdmin)
            {
                var adminItem = new ToolStripMenuItem("Администрирование");
                adminItem.Click += (s, e) => OpenAdminForm();
                menuStrip.Items.Add(adminItem);
            }
        }

        private void OpenAdminForm()
        {
            AdminForm admin = new AdminForm();
            admin.ShowDialog();
        }

        public void LoadProducts()
        {
            flowLayoutPanel.Controls.Clear();
            var products = productRepo.GetAll();
            foreach (var product in products)
            {
                var card = new ProductCardControl(product);
                card.AddToCartClicked += (s, e) =>
                {
                    if (product.Quantity <= 0)
                    {
                        MessageBox.Show("Товара нет в наличии.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (CartService.AddToCart(product, 1))
                    {
                        MessageBox.Show($"Товар \"{product.Name}\" добавлен в корзину.", "Корзина", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Нельзя добавить больше, чем есть в наличии. Доступно: {product.Quantity}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }; flowLayoutPanel.Controls.Add(card);
            }
        }

        private void каталогToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void корзинаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CartForm cart = new CartForm();
            cart.ShowDialog();
            LoadProducts();
        }

        private void моиЗаказыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrderHistoryForm history = new OrderHistoryForm();
            history.ShowDialog();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AuthService.Logout();
            Application.Restart();
        }
    }
}