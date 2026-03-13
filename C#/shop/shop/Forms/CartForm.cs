using System;
using System.Linq;
using System.Windows.Forms;
using shop.Entities;
using shop.Services;
using shop.UI;

namespace shop.Forms
{
    public partial class CartForm : Form
    {
        public CartForm()
        {
            InitializeComponent();
            UiTheme.Apply(this);
            RefreshCart();
        }

        private void RefreshCart()
        {
            dataGridView.Rows.Clear();
            foreach (var item in CartService.Items)
            {
                dataGridView.Rows.Add(
                    item.Product.Id,
                    item.Product.Name,
                    item.Product.Price,
                    item.Quantity,
                    item.TotalPrice
                );
            }
            lblTotal.Text = $"Итого: {CartService.TotalAmount:C}";
            btnCheckout.Enabled = CartService.Items.Any();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null) return;
            int productId = Convert.ToInt32(dataGridView.CurrentRow.Cells[0].Value);
            CartService.RemoveFromCart(productId);
            RefreshCart();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            CartService.Clear();
            RefreshCart();
        }

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            if (!AuthService.IsAuthenticated)
            {
                MessageBox.Show("Необходимо войти в систему.");
                return;
            }

            CheckoutForm checkout = new CheckoutForm();
            if (checkout.ShowDialog() == DialogResult.OK)
            {
                CartService.Clear();
                RefreshCart();
                MessageBox.Show("Заказ оформлен!");
            }
        }

        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                int productId = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells[0].Value);
                int newQty;
                if (int.TryParse(dataGridView.Rows[e.RowIndex].Cells[3].Value?.ToString(), out newQty) && newQty > 0)
                {
                    var item = CartService.Items.FirstOrDefault(i => i.Product.Id == productId);
                    if (item != null)
                    {
                        if (newQty > item.Product.Quantity)
                        {
                            MessageBox.Show($"Нельзя установить количество больше доступного ({item.Product.Quantity}).", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            RefreshCart();
                            return;
                        }
                        CartService.UpdateQuantity(productId, newQty);
                    }
                }
                else
                {
                    MessageBox.Show("Введите корректное количество.");
                }
                RefreshCart();
            }
        }
    }
}