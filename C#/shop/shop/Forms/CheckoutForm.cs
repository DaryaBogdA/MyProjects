using MySql.Data.MySqlClient;
using shop.DataAccess;
using shop.Entities;
using shop.Services;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using shop.UI;

namespace shop.Forms
{
    public partial class CheckoutForm : Form
    {
        public string CustomerName { get; private set; }

        public CheckoutForm()
        {
            InitializeComponent();
            UiTheme.Apply(this);
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string customer = txtCustomerName.Text.Trim();
            if (string.IsNullOrEmpty(customer))
            {
                MessageBox.Show("Введите имя покупателя.");
                return;
            }

            foreach (var item in CartService.Items)
            {
                if (item.Quantity > item.Product.Quantity)
                {
                    MessageBox.Show($"Недостаточно товара \"{item.Product.Name}\" на складе. Доступно: {item.Product.Quantity}");
                    return;
                }
            }

            using (var connection = new MySqlConnection(DatabaseHelper.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string insertSale = "INSERT INTO sales (user_id, total_amount, customer_name) VALUES (@userId, @total, @customer); SELECT LAST_INSERT_ID();";
                        MySqlCommand cmdSale = new MySqlCommand(insertSale, connection, transaction);
                        cmdSale.Parameters.AddWithValue("@userId", AuthService.CurrentUser.Id);
                        cmdSale.Parameters.AddWithValue("@total", CartService.TotalAmount);
                        cmdSale.Parameters.AddWithValue("@customer", customer);
                        int saleId = Convert.ToInt32(cmdSale.ExecuteScalar());

                        foreach (var item in CartService.Items)
                        {
                            string insertItem = "INSERT INTO sale_items (sale_id, product_id, quantity, price_at_moment) VALUES (@saleId, @productId, @qty, @price)";
                            MySqlCommand cmdItem = new MySqlCommand(insertItem, connection, transaction);
                            cmdItem.Parameters.AddWithValue("@saleId", saleId);
                            cmdItem.Parameters.AddWithValue("@productId", item.Product.Id);
                            cmdItem.Parameters.AddWithValue("@qty", item.Quantity);
                            cmdItem.Parameters.AddWithValue("@price", item.Product.Price);
                            cmdItem.ExecuteNonQuery();

                            string updateProduct = "UPDATE products SET quantity = quantity - @qty WHERE id = @id";
                            MySqlCommand cmdUpdate = new MySqlCommand(updateProduct, connection, transaction);
                            cmdUpdate.Parameters.AddWithValue("@qty", item.Quantity);
                            cmdUpdate.Parameters.AddWithValue("@id", item.Product.Id);
                            cmdUpdate.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        CustomerName = customer;
                        DialogResult = DialogResult.OK;

                        var saleItems = CartService.Items.Select(item => new SaleItem
                        {
                            Product = item.Product,
                            Quantity = item.Quantity,
                            PriceAtMoment = item.Product.Price
                        }).ToList();

                        var sale = new Sale
                        {
                            Id = saleId,
                            SaleDate = DateTime.Now,
                            TotalAmount = CartService.TotalAmount,
                            CustomerName = customer
                        };

                        ReceiptService.SaveReceiptToFile(sale, saleItems);

                        Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Ошибка при оформлении заказа: " + ex.Message);
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}