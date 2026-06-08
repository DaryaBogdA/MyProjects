using System;
using System.Drawing;
using System.Windows.Forms;

namespace TattooYou.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblWelcome;
        private TabControl tabControl;
        private TabPage tabServices;
        private TabPage tabMyAppointments;
        private TabPage tabAdminServices;
        private TabPage tabAdminUsers;
        private TabPage tabAllAppointments;
        private TabPage tabStatistics;
        private TabPage tabMasters;
        private Panel panelHeader;
        private Panel panelStatsCards;
        private Panel pnlChart;
        private ComboBox cmbStatsYear;
        private Button btnRefreshStats;
        private Label lblStatsYear;
        private Label lblChartTitle;
        private DataGridView dgvMonthlyStats;
        private DataGridView dgvYearlyStats;
        private Label lblMonthlyTable;
        private Label lblYearlyTable;
        private Button btnLogout;
        private DataGridView dgvServices;
        private DataGridView dgvMyAppointments;

        private Button btnBook;
        private Button btnStyles;
        private Button btnMasters;
        private Button btnServicesAdmin;
        private Button btnUsersAdmin;
        private Button btnAllAppointments;

        private DataGridView dgvMastersList;
        private PictureBox pbMasterPhoto;
        private Button btnBookFromMaster;
        private Button btnExportMyAppointments;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblWelcome = new Label();
            this.tabControl = new TabControl();
            this.tabServices = new TabPage();
            this.tabMyAppointments = new TabPage();
            this.tabAdminServices = new TabPage();
            this.tabAdminUsers = new TabPage();
            this.tabAllAppointments = new TabPage();
            this.tabStatistics = new TabPage();
            this.tabMasters = new TabPage();
            this.panelHeader = new Panel();
            this.panelStatsCards = new Panel();
            this.pnlChart = new Panel();
            this.cmbStatsYear = new ComboBox();
            this.btnRefreshStats = new Button();
            this.lblStatsYear = new Label();
            this.lblChartTitle = new Label();
            this.dgvMonthlyStats = new DataGridView();
            this.dgvYearlyStats = new DataGridView();
            this.lblMonthlyTable = new Label();
            this.lblYearlyTable = new Label();
            this.btnLogout = new Button();
            this.dgvServices = new DataGridView();
            this.dgvMyAppointments = new DataGridView();

            this.btnBook = new Button();
            this.btnStyles = new Button();
            this.btnMasters = new Button();
            this.btnServicesAdmin = new Button();
            this.btnUsersAdmin = new Button();
            this.btnAllAppointments = new Button();

            this.dgvMastersList = new DataGridView();
            this.pbMasterPhoto = new PictureBox();
            this.btnBookFromMaster = new Button();

            ((System.ComponentModel.ISupportInitialize)(this.dgvServices)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyAppointments)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMastersList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMasterPhoto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMonthlyStats)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvYearlyStats)).BeginInit();
            this.panelHeader.SuspendLayout();
            this.tabStatistics.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabServices.SuspendLayout();
            this.tabMyAppointments.SuspendLayout();
            this.tabAdminServices.SuspendLayout();
            this.tabAdminUsers.SuspendLayout();
            this.tabAllAppointments.SuspendLayout();
            this.tabMasters.SuspendLayout();
            this.SuspendLayout();
            //
            // panelHeader
            //
            this.panelHeader.Dock = DockStyle.Top;
            this.panelHeader.Height = 56;
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Padding = new Padding(12, 8, 12, 8);
            this.panelHeader.Controls.Add(this.lblWelcome);
            this.panelHeader.Controls.Add(this.btnLogout);
            //
            // lblWelcome
            //
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            this.lblWelcome.Location = new Point(4, 10);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new Size(200, 25);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "Добро пожаловать";
            this.lblWelcome.ForeColor = Color.Black;
            //
            // tabControl
            //
            this.tabControl.Controls.Add(this.tabServices);
            this.tabControl.Controls.Add(this.tabMyAppointments);
            this.tabControl.Controls.Add(this.tabMasters);
            this.tabControl.Controls.Add(this.tabStatistics);
            this.tabControl.Controls.Add(this.tabAdminServices);
            this.tabControl.Controls.Add(this.tabAdminUsers);
            this.tabControl.Controls.Add(this.tabAllAppointments);
            this.tabControl.Location = new Point(0, 52);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new Size(784, 409);
            this.tabControl.TabIndex = 1;
            this.tabControl.Dock = DockStyle.Fill;
            this.tabControl.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            this.tabControl.Padding = new Point(12, 6);
            //
            // tabServices
            //
            this.tabServices.Controls.Add(this.dgvServices);
            this.tabServices.Controls.Add(this.btnBook);
            this.tabServices.Location = new Point(4, 24);
            this.tabServices.Name = "tabServices";
            this.tabServices.Padding = new Padding(3);
            this.tabServices.Size = new Size(752, 372);
            this.tabServices.TabIndex = 0;
            this.tabServices.Text = "Услуги";
            this.tabServices.UseVisualStyleBackColor = true;
            //
            // dgvServices
            //
            this.dgvServices.AllowUserToAddRows = false;
            this.dgvServices.AllowUserToDeleteRows = false;
            this.dgvServices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvServices.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvServices.Location = new Point(6, 6);
            this.dgvServices.Name = "dgvServices";
            this.dgvServices.ReadOnly = true;
            this.dgvServices.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvServices.Size = new Size(740, 300); this.dgvServices.TabIndex = 0;
            //
            // btnExportMyAppointments
            //
            this.btnExportMyAppointments = new Button();
            this.btnExportMyAppointments.Location = new Point(160, 315);
            this.btnExportMyAppointments.Name = "btnExportMyAppointments";
            this.btnExportMyAppointments.Size = new Size(140, 35);
            this.btnExportMyAppointments.TabIndex = 2;
            this.btnExportMyAppointments.Text = "Экспорт в файл";
            this.btnExportMyAppointments.UseVisualStyleBackColor = false;
            this.btnExportMyAppointments.Click += new EventHandler(this.btnExportMyAppointments_Click);
            this.btnExportMyAppointments.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            //
            // btnBook
            //
            this.btnBook.Location = new Point(6, 315);
            this.btnBook.Name = "btnBook";
            this.btnBook.Size = new Size(120, 35);
            this.btnBook.TabIndex = 1;
            this.btnBook.Text = "Записаться";
            this.btnBook.UseVisualStyleBackColor = false;
            this.btnBook.Click += new EventHandler(this.btnBook_Click);
            this.btnBook.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            //
            // tabMyAppointments
            //
            this.tabMyAppointments.Controls.Add(this.dgvMyAppointments);
            this.tabMyAppointments.Controls.Add(this.btnExportMyAppointments);
            this.tabMyAppointments.Location = new Point(4, 24);
            this.tabMyAppointments.Name = "tabMyAppointments";
            this.tabMyAppointments.Padding = new Padding(3);
            this.tabMyAppointments.Size = new Size(752, 372);
            this.tabMyAppointments.TabIndex = 1;
            this.tabMyAppointments.Text = "Мои записи";
            this.tabMyAppointments.UseVisualStyleBackColor = true;
            //
            // dgvMyAppointments
            //
            this.dgvMyAppointments.AllowUserToAddRows = false;
            this.dgvMyAppointments.AllowUserToDeleteRows = false;
            this.dgvMyAppointments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMyAppointments.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMyAppointments.Location = new Point(6, 6);
            this.dgvMyAppointments.Name = "dgvMyAppointments";
            this.dgvMyAppointments.ReadOnly = true;
            this.dgvMyAppointments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvMyAppointments.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvMyAppointments.Size = new Size(740, 300);
            this.dgvMyAppointments.TabIndex = 0;
            //
            // tabMasters
            //
            this.tabMasters.Controls.Add(this.dgvMastersList);
            this.tabMasters.Controls.Add(this.pbMasterPhoto);
            this.tabMasters.Controls.Add(this.btnBookFromMaster);
            this.tabMasters.Location = new Point(4, 24);
            this.tabMasters.Name = "tabMasters";
            this.tabMasters.Padding = new Padding(3);
            this.tabMasters.Size = new Size(752, 372);
            this.tabMasters.TabIndex = 5;
            this.tabMasters.Text = "Мастера";
            this.tabMasters.UseVisualStyleBackColor = true;
            //
            // dgvMastersList
            //
            this.dgvMastersList.AllowUserToAddRows = false;
            this.dgvMastersList.AllowUserToDeleteRows = false;
            this.dgvMastersList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMastersList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMastersList.Location = new Point(6, 6);
            this.dgvMastersList.Name = "dgvMastersList";
            this.dgvMastersList.ReadOnly = true;
            this.dgvMastersList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvMastersList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            this.dgvMastersList.Size = new Size(400, 300);
            this.dgvMastersList.TabIndex = 0;
            this.dgvMastersList.SelectionChanged += new EventHandler(this.dgvMastersList_SelectionChanged);
            //
            // pbMasterPhoto
            //
            this.pbMasterPhoto.Location = new Point(420, 6);
            this.pbMasterPhoto.Name = "pbMasterPhoto";
            this.pbMasterPhoto.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.pbMasterPhoto.Size = new Size(200, 200);
            this.pbMasterPhoto.SizeMode = PictureBoxSizeMode.Zoom;
            this.pbMasterPhoto.TabIndex = 1;
            this.pbMasterPhoto.TabStop = false;
            //
            // btnBookFromMaster
            //
            this.btnBookFromMaster.Location = new Point(420, 220);
            this.btnBookFromMaster.Name = "btnBookFromMaster";
            this.btnBookFromMaster.Size = new Size(200, 35);
            this.btnBookFromMaster.TabIndex = 2;
            this.btnBookFromMaster.Text = "Записаться к этому мастеру";
            this.btnBookFromMaster.UseVisualStyleBackColor = false;
            this.btnBookFromMaster.Click += new EventHandler(this.btnBookFromMaster_Click);
            //
            // tabAdminServices
            //
            this.tabAdminServices.Controls.Add(this.btnStyles);
            this.tabAdminServices.Controls.Add(this.btnMasters);
            this.tabAdminServices.Controls.Add(this.btnServicesAdmin);
            this.tabAdminServices.Location = new Point(4, 28);
            this.tabAdminServices.Name = "tabAdminServices";
            this.tabAdminServices.Padding = new Padding(24, 20, 24, 20);
            this.tabAdminServices.Size = new Size(752, 372);
            this.tabAdminServices.TabIndex = 2;
            this.btnBookFromMaster.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.tabAdminServices.Text = "Управление";
            this.tabAdminServices.UseVisualStyleBackColor = true;
            //
            // btnStyles
            //
            this.btnStyles.Location = new Point(24, 24);
            this.btnStyles.Name = "btnStyles";
            this.btnStyles.Size = new Size(200, 42);
            this.btnStyles.TabIndex = 0;
            this.btnStyles.Text = "Стили";
            this.btnStyles.UseVisualStyleBackColor = false;
            this.btnStyles.Click += new EventHandler(this.btnStyles_Click);
            //
            // btnMasters
            //
            this.btnMasters.Location = new Point(24, 78);
            this.btnMasters.Name = "btnMasters";
            this.btnMasters.Size = new Size(200, 42);
            this.btnMasters.TabIndex = 1;
            this.btnMasters.Text = "Мастера";
            this.btnMasters.UseVisualStyleBackColor = false;
            this.btnMasters.Click += new EventHandler(this.btnMasters_Click);
            //
            // btnServicesAdmin
            //
            this.btnServicesAdmin.Location = new Point(24, 132);
            this.btnServicesAdmin.Name = "btnServicesAdmin";
            this.btnServicesAdmin.Size = new Size(200, 42);
            this.btnServicesAdmin.TabIndex = 2;
            this.btnServicesAdmin.Text = "Услуги";
            this.btnServicesAdmin.UseVisualStyleBackColor = false;
            this.btnServicesAdmin.Click += new EventHandler(this.btnServicesAdmin_Click);
            //
            // tabAdminUsers
            //
            this.tabAdminUsers.Controls.Add(this.btnUsersAdmin);
            this.tabAdminUsers.Location = new Point(4, 24);
            this.tabAdminUsers.Name = "tabAdminUsers";
            this.tabAdminUsers.Size = new Size(752, 372);
            this.tabAdminUsers.TabIndex = 3;
            this.tabAdminUsers.Text = "Управление пользователями";
            this.tabAdminUsers.UseVisualStyleBackColor = true;
            //
            // btnUsersAdmin
            //
            this.btnUsersAdmin.Location = new Point(20, 20);
            this.btnUsersAdmin.Name = "btnUsersAdmin";
            this.btnUsersAdmin.Size = new Size(200, 35);
            this.btnUsersAdmin.TabIndex = 0;
            this.btnUsersAdmin.Text = "Управление пользователями";
            this.btnUsersAdmin.UseVisualStyleBackColor = false;
            this.btnUsersAdmin.Click += new EventHandler(this.btnUsersAdmin_Click);
            //
            // tabAllAppointments
            //
            this.tabAllAppointments.Controls.Add(this.btnAllAppointments);
            this.tabAllAppointments.Location = new Point(4, 24);
            this.tabAllAppointments.Name = "tabAllAppointments";
            this.tabAllAppointments.Size = new Size(752, 372);
            this.tabAllAppointments.TabIndex = 4;
            this.tabAllAppointments.Text = "Все записи";
            this.tabAllAppointments.UseVisualStyleBackColor = true;
            //
            // btnAllAppointments
            //
            this.btnAllAppointments.Location = new Point(20, 20);
            this.btnAllAppointments.Name = "btnAllAppointments";
            this.btnAllAppointments.Size = new Size(200, 35);
            this.btnAllAppointments.TabIndex = 0;
            this.btnAllAppointments.Text = "Просмотреть все записи";
            this.btnAllAppointments.UseVisualStyleBackColor = false;
            this.btnAllAppointments.Click += new EventHandler(this.btnAllAppointments_Click);
            //
            // tabStatistics
            //
            this.tabStatistics.Controls.Add(this.dgvYearlyStats);
            this.tabStatistics.Controls.Add(this.lblYearlyTable);
            this.tabStatistics.Controls.Add(this.dgvMonthlyStats);
            this.tabStatistics.Controls.Add(this.lblMonthlyTable);
            this.tabStatistics.Controls.Add(this.pnlChart);
            this.tabStatistics.Controls.Add(this.lblChartTitle);
            this.tabStatistics.Controls.Add(this.btnRefreshStats);
            this.tabStatistics.Controls.Add(this.cmbStatsYear);
            this.tabStatistics.Controls.Add(this.lblStatsYear);
            this.tabStatistics.Controls.Add(this.panelStatsCards);
            this.tabStatistics.Location = new Point(4, 28);
            this.tabStatistics.Name = "tabStatistics";
            this.tabStatistics.Padding = new Padding(8);
            this.tabStatistics.Size = new Size(776, 377);
            this.tabStatistics.TabIndex = 6;
            this.tabStatistics.Text = "Статистика";
            this.tabStatistics.UseVisualStyleBackColor = true;
            //
            // panelStatsCards
            //
            this.panelStatsCards.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.panelStatsCards.Location = new Point(8, 8);
            this.panelStatsCards.Name = "panelStatsCards";
            this.panelStatsCards.Size = new Size(760, 96);
            this.panelStatsCards.TabIndex = 0;
            //
            // lblStatsYear
            //
            this.lblStatsYear.AutoSize = true;
            this.lblStatsYear.Location = new Point(8, 112);
            this.lblStatsYear.Name = "lblStatsYear";
            this.lblStatsYear.Size = new Size(30, 15);
            this.lblStatsYear.TabIndex = 1;
            this.lblStatsYear.Text = "Год:";
            //
            // cmbStatsYear
            //
            this.cmbStatsYear.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatsYear.Location = new Point(48, 108);
            this.cmbStatsYear.Name = "cmbStatsYear";
            this.cmbStatsYear.Size = new Size(90, 23);
            this.cmbStatsYear.TabIndex = 2;
            this.cmbStatsYear.SelectedIndexChanged += new EventHandler(this.cmbStatsYear_SelectedIndexChanged);
            //
            // btnRefreshStats
            //
            this.btnRefreshStats.Location = new Point(148, 106);
            this.btnRefreshStats.Name = "btnRefreshStats";
            this.btnRefreshStats.Size = new Size(100, 28);
            this.btnRefreshStats.TabIndex = 3;
            this.btnRefreshStats.Text = "Обновить";
            this.btnRefreshStats.UseVisualStyleBackColor = false;
            this.btnRefreshStats.Click += new EventHandler(this.btnRefreshStats_Click);
            //
            // lblChartTitle
            //
            this.lblChartTitle.AutoSize = true;
            this.lblChartTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblChartTitle.Location = new Point(8, 140);
            this.lblChartTitle.Name = "lblChartTitle";
            this.lblChartTitle.Size = new Size(180, 15);
            this.lblChartTitle.TabIndex = 4;
            this.lblChartTitle.Text = "Выручка по месяцам (BYN)";
            //
            // pnlChart
            //
            this.pnlChart.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.pnlChart.Location = new Point(8, 160);
            this.pnlChart.Name = "pnlChart";
            this.pnlChart.Size = new Size(760, 110);
            this.pnlChart.TabIndex = 5;
            this.pnlChart.Paint += new PaintEventHandler(this.pnlChart_Paint);
            //
            // lblMonthlyTable
            //
            this.lblMonthlyTable.AutoSize = true;
            this.lblMonthlyTable.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblMonthlyTable.Location = new Point(8, 278);
            this.lblMonthlyTable.Name = "lblMonthlyTable";
            this.lblMonthlyTable.Size = new Size(130, 15);
            this.lblMonthlyTable.TabIndex = 6;
            this.lblMonthlyTable.Text = "По месяцам";
            //
            // dgvMonthlyStats
            //
            this.dgvMonthlyStats.AllowUserToAddRows = false;
            this.dgvMonthlyStats.AllowUserToDeleteRows = false;
            this.dgvMonthlyStats.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMonthlyStats.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMonthlyStats.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvMonthlyStats.Location = new Point(8, 296);
            this.dgvMonthlyStats.Name = "dgvMonthlyStats";
            this.dgvMonthlyStats.ReadOnly = true;
            this.dgvMonthlyStats.RowHeadersVisible = false;
            this.dgvMonthlyStats.Size = new Size(370, 72);
            this.dgvMonthlyStats.TabIndex = 7;
            //
            // lblYearlyTable
            //
            this.lblYearlyTable.AutoSize = true;
            this.lblYearlyTable.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblYearlyTable.Location = new Point(390, 278);
            this.lblYearlyTable.Name = "lblYearlyTable";
            this.lblYearlyTable.Size = new Size(100, 15);
            this.lblYearlyTable.TabIndex = 8;
            this.lblYearlyTable.Text = "По годам";
            //
            // dgvYearlyStats
            //
            this.dgvYearlyStats.AllowUserToAddRows = false;
            this.dgvYearlyStats.AllowUserToDeleteRows = false;
            this.dgvYearlyStats.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvYearlyStats.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvYearlyStats.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvYearlyStats.Location = new Point(390, 296);
            this.dgvYearlyStats.Name = "dgvYearlyStats";
            this.dgvYearlyStats.ReadOnly = true;
            this.dgvYearlyStats.RowHeadersVisible = false;
            this.dgvYearlyStats.Size = new Size(378, 72);
            this.dgvYearlyStats.TabIndex = 9;
            //
            // btnLogout
            //
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new Size(110, 34);
            this.btnLogout.TabIndex = 2;
            this.btnLogout.Text = "Выйти";
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new EventHandler(this.btnLogout_Click);
            //
            // MainForm
            //
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(900, 520);
            this.MinimumSize = new Size(820, 480);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.panelHeader);
            this.FormClosed += new FormClosedEventHandler(this.MainForm_FormClosed);
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Тату-салон";

            ((System.ComponentModel.ISupportInitialize)(this.dgvServices)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyAppointments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMastersList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMasterPhoto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMonthlyStats)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvYearlyStats)).EndInit();
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.tabStatistics.ResumeLayout(false);
            this.tabStatistics.PerformLayout();
            this.tabServices.ResumeLayout(false);
            this.tabMyAppointments.ResumeLayout(false);
            this.tabMasters.ResumeLayout(false);
            this.tabAdminServices.ResumeLayout(false);
            this.tabAdminUsers.ResumeLayout(false);
            this.tabAllAppointments.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}