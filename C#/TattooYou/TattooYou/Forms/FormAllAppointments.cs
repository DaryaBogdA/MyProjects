using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using TattooYou.Database;

namespace TattooYou.Forms
{
    public partial class FormAllAppointments : Form
    {
        public FormAllAppointments()
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
                    SELECT a.id, u.username AS Клиент, a.appointment_date AS Дата, a.appointment_time AS Время,
                           s.name AS Услуга, s.price AS Цена, st.name AS Стиль, m.full_name AS Мастер,
                           a.size AS Размер, a.status AS Статус
                    FROM appointments a
                    JOIN users u ON a.user_id = u.id
                    JOIN services s ON a.service_id = s.id
                    LEFT JOIN styles st ON a.style_id = st.id
                    LEFT JOIN masters m ON a.master_id = m.id
                    ORDER BY a.appointment_date DESC, a.appointment_time DESC";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvAppointments.DataSource = dt;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadAppointments();
        }
    }
}