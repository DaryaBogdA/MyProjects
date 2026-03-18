using System;
using System.Windows.Forms;
using Avto.Data;
using Avto.Models;

namespace Avto.Forms
{
    public partial class RentForm : Form
    {
        private Vehicle _vehicle;
        private int _userId;

        public RentForm(Vehicle vehicle, int userId)
        {
            InitializeComponent();
            _vehicle = vehicle;
            _userId = userId;
            lblVehicleInfo.Text = $"{vehicle.Make} ({vehicle.Number})";
            lblPricePerHour.Text = $"Цена: {vehicle.PricePerHour:C2} в час";
            UpdateEstimatedCost();

            dtpStart.ValueChanged += (s, e) => UpdateEstimatedCost();
            dtpEnd.ValueChanged += (s, e) => UpdateEstimatedCost();
        }

        private void UpdateEstimatedCost()
        {
            TimeSpan duration = dtpEnd.Value - dtpStart.Value;
            if (duration.TotalHours > 0)
            {
                decimal cost = _vehicle.PricePerHour * (decimal)duration.TotalHours;
                lblEstimatedCost.Text = $"Предварительная стоимость: {cost:C2}";
            }
            else
            {
                lblEstimatedCost.Text = "Предварительная стоимость: 0 ₽";
            }
        }

        private void btnRent_Click(object sender, EventArgs e)
        {
            DateTime start = dtpStart.Value;
            DateTime end = dtpEnd.Value;

            if (start >= end)
            {
                MessageBox.Show("Время окончания должно быть позже времени начала.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (start < DateTime.Now)
            {
                MessageBox.Show("Время начала не может быть в прошлом.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool result = DatabaseHelper.CreateOrder(_vehicle.Id, _userId, start, end);
                if (result)
                {
                    MessageBox.Show("Транспорт успешно арендован!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Не удалось арендовать. Возможно, транспорт уже недоступен.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}