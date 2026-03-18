using Avto.Data;
using Avto.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Avto.Forms
{
    public partial class MyOrdersForm : Form
    {
        private int _userId;
        private List<Order> _orders;
        private int? _selectedOrderId;

        public MyOrdersForm(int userId)
        {
            InitializeComponent();
            _userId = userId;
        }

        private void MyOrdersForm_Load(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                _orders = DatabaseHelper.GetUserOrders(_userId);
                dgvOrders.DataSource = null;
                dgvOrders.DataSource = _orders;

                dgvOrders.Columns["Id"].HeaderText = "ID";
                dgvOrders.Columns["VehicleMake"].HeaderText = "Марка";
                dgvOrders.Columns["VehicleNumber"].HeaderText = "Госномер";
                dgvOrders.Columns["StartTime"].HeaderText = "Начало";
                dgvOrders.Columns["EndTime"].HeaderText = "Окончание";
                dgvOrders.Columns["TotalCost"].HeaderText = "Стоимость";
                dgvOrders.Columns["TotalCost"].DefaultCellStyle.Format = "C2";
                dgvOrders.Columns["Status"].HeaderText = "Статус";
                dgvOrders.Columns["CreatedAt"].HeaderText = "Дата заказа";
                dgvOrders.Columns["VehicleId"].Visible = false;
                dgvOrders.Columns["UserId"].Visible = false;

                dgvOrders.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvOrders_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvOrders.SelectedRows.Count > 0)
            {
                var selected = dgvOrders.SelectedRows[0].DataBoundItem as Order;
                _selectedOrderId = selected?.Id;
            }
            else
            {
                _selectedOrderId = null;
            }
        }

        private void btnComplete_Click(object sender, EventArgs e)
        {
            if (_selectedOrderId == null)
            {
                MessageBox.Show("Выберите заказ.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var order = _orders.Find(o => o.Id == _selectedOrderId);
            if (order.Status != OrderStatus.active)
            {
                MessageBox.Show("Можно завершить только активный заказ.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Подтвердите завершение аренды.", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.CompleteOrder(_selectedOrderId.Value);
                    LoadOrders();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnExportOrders_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*";
            saveDialog.DefaultExt = "csv";
            saveDialog.FileName = $"orders_{_userId}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                ExportOrdersToCsv(saveDialog.FileName);
            }
        }

        private void ExportOrdersToCsv(string filePath)
        {
            try
            {
                using (var writer = new System.IO.StreamWriter(filePath, false, Encoding.UTF8))
                {
                    writer.WriteLine("ID заказа;Марка;Госномер;Начало;Окончание;Стоимость;Статус;Дата заказа");

                    foreach (var o in _orders)
                    {
                        string line = string.Join(";",
                            o.Id,
                            EscapeCsv(o.VehicleMake),
                            EscapeCsv(o.VehicleNumber),
                            o.StartTime.ToString("yyyy-MM-dd HH:mm"),
                            o.EndTime.ToString("yyyy-MM-dd HH:mm"),
                            o.TotalCost.ToString("F2"),
                            o.Status.ToString(),
                            o.CreatedAt.ToString("yyyy-MM-dd HH:mm")
                        );
                        writer.WriteLine(line);
                    }
                }
                MessageBox.Show($"Заказы успешно сохранены в файл:\n{filePath}", "Экспорт завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_selectedOrderId == null)
            {
                MessageBox.Show("Выберите заказ.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var order = _orders.Find(o => o.Id == _selectedOrderId);
            if (order.Status != OrderStatus.active)
            {
                MessageBox.Show("Можно отменить только активный заказ.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Отменить аренду?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.CancelOrder(_selectedOrderId.Value);
                    LoadOrders();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}