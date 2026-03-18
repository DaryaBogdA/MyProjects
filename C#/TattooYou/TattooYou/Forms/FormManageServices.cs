using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using TattooYou.Database;
using TattooYou.Helpers;

namespace TattooYou.Forms
{
    public partial class FormManageServices : Form
    {
        public FormManageServices()
        {
            InitializeComponent();
            ApplyColorScheme();
            LoadServices();
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
            dgvServices.BackgroundColor = Color.White;
            dgvServices.DefaultCellStyle.BackColor = Color.White;
            dgvServices.DefaultCellStyle.ForeColor = Color.Black;
            dgvServices.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dgvServices.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvServices.EnableHeadersVisualStyles = false;
        }

        private void LoadServices()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT id, name, description, price FROM services ORDER BY name";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvServices.DataSource = dt;

                if (dgvServices.Columns.Count > 0)
                {
                    dgvServices.Columns["id"].HeaderText = "ID";
                    dgvServices.Columns["name"].HeaderText = "Название";
                    dgvServices.Columns["description"].HeaderText = "Описание";
                    dgvServices.Columns["price"].HeaderText = "Цена (BYN)";
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FormEditService form = new FormEditService();
            if (form.ShowDialog() == DialogResult.OK)
                LoadServices();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvServices.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите услугу для редактирования");
                return;
            }
            int id = Convert.ToInt32(dgvServices.SelectedRows[0].Cells["id"].Value);
            FormEditService form = new FormEditService(id);
            if (form.ShowDialog() == DialogResult.OK)
                LoadServices();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (dgvServices.DataSource == null)
            {
                MessageBox.Show("Нет данных для экспорта.");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV файлы (*.csv)|*.csv|Текстовые файлы (*.txt)|*.txt";
            sfd.DefaultExt = "csv";
            sfd.FileName = $"Services_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                DataTable dt = (DataTable)dgvServices.DataSource;
                ExportHelper.DataTableToCsv(dt, sfd.FileName);
                MessageBox.Show("Услуги сохранены.", "Экспорт", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvServices.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите услугу для удаления");
                return;
            }
            int id = Convert.ToInt32(dgvServices.SelectedRows[0].Cells["id"].Value);
            DialogResult dr = MessageBox.Show("Удалить услугу?", "Подтверждение", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM services WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                LoadServices();
            }
        }
    }
}