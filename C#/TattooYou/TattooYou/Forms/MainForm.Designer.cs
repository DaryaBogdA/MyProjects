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
        private TabPage tabMasters;
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
            this.tabMasters = new TabPage();
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
            this.tabControl.SuspendLayout();
            this.tabServices.SuspendLayout();
            this.tabMyAppointments.SuspendLayout();
            this.tabAdminServices.SuspendLayout();
            this.tabAdminUsers.SuspendLayout();
            this.tabAllAppointments.SuspendLayout();
            this.tabMasters.SuspendLayout();
            this.SuspendLayout();
            //
            // lblWelcome
            //
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblWelcome.Location = new Point(12, 9);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new Size(200, 21);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "Добро пожаловать";
            this.lblWelcome.ForeColor = Color.Black;
            //
            // tabControl
            //
            this.tabControl.Controls.Add(this.tabServices);
            this.tabControl.Controls.Add(this.tabMyAppointments);
            this.tabControl.Controls.Add(this.tabMasters);
            this.tabControl.Controls.Add(this.tabAdminServices);
            this.tabControl.Controls.Add(this.tabAdminUsers);
            this.tabControl.Controls.Add(this.tabAllAppointments);
            this.tabControl.Location = new Point(12, 50);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new Size(760, 400);
            this.tabControl.TabIndex = 1;
            this.tabControl.Dock = DockStyle.Fill;
            this.tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
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
            this.tabMyAppointments.Controls.Add(this.btnExportMyAppointments);
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
            this.dgvMyAppointments.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvMyAppointments.Size = new Size(740, 360); this.dgvMyAppointments.TabIndex = 0;
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
            this.tabAdminServices.Location = new Point(4, 24);
            this.tabAdminServices.Name = "tabAdminServices";
            this.tabAdminServices.Padding = new Padding(3);
            this.tabAdminServices.Size = new Size(752, 372);
            this.tabAdminServices.TabIndex = 2;
            this.btnBookFromMaster.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.tabAdminServices.Text = "Управление услугами";
            this.tabAdminServices.UseVisualStyleBackColor = true;
            //
            // btnStyles
            //
            this.btnStyles.Location = new Point(20, 20);
            this.btnStyles.Name = "btnStyles";
            this.btnStyles.Size = new Size(150, 35);
            this.btnStyles.TabIndex = 0;
            this.btnStyles.Text = "Стили";
            this.btnStyles.UseVisualStyleBackColor = false;
            this.btnStyles.Click += new EventHandler(this.btnStyles_Click);
            //
            // btnMasters
            //
            this.btnMasters.Location = new Point(20, 70);
            this.btnMasters.Name = "btnMasters";
            this.btnMasters.Size = new Size(150, 35);
            this.btnMasters.TabIndex = 1;
            this.btnMasters.Text = "Мастера";
            this.btnMasters.UseVisualStyleBackColor = false;
            this.btnMasters.Click += new EventHandler(this.btnMasters_Click);
            //
            // btnServicesAdmin
            //
            this.btnServicesAdmin.Location = new Point(20, 120);
            this.btnServicesAdmin.Name = "btnServicesAdmin";
            this.btnServicesAdmin.Size = new Size(150, 35);
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
            // btnLogout
            //
            this.btnLogout.Location = new Point(680, 9);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new Size(90, 30);
            this.btnLogout.TabIndex = 2;
            this.btnLogout.Text = "Выход";
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new EventHandler(this.btnLogout_Click);
            this.btnLogout.Anchor = AnchorStyles.Top | AnchorStyles.Right; 
            //
            // MainForm
            //
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(784, 461);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.lblWelcome);
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Тату-салон";

            ((System.ComponentModel.ISupportInitialize)(this.dgvServices)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyAppointments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMastersList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMasterPhoto)).EndInit();
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