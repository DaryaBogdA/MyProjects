using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using TattooYou.Database;
using TattooYou.Helpers;

namespace TattooYou.Forms
{
    public partial class MainForm : Form
    {
        private bool _returningToLogin;

        private Label lblCardMonthRevenue;
        private Label lblCardYearRevenue;
        private Label lblCardMonthCount;
        private Label lblCardYearCount;
        private Dictionary<int, decimal> _chartData = new Dictionary<int, decimal>();

        public MainForm()
        {
            InitializeComponent();
            SetupHeaderLayout();
            FixTabPageLayouts();
            InitStatisticsCards();
            LoadUserData();
            ConfigureTabsByRole();
            ApplyColorScheme();
            LoadServices();
            LoadMasters();
            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
        }

        private void InitStatisticsCards()
        {
            lblCardMonthRevenue = new Label();
            lblCardYearRevenue = new Label();
            lblCardMonthCount = new Label();
            lblCardYearCount = new Label();

            var cards = new[]
            {
                UiTheme.CreateStatCard("Выручка за месяц", lblCardMonthRevenue, 200, 88),
                UiTheme.CreateStatCard("Выручка за год", lblCardYearRevenue, 200, 88),
                UiTheme.CreateStatCard("Записей за месяц", lblCardMonthCount, 180, 88),
                UiTheme.CreateStatCard("Записей за год", lblCardYearCount, 180, 88)
            };

            int x = 0;
            foreach (var card in cards)
            {
                card.Location = new Point(x, 4);
                panelStatsCards.Controls.Add(card);
                x += card.Width + 4;
            }
        }

        private void ApplyColorScheme()
        {
            BackColor = UiTheme.Background;
            panelHeader.BackColor = UiTheme.HeaderBg;
            lblWelcome.ForeColor = UiTheme.TextPrimary;

            UiTheme.ApplyPrimaryButton(btnLogout);
            UiTheme.ApplyPrimaryButton(btnBook);
            UiTheme.ApplyPrimaryButton(btnBookFromMaster);
            UiTheme.ApplyPrimaryButton(btnExportMyAppointments);
            UiTheme.ApplyPrimaryButton(btnRefreshStats);
            UiTheme.ApplyPrimaryButton(btnStyles);
            UiTheme.ApplyPrimaryButton(btnMasters);
            UiTheme.ApplyPrimaryButton(btnServicesAdmin);
            UiTheme.ApplyPrimaryButton(btnUsersAdmin);
            UiTheme.ApplyPrimaryButton(btnAllAppointments);

            tabControl.BackColor = UiTheme.Background;
            tabControl.ForeColor = UiTheme.TextPrimary;

            foreach (TabPage page in tabControl.TabPages)
                page.BackColor = UiTheme.Background;

            UiTheme.ApplyDataGrid(dgvServices);
            UiTheme.ApplyDataGrid(dgvMastersList);
            UiTheme.ApplyDataGrid(dgvMyAppointments);
            UiTheme.ApplyDataGrid(dgvMonthlyStats);
            UiTheme.ApplyDataGrid(dgvYearlyStats);

            lblStatsYear.ForeColor = UiTheme.TextMuted;
            lblChartTitle.ForeColor = UiTheme.AccentDark;
            lblMonthlyTable.ForeColor = UiTheme.AccentDark;
            lblYearlyTable.ForeColor = UiTheme.AccentDark;
            pnlChart.BackColor = UiTheme.AccentLight;

            panelHeader.Paint += (s, e) =>
            {
                using (var brush = new SolidBrush(UiTheme.Accent))
                    e.Graphics.FillRectangle(brush, 0, panelHeader.Height - 3, panelHeader.Width, 3);
            };

        }

        private void SetupHeaderLayout()
        {
            lblWelcome.AutoSize = false;
            lblWelcome.TextAlign = ContentAlignment.MiddleLeft;
            btnLogout.Text = "Выйти";
            btnLogout.Size = new Size(110, 36);
            btnLogout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panelHeader.Resize += (s, e) => PositionHeaderControls();
            PositionHeaderControls();
        }

        private void PositionHeaderControls()
        {
            int pad = 12;
            btnLogout.Location = new Point(panelHeader.ClientSize.Width - btnLogout.Width - pad, 10);
            lblWelcome.Location = new Point(pad, 12);
            lblWelcome.Size = new Size(Math.Max(100, btnLogout.Left - pad * 2), 28);
        }

        private void FixTabPageLayouts()
        {
            const int bottomBarHeight = 48;
            const int padding = 6;

            LayoutTabWithBottomBar(tabServices, dgvServices, bottomBarHeight, padding, btnBook);
            LayoutTabWithBottomBar(tabMyAppointments, dgvMyAppointments, bottomBarHeight, padding, btnExportMyAppointments);
            LayoutMastersTab();

            tabServices.Resize += (s, e) => LayoutTabWithBottomBar(tabServices, dgvServices, bottomBarHeight, padding, btnBook);
            tabMyAppointments.Resize += (s, e) => LayoutTabWithBottomBar(tabMyAppointments, dgvMyAppointments, bottomBarHeight, padding, btnExportMyAppointments);
            tabMasters.Resize += (s, e) => LayoutMastersTab();
        }

        private void LayoutMastersTab()
        {
            const int padding = 6;
            const int rightPanelWidth = 220;
            const int bottomBarHeight = 48;

            if (tabMasters.ClientSize.Width <= 0 || tabMasters.ClientSize.Height <= 0)
                return;

            int barTop = tabMasters.ClientSize.Height - bottomBarHeight;
            int contentHeight = barTop - padding;
            int listWidth = Math.Max(200, tabMasters.ClientSize.Width - rightPanelWidth - padding * 3);

            dgvMastersList.SetBounds(padding, padding, listWidth, Math.Max(80, contentHeight));
            dgvMastersList.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;

            int rightX = padding + listWidth + padding;
            pbMasterPhoto.SetBounds(rightX, padding, rightPanelWidth, Math.Min(200, contentHeight - 50));
            pbMasterPhoto.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            btnBookFromMaster.Location = new Point(rightX, barTop + (bottomBarHeight - btnBookFromMaster.Height) / 2);
            btnBookFromMaster.Width = rightPanelWidth;
            btnBookFromMaster.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnBookFromMaster.BringToFront();
        }

        private static void LayoutTabWithBottomBar(TabPage tab, DataGridView grid, int bottomBarHeight, int padding, params Control[] bottomControls)
        {
            if (tab.ClientSize.Width <= 0 || tab.ClientSize.Height <= 0)
                return;

            int barTop = tab.ClientSize.Height - bottomBarHeight;
            int contentWidth = tab.ClientSize.Width - padding * 2;
            int contentHeight = barTop - padding;

            grid.SetBounds(padding, padding, Math.Max(100, contentWidth), Math.Max(80, contentHeight));
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            int x = padding;
            foreach (Control ctrl in bottomControls)
            {
                ctrl.Location = new Point(x, barTop + (bottomBarHeight - ctrl.Height) / 2);
                ctrl.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                ctrl.BringToFront();
                x += ctrl.Width + 8;
            }
        }

        private void LoadUserData()
        {
            string role = Session.CurrentUserRole == "admin" ? "администратор" : "пользователь";
            lblWelcome.Text = $"Добро пожаловать, {Session.CurrentUserName}  ·  {role}";
        }

        private void ConfigureTabsByRole()
        {
            if (Session.CurrentUserRole == "user")
            {
                tabControl.TabPages.Remove(tabStatistics);
                tabControl.TabPages.Remove(tabAdminServices);
                tabControl.TabPages.Remove(tabAdminUsers);
                tabControl.TabPages.Remove(tabAllAppointments);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            ReturnToLogin();
        }

        private void ReturnToLogin()
        {
            _returningToLogin = true;

            Session.CurrentUserId = 0;
            Session.CurrentUserName = null;
            Session.CurrentUserRole = null;

            if (Session.LoginForm != null && !Session.LoginForm.IsDisposed)
                Session.LoginForm.ReturnToLoginScreen();
            else
            {
                Session.LoginForm = new FormLogin();
                Session.LoginForm.Show();
            }

            Close();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_returningToLogin)
                return;

            Session.CurrentUserId = 0;
            Session.CurrentUserName = null;
            Session.CurrentUserRole = null;

            if (Session.LoginForm != null && !Session.LoginForm.IsDisposed)
                Session.LoginForm.ReturnToLoginScreen();
        }

        private void LoadServices()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT id, name, description, price FROM services ORDER BY name";
                var adapter = new MySqlDataAdapter(query, conn);
                var dt = new DataTable();
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
                LoadServices();
            else if (tabControl.SelectedTab == tabMasters)
                LoadMasters();
            else if (tabControl.SelectedTab == tabMyAppointments)
                LoadMyAppointments();
            else if (tabControl.SelectedTab == tabStatistics)
                LoadStatistics();
        }

        private void LoadStatistics()
        {
            var years = StatisticsHelper.GetAvailableYears();
            int selectedYear = DateTime.Now.Year;

            cmbStatsYear.SelectedIndexChanged -= cmbStatsYear_SelectedIndexChanged;
            cmbStatsYear.Items.Clear();
            foreach (int y in years)
                cmbStatsYear.Items.Add(y);

            if (cmbStatsYear.Items.Count > 0)
            {
                if (years.Contains(DateTime.Now.Year))
                    cmbStatsYear.SelectedItem = DateTime.Now.Year;
                else
                    cmbStatsYear.SelectedIndex = 0;
                selectedYear = (int)cmbStatsYear.SelectedItem;
            }
            cmbStatsYear.SelectedIndexChanged += cmbStatsYear_SelectedIndexChanged;

            var monthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);
            var yearStart = new DateTime(selectedYear, 1, 1);
            var yearEnd = new DateTime(selectedYear, 12, 31);

            var monthSummary = StatisticsHelper.GetSummaryForPeriod(monthStart, monthEnd);
            var yearSummary = StatisticsHelper.GetSummaryForPeriod(yearStart, yearEnd);

            lblCardMonthRevenue.Text = $"{monthSummary.TotalRevenue:N2} BYN";
            lblCardYearRevenue.Text = $"{yearSummary.TotalRevenue:N2} BYN";
            lblCardMonthCount.Text = monthSummary.AppointmentCount.ToString();
            lblCardYearCount.Text = yearSummary.AppointmentCount.ToString();

            dgvMonthlyStats.DataSource = StatisticsHelper.GetMonthlyStatsForYear(selectedYear);
            dgvYearlyStats.DataSource = StatisticsHelper.GetYearlyStats();

            _chartData = StatisticsHelper.GetMonthlyRevenueMap(selectedYear);
            pnlChart.Invalidate();
        }

        private void cmbStatsYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbStatsYear.SelectedItem == null) return;
            int year = (int)cmbStatsYear.SelectedItem;

            var yearStart = new DateTime(year, 1, 1);
            var yearEnd = new DateTime(year, 12, 31);
            var yearSummary = StatisticsHelper.GetSummaryForPeriod(yearStart, yearEnd);
            lblCardYearRevenue.Text = $"{yearSummary.TotalRevenue:N2} BYN";
            lblCardYearCount.Text = yearSummary.AppointmentCount.ToString();

            dgvMonthlyStats.DataSource = StatisticsHelper.GetMonthlyStatsForYear(year);
            _chartData = StatisticsHelper.GetMonthlyRevenueMap(year);
            pnlChart.Invalidate();
        }

        private void btnRefreshStats_Click(object sender, EventArgs e)
        {
            LoadStatistics();
        }

        private void pnlChart_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int padLeft = 36;
            int padRight = 12;
            int padTop = 12;
            int padBottom = 28;
            int chartW = pnlChart.Width - padLeft - padRight;
            int chartH = pnlChart.Height - padTop - padBottom;

            if (chartW <= 0 || chartH <= 0) return;

            decimal maxVal = _chartData.Values.DefaultIfEmpty(0).Max();
            if (maxVal <= 0) maxVal = 1;

            float barW = chartW / 12f - 4f;
            string[] months = { "Я", "Ф", "М", "А", "М", "И", "И", "А", "С", "О", "Н", "Д" };

            for (int i = 1; i <= 12; i++)
            {
                decimal val = _chartData.ContainsKey(i) ? _chartData[i] : 0;
                float barH = (float)((double)val / (double)maxVal * chartH);
                float x = padLeft + (i - 1) * (chartW / 12f) + 2;
                float y = padTop + chartH - barH;

                using (var brush = new SolidBrush(UiTheme.Accent))
                    g.FillRectangle(brush, x, y, barW, barH);

                using (var font = new Font("Segoe UI", 7F))
                using (var brush = new SolidBrush(UiTheme.TextMuted))
                    g.DrawString(months[i - 1], font, brush, x, padTop + chartH + 4);
            }

            using (var font = new Font("Segoe UI", 7.5F))
            using (var brush = new SolidBrush(UiTheme.AccentDark))
                g.DrawString($"макс. {maxVal:N0} BYN", font, brush, padLeft, 2);
        }

        private void btnExportMyAppointments_Click(object sender, EventArgs e)
        {
            if (dgvMyAppointments.DataSource == null)
            {
                MessageBox.Show("Нет данных для экспорта.");
                return;
            }

            var sfd = new SaveFileDialog
            {
                Filter = "CSV файлы (*.csv)|*.csv|Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
                DefaultExt = "csv",
                FileName = $"MyAppointments_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var dt = (DataTable)dgvMyAppointments.DataSource;
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
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", Session.CurrentUserId);
                var adapter = new MySqlDataAdapter(cmd);
                var dt = new DataTable();
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
                var adapter = new MySqlDataAdapter(query, conn);
                var dt = new DataTable();
                adapter.Fill(dt);
                dgvMastersList.DataSource = dt;

                if (dgvMastersList.Columns.Contains("photo_url"))
                    dgvMastersList.Columns["photo_url"].Visible = false;
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
            var form = new FormBookAppointment(masterId);
            form.ShowDialog();
        }

        private void btnBook_Click(object sender, EventArgs e)
        {
            var form = new FormBookAppointment();
            form.ShowDialog();
        }

        private void btnStyles_Click(object sender, EventArgs e)
        {
            new FormStyles().ShowDialog();
        }

        private void btnMasters_Click(object sender, EventArgs e)
        {
            new FormMasters().ShowDialog();
        }

        private void btnServicesAdmin_Click(object sender, EventArgs e)
        {
            new FormManageServices().ShowDialog();
        }

        private void btnUsersAdmin_Click(object sender, EventArgs e)
        {
            new FormManageUsers().ShowDialog();
        }

        private void btnAllAppointments_Click(object sender, EventArgs e)
        {
            new FormAllAppointments().ShowDialog();
        }
    }
}
