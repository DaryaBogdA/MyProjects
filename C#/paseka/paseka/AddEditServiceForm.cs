using MySql.Data.MySqlClient;
using paseka.DataAccess;
using paseka.Helpers;
using System;
using System.Data;
using System.Windows.Forms;

namespace paseka
{
    public partial class AddEditServiceForm : Form
    {
        private int serviceId = 0;

        public AddEditServiceForm()
        {
            InitializeComponent();
            ThemeHelper.ApplyDarkTheme(this);
            this.StartPosition = FormStartPosition.CenterScreen;

            TextBoxWatermark.Set(txtName, "Название");
            TextBoxWatermark.Set(txtPrice, "0.00");
            TextBoxWatermark.Set(txtDuration, "0");
            TextBoxWatermark.Set(txtDescription, "Описание");
            TextBoxWatermark.Set(txtQuantity, "0");

            this.Text = "Добавление услуги";
        }

        public AddEditServiceForm(int id) : this()
        {
            serviceId = id;
            this.Text = "Редактирование услуги";
            LoadServiceData();
        }

        private void LoadServiceData()
        {
            string query = "SELECT * FROM services WHERE id = @id";
            var dt = DatabaseHelper.ExecuteQuery(query, new MySqlParameter("@id", serviceId));
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                txtName.Text = row["name"].ToString();
                cmbType.SelectedItem = row["type"].ToString();
                txtPrice.Text = row["price"].ToString();
                txtDuration.Text = row["duration"].ToString();
                txtDescription.Text = row["description"].ToString();
                txtQuantity.Text = row["quantity"].ToString();
                chkIsRoom.Checked = Convert.ToBoolean(row["is_room"]);
                chkIsAvailable.Checked = Convert.ToBoolean(row["is_available"]);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Заполните обязательные поля (Название, Цена)", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query;
            MySqlParameter[] parameters;

            if (serviceId == 0)
            {
                query = @"INSERT INTO services (name, type, price, duration, description, quantity, is_room, is_available)
                          VALUES (@name, @type, @price, @duration, @description, @quantity, @is_room, @is_available)";
            }
            else
            {
                query = @"UPDATE services SET name=@name, type=@type, price=@price, duration=@duration,
                          description=@description, quantity=@quantity, is_room=@is_room, is_available=@is_available
                          WHERE id=@id";
            }

            parameters = new MySqlParameter[]
            {
                new MySqlParameter("@name", txtName.Text.Trim()),
                new MySqlParameter("@type", cmbType.SelectedItem?.ToString() ?? "game"),
                new MySqlParameter("@price", Convert.ToDecimal(txtPrice.Text)),
                new MySqlParameter("@duration", Convert.ToInt32(txtDuration.Text)),
                new MySqlParameter("@description", txtDescription.Text.Trim()),
                new MySqlParameter("@quantity", Convert.ToInt32(txtQuantity.Text)),
                new MySqlParameter("@is_room", chkIsRoom.Checked),
                new MySqlParameter("@is_available", chkIsAvailable.Checked)
            };

            if (serviceId != 0)
            {
                Array.Resize(ref parameters, parameters.Length + 1);
                parameters[parameters.Length - 1] = new MySqlParameter("@id", serviceId);
            }

            try
            {
                DatabaseHelper.ExecuteNonQuery(query, parameters);
                MessageBox.Show("Данные сохранены", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}