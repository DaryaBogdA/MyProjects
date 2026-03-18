using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using TattooYou.Database;
using TattooYou.Helpers;
using TattooYou.Models;

namespace TattooYou.Forms
{
    public partial class FormMasters : Form
    {
        public FormMasters()
        {
            InitializeComponent();
            ApplyColorScheme();
            LoadMasters();
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
            dgvMasters.BackgroundColor = Color.White;
            dgvMasters.DefaultCellStyle.BackColor = Color.White;
            dgvMasters.DefaultCellStyle.ForeColor = Color.Black;
            dgvMasters.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dgvMasters.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvMasters.EnableHeadersVisualStyles = false;
        }

        private void LoadMasters()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT m.id, m.full_name, m.bio, m.photo_url, m.is_active,
                           GROUP_CONCAT(s.name SEPARATOR ', ') AS styles
                    FROM masters m
                    LEFT JOIN master_styles ms ON m.id = ms.master_id
                    LEFT JOIN styles s ON ms.style_id = s.id
                    GROUP BY m.id
                    ORDER BY m.full_name";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvMasters.DataSource = dt;

                if (dgvMasters.Columns.Count > 0)
                {
                    dgvMasters.Columns["id"].HeaderText = "ID";
                    dgvMasters.Columns["full_name"].HeaderText = "ФИО";
                    dgvMasters.Columns["bio"].HeaderText = "Биография";
                    dgvMasters.Columns["photo_url"].HeaderText = "Фото";
                    dgvMasters.Columns["is_active"].HeaderText = "Активен";
                    dgvMasters.Columns["styles"].HeaderText = "Стили";
                }
            }
        }

    private void btnExport_Click(object sender, EventArgs e)
    {
        if (dgvMasters.DataSource == null)
        {
            MessageBox.Show("Нет данных для экспорта.");
            return;
        }

        SaveFileDialog sfd = new SaveFileDialog();
        sfd.Filter = "CSV файлы (*.csv)|*.csv|Текстовые файлы (*.txt)|*.txt";
        sfd.DefaultExt = "csv";
        sfd.FileName = $"Masters_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

        if (sfd.ShowDialog() == DialogResult.OK)
        {
            DataTable dt = (DataTable)dgvMasters.DataSource;
            ExportHelper.DataTableToCsv(dt, sfd.FileName);
            MessageBox.Show("Мастера сохранены.", "Экспорт", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
    private void btnAdd_Click(object sender, EventArgs e)
        {
            FormEditMaster form = new FormEditMaster();
            if (form.ShowDialog() == DialogResult.OK)
                LoadMasters();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvMasters.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите мастера для редактирования");
                return;
            }
            int id = Convert.ToInt32(dgvMasters.SelectedRows[0].Cells["id"].Value);
            FormEditMaster form = new FormEditMaster(id);
            if (form.ShowDialog() == DialogResult.OK)
                LoadMasters();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvMasters.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите мастера для удаления");
                return;
            }
            int id = Convert.ToInt32(dgvMasters.SelectedRows[0].Cells["id"].Value);
            DialogResult dr = MessageBox.Show("Удалить мастера?", "Подтверждение", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM masters WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                LoadMasters();
            }
        }
    }
}