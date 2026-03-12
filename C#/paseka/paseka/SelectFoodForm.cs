using MySql.Data.MySqlClient;
using paseka.DataAccess;
using paseka.Helpers;
using paseka.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace paseka
{
    public partial class SelectFoodForm : Form
    {
        private int bookingId;
        private List<SelectedFoodItem> selectedItems = new List<SelectedFoodItem>();

        public SelectFoodForm(int bookingId)
        {
            InitializeComponent();
            ThemeHelper.ApplyDarkTheme(this);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.bookingId = bookingId;
            LoadFood();
        }

        private void LoadFood()
        {
            string query = "SELECT id, name, price FROM services WHERE type='food' AND is_available=1 ORDER BY name";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            dgvFood.Rows.Clear();
            foreach (DataRow row in dt.Rows)
            {
                int rowIndex = dgvFood.Rows.Add();
                DataGridViewRow dgvRow = dgvFood.Rows[rowIndex];
                dgvRow.Cells["colSelect"].Value = false;
                dgvRow.Cells["colName"].Value = row["name"].ToString();
                dgvRow.Cells["colPrice"].Value = Convert.ToDecimal(row["price"]).ToString("F2");
                dgvRow.Cells["colQuantity"].Value = 1;
                dgvRow.Tag = Convert.ToInt32(row["id"]);
            }
        }

        public List<SelectedFoodItem> GetSelectedItems()
        {
            return selectedItems;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            selectedItems.Clear();
            foreach (DataGridViewRow row in dgvFood.Rows)
            {
                bool selected = Convert.ToBoolean(row.Cells["colSelect"].Value ?? false);
                if (selected)
                {
                    int serviceId = (int)row.Tag;
                    string name = row.Cells["colName"].Value.ToString();
                    decimal price = Convert.ToDecimal(row.Cells["colPrice"].Value);
                    int quantity = Convert.ToInt32(row.Cells["colQuantity"].Value);
                    if (quantity <= 0) quantity = 1;

                    selectedItems.Add(new SelectedFoodItem
                    {
                        ServiceId = serviceId,
                        Name = name,
                        Price = price,
                        Quantity = quantity
                    });
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}