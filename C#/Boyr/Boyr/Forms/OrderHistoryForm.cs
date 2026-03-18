using Boyr.DataAccess;
using Boyr.Entities;
using Boyr.Services;
using Boyr.UI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Boyr.Forms
{
    public partial class OrderHistoryForm : Form
    {
        private SaleRepository saleRepo = new SaleRepository();
        private List<Sale> _sales;

        public OrderHistoryForm()
        {
            InitializeComponent();
            UiTheme.Apply(this);
            LoadOrders();
        }

        private void LoadOrders()
        {
            _sales = saleRepo.GetByUser(AuthService.CurrentUser.Id);
            dataGridView.Rows.Clear();
            foreach (var sale in _sales)
            {
                dataGridView.Rows.Add(
                    sale.Id,
                    sale.SaleDate.ToShortDateString(),
                    sale.CustomerName,
                    sale.TotalAmount.ToString("C")
                );
            }
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null) return;
            int saleId = Convert.ToInt32(dataGridView.CurrentRow.Cells[0].Value);
            LoadSaleItems(saleId);
        }

        private void LoadSaleItems(int saleId)
        {
            string query = @"SELECT p.name, p.metal, p.purity, p.weight, p.gemstone, si.quantity, si.price_at_moment 
                             FROM sale_items si
                             JOIN products p ON si.product_id = p.id
                             WHERE si.sale_id = @saleId";
            var param = new MySqlParameter("@saleId", saleId);
            DataTable dt = DatabaseHelper.ExecuteQuery(query, param);
            dataGridViewItems.Rows.Clear();
            foreach (DataRow row in dt.Rows)
            {
                dataGridViewItems.Rows.Add(
                    row["name"],
                    $"{row["metal"]} {row["purity"]}",
                    row["weight"],
                    row["gemstone"],
                    row["quantity"],
                    Convert.ToDecimal(row["price_at_moment"]).ToString("C")
                );
            }
        }
    }
}