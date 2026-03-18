using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using TattooYou.Database;
using TattooYou.Helpers;

namespace TattooYou.Forms
{
    public partial class FormStyles : Form
    {
        public FormStyles()
        {
            InitializeComponent();
            ApplyColorScheme();
            LoadStyles();
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
            dgvStyles.BackgroundColor = Color.White;
            dgvStyles.DefaultCellStyle.BackColor = Color.White;
            dgvStyles.DefaultCellStyle.ForeColor = Color.Black;
            dgvStyles.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dgvStyles.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvStyles.EnableHeadersVisualStyles = false;
        }

        private void LoadStyles()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT id, name, description FROM styles ORDER BY name";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvStyles.DataSource = dt;

                if (dgvStyles.Columns.Count > 0)
                {
                    dgvStyles.Columns["id"].HeaderText = "ID";
                    dgvStyles.Columns["name"].HeaderText = "Название";
                    dgvStyles.Columns["description"].HeaderText = "Описание";
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FormEditStyle form = new FormEditStyle();
            if (form.ShowDialog() == DialogResult.OK)
                LoadStyles();
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (dgvStyles.DataSource == null)
            {
                MessageBox.Show("Нет данных для экспорта.");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV файлы (*.csv)|*.csv|Текстовые файлы (*.txt)|*.txt";
            sfd.DefaultExt = "csv";
            sfd.FileName = $"Styles_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                DataTable dt = (DataTable)dgvStyles.DataSource;
                ExportHelper.DataTableToCsv(dt, sfd.FileName);
                MessageBox.Show("Стили сохранены.", "Экспорт", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvStyles.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите стиль для редактирования");
                return;
            }
            int id = Convert.ToInt32(dgvStyles.SelectedRows[0].Cells["id"].Value);
            FormEditStyle form = new FormEditStyle(id);
            if (form.ShowDialog() == DialogResult.OK)
                LoadStyles();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvStyles.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите стиль для удаления");
                return;
            }
            int id = Convert.ToInt32(dgvStyles.SelectedRows[0].Cells["id"].Value);
            DialogResult dr = MessageBox.Show("Удалить стиль?", "Подтверждение", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM styles WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                LoadStyles();
            }
        }
    }
}