using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using TattooYou.Database;
using TattooYou.Helpers;

namespace TattooYou.Forms
{
    public partial class FormMyAppointments : Form
    {
        public FormMyAppointments()
        {
            InitializeComponent();
            ApplyColorScheme();
            LoadAppointments();
        }

        private void ApplyColorScheme()
        {
            this.BackColor = Color.White;
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Label lbl)
                    lbl.ForeColor = Color.Black;
                if (ctrl is Button btn)
                {
                    btn.BackColor = Color.FromArgb(155, 89, 182);
                    btn.ForeColor = Color.White;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                }
            }
            dgvAppointments.BackgroundColor = Color.White;
            dgvAppointments.DefaultCellStyle.BackColor = Color.White;
            dgvAppointments.DefaultCellStyle.ForeColor = Color.Black;
            dgvAppointments.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dgvAppointments.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvAppointments.EnableHeadersVisualStyles = false;
        }

        private void LoadAppointments()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT a.id, a.appointment_date AS Дата, a.appointment_time AS Время,
                           s.name AS Услуга, st.name AS Стиль, m.full_name AS Мастер,
                           a.size AS Размер, a.status AS Статус
                    FROM appointments a
                    JOIN services s ON a.service_id = s.id
                    LEFT JOIN styles st ON a.style_id = st.id
                    LEFT JOIN masters m ON a.master_id = m.id
                    WHERE a.user_id = @userId
                    ORDER BY a.appointment_date DESC, a.appointment_time DESC";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", Session.CurrentUserId);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvAppointments.DataSource = dt;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите запись для отмены");
                return;
            }
            int id = Convert.ToInt32(dgvAppointments.SelectedRows[0].Cells["id"].Value);
            DialogResult dr = MessageBox.Show("Отменить запись?", "Подтверждение", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string update = "UPDATE appointments SET status = 'cancelled' WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(update, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                LoadAppointments();
            }
        }
    }
}