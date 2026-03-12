using MySql.Data.MySqlClient;
using paseka.DataAccess;
using paseka.Helpers;
using System;
using System.Data;
using System.Windows.Forms;

namespace paseka
{
    public partial class InventoryForm : Form
    {
        public InventoryForm()
        {
            InitializeComponent();
            ThemeHelper.ApplyDarkTheme(this);
            this.StartPosition = FormStartPosition.CenterScreen;
            LoadFood();
        }

        private void LoadFood()
        {
            string query = "SELECT id, name, quantity FROM services WHERE type='food' ORDER BY name";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            dgvFood.DataSource = dt;
            dgvFood.Columns["id"].HeaderText = "ID";
            dgvFood.Columns["name"].HeaderText = "Наименование";
            dgvFood.Columns["quantity"].HeaderText = "Количество";
        }

        private void btnUpdateQuantity_Click(object sender, EventArgs e)
        {
            if (dgvFood.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(dgvFood.SelectedRows[0].Cells["id"].Value);
            string newQty = Microsoft.VisualBasic.Interaction.InputBox("Введите новое количество:", "Изменение количества", "0");
            if (int.TryParse(newQty, out int quantity))
            {
                string query = "UPDATE services SET quantity = @qty WHERE id = @id";
                DatabaseHelper.ExecuteNonQuery(query,
                    new MySqlParameter("@qty", quantity),
                    new MySqlParameter("@id", id));
                LoadFood();
            }
            else
            {
                MessageBox.Show("Введите целое число.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}