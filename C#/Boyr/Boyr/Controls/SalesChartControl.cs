using Boyr.DataAccess;
using Boyr.UI;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Boyr.Controls
{
    public partial class SalesChartControl : UserControl
    {
        private Chart chartSales;
        private Button btnRefresh;
        private DateTimePicker dtpStart;
        private DateTimePicker dtpEnd;
        private Label lblStart;
        private Label lblEnd;
        private Panel panelTop;

        public SalesChartControl()
        {
            InitializeComponent();
            UiTheme.ApplyToControlTree(this);
            LoadChart();
        }

        private void InitializeComponent()
        {
            this.panelTop = new Panel();
            this.btnRefresh = new Button();
            this.dtpEnd = new DateTimePicker();
            this.dtpStart = new DateTimePicker();
            this.lblEnd = new Label();
            this.lblStart = new Label();
            this.chartSales = new Chart();

            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartSales)).BeginInit();
            this.SuspendLayout();

            this.panelTop.Dock = DockStyle.Top;
            this.panelTop.Height = 45;
            this.panelTop.BackColor = Color.WhiteSmoke;
            this.panelTop.Controls.Add(this.btnRefresh);
            this.panelTop.Controls.Add(this.dtpEnd);
            this.panelTop.Controls.Add(this.dtpStart);
            this.panelTop.Controls.Add(this.lblEnd);
            this.panelTop.Controls.Add(this.lblStart);

            this.lblStart.AutoSize = true;
            this.lblStart.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblStart.ForeColor = Color.Teal;
            this.lblStart.Location = new Point(15, 12);
            this.lblStart.Text = "Дата с:";
            this.lblStart.Size = new Size(52, 15);

            this.dtpStart.Location = new Point(75, 9);
            this.dtpStart.Size = new Size(130, 23);
            this.dtpStart.Value = DateTime.Today.AddDays(-30);

            this.lblEnd.AutoSize = true;
            this.lblEnd.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblEnd.ForeColor = Color.Teal;
            this.lblEnd.Location = new Point(220, 12);
            this.lblEnd.Text = "по:";
            this.lblEnd.Size = new Size(24, 15);

            this.dtpEnd.Location = new Point(250, 9);
            this.dtpEnd.Size = new Size(130, 23);
            this.dtpEnd.Value = DateTime.Today;

            this.btnRefresh.BackColor = Color.Teal;
            this.btnRefresh.FlatStyle = FlatStyle.Flat;
            this.btnRefresh.ForeColor = Color.White;
            this.btnRefresh.Location = new Point(410, 7);
            this.btnRefresh.Size = new Size(100, 30);
            this.btnRefresh.Text = "Обновить";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += BtnRefresh_Click;

            this.chartSales.Dock = DockStyle.Fill;
            this.chartSales.Location = new Point(0, 45);
            this.chartSales.Name = "chartSales";
            this.chartSales.Size = new Size(780, 465);
            this.chartSales.TabIndex = 1;

            this.Controls.Add(this.chartSales);
            this.Controls.Add(this.panelTop);
            this.Name = "SalesChartControl";
            this.Size = new Size(780, 510);

            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartSales)).EndInit();
            this.ResumeLayout(false);
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadChart();
        }

        private void LoadChart()
        {
            try
            {
                string query = @"
                    SELECT DATE(sale_date) as SaleDate, SUM(total_amount) as TotalAmount
                    FROM sales 
                    WHERE sale_date BETWEEN @startDate AND @endDate
                    GROUP BY DATE(sale_date)
                    ORDER BY SaleDate";

                var parameters = new[]
                {
                    new MySqlParameter("@startDate", dtpStart.Value.Date),
                    new MySqlParameter("@endDate", dtpEnd.Value.Date.AddDays(1).AddSeconds(-1))
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

                chartSales.Series.Clear();
                chartSales.ChartAreas.Clear();

                ChartArea chartArea = new ChartArea();
                chartArea.AxisX.Title = "Дата";
                chartArea.AxisX.TitleFont = new Font("Segoe UI", 10F, FontStyle.Bold);
                chartArea.AxisX.Interval = 1;
                chartArea.AxisX.LabelStyle.Angle = -45;
                chartArea.AxisY.Title = "Сумма продаж (Br)";
                chartArea.AxisY.TitleFont = new Font("Segoe UI", 10F, FontStyle.Bold);
                chartArea.AxisY.LabelStyle.Format = "{0:N0} Br";
                chartSales.ChartAreas.Add(chartArea);

                Series series = new Series();
                series.Name = "Продажи";
                series.ChartType = SeriesChartType.Column;
                series.Color = Color.Teal;
                series.BorderWidth = 2;
                series.ShadowOffset = 1;
                series.IsValueShownAsLabel = false;

                foreach (DataRow row in dt.Rows)
                {
                    DateTime date = Convert.ToDateTime(row["SaleDate"]);
                    decimal amount = Convert.ToDecimal(row["TotalAmount"]);
                    series.Points.AddXY(date.ToString("dd.MM"), (double)amount);
                }

                chartSales.Series.Add(series);

                if (dt.Rows.Count == 0)
                {
                    series.Points.AddXY("Нет данных", 0);
                    chartSales.Titles.Clear();
                    Title title = new Title("Нет продаж за выбранный период", Docking.Top, new Font("Segoe UI", 12F), Color.Gray);
                    chartSales.Titles.Add(title);
                }
                else
                {
                    chartSales.Titles.Clear();
                    Title mainTitle = new Title($"Динамика продаж с {dtpStart.Value:dd.MM.yyyy} по {dtpEnd.Value:dd.MM.yyyy}", Docking.Top, new Font("Segoe UI", 12F, FontStyle.Bold), Color.Teal);
                    chartSales.Titles.Add(mainTitle);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки графика продаж: " + ex.Message);
            }
        }
    }
}