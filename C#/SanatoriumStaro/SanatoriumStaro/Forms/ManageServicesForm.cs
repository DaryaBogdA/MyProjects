using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SanatoriumStaro.Models;

namespace SanatoriumStaro
{
    public partial class ManageServicesForm : Form
    {
        public ManageServicesForm()
        {
            InitializeComponent();
            LoadServices();
        }

        private void LoadServices()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT s.*, sc.name as category_name 
                                 FROM services s
                                 LEFT JOIN service_categories sc ON s.category_id = sc.id
                                 ORDER BY s.is_active DESC, s.name";
                var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();
                var services = new List<Service>();
                while (reader.Read())
                {
                    services.Add(new Service
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        Description = reader["description"]?.ToString(),
                        Price = Convert.ToDecimal(reader["price"]),
                        Duration = reader["duration"] as int?,
                        CategoryId = reader["category_id"] as int?,
                        CategoryName = reader["category_name"]?.ToString(),
                        IsActive = Convert.ToBoolean(reader["is_active"]),
                        CreatedAt = Convert.ToDateTime(reader["created_at"])
                    });
                }
                dgvServices.DataSource = null;
                dgvServices.DataSource = services;
            }
            if (dgvServices.Columns.Count > 0)
            {
                dgvServices.Columns["Id"].Visible = false;
                dgvServices.Columns["CategoryId"].Visible = false;
                dgvServices.Columns["IsActive"].HeaderText = "Активна";
                dgvServices.Columns["CreatedAt"].Visible = false;
                dgvServices.Columns["Name"].HeaderText = "Название";
                dgvServices.Columns["Description"].HeaderText = "Описание";
                dgvServices.Columns["Price"].HeaderText = "Цена";
                dgvServices.Columns["Duration"].HeaderText = "Длит.";
                dgvServices.Columns["CategoryName"].HeaderText = "Категория";
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ServiceEditForm editForm = new ServiceEditForm(null);
            if (editForm.ShowDialog() == DialogResult.OK)
                LoadServices();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvServices.SelectedRows.Count == 0) return;
            var service = (Service)dgvServices.SelectedRows[0].DataBoundItem;
            ServiceEditForm editForm = new ServiceEditForm(service);
            if (editForm.ShowDialog() == DialogResult.OK)
                LoadServices();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvServices.SelectedRows.Count == 0) return;
            var service = (Service)dgvServices.SelectedRows[0].DataBoundItem;
            if (MessageBox.Show($"Удалить услугу '{service.Name}'?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (DatabaseHelper.DeleteService(service.Id))
                {
                    MessageBox.Show("Услуга деактивирована.");
                    LoadServices();
                }
                else
                    MessageBox.Show("Ошибка при удалении.");
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadServices();
        }
    }
}