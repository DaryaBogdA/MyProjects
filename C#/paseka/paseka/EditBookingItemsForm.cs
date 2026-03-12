using MySql.Data.MySqlClient;
using paseka.DataAccess;
using paseka.Helpers;
using paseka.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace paseka
{
    public partial class EditBookingItemsForm : Form
    {
        private int bookingId;
        private DataTable currentItems;
        private List<int> itemsToDelete = new List<int>();

        public EditBookingItemsForm(int bookingId)
        {
            InitializeComponent();
            ThemeHelper.ApplyDarkTheme(this);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.bookingId = bookingId;
            LoadItems();
        }

        private void LoadItems()
        {
            string query = @"
            SELECT bi.id, s.name AS service_name, s.price, bi.quantity
            FROM booking_items bi
            JOIN services s ON bi.service_id = s.id
            WHERE bi.booking_id = @bid AND bi.is_active = 1";
            MySqlParameter[] parameters = { new MySqlParameter("@bid", bookingId) };
            currentItems = DatabaseHelper.ExecuteQuery(query, parameters);
            dgvItems.DataSource = currentItems;

            dgvItems.Columns["id"].Visible = false;
            dgvItems.Columns["service_name"].HeaderText = "Название";
            dgvItems.Columns["price"].HeaderText = "Цена";
            dgvItems.Columns["quantity"].HeaderText = "Количество";

            dgvItems.Columns["service_name"].ReadOnly = true;
            dgvItems.Columns["price"].ReadOnly = true;

            if (!dgvItems.Columns.Contains("delete"))
            {
                DataGridViewCheckBoxColumn colDelete = new DataGridViewCheckBoxColumn();
                colDelete.HeaderText = "Удалить";
                colDelete.Name = "delete";
                colDelete.Width = 50;
                dgvItems.Columns.Add(colDelete);
            }
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            SelectFoodForm selectForm = new SelectFoodForm(bookingId);
            if (selectForm.ShowDialog() == DialogResult.OK)
            {
                List<SelectedFoodItem> selected = selectForm.GetSelectedItems();
                foreach (var item in selected)
                {
                    DataRow row = currentItems.NewRow();
                    row["id"] = 0;
                    row["service_name"] = item.Name;
                    row["price"] = item.Price;
                    row["quantity"] = item.Quantity;
                    currentItems.Rows.Add(row);
                }
                dgvItems.DataSource = currentItems;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            List<BookingItem> newItems = new List<BookingItem>();
            List<BookingItem> updatedItems = new List<BookingItem>();
            List<int> deletedIds = new List<int>();

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                if (row.IsNewRow) continue;
                bool toDelete = Convert.ToBoolean(row.Cells["delete"].Value ?? false);
                if (toDelete) continue;
                string name = row.Cells["service_name"].Value.ToString();
                int quantity = Convert.ToInt32(row.Cells["quantity"].Value);
                int available = GetAvailableQuantity(name);
                if (quantity > available)
                {
                    MessageBox.Show($"Для товара '{name}' доступно только {available} ед.\nВы запросили {quantity}.",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                if (row.IsNewRow) continue;

                bool toDelete = Convert.ToBoolean(row.Cells["delete"].Value ?? false);
                int id = Convert.ToInt32(row.Cells["id"].Value);
                string name = row.Cells["service_name"].Value.ToString();
                decimal price = Convert.ToDecimal(row.Cells["price"].Value);
                int quantity = Convert.ToInt32(row.Cells["quantity"].Value);

                if (toDelete)
                {
                    if (id > 0)
                        deletedIds.Add(id);
                }
                else
                {
                    if (id == 0)
                    {
                        int serviceId = GetServiceIdByName(name);
                        if (serviceId > 0)
                        {
                            newItems.Add(new BookingItem
                            {
                                BookingId = bookingId,
                                ServiceId = serviceId,
                                Quantity = quantity,
                                Price = price,
                                IsActive = true
                            });
                        }
                    }
                    else
                    {
                        updatedItems.Add(new BookingItem { Id = id, Quantity = quantity });
                    }
                }
            }

            using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.connectionString))
            {
                conn.Open();
                MySqlTransaction trans = conn.BeginTransaction();

                try
                {
                    foreach (var item in newItems)
                    {
                        string insert = @"INSERT INTO booking_items (booking_id, service_id, quantity, price, is_active, created_at)
                                  VALUES (@bid, @sid, @qty, @price, 1, NOW())";
                        MySqlCommand cmd = new MySqlCommand(insert, conn, trans);
                        cmd.Parameters.AddWithValue("@bid", item.BookingId);
                        cmd.Parameters.AddWithValue("@sid", item.ServiceId);
                        cmd.Parameters.AddWithValue("@qty", item.Quantity);
                        cmd.Parameters.AddWithValue("@price", item.Price);
                        cmd.ExecuteNonQuery();
                    }

                    foreach (var item in updatedItems)
                    {
                        string update = "UPDATE booking_items SET quantity = @qty WHERE id = @id";
                        MySqlCommand cmd = new MySqlCommand(update, conn, trans);
                        cmd.Parameters.AddWithValue("@qty", item.Quantity);
                        cmd.Parameters.AddWithValue("@id", item.Id);
                        cmd.ExecuteNonQuery();
                    }

                    foreach (int id in deletedIds)
                    {
                        string delete = "UPDATE booking_items SET is_active = 0 WHERE id = @id";
                        MySqlCommand cmd = new MySqlCommand(delete, conn, trans);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                    MessageBox.Show("Состав заказа обновлён.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private int GetAvailableQuantity(string serviceName)
        {
            string query = "SELECT quantity FROM services WHERE name = @name AND type='food'";
            DataTable dt = DatabaseHelper.ExecuteQuery(query, new MySqlParameter("@name", serviceName));
            if (dt.Rows.Count > 0)
                return Convert.ToInt32(dt.Rows[0][0]);
            return 0;
        }

        private int GetServiceIdByName(string name)
        {
            string query = "SELECT id FROM services WHERE name = @name AND type='food'";
            DataTable dt = DatabaseHelper.ExecuteQuery(query, new MySqlParameter("@name", name));
            if (dt.Rows.Count > 0)
                return Convert.ToInt32(dt.Rows[0][0]);
            return 0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}