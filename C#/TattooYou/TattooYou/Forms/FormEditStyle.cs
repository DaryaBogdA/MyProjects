using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;
using TattooYou.Database;

namespace TattooYou.Forms
{
    public partial class FormEditStyle : Form
    {
        private int? styleId;

        public FormEditStyle()
        {
            InitializeComponent();
            ApplyColorScheme();
        }

        public FormEditStyle(int id) : this()
        {
            styleId = id;
            LoadStyleData();
        }

        private void ApplyColorScheme()
        {
            this.BackColor = Color.White;
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Label lbl)
                    lbl.ForeColor = Color.Black;
                if (ctrl is Button btn)
                {
                    btn.BackColor = Color.FromArgb(155, 89, 182);
                    btn.ForeColor = Color.White;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                }
            }
        }

        private void LoadStyleData()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT name, description FROM styles WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", styleId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtName.Text = reader.GetString("name");
                        txtDescription.Text = reader.IsDBNull(1) ? "" : reader.GetString("description");
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название стиля");
                return;
            }

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                if (styleId == null)
                {
                    string insert = "INSERT INTO styles (name, description) VALUES (@name, @description)";
                    MySqlCommand cmd = new MySqlCommand(insert, conn);
                    cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@description", txtDescription.Text.Trim());
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    string update = "UPDATE styles SET name = @name, description = @description WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(update, conn);
                    cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@description", txtDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", styleId);
                    cmd.ExecuteNonQuery();
                }
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}