using Avto.Data;
using Avto.Models;
using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Avto.Forms
{
    public partial class MainForm : Form
    {
        private User _currentUser;
        private List<Vehicle> _vehicles;
        private int? _selectedVehicleId = null;

        public delegate void UpdateStatisticsEventHandler(string message, int count);
        public event UpdateStatisticsEventHandler StatisticsUpdated;

        public delegate void DataLoadCompletedEventHandler(string status);
        public event DataLoadCompletedEventHandler DataLoadCompleted;

        private delegate void SafeCallDelegate(string text);

        // Поток 
        private Thread _backgroundLoadThread;
        // Поток
        private Thread _timerThread;
        private bool _isRunning = true;

        public MainForm(User user)
        {
            InitializeComponent();
            _currentUser = user;

            // Подписываемся на события
            this.StatisticsUpdated += OnStatisticsUpdated;
            this.DataLoadCompleted += OnDataLoadCompleted;

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

            // Запускаем фоновые потоки
            StartBackgroundThreads();
        }

        // Запуск потоков
        private void StartBackgroundThreads()
        {
            _backgroundLoadThread = new Thread(new ThreadStart(BackgroundLoadData));
            _backgroundLoadThread.IsBackground = true;
            _backgroundLoadThread.Start();

            _timerThread = new Thread(new ThreadStart(UpdateTimeThread));
            _timerThread.IsBackground = true;
            _timerThread.Start();
        }

        private void BackgroundLoadData()
        {
            Thread.Sleep(500);

            StatisticsUpdated?.Invoke("Фоновая загрузка завершена", 100);

            DataLoadCompleted?.Invoke("Данные успешно синхронизированы");
        }

        private void UpdateTimeThread()
        {
            while (_isRunning)
            {
                UpdateStatusLabel($"Текущее время: {DateTime.Now:HH:mm:ss} | Пользователь: {_currentUser.Username}");

                Thread.Sleep(1000);
            }
        }

        private void UpdateStatusLabel(string text)
        {
            if (lblStatus.InvokeRequired)
            {
                SafeCallDelegate d = new SafeCallDelegate(UpdateStatusLabel);
                lblStatus.Invoke(d, new object[] { text });
            }
            else
            {
                lblStatus.Text = text;
            }
        }

        private void OnStatisticsUpdated(string message, int count)
        {
            if (lblEventLog.InvokeRequired)
            {
                lblEventLog.Invoke(new Action(() =>
                {
                    lblEventLog.Text = $"[{DateTime.Now:HH:mm:ss}] {message} (Count: {count})";
                }));
            }
            else
            {
                lblEventLog.Text = $"[{DateTime.Now:HH:mm:ss}] {message} (Count: {count})";
            }
        }

        private void OnDataLoadCompleted(string status)
        {
            if (lblEventLog.InvokeRequired)
            {
                lblEventLog.Invoke(new Action(() =>
                {
                    lblEventLog.Text += $"\n[{DateTime.Now:HH:mm:ss}] {status}";
                }));
            }
            else
            {
                lblEventLog.Text += $"\n[{DateTime.Now:HH:mm:ss}] {status}";
            }
        }

        private void RaiseVehicleAddedEvent(Vehicle vehicle)
        {
            // Используем делегат
            StatisticsUpdated?.Invoke($"Добавлено ТС: {vehicle.Make} ({vehicle.Number})", 1);
        }

        private void RaiseVehicleDeletedEvent(int vehicleId)
        {
            StatisticsUpdated?.Invoke($"Удалено ТС с ID: {vehicleId}", -1);
        }

        private void SetButtonsAccess()
        {
            bool isAdmin = _currentUser.Role == UserRole.admin;

            btnAdd.Visible = isAdmin;
            btnEdit.Visible = isAdmin;
            btnDelete.Visible = isAdmin;
            btnUsers.Visible = isAdmin;
            btnExportVehicles.Visible = isAdmin;
            btnAnalytics.Visible = isAdmin;

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
                // Вызываем событие 
                StatisticsUpdated?.Invoke("Транспортное средство добавлено", _vehicles?.Count ?? 0);
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
                    StatisticsUpdated?.Invoke($"ТС отредактировано: {selected.Make}", _vehicles.Count);
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
                StatisticsUpdated?.Invoke($"Экспорт CSV выполнен", vehicles.Count);
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
                    int deletedId = _selectedVehicleId.Value;
                    DatabaseHelper.DeleteVehicle(deletedId);
                    LoadVehicles();
                    RaiseVehicleDeletedEvent(deletedId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // Запускаем фоновую загрузку при обновлении
            Thread refreshThread = new Thread(() =>
            {
                Thread.Sleep(100);
                LoadVehicles();

                // Вызываем событие через Invoke
                if (lblEventLog.InvokeRequired)
                {
                    lblEventLog.Invoke(new Action(() =>
                    {
                        StatisticsUpdated?.Invoke("Данные обновлены в фоновом режиме", _vehicles?.Count ?? 0);
                    }));
                }
                else
                {
                    StatisticsUpdated?.Invoke("Данные обновлены в фоновом режиме", _vehicles?.Count ?? 0);
                }
            });
            refreshThread.IsBackground = true;
            refreshThread.Start();

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
                    StatisticsUpdated?.Invoke($"Арендовано ТС: {selected.Make}", _vehicles.Count);
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

        private void btnAnalytics_Click(object sender, EventArgs e)
        {
            if (_currentUser.Role != UserRole.admin) return;

            AnalyticsForm form = new AnalyticsForm();
            form.ShowDialog();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            _isRunning = false;
            this.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _isRunning = false;

            if (_backgroundLoadThread != null && _backgroundLoadThread.IsAlive)
                _backgroundLoadThread.Join(500);

            if (_timerThread != null && _timerThread.IsAlive)
                _timerThread.Join(500);

            Application.Exit();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F1 && _currentUser.Role == UserRole.admin && btnAdd.Visible)
            {
                btnAdd.PerformClick();
                return true;
            }

            if (keyData == Keys.F2 && _currentUser.Role == UserRole.admin && btnEdit.Visible)
            {
                btnEdit.PerformClick();
                return true;
            }

            if (keyData == Keys.F3)
            {
                btnRefresh.PerformClick();
                return true;
            }

            if (keyData == Keys.F4 && _currentUser.Role == UserRole.admin && btnAnalytics.Visible)
            {
                btnAnalytics.PerformClick();
                return true;
            }

            if (keyData == Keys.F5 && _currentUser.Role == UserRole.user && btnMyOrders.Visible)
            {
                btnMyOrders.PerformClick();
                return true;
            }

            if (keyData == (Keys.Control | Keys.Q))
            {
                btnLogout.PerformClick();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}