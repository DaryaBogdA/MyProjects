using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Avto.Data;

namespace Avto.Forms
{
    public partial class AnalyticsForm : Form
    {
        public AnalyticsForm()
        {
            InitializeComponent();
        }

        private void AnalyticsForm_Load(object sender, EventArgs e)
        {
            LoadCharts();
        }

        private void LoadCharts()
        {
            try
            {
                var statsByType = DatabaseHelper.GetVehicleStatsByType();
                chartByType.Series.Clear();
                Series series1 = new Series("Количество ТС");
                series1.ChartType = SeriesChartType.Column;
                series1.Color = Color.FromArgb(0, 122, 204);

                foreach (var item in statsByType)
                {
                    series1.Points.AddXY(item.TypeName, item.Count);
                }
                chartByType.Series.Add(series1);
                chartByType.Titles.Clear();
                chartByType.Titles.Add("Количество ТС по типам");
                chartByType.ChartAreas[0].AxisX.Title = "Тип ТС";
                chartByType.ChartAreas[0].AxisY.Title = "Количество";

                var orderStats = DatabaseHelper.GetOrderStats();
                chartOrders.Series.Clear();
                Series series2 = new Series("Заказы");
                series2.ChartType = SeriesChartType.Pie;
                series2.Color = Color.FromArgb(0, 122, 204);

                foreach (var item in orderStats)
                {
                    series2.Points.AddXY(item.Status, item.Count);
                    series2.Points[series2.Points.Count - 1].LegendText = $"{item.Status} ({item.Count})";
                }
                chartOrders.Series.Add(series2);
                chartOrders.Titles.Clear();
                chartOrders.Titles.Add("Распределение заказов по статусам");

                var availabilityStats = DatabaseHelper.GetVehicleAvailabilityStats();
                chartAvailability.Series.Clear();
                Series series3 = new Series("Доступность");
                series3.ChartType = SeriesChartType.Pie;

                int available = 0, notAvailable = 0;
                foreach (var item in availabilityStats)
                {
                    if (item.IsAvailable)
                        available = item.Count;
                    else
                        notAvailable = item.Count;
                }

                series3.Points.AddXY("Доступны", available);
                series3.Points[0].Color = Color.FromArgb(0, 150, 136);
                series3.Points.AddXY("Недоступны", notAvailable);
                series3.Points[1].Color = Color.FromArgb(204, 0, 0);
                series3.Points[0].LegendText = $"Доступны ({available})";
                series3.Points[1].LegendText = $"Недоступны ({notAvailable})";

                chartAvailability.Series.Add(series3);
                chartAvailability.Titles.Clear();
                chartAvailability.Titles.Add("Статус доступности ТС");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки графиков: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCharts();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}