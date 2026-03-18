using System;
using System.Windows.Forms;
using SanatoriumStaro.Models;

namespace SanatoriumStaro
{
    public partial class ServicesForm : Form
    {
        private User currentUser;

        public ServicesForm(User user)
        {
            currentUser = user;
            InitializeComponent();
            LoadServices();
        }

        private void LoadServices()
        {
            var services = DatabaseHelper.GetAllServices();
            dgvServices.DataSource = null;
            dgvServices.DataSource = services;
            if (dgvServices.Columns.Count > 0)
            {
                dgvServices.Columns["Id"].Visible = false;
                dgvServices.Columns["CategoryId"].Visible = false;
                dgvServices.Columns["IsActive"].Visible = false;
                dgvServices.Columns["CreatedAt"].Visible = false;
                dgvServices.Columns["Name"].HeaderText = "Название";
                dgvServices.Columns["Description"].HeaderText = "Описание";
                dgvServices.Columns["Price"].HeaderText = "Цена (руб)";
                dgvServices.Columns["Duration"].HeaderText = "Длительность (мин)";
                dgvServices.Columns["CategoryName"].HeaderText = "Категория";
                dgvServices.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
        }

        private void btnBook_Click(object sender, EventArgs e)
        {
            if (dgvServices.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите услугу для записи.");
                return;
            }
            var selectedService = (Service)dgvServices.SelectedRows[0].DataBoundItem;
            using (var bookingForm = new BookingForm(currentUser, selectedService))
            {
                if (bookingForm.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Запись успешно создана!");
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}