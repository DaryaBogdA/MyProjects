using SanatoriumStaro.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SanatoriumStaro
{
    public partial class ServiceEditForm : Form
    {
        private Service service;
        private List<ServiceCategory> categories;

        public ServiceEditForm(Service existingService)
        {
            service = existingService ?? new Service { IsActive = true };
            InitializeComponent();
            LoadCategories();
            if (existingService != null)
                LoadData();
        }

        private void LoadCategories()
        {
            categories = DatabaseHelper.GetAllCategories();
            cmbCategory.DataSource = categories;
            cmbCategory.DisplayMember = "Name";
            cmbCategory.ValueMember = "Id";
            cmbCategory.SelectedIndex = -1;
        }

        private void LoadData()
        {
            txtName.Text = service.Name;
            txtDescription.Text = service.Description;
            txtPrice.Text = service.Price.ToString("F2");
            txtDuration.Text = service.Duration?.ToString();
            if (service.CategoryId.HasValue)
                cmbCategory.SelectedValue = service.CategoryId.Value;
            chkActive.Checked = service.IsActive;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название услуги.");
                return;
            }
            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену.");
                return;
            }

            service.Name = txtName.Text.Trim();
            service.Description = txtDescription.Text.Trim();
            service.Price = price;
            if (int.TryParse(txtDuration.Text, out int dur))
                service.Duration = dur;
            else
                service.Duration = null;

            service.CategoryId = cmbCategory.SelectedValue as int?;
            service.IsActive = chkActive.Checked;

            bool result;
            if (service.Id == 0)
                result = DatabaseHelper.AddService(service);
            else
                result = DatabaseHelper.UpdateService(service);

            if (result)
            {
                MessageBox.Show("Сохранено.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка сохранения.");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}