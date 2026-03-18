using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using TattooYou.Database;
using TattooYou.Helpers;

namespace TattooYou.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            LoadUserData();
            ConfigureTabsByRole();
            ApplyColorScheme();
            LoadServices();
            LoadMasters();
            this.tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
        }

        private void ApplyColorScheme()
        {
            this.BackColor = Color.White;
            lblWelcome.ForeColor = Color.Black;
            btnLogout.BackColor = Color.FromArgb(155, 89, 182);
            btnLogout.ForeColor = Color.White;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.FlatAppearance.BorderSize = 0;
            tabControl.BackColor = Color.White;
            tabControl.ForeColor = Color.Black;

            foreach (TabPage page in tabControl.TabPages)
            {
                page.BackColor = Color.White;
            }

            dgvServices.BackgroundColor = Color.White;
            dgvServices.DefaultCellStyle.BackColor = Color.White;
            dgvServices.DefaultCellStyle.ForeColor = Color.Black;
            dgvServices.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dgvServices.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvServices.EnableHeadersVisualStyles = false;

            dgvMastersList.BackgroundColor = Color.White;
            dgvMastersList.DefaultCellStyle.BackColor = Color.White;
            dgvMastersList.DefaultCellStyle.ForeColor = Color.Black;
            dgvMastersList.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dgvMastersList.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvMastersList.EnableHeadersVisualStyles = false;

            dgvMyAppointments.BackgroundColor = Color.White;
            dgvMyAppointments.DefaultCellStyle.BackColor = Color.White;
            dgvMyAppointments.DefaultCellStyle.ForeColor = Color.Black;
            dgvMyAppointments.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dgvMyAppointments.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvMyAppointments.EnableHeadersVisualStyles = false;
        }

        private void LoadUserData()
        {
            lblWelcome.Text = $"Добро пожаловать, {Session.CurrentUserName} ({(Session.CurrentUserRole == "admin" ? "администратор" : "пользователь")})";
        }

        private void ConfigureTabsByRole()
        {
            if (Session.CurrentUserRole == "user")
            {
                tabControl.TabPages.Remove(tabAdminServices);
                tabControl.TabPages.Remove(tabAdminUsers);
                tabControl.TabPages.Remove(tabAllAppointments);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Session.CurrentUserId = 0;
            Session.CurrentUserName = null;
            Session.CurrentUserRole = null;

            FormLogin loginForm = new FormLogin();
            loginForm.Show();
            this.Close();
        }

        private void LoadServices()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT id, name, description, price FROM services ORDER BY name";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvServices.DataSource = dt;

                if (dgvServices.Columns.Count > 0)
                {
                    dgvServices.Columns["id"].HeaderText = "ID";
                    dgvServices.Columns["name"].HeaderText = "Название";
                    dgvServices.Columns["description"].HeaderText = "Описание";
                    dgvServices.Columns["price"].HeaderText = "Цена (BYN)";
                }
            }
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabServices)
            {
                LoadServices();
            }
            else if (tabControl.SelectedTab == tabMasters)
            {
                LoadMasters();
            }
            else if (tabControl.SelectedTab == tabMyAppointments)
            {
                LoadMyAppointments();
            }
        }
        private void btnExportMyAppointments_Click(object sender, EventArgs e)
        {
            if (dgvMyAppointments.DataSource == null)
            {
                MessageBox.Show("Нет данных для экспорта.");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV файлы (*.csv)|*.csv|Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            sfd.DefaultExt = "csv";
            sfd.FileName = $"MyAppointments_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                DataTable dt = (DataTable)dgvMyAppointments.DataSource;
                ExportHelper.DataTableToCsv(dt, sfd.FileName);
                MessageBox.Show("Данные сохранены.", "Экспорт", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void LoadMyAppointments()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT a.id, a.appointment_date AS Дата, a.appointment_time AS Время,
                           s.name AS Услуга, s.price AS Цена, st.name AS Стиль, m.full_name AS Мастер,
                           a.size AS Размер, a.status AS Статус
                    FROM appointments a
                    JOIN services s ON a.service_id = s.id
                    LEFT JOIN styles st ON a.style_id = st.id
                    LEFT JOIN masters m ON a.master_id = m.id
                    WHERE a.user_id = @userId
                    ORDER BY a.appointment_date DESC, a.appointment_time DESC";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", Session.CurrentUserId);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgvMyAppointments.DataSource = dt;
            }
        }

        private void LoadMasters()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT m.id, m.full_name AS ФИО, 
                           GROUP_CONCAT(s.name SEPARATOR ', ') AS Стили,
                           m.photo_url
                    FROM masters m
                    LEFT JOIN master_styles ms ON m.id = ms.master_id
                    LEFT JOIN styles s ON ms.style_id = s.id
                    WHERE m.is_active = 1
                    GROUP BY m.id
                    ORDER BY m.full_name";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvMastersList.DataSource = dt;

                if (dgvMastersList.Columns.Contains("photo_url"))
                {
                    dgvMastersList.Columns["photo_url"].Visible = false;
                }
            }
        }

        private void dgvMastersList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMastersList.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvMastersList.SelectedRows[0];
                if (row.Cells["photo_url"].Value != DBNull.Value)
                {
                    string photoUrl = row.Cells["photo_url"].Value.ToString();
                    try
                    {
                        pbMasterPhoto.LoadAsync(photoUrl);
                    }
                    catch
                    {
                        pbMasterPhoto.Image = null;
                    }
                }
                else
                {
                    pbMasterPhoto.Image = null;
                }
            }
        }

        private void btnBookFromMaster_Click(object sender, EventArgs e)
        {
            if (dgvMastersList.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите мастера из списка");
                return;
            }
            int masterId = Convert.ToInt32(dgvMastersList.SelectedRows[0].Cells["id"].Value);
            FormBookAppointment form = new FormBookAppointment(masterId);
            form.ShowDialog();
        }

        private void btnBook_Click(object sender, EventArgs e)
        {
            FormBookAppointment form = new FormBookAppointment();
            form.ShowDialog();
        }

        private void btnStyles_Click(object sender, EventArgs e)
        {
            FormStyles form = new FormStyles();
            form.ShowDialog();
        }

        private void btnMasters_Click(object sender, EventArgs e)
        {
            FormMasters form = new FormMasters();
            form.ShowDialog();
        }

        private void btnServicesAdmin_Click(object sender, EventArgs e)
        {
            FormManageServices form = new FormManageServices();
            form.ShowDialog();
        }

        private void btnUsersAdmin_Click(object sender, EventArgs e)
        {
            FormManageUsers form = new FormManageUsers();
            form.ShowDialog();
        }

        private void btnAllAppointments_Click(object sender, EventArgs e)
        {
            FormAllAppointments form = new FormAllAppointments();
            form.ShowDialog();
        }
    }
}