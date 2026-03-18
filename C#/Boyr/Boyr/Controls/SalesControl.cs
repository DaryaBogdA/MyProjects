using Boyr.DataAccess;
using Boyr.UI;
using System;
using System.Windows.Forms;

namespace Boyr.Controls
{
    public partial class SalesControl : UserControl
    {
        private SaleRepository saleRepo = new SaleRepository();

        public SalesControl()
        {
            InitializeComponent();
            UiTheme.ApplyToControlTree(this);
            LoadSales();
        }

        private void LoadSales()
        {
            try
            {
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