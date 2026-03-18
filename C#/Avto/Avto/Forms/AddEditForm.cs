using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Avto.Data;
using Avto.Models;

namespace Avto.Forms
{
    public partial class AddEditForm : Form
    {
        private Vehicle _vehicle;
        private int _currentUserId;
        private List<VehicleType> _types;

        public AddEditForm(int currentUserId)
        {
            InitializeComponent();
            _currentUserId = currentUserId;
            _vehicle = null;
        }

        public AddEditForm(Vehicle vehicle, int currentUserId)
        {
            InitializeComponent();
            _vehicle = vehicle;
            _currentUserId = currentUserId;
        }

        private void AddEditForm_Load(object sender, EventArgs e)
        {
            try
            {
                _types = DatabaseHelper.GetVehicleTypes();
                cmbType.DataSource = _types;
                cmbType.DisplayMember = "TypeName";
                cmbType.ValueMember = "Id";
                cmbType.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            cmbFuel.DataSource = Enum.GetValues(typeof(FuelType));
            cmbStatus.DataSource = Enum.GetValues(typeof(VehicleStatus));

            if (_vehicle != null)
            {
                this.Text = "Редактирование ТС";
                cmbType.SelectedValue = _vehicle.VehicleTypeId;
                txtMake.Text = _vehicle.Make;
                txtYear.Text = _vehicle.Year?.ToString();
                txtPrice.Text = _vehicle.PricePerHour.ToString("F2");
                txtNumber.Text = _vehicle.Number;
                txtColor.Text = _vehicle.Color;
                cmbFuel.SelectedItem = _vehicle.FuelType;
                if (_vehicle.RegistrationDate.HasValue)
                    dtpRegDate.Value = _vehicle.RegistrationDate.Value;
                cmbStatus.SelectedItem = _vehicle.Status;
                txtNotes.Text = _vehicle.Notes;
            }
            else
            {
                this.Text = "Добавление ТС";
                dtpRegDate.Value = DateTime.Now;
                cmbFuel.SelectedItem = FuelType.Бензин;
                cmbStatus.SelectedItem = VehicleStatus.В_наличии;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbType.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип ТС.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtMake.Text))
            {
                MessageBox.Show("Введите марку.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Vehicle v = _vehicle ?? new Vehicle();
            v.VehicleTypeId = (int)cmbType.SelectedValue;
            v.Make = txtMake.Text.Trim();
            if (int.TryParse(txtYear.Text, out int year))
                v.Year = year;
            else
                v.Year = null;
            v.Number = txtNumber.Text.Trim();
            v.Color = txtColor.Text.Trim();
            v.FuelType = (FuelType)cmbFuel.SelectedItem;
            v.RegistrationDate = dtpRegDate.Value;
            v.Status = (VehicleStatus)cmbStatus.SelectedItem;
            v.Notes = txtNotes.Text.Trim();
            if (decimal.TryParse(txtPrice.Text, out decimal price))
                v.PricePerHour = price;
            else
                v.PricePerHour = 0;

            bool result;
            if (_vehicle == null)
            {
                v.CreatedBy = _currentUserId;
                result = DatabaseHelper.AddVehicle(v);
            }
            else
            {
                result = DatabaseHelper.UpdateVehicle(v);
            }

            if (result)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Не удалось сохранить запись.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}