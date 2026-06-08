using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using SanatoriumStaro.Models;

namespace SanatoriumStaro
{
    public partial class StatisticsForm : Form
    {
        private User currentUser;
        private List<Appointment> allAppointments;
        private List<Service> allServices;

        public StatisticsForm(User user)
        {
            currentUser = user;
            InitializeComponent();
            LoadData();
            LoadCharts();

        }

        private void LoadData()
        {
            allAppointments = DatabaseHelper.GetAllAppointments();
            allServices = DatabaseHelper.GetAllServices();
        }

        private void LoadCharts()
        {
            LoadChartByService();
            LoadChartByDay();
        }

        private void LoadChartByService()
        {
            chartByService.Series.Clear();
            chartByService.Titles.Clear();

            chartByService.Titles.Add("Популярность услуг");
            chartByService.Titles[0].Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            chartByService.Titles[0].ForeColor = System.Drawing.Color.DarkSlateBlue;

            Series series = new Series("Количество записей");
            series.ChartType = SeriesChartType.Column;
            series.Color = System.Drawing.Color.SteelBlue;
            series.IsValueShownAsLabel = true;
            series.LabelFormat = "{0}";
            series.Font = new System.Drawing.Font("Segoe UI", 8F);

            var serviceCounts = allAppointments
                .GroupBy(a => a.ServiceName)
                .Select(g => new { ServiceName = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10);

            foreach (var item in serviceCounts)
            {
                series.Points.AddXY(item.ServiceName, item.Count);
            }

            chartByService.Series.Add(series);

            chartByService.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            chartByService.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 8F);
            chartByService.ChartAreas[0].AxisX.Interval = 1;

            chartByService.ChartAreas[0].AxisY.Title = "Количество записей";
            chartByService.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Segoe UI", 9F);

            chartByService.Legends.Clear();
            Legend legend = new Legend();
            legend.Docking = Docking.Top;
            legend.Font = new System.Drawing.Font("Segoe UI", 8F);
            chartByService.Legends.Add(legend);
        }

        private void LoadChartByDay()
        {
            chartByDay.Series.Clear();
            chartByDay.Titles.Clear();

            chartByDay.Titles.Add("Динамика записей по дням");
            chartByDay.Titles[0].Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            chartByDay.Titles[0].ForeColor = System.Drawing.Color.DarkSlateBlue;

            DateTime startDate = GetStartDate();
            var filteredAppointments = allAppointments.Where(a => a.AppointmentDate >= startDate).ToList();

            var dailyCounts = filteredAppointments
                .GroupBy(a => a.AppointmentDate.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToList();

            Series lineSeries = new Series("Записи");
            lineSeries.ChartType = SeriesChartType.Line;
            lineSeries.Color = System.Drawing.Color.SteelBlue;
            lineSeries.BorderWidth = 3;
            lineSeries.MarkerStyle = MarkerStyle.Circle;
            lineSeries.MarkerSize = 6;
            lineSeries.MarkerColor = System.Drawing.Color.DarkOrange;
            lineSeries.IsValueShownAsLabel = false;

            Series columnSeries = new Series("Количество");
            columnSeries.ChartType = SeriesChartType.Column;
            columnSeries.Color = System.Drawing.Color.LightSteelBlue;
            columnSeries.IsValueShownAsLabel = true;
            columnSeries.LabelFormat = "{0}";

            foreach (var item in dailyCounts)
            {
                string dateStr = item.Date.ToString("dd.MM");
                lineSeries.Points.AddXY(dateStr, item.Count);
                columnSeries.Points.AddXY(dateStr, item.Count);
            }

            chartByDay.Series.Add(lineSeries);
            chartByDay.Series.Add(columnSeries);

            chartByDay.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            chartByDay.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 7F);
            chartByDay.ChartAreas[0].AxisX.Interval = 1;
            chartByDay.ChartAreas[0].AxisY.Title = "Количество записей";
            chartByDay.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Segoe UI", 9F);

            chartByDay.Legends.Clear();
            Legend legend = new Legend();
            legend.Docking = Docking.Top;
            legend.Font = new System.Drawing.Font("Segoe UI", 8F);
            chartByDay.Legends.Add(legend);
        }

        private DateTime GetStartDate()
        {
            DateTime now = DateTime.Now;
            switch (cmbPeriod.SelectedIndex)
            {
                case 1:
                    return now.AddDays(-7);
                case 2:
                    return now.AddDays(-30);
                case 3:
                    return new DateTime(now.Year, now.Month, 1);
                case 4:
                    return new DateTime(now.Year, now.Month, 1).AddMonths(-1);
                default:
                    return DateTime.MinValue;
            }
        }

        private void cmbPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadChartByDay();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
            LoadCharts();
            MessageBox.Show("Данные обновлены!", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       
    }
}