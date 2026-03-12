using MySql.Data.MySqlClient;
using paseka.DataAccess;
using paseka.Helpers;
using paseka.Models;
using System;
using System.Data;
using System.Windows.Forms;

namespace paseka
{
    public partial class BookingForm : Form
    {
        private DataTable servicesTable;

        public BookingForm()
        {
            InitializeComponent();
            ThemeHelper.ApplyDarkTheme(this);
            this.StartPosition = FormStartPosition.CenterScreen;
            LoadServices();
            LoadFood();
            dtpDate.Value = DateTime.Today;
            dtpTime.Format = DateTimePickerFormat.Time;
            dtpTime.ShowUpDown = true;

            dgvFood.CellValueChanged += DgvFood_CellValueChanged;
            dgvFood.CurrentCellDirtyStateChanged += DgvFood_CurrentCellDirtyStateChanged;
        }

        private void DgvFood_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvFood.IsCurrentCellDirty)
            {
                dgvFood.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        private void DgvFood_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            UpdateTotalPrice();
        }

        private void LoadServices()
        {
            string query = "SELECT id, name, type, duration, price FROM services WHERE type='game' AND is_available=1";
            servicesTable = DatabaseHelper.ExecuteQuery(query);
            cmbService.DisplayMember = "name";
            cmbService.ValueMember = "id";
            cmbService.DataSource = servicesTable;
        }

        private void cmbService_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEndTime();
            UpdateTotalPrice();
        }

        private void dtpTime_ValueChanged(object sender, EventArgs e)
        {
            UpdateEndTime();
        }

        private void UpdateEndTime()
        {
            if (cmbService.SelectedItem == null) return;
            DataRowView row = cmbService.SelectedItem as DataRowView;
            int duration = Convert.ToInt32(row["duration"]);
            DateTime start = dtpDate.Value.Date + dtpTime.Value.TimeOfDay;
            DateTime end = start.AddMinutes(duration);
            lblEndTime.Text = "Окончание: " + end.ToShortTimeString();
        }
        private void UpdateTotalPrice()
        {
            decimal total = 0;

            if (cmbService.SelectedItem != null)
            {
                DataRowView row = cmbService.SelectedItem as DataRowView;
                total += Convert.ToDecimal(row["price"]);
            }

            foreach (DataGridViewRow r in dgvFood.Rows)
            {
                bool selected = Convert.ToBoolean(r.Cells["colSelect"].Value ?? false);
                if (selected)
                {
                    decimal price = Convert.ToDecimal(r.Cells["colPrice"].Value);
                    int quantity = Convert.ToInt32(r.Cells["colQuantity"].Value);
                    if (quantity <= 0) quantity = 1;
                    total += price * quantity;
                }
            }

            lblTotalPrice.Text = $"Итого: {total:F2} руб.";
        }

        private void btnBook_Click(object sender, EventArgs e)
        {
            if (cmbService.SelectedItem == null)
            {
                MessageBox.Show("Выберите услугу", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            int serviceId = (int)cmbService.SelectedValue;
            DateTime bookingDate = dtpDate.Value.Date;
            TimeSpan startTime = dtpTime.Value.TimeOfDay;
            DataRowView row = cmbService.SelectedItem as DataRowView;
            int duration = Convert.ToInt32(row["duration"]);
            TimeSpan endTime = startTime.Add(TimeSpan.FromMinutes(duration));

            string checkQuery = @"SELECT COUNT(*) FROM bookings 
                           WHERE service_id = @sid AND booking_date = @bdate 
                           AND start_time < @etime AND end_time > @stime AND is_active = 1";
                    MySqlParameter[] checkParams = {
                new MySqlParameter("@sid", serviceId),
                new MySqlParameter("@bdate", bookingDate),
                new MySqlParameter("@stime", startTime),
                new MySqlParameter("@etime", endTime)
            };
            DataTable dt = DatabaseHelper.ExecuteQuery(checkQuery, checkParams);
            if (Convert.ToInt32(dt.Rows[0][0]) > 0)
            {
                MessageBox.Show("Это время уже занято. Выберите другое.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string insertBookingQuery = @"INSERT INTO bookings 
                (user_id, service_id, booking_date, start_time, end_time, is_active, created_at) 
                VALUES (@uid, @sid, @bdate, @stime, @etime, 1, NOW());
                SELECT LAST_INSERT_ID();";

                    MySqlParameter[] insertParams = {
                new MySqlParameter("@uid", CurrentUser.Id),
                new MySqlParameter("@sid", serviceId),
                new MySqlParameter("@bdate", bookingDate),
                new MySqlParameter("@stime", startTime),
                new MySqlParameter("@etime", endTime)
            };

            int bookingId = 0;
            using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(insertBookingQuery, conn);
                cmd.Parameters.AddRange(insertParams);
                bookingId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            foreach (DataGridViewRow r in dgvFood.Rows)
            {
                bool selected = Convert.ToBoolean(r.Cells["colSelect"].Value);
                if (selected)
                {
                    int foodServiceId = (int)r.Tag;
                    string name = r.Cells["colName"].Value.ToString();
                    decimal price = Convert.ToDecimal(r.Cells["colPrice"].Value);
                    int quantity = Convert.ToInt32(r.Cells["colQuantity"].Value);
                    if (quantity <= 0) quantity = 1;

                    string insertItemQuery = @"INSERT INTO booking_items 
                (booking_id, service_id, quantity, price, is_active, created_at) 
                VALUES (@bid, @sid, @qty, @price, 1, NOW())";
                    MySqlParameter[] itemParams = {
                new MySqlParameter("@bid", bookingId),
                new MySqlParameter("@sid", foodServiceId),
                new MySqlParameter("@qty", quantity),
                new MySqlParameter("@price", price)
            };
                    DatabaseHelper.ExecuteNonQuery(insertItemQuery, itemParams);
                }
            }

            MessageBox.Show("Бронирование выполнено!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void LoadFood()
        {
            string query = "SELECT id, name, price FROM services WHERE type='food' AND is_available=1 ORDER BY name";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            dgvFood.Rows.Clear();
            foreach (DataRow row in dt.Rows)
            {
                int rowIndex = dgvFood.Rows.Add();
                DataGridViewRow dgvRow = dgvFood.Rows[rowIndex];
                dgvRow.Cells["colSelect"].Value = false;
                dgvRow.Cells["colName"].Value = row["name"].ToString();
                dgvRow.Cells["colPrice"].Value = Convert.ToDecimal(row["price"]).ToString("F2");
                dgvRow.Cells["colQuantity"].Value = 1;
                dgvRow.Tag = Convert.ToInt32(row["id"]);
            }
        }
    }
}