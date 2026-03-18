using Avto.Data;
using Avto.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Avto.Forms
{
    public partial class MainForm : Form
    {
        private User _currentUser;
        private List<Vehicle> _vehicles;
        private int? _selectedVehicleId = null;

        public MainForm(User user)
        {
            InitializeComponent();
            _currentUser = user;
            lblUserInfo.Text = $"Пользователь: {_currentUser.FullName ?? _currentUser.Username} ({_currentUser.Role})";
            SetButtonsAccess();

            dgvVehicles.BackgroundColor = Color.White;
            dgvVehicles.BorderStyle = BorderStyle.None;
            dgvVehicles.EnableHeadersVisualStyles = false;
            dgvVehicles.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 122, 204);
            dgvVehicles.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvVehicles.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvVehicles.RowHeadersVisible = false;
            dgvVehicles.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 122, 204);
            dgvVehicles.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvVehicles.DefaultCellStyle.Font = new Font("Segoe UI", 9);
        }
        private void SetButtonsAccess()
        {
            bool isAdmin = _currentUser.Role == UserRole.admin;

            btnAdd.Visible = isAdmin;
            btnEdit.Visible = isAdmin;
            btnDelete.Visible = isAdmin;
            btnUsers.Visible = isAdmin;
            btnExportVehicles.Visible = isAdmin; 

            btnRent.Visible = !isAdmin;
            btnMyOrders.Visible = !isAdmin;

            btnRefresh.Visible = true;
            btnLogout.Visible = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadVehicles();
        }

       private void LoadVehicles()
{
    try
    {
        if (_currentUser.Role == UserRole.admin)
            _vehicles = DatabaseHelper.GetVehicles();
        else
            _vehicles = DatabaseHelper.GetAvailableVehicles();

        dgvVehicles.DataSource = null;
        BindingSource bs = new BindingSource();
        bs.DataSource = _vehicles;
        dgvVehicles.DataSource = bs;

        dgvVehicles.Columns["Id"].HeaderText = "ID";
        dgvVehicles.Columns["VehicleTypeName"].HeaderText = "Тип ТС";
        dgvVehicles.Columns["Make"].HeaderText = "Марка";
        dgvVehicles.Columns["Year"].HeaderText = "Год";
        dgvVehicles.Columns["Number"].HeaderText = "Госномер";
        dgvVehicles.Columns["Color"].HeaderText = "Цвет";
        dgvVehicles.Columns["FuelType"].HeaderText = "Топливо";
        dgvVehicles.Columns["RegistrationDate"].HeaderText = "Дата регистрации";
        dgvVehicles.Columns["Status"].HeaderText = "Статус";
        dgvVehicles.Columns["Notes"].HeaderText = "Примечания";
        dgvVehicles.Columns["CreatedAt"].HeaderText = "Дата создания";
        dgvVehicles.Columns["VehicleTypeId"].Visible = false;
        dgvVehicles.Columns["CreatedBy"].Visible = false;
        dgvVehicles.Columns["IsAvailable"].HeaderText = "Доступен";
        dgvVehicles.Columns["IsAvailable"].Visible = _currentUser.Role == UserRole.admin;
        dgvVehicles.Columns["PricePerHour"].HeaderText = "Цена/час";
        dgvVehicles.Columns["PricePerHour"].DefaultCellStyle.Format = "C2";
        dgvVehicles.Columns["PricePerHour"].Visible = true;

        dgvVehicles.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}

        private void dgvVehicles_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvVehicles.SelectedRows.Count > 0)
            {
                var selected = dgvVehicles.SelectedRows[0].DataBoundItem as Vehicle;
                if (selected != null)
                    _selectedVehicleId = selected.Id;
                else
                    _selectedVehicleId = null;
            }
            else
            {
                _selectedVehicleId = null;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (_currentUser.Role != UserRole.admin) return;

            AddEditForm form = new AddEditForm(_currentUser.Id);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadVehicles();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (_currentUser.Role != UserRole.admin) return;

            if (_selectedVehicleId == null)
            {
                MessageBox.Show("Выберите запись для редактирования.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Vehicle selected = _vehicles.Find(v => v.Id == _selectedVehicleId);
            if (selected != null)
            {
                AddEditForm form = new AddEditForm(selected, _currentUser.Id);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadVehicles();
                }
            }
        }

        private void btnExportVehicles_Click(object sender, EventArgs e)
        {
            if (_currentUser.Role != UserRole.admin) return;

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*";
            saveDialog.DefaultExt = "csv";
            saveDialog.FileName = $"vehicles_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                ExportVehiclesToCsv(saveDialog.FileName);
            }
        }

        private void ExportVehiclesToCsv(string filePath)
        {
            try
            {
                var vehicles = DatabaseHelper.GetVehicles();
                using (var writer = new System.IO.StreamWriter(filePath, false, Encoding.UTF8))
                {
                    writer.WriteLine("ID;Тип;Марка;Год;Госномер;Цвет;Топливо;Дата регистрации;Статус;Доступен;Цена/час;Примечания");

                    foreach (var v in vehicles)
                    {
                        string line = string.Join(";",
                            v.Id,
                            EscapeCsv(v.VehicleTypeName),
                            EscapeCsv(v.Make),
                            v.Year?.ToString() ?? "",
                            EscapeCsv(v.Number),
                            EscapeCsv(v.Color),
                            v.FuelType.ToString(),
                            v.RegistrationDate?.ToString("yyyy-MM-dd") ?? "",
                            v.Status.ToString(),
                            v.IsAvailable ? "Да" : "Нет",
                            v.PricePerHour.ToString("F2"),
                            EscapeCsv(v.Notes)
                        );
                        writer.WriteLine(line);
                    }
                }
                MessageBox.Show($"Данные успешно сохранены в файл:\n{filePath}", "Экспорт завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string EscapeCsv(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            if (input.Contains(";") || input.Contains("\"") || input.Contains("\n"))
            {
                return "\"" + input.Replace("\"", "\"\"") + "\"";
            }
            return input;
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_currentUser.Role != UserRole.admin) return;

            if (_selectedVehicleId == null)
            {
                MessageBox.Show("Выберите запись для удаления.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Удалить выбранное транспортное средство?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.DeleteVehicle(_selectedVehicleId.Value);
                    LoadVehicles();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadVehicles();
        }

        private void btnRent_Click(object sender, EventArgs e)
        {
            if (_currentUser.Role != UserRole.user) return;

            if (_selectedVehicleId == null)
            {
                MessageBox.Show("Выберите транспортное средство для аренды.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Vehicle selected = _vehicles.Find(v => v.Id == _selectedVehicleId);
            if (selected != null)
            {
                RentForm rentForm = new RentForm(selected, _currentUser.Id);
                if (rentForm.ShowDialog() == DialogResult.OK)
                {
                    LoadVehicles();
                }
            }
        }

        private void btnMyOrders_Click(object sender, EventArgs e)
        {
            if (_currentUser.Role != UserRole.user) return;
            MyOrdersForm form = new MyOrdersForm(_currentUser.Id);
            form.ShowDialog();
            LoadVehicles();
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            if (_currentUser.Role != UserRole.admin) return;
            UserManagementForm form = new UserManagementForm();
            form.ShowDialog();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}