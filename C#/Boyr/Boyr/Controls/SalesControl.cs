using Boyr.DataAccess;
using Boyr.UI;
using System;
using System.Windows.Forms;

namespace Boyr.Controls
{
    public partial class SalesControl : UserControl
    {
        private SalesChartControl salesChart;

        public SalesControl()
        {
            InitializeComponent();
            UiTheme.ApplyToControlTree(this);

            salesChart = new SalesChartControl();
            salesChart.Dock = DockStyle.Fill;
            tabChart.Controls.Add(salesChart);

            LoadSales();
        }

        private void LoadSales()
        {
            try
            {
                var saleRepo = new SaleRepository();
                dataGridViewSales.AutoGenerateColumns = true;
                dataGridViewSales.DataSource = null;
                dataGridViewSales.DataSource = saleRepo.GetAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки продаж: " + ex.Message);
            }
        }
    }
}