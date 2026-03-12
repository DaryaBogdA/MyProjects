using MySql.Data.MySqlClient;
using paseka.DataAccess;
using paseka.Helpers;
using paseka.Models;
using System;
using System.Data;
using System.Windows.Forms;
using System.Text;

namespace paseka
{
    public partial class MyBookingsForm : Form
    {
        public MyBookingsForm()
        {
            InitializeComponent();
            ThemeHelper.ApplyDarkTheme(this);
            this.StartPosition = FormStartPosition.CenterScreen;
            LoadBookings();
        }

        private void LoadBookings()
        {
            string query = @"
                SELECT 
                    b.id, 
                    s.name AS service_name, 
                    b.booking_date, 
                    b.start_time, 
                    b.end_time, 
                    b.is_active,
                    (s.price + IFNULL((SELECT SUM(bi.quantity * bi.price) FROM booking_items bi WHERE bi.booking_id = b.id AND bi.is_active = 1), 0)) AS total_price
                FROM bookings b
                JOIN services s ON b.service_id = s.id
                WHERE b.user_id = @uid AND b.is_active = 1
                ORDER BY b.booking_date DESC, b.start_time DESC";

            MySqlParameter[] parameters = { new MySqlParameter("@uid", CurrentUser.Id) };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            dgvBookings.DataSource = dt;

            dgvBookings.Columns["id"].HeaderText = "ID";
            dgvBookings.Columns["service_name"].HeaderText = "Услуга";
            dgvBookings.Columns["booking_date"].HeaderText = "Дата";
            dgvBookings.Columns["start_time"].HeaderText = "Начало";
            dgvBookings.Columns["end_time"].HeaderText = "Окончание";
            dgvBookings.Columns["is_active"].HeaderText = "Активно";
            dgvBookings.Columns["total_price"].HeaderText = "Стоимость";
            dgvBookings.Columns["total_price"].DefaultCellStyle.Format = "F2";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (dgvBookings.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(dgvBookings.SelectedRows[0].Cells["id"].Value);
            bool isActive = Convert.ToBoolean(dgvBookings.SelectedRows[0].Cells["is_active"].Value);
            if (!isActive)
            {
                MessageBox.Show("Это бронирование уже отменено.", "Инфо", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Отменить выбранное бронирование?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string query = "UPDATE bookings SET is_active = 0 WHERE id = @id";
                DatabaseHelper.ExecuteNonQuery(query, new MySqlParameter("@id", id));

                string itemQuery = "UPDATE booking_items SET is_active = 0 WHERE booking_id = @id";
                DatabaseHelper.ExecuteNonQuery(itemQuery, new MySqlParameter("@id", id));

                LoadBookings();
            }
        }
        private void btnSaveReceipt_Click(object sender, EventArgs e)
        {
            if (dgvBookings.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите бронирование для сохранения чека.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int bookingId = Convert.ToInt32(dgvBookings.SelectedRows[0].Cells["id"].Value);
            SaveReceipt(bookingId);
        }

        private void SaveReceipt(int bookingId)
        {
            string bookingQuery = @"
                SELECT b.booking_date, b.start_time, b.end_time, s.name AS service_name, s.price AS service_price
                FROM bookings b
                JOIN services s ON b.service_id = s.id
                WHERE b.id = @bid";
            MySqlParameter[] bookingParams = { new MySqlParameter("@bid", bookingId) };
            DataTable bookingDt = DatabaseHelper.ExecuteQuery(bookingQuery, bookingParams);
            if (bookingDt.Rows.Count == 0) return;
            DataRow bookingRow = bookingDt.Rows[0];

            string itemsQuery = @"
                SELECT s.name, bi.quantity, bi.price, (bi.quantity * bi.price) AS item_total
                FROM booking_items bi
                JOIN services s ON bi.service_id = s.id
                WHERE bi.booking_id = @bid AND bi.is_active = 1";
            MySqlParameter[] itemsParams = { new MySqlParameter("@bid", bookingId) };
            DataTable itemsDt = DatabaseHelper.ExecuteQuery(itemsQuery, itemsParams);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ЧЕК № " + bookingId);
            sb.AppendLine("Дата брони: " + Convert.ToDateTime(bookingRow["booking_date"]).ToShortDateString());
            sb.AppendLine("Время: " + bookingRow["start_time"] + " - " + bookingRow["end_time"]);
            sb.AppendLine("Услуга: " + bookingRow["service_name"]);
            sb.AppendLine("Стоимость услуги: " + Convert.ToDecimal(bookingRow["service_price"]).ToString("F2") + " руб.");
            sb.AppendLine("--- Дополнительные позиции ---");
            decimal total = Convert.ToDecimal(bookingRow["service_price"]);
            if (itemsDt.Rows.Count == 0)
            {
                sb.AppendLine("Нет дополнительных позиций");
            }
            else
            {
                foreach (DataRow row in itemsDt.Rows)
                {
                    string name = row["name"].ToString();
                    int qty = Convert.ToInt32(row["quantity"]);
                    decimal price = Convert.ToDecimal(row["price"]);
                    decimal itemTotal = Convert.ToDecimal(row["item_total"]);
                    sb.AppendLine($"{name} x{qty} = {itemTotal:F2} руб. (по {price:F2} за шт.)");
                    total += itemTotal;
                }
            }
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"ИТОГО: {total:F2} руб.");
            sb.AppendLine("Спасибо за посещение!");

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.FileName = $"receipt_{bookingId}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllText(saveFileDialog.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("Чек сохранён.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnEditItems_Click(object sender, EventArgs e)
        {
            if (dgvBookings.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите бронирование для редактирования.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int bookingId = Convert.ToInt32(dgvBookings.SelectedRows[0].Cells["id"].Value);
            EditBookingItemsForm form = new EditBookingItemsForm(bookingId);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadBookings();
            }
        }
    }
}