using shop.Entities;
using shop.Services;
using shop.UI;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace shop.Controls
{
    public partial class ProductCardControl : UserControl
    {
        private Product _product;
        public event EventHandler AddToCartClicked;

        public ProductCardControl(Product product)
        {
            InitializeComponent();
            UiTheme.ApplyToControlTree(this);
            _product = product;
            LoadProduct();
        }

        private void LoadProduct()
        {
            lblName.Text = _product.Name;
            lblPrice.Text = $"{_product.Price:C}";
            lblSize.Text = $"Размер: {_product.Size}";
            lblColor.Text = $"Цвет: {_product.Color}";
            lblStock.Text = $"В наличии: {_product.Quantity}";

            if (!string.IsNullOrEmpty(_product.ImageUrl))
            {
                try
                {
                    pictureBox.LoadAsync(_product.ImageUrl);
                }
                catch
                {
                    pictureBox.Image = null;
                }
            }
            else
            {
                pictureBox.Image = null;
            }
        }

        private void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (_product.Quantity <= 0)
            {
                MessageBox.Show("Товара нет в наличии.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (CartService.AddToCart(_product, 1))
            {
                MessageBox.Show($"Товар \"{_product.Name}\" добавлен в корзину.", "Корзина", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Нельзя добавить больше, чем есть в наличии. Доступно: {_product.Quantity}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}