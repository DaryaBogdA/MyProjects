using Mysqlx.Crud;
using SanatoriumStaro.Models;
using System;
using System.Windows.Forms;

namespace SanatoriumStaro
{
    public partial class BookingForm : Form
    {
        private User currentUser;
        private Service service;

        public BookingForm(User user, Service selectedService)
        {
            currentUser = user;
            service = selectedService;
            InitializeComponent();
            lblService.Text = $"Услуга: {service.Name}";
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            DateTime selectedDate = dtpDate.Value.Date;
            int hour = (int)cbHour.SelectedItem;
            int minute = (int)cbMinute.SelectedItem;
            DateTime appointmentDate = selectedDate.AddHours(hour).AddMinutes(minute);

            if (appointmentDate <= DateTime.Now)
            {
                MessageBox.Show("Дата и время должны быть в будущем.");
                return;
            }

            bool result = DatabaseHelper.CreateAppointment(currentUser.Id, service.Id, appointmentDate, txtNotes.Text);
            if (result)
            {
                MessageBox.Show("Запись создана!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка при создании записи.");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}