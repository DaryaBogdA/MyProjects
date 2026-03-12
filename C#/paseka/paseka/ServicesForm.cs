using MySql.Data.MySqlClient;
using paseka.DataAccess;
using paseka.Helpers;
using System;
using System.Data;
using System.Windows.Forms;

namespace paseka
{
    public partial class ServicesForm : Form
    {
        private bool isAdmin;

        public ServicesForm(bool isAdmin)
        {
            InitializeComponent();
            ThemeHelper.ApplyDarkTheme(this);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.isAdmin = isAdmin;
            btnAdd.Visible = isAdmin;
            btnEdit.Visible = isAdmin;
            btnDelete.Visible = isAdmin;

            LoadServices();
        }

        private void LoadServices(string orderBy = "id")
        {
            string query = $"SELECT * FROM services ORDER BY {orderBy}";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            dgvServices.DataSource = dt;

            dgvServices.Columns["id"].HeaderText = "ID";
            dgvServices.Columns["name"].HeaderText = "Название";
            dgvServices.Columns["type"].HeaderText = "Тип";
            dgvServices.Columns["price"].HeaderText = "Цена";
            dgvServices.Columns["duration"].HeaderText = "Длительность (мин)";
            dgvServices.Columns["description"].HeaderText = "Описание";
            dgvServices.Columns["quantity"].HeaderText = "Кол-во";
            dgvServices.Columns["is_room"].HeaderText = "Комната";
            dgvServices.Columns["is_available"].HeaderText = "Доступно";
        }

        private void cmbSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = cmbSort.SelectedItem.ToString();
            string orderField = selected switch
            {
                "По ID" => "id",
                "По названию" => "name",
                "По цене" => "price",
                "По типу" => "type",
                _ => "id"
            };
            LoadServices(orderField);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddEditServiceForm form = new AddEditServiceForm();
            if (form.ShowDialog() == DialogResult.OK)
                LoadServices();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvServices.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(dgvServices.SelectedRows[0].Cells["id"].Value);
            AddEditServiceForm form = new AddEditServiceForm(id);
            if (form.ShowDialog() == DialogResult.OK)
                LoadServices();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvServices.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(dgvServices.SelectedRows[0].Cells["id"].Value);
            if (MessageBox.Show("Удалить выбранную услугу?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string query = "DELETE FROM services WHERE id = @id";
                DatabaseHelper.ExecuteNonQuery(query, new MySqlParameter("@id", id));
                LoadServices();
            }
        }
    }
}