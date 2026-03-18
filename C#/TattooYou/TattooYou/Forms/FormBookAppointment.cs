using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TattooYou.Database;
using TattooYou.Helpers;
using TattooYou.Models;

namespace TattooYou.Forms
{
    public partial class FormBookAppointment : Form
    {
        private List<Service> services = new List<Service>();
        private List<Style> allStyles = new List<Style>();
        private List<Style> masterStyles = new List<Style>();
        private List<Master> masters = new List<Master>();
        private int selectedServiceId;
        private int selectedStyleId;
        private int selectedMasterId;
        private string selectedSize = "medium";
        private int? preselectedMasterId;

        public FormBookAppointment() : this(null) { }

        public FormBookAppointment(int? masterId)
        {
            InitializeComponent();
            ApplyColorScheme();
            preselectedMasterId = masterId;
            LoadServices();
            LoadStyles();
            if (preselectedMasterId.HasValue)
            {
                LoadMasterData(preselectedMasterId.Value);
                cmbMaster.Enabled = false;
            }
            LoadAvailableDates();
            cmbSize.SelectedIndex = 1;
            UpdatePrice();
        }

        private void ApplyColorScheme()
        {
            this.BackColor = Color.White;
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Label) ctrl.ForeColor = Color.Black;
                if (ctrl is Button btn)
                {
                    btn.BackColor = Color.FromArgb(155, 89, 182);
                    btn.ForeColor = Color.White;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                }
            }
        }

        private void LoadServices()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT id, name FROM services ORDER BY name";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        services.Add(new Service
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name")
                        });
                    }
                }
            }
            cmbService.DataSource = services;
            cmbService.DisplayMember = "Name";
            cmbService.ValueMember = "Id";
        }

        private void LoadStyles()
        {
            if (preselectedMasterId.HasValue)
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT s.id, s.name
                        FROM styles s
                        JOIN master_styles ms ON s.id = ms.style_id
                        WHERE ms.master_id = @masterId
                        ORDER BY s.name";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@masterId", preselectedMasterId.Value);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            masterStyles.Add(new Style
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.GetString("name")
                            });
                        }
                    }
                }
                cmbStyle.DataSource = masterStyles;
            }
            else
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT id, name FROM styles ORDER BY name";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            allStyles.Add(new Style
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.GetString("name")
                            });
                        }
                    }
                }
                cmbStyle.DataSource = allStyles;
            }
            cmbStyle.DisplayMember = "Name";
            cmbStyle.ValueMember = "Id";
        }

        private void LoadMasterData(int masterId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT full_name FROM masters WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", masterId);
                object result = cmd.ExecuteScalar();
            }
            selectedMasterId = masterId;
            LoadMastersByStyle(0);
        }

        private void LoadMastersByStyle(int styleId)
        {
            masters.Clear();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query;
                if (preselectedMasterId.HasValue)
                {
                    query = "SELECT id, full_name FROM masters WHERE id = @masterId AND is_active = 1";
                }
                else
                {
                    query = @"
                        SELECT m.id, m.full_name 
                        FROM masters m
                        JOIN master_styles ms ON m.id = ms.master_id
                        WHERE ms.style_id = @styleId AND m.is_active = 1
                        ORDER BY m.full_name";
                }
                MySqlCommand cmd = new MySqlCommand(query, conn);
                if (preselectedMasterId.HasValue)
                {
                    cmd.Parameters.AddWithValue("@masterId", preselectedMasterId.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@styleId", styleId);
                }
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        masters.Add(new Master
                        {
                            Id = reader.GetInt32("id"),
                            FullName = reader.GetString("full_name")
                        });
                    }
                }
            }
            cmbMaster.DataSource = masters;
            cmbMaster.DisplayMember = "FullName";
            cmbMaster.ValueMember = "Id";
            if (preselectedMasterId.HasValue && masters.Count > 0)
            {
                cmbMaster.SelectedValue = preselectedMasterId.Value;
            }
        }

        private void LoadAvailableDates()
        {
            monthCalendar.MinDate = DateTime.Today;
            monthCalendar.MaxDate = DateTime.Today.AddMonths(1);
        }

        private void cmbStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbStyle.SelectedValue != null && cmbStyle.SelectedValue is int styleId)
            {
                selectedStyleId = styleId;
                if (!preselectedMasterId.HasValue)
                {
                    LoadMastersByStyle(styleId);
                }
            }
        }

        private void cmbService_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbService.SelectedValue != null && cmbService.SelectedValue is int serviceId)
            {
                selectedServiceId = serviceId;
                UpdatePrice();
            }
        }

        private void cmbMaster_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMaster.SelectedValue != null && cmbMaster.SelectedValue is int masterId)
            {
                selectedMasterId = masterId;
                UpdatePrice();
            }
        }

        private void cmbSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedSize = cmbSize.SelectedItem.ToString().ToLower();
        }

        private void UpdatePrice()
        {
            if (selectedServiceId > 0)
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT price FROM services WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", selectedServiceId);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        lblPrice.Text = $"Цена: {price} BYN";
                    }
                }
            }
            else
            {
                lblPrice.Text = "Цена:";
            }
        }

        private void btnBook_Click(object sender, EventArgs e)
        {
            if (selectedServiceId == 0)
            {
                MessageBox.Show("Выберите услугу");
                return;
            }
            if (selectedStyleId == 0)
            {
                MessageBox.Show("Выберите стиль");
                return;
            }
            if (selectedMasterId == 0)
            {
                MessageBox.Show("Выберите мастера");
                return;
            }
            if (monthCalendar.SelectionStart.Date < DateTime.Today)
            {
                MessageBox.Show("Выберите будущую дату");
                return;
            }
            if (string.IsNullOrEmpty(selectedSize))
            {
                MessageBox.Show("Выберите размер");
                return;
            }

            DateTime selectedDate = monthCalendar.SelectionStart.Date;
            TimeSpan selectedTime = TimeSpan.Parse(cmbTime.SelectedItem.ToString());

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string checkQuery = "SELECT COUNT(*) FROM appointments WHERE appointment_date = @date AND appointment_time = @time";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@date", selectedDate);
                checkCmd.Parameters.AddWithValue("@time", selectedTime);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (count > 0)
                {
                    MessageBox.Show("Это время уже занято, выберите другое");
                    return;
                }

                string insertQuery = @"
                    INSERT INTO appointments 
                    (appointment_date, appointment_time, user_id, service_id, style_id, master_id, size, status) 
                    VALUES (@date, @time, @userId, @serviceId, @styleId, @masterId, @size, 'confirmed')";
                MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@date", selectedDate);
                insertCmd.Parameters.AddWithValue("@time", selectedTime);
                insertCmd.Parameters.AddWithValue("@userId", Session.CurrentUserId);
                insertCmd.Parameters.AddWithValue("@serviceId", selectedServiceId);
                insertCmd.Parameters.AddWithValue("@styleId", selectedStyleId);
                insertCmd.Parameters.AddWithValue("@masterId", selectedMasterId);
                insertCmd.Parameters.AddWithValue("@size", selectedSize);

                try
                {
                    insertCmd.ExecuteNonQuery();
                    MessageBox.Show("Запись успешно создана!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при записи: " + ex.Message);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}