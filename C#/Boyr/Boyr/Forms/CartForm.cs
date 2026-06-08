using Boyr.Services;
using Boyr.UI;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Boyr.Forms
{
    public partial class CartForm : Form
    {
        public CartForm()
        {
            InitializeComponent();
            UiTheme.Apply(this);
            RefreshCart();
            SetupShortcuts();
        }

        private void SetupShortcuts()
        {
            this.KeyPreview = true;
            this.KeyDown += CartForm_KeyDown;
        }

        private void CartForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && !e.Control)
            {
                btnRemove_Click(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.Delete)
            {
                btnClear_Click(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.Enter)
            {
                btnCheckout_Click(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
                e.SuppressKeyPress = true;
            }
        }

        private void RefreshCart()
        {
            dataGridView.Rows.Clear();
            foreach (var item in CartService.Instance.Items)
            {
                dataGridView.Rows.Add(
                    item.Product.Id,
                    item.Product.Name,
                    $"{item.Product.Metal} {item.Product.Purity}",
                    item.Product.Weight,
                    item.Product.Gemstone,
                    item.Product.Price,
                    item.Quantity,
                    item.TotalPrice
                );
            }
            lblTotal.Text = $"Итого: {CartService.Instance.TotalAmount:C}";
            btnCheckout.Enabled = CartService.Instance.Items.Any();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null) return;
            int productId = Convert.ToInt32(dataGridView.CurrentRow.Cells[0].Value);
            CartService.Instance.RemoveFromCart(productId);
            RefreshCart();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            CartService.Instance.Clear();
            RefreshCart();
        }

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            if (!AuthService.Instance.IsAuthenticated)
            {
                MessageBox.Show("Необходимо войти в систему.");
                return;
            }

            CheckoutForm checkout = new CheckoutForm();
            if (checkout.ShowDialog() == DialogResult.OK)
            {
                CartService.Instance.Clear();
                RefreshCart();
                MessageBox.Show("Заказ оформлен!");
            }
        }

        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 6)
            {
                int productId = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells[0].Value);
                int newQty;
                if (int.TryParse(dataGridView.Rows[e.RowIndex].Cells[6].Value?.ToString(), out newQty) && newQty > 0)
                {
                    var item = CartService.Instance.Items.FirstOrDefault(i => i.Product.Id == productId);
                    if (item != null)
                    {
                        if (newQty > item.Product.Quantity)
                        {
                            MessageBox.Show($"Нельзя установить количество больше доступного ({item.Product.Quantity}).", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            RefreshCart();
                            return;
                        }
                        CartService.Instance.UpdateQuantity(productId, newQty);
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