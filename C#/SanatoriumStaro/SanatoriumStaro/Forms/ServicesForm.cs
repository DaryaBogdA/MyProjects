using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SanatoriumStaro.Models;

namespace SanatoriumStaro
{
    public delegate int ServiceCompareDelegate(Service x, Service y);

    public partial class ServicesForm : Form
    {
        private User currentUser;
        private List<Service> services;

        public ServicesForm(User user)
        {
            currentUser = user;
            InitializeComponent();
            LoadServices();

            dgvServices.ColumnHeaderMouseClick += dgvServices_ColumnHeaderMouseClick;
        }

        private void LoadServices()
        {
            services = DatabaseHelper.GetAllServices();
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

        private void SortServices(ServiceCompareDelegate comparer)
        {
            if (services != null)
            {
                services.Sort(new Comparison<Service>(comparer));
                dgvServices.DataSource = null;
                dgvServices.DataSource = services;
            }
        }

        private int PriceCompare(Service x, Service y)
        {
            return x.Price.CompareTo(y.Price);
        }

        private int NameCompare(Service x, Service y)
        {
            return x.Name.CompareTo(y.Name);
        }

        private int DurationCompare(Service x, Service y)
        {
            return (x.Duration ?? 0).CompareTo(y.Duration ?? 0);
        }

        private int CategoryCompare(Service x, Service y)
        {
            return (x.CategoryName ?? "").CompareTo(y.CategoryName ?? "");
        }

        private void dgvServices_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string columnName = dgvServices.Columns[e.ColumnIndex].Name;

            switch (columnName)
            {
                case "Price":
                    SortServices(PriceCompare);
                    break;
                case "Name":
                    SortServices(NameCompare);
                    break;
                case "Duration":
                    SortServices(DurationCompare);
                    break;
                case "CategoryName":
                    SortServices(CategoryCompare);
                    break;
            }
        }

        private void btnBook_Click(object sender, EventArgs e)
        {
            if (dgvServices.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите услугу для записи.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var selectedService = (Service)dgvServices.SelectedRows[0].DataBoundItem;
            using (var bookingForm = new BookingForm(currentUser, selectedService))
            {
                if (bookingForm.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Запись успешно создана!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}