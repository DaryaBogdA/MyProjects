using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TattooYou.Database;
using TattooYou.Models;

namespace TattooYou.Forms
{
    public partial class FormEditMaster : Form
    {
        private int? masterId;
        private List<Style> allStyles = new List<Style>();

        public FormEditMaster()
        {
            InitializeComponent();
            ApplyColorScheme();
            LoadStyles();
        }

        public FormEditMaster(int id) : this()
        {
            masterId = id;
            LoadMasterData();
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

        private void LoadStyles()
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
            clbStyles.DataSource = allStyles;
            clbStyles.DisplayMember = "Name";
            clbStyles.ValueMember = "Id";
        }

        private void LoadMasterData()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT full_name, bio, photo_url, is_active FROM masters WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", masterId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtFullName.Text = reader.GetString("full_name");
                        txtBio.Text = reader.IsDBNull(1) ? "" : reader.GetString("bio");
                        txtPhotoUrl.Text = reader.IsDBNull(2) ? "" : reader.GetString("photo_url");
                        chkIsActive.Checked = reader.GetBoolean("is_active");
                    }
                }
            }

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT style_id FROM master_styles WHERE master_id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", masterId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int styleId = reader.GetInt32("style_id");
                        for (int i = 0; i < clbStyles.Items.Count; i++)
                        {
                            var style = clbStyles.Items[i] as Style;
                            if (style.Id == styleId)
                                clbStyles.SetItemChecked(i, true);
                        }
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Введите ФИО мастера");
                return;
            }

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                MySqlTransaction trans = conn.BeginTransaction();

                try
                {
                    if (masterId == null)
                    {
                        string insertMaster = @"
                            INSERT INTO masters (full_name, bio, photo_url, is_active)
                            VALUES (@full_name, @bio, @photo_url, @is_active);
                            SELECT LAST_INSERT_ID();";
                        MySqlCommand cmd = new MySqlCommand(insertMaster, conn, trans);
                        cmd.Parameters.AddWithValue("@full_name", txtFullName.Text.Trim());
                        cmd.Parameters.AddWithValue("@bio", txtBio.Text.Trim());
                        cmd.Parameters.AddWithValue("@photo_url", txtPhotoUrl.Text.Trim());
                        cmd.Parameters.AddWithValue("@is_active", chkIsActive.Checked);
                        masterId = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        string updateMaster = @"
                            UPDATE masters 
                            SET full_name = @full_name, bio = @bio, photo_url = @photo_url, is_active = @is_active
                            WHERE id = @id";
                        MySqlCommand cmd = new MySqlCommand(updateMaster, conn, trans);
                        cmd.Parameters.AddWithValue("@full_name", txtFullName.Text.Trim());
                        cmd.Parameters.AddWithValue("@bio", txtBio.Text.Trim());
                        cmd.Parameters.AddWithValue("@photo_url", txtPhotoUrl.Text.Trim());
                        cmd.Parameters.AddWithValue("@is_active", chkIsActive.Checked);
                        cmd.Parameters.AddWithValue("@id", masterId);
                        cmd.ExecuteNonQuery();

                        string deleteStyles = "DELETE FROM master_styles WHERE master_id = @id";
                        MySqlCommand delCmd = new MySqlCommand(deleteStyles, conn, trans);
                        delCmd.Parameters.AddWithValue("@id", masterId);
                        delCmd.ExecuteNonQuery();
                    }

                    foreach (var item in clbStyles.CheckedItems)
                    {
                        var style = item as Style;
                        string insertStyle = "INSERT INTO master_styles (master_id, style_id) VALUES (@master_id, @style_id)";
                        MySqlCommand styleCmd = new MySqlCommand(insertStyle, conn, trans);
                        styleCmd.Parameters.AddWithValue("@master_id", masterId);
                        styleCmd.Parameters.AddWithValue("@style_id", style.Id);
                        styleCmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                    DialogResult = DialogResult.OK;
                    Close();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("Ошибка сохранения: " + ex.Message);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}