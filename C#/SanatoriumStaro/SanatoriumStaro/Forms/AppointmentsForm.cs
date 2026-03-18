using Org.BouncyCastle.Asn1.Cmp;
using SanatoriumStaro.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SanatoriumStaro
{
    public partial class AppointmentsForm : Form
    {
        private User currentUser;
        private bool isAdminView;
        private List<AppointmentStatus> statuses;

        public AppointmentsForm(User user, bool adminView)
        {
            currentUser = user;
            isAdminView = adminView;
            InitializeComponent();
            LoadAppointments();
        }

        private void LoadAppointments()
        {
            List<Appointment> appointments;
            if (isAdminView)
                appointments = DatabaseHelper.GetAllAppointments();
            else
                appointments = DatabaseHelper.GetAppointmentsByUser(currentUser.Id);

            dgvAppointments.DataSource = null;
            dgvAppointments.DataSource = appointments;

            if (dgvAppointments.Columns.Count > 0)
            {
                dgvAppointments.Columns["Id"].Visible = false;
                dgvAppointments.Columns["UserId"].Visible = false;
                dgvAppointments.Columns["ServiceId"].Visible = false;
                dgvAppointments.Columns["StatusId"].Visible = false;
                dgvAppointments.Columns["UserFullName"].HeaderText = "Клиент";
                dgvAppointments.Columns["ServiceName"].HeaderText = "Услуга";
                dgvAppointments.Columns["AppointmentDate"].HeaderText = "Дата и время";
                dgvAppointments.Columns["StatusName"].HeaderText = "Статус";
                dgvAppointments.Columns["Notes"].HeaderText = "Примечания";
                dgvAppointments.Columns["CreatedAt"].HeaderText = "Создано";
                dgvAppointments.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
        }

        private void dgvAppointments_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAppointments.SelectedRows.Count > 0)
            {
                cmbStatus.Visible = true;
                btnChangeStatus.Visible = true;
                btnCancel.Visible = true;

                if (statuses == null)
                {
                    statuses = DatabaseHelper.GetAllStatuses();
                    cmbStatus.DataSource = statuses;
                    cmbStatus.DisplayMember = "Name";
                    cmbStatus.ValueMember = "Id";
                }

                var appointment = (Appointment)dgvAppointments.SelectedRows[0].DataBoundItem;
                cmbStatus.SelectedValue = appointment.StatusId;
            }
            else
            {
                cmbStatus.Visible = false;
                btnChangeStatus.Visible = false;
                btnCancel.Visible = false;
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для сохранения.");
                return;
            }

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*";
                saveDialog.DefaultExt = "csv";
                saveDialog.FileName = $"Appointments_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                saveDialog.Title = "Сохранить записи";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ExportToCsv(saveDialog.FileName);
                        MessageBox.Show("Данные успешно сохранены.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ExportToCsv(string filePath)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath, false, Encoding.UTF8))
            {
                List<string> headers = new List<string>();
                foreach (DataGridViewColumn col in dgvAppointments.Columns)
                {
                    if (col.Visible)
                        headers.Add(EscapeCsvField(col.HeaderText));
                }
                sw.WriteLine(string.Join(";", headers));

                foreach (DataGridViewRow row in dgvAppointments.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        List<string> fields = new List<string>();
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (cell.Visible)
                            {
                                string value = cell.Value?.ToString() ?? "";
                                fields.Add(EscapeCsvField(value));
                            }
                        }
                        sw.WriteLine(string.Join(";", fields));
                    }
                }
            }
        }

        private string EscapeCsvField(string field)
        {
            if (field.Contains(";") || field.Contains("\"") || field.Contains("\n"))
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            else
                return field;
        }
        private void btnChangeStatus_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.SelectedRows.Count == 0) return;
            var appointment = (Appointment)dgvAppointments.SelectedRows[0].DataBoundItem;
            int newStatusId = (int)cmbStatus.SelectedValue;

            if (newStatusId == appointment.StatusId)
            {
                MessageBox.Show("Статус не изменился.");
                return;
            }

            if (DatabaseHelper.UpdateAppointmentStatus(appointment.Id, newStatusId))
            {
                MessageBox.Show("Статус обновлён.");
                LoadAppointments();
            }
            else
            {
                MessageBox.Show("Ошибка при обновлении статуса.");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.SelectedRows.Count == 0) return;
            var appointment = (Appointment)dgvAppointments.SelectedRows[0].DataBoundItem;

            var confirm = MessageBox.Show("Отменить запись?", "Подтверждение", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                if (DatabaseHelper.CancelAppointment(appointment.Id))
                {
                    MessageBox.Show("Запись отменена.");
                    LoadAppointments();
                }
                else
                {
                    MessageBox.Show("Ошибка при отмене.");
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}