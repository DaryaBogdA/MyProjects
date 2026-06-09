namespace Avto.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvVehicles;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Label lblUserInfo;
        private System.Windows.Forms.Button btnRent;
        private System.Windows.Forms.Button btnMyOrders;
        private System.Windows.Forms.Button btnUsers;
        private System.Windows.Forms.Button btnExportVehicles;
        private System.Windows.Forms.Button btnAnalytics;
        private System.Windows.Forms.Label lblHotkeys;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblEventLog;

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
            this.dgvVehicles = new System.Windows.Forms.DataGridView();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnRent = new System.Windows.Forms.Button();
            this.btnMyOrders = new System.Windows.Forms.Button();
            this.btnUsers = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.lblUserInfo = new System.Windows.Forms.Label();
            this.btnExportVehicles = new System.Windows.Forms.Button();
            this.btnAnalytics = new System.Windows.Forms.Button();
            this.lblHotkeys = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblEventLog = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVehicles)).BeginInit();
            this.SuspendLayout();

            // dgvVehicles
            this.dgvVehicles.AllowUserToAddRows = false;
            this.dgvVehicles.AllowUserToDeleteRows = false;
            this.dgvVehicles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            this.dgvVehicles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVehicles.Location = new System.Drawing.Point(12, 42);
            this.dgvVehicles.MultiSelect = false;
            this.dgvVehicles.Name = "dgvVehicles";
            this.dgvVehicles.ReadOnly = true;
            this.dgvVehicles.RowHeadersVisible = false;
            this.dgvVehicles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvVehicles.Size = new System.Drawing.Size(1060, 400);
            this.dgvVehicles.TabIndex = 0;
            this.dgvVehicles.SelectionChanged += new System.EventHandler(this.dgvVehicles_SelectionChanged);

            // btnExportVehicles
            this.btnExportVehicles.BackColor = System.Drawing.Color.FromArgb(0, 122, 204);
            this.btnExportVehicles.FlatAppearance.BorderSize = 0;
            this.btnExportVehicles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportVehicles.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnExportVehicles.ForeColor = System.Drawing.Color.White;
            this.btnExportVehicles.Location = new System.Drawing.Point(799, 510);
            this.btnExportVehicles.Name = "btnExportVehicles";
            this.btnExportVehicles.Size = new System.Drawing.Size(120, 35);
            this.btnExportVehicles.TabIndex = 9;
            this.btnExportVehicles.Text = "Экспорт ТС";
            this.btnExportVehicles.UseVisualStyleBackColor = false;
            this.btnExportVehicles.Click += new System.EventHandler(this.btnExportVehicles_Click);

            // btnAnalytics
            this.btnAnalytics.BackColor = System.Drawing.Color.FromArgb(0, 122, 204);
            this.btnAnalytics.FlatAppearance.BorderSize = 0;
            this.btnAnalytics.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAnalytics.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAnalytics.ForeColor = System.Drawing.Color.White;
            this.btnAnalytics.Location = new System.Drawing.Point(444, 510);
            this.btnAnalytics.Name = "btnAnalytics";
            this.btnAnalytics.Size = new System.Drawing.Size(100, 35);
            this.btnAnalytics.TabIndex = 10;
            this.btnAnalytics.Text = "Графики";
            this.btnAnalytics.UseVisualStyleBackColor = false;
            this.btnAnalytics.Click += new System.EventHandler(this.btnAnalytics_Click);

            // btnAdd
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(0, 122, 204);
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(12, 510);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(100, 35);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Добавить";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);

            // btnEdit
            this.btnEdit.BackColor = System.Drawing.Color.FromArgb(0, 122, 204);
            this.btnEdit.FlatAppearance.BorderSize = 0;
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnEdit.ForeColor = System.Drawing.Color.White;
            this.btnEdit.Location = new System.Drawing.Point(120, 510);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(100, 35);
            this.btnEdit.TabIndex = 2;
            this.btnEdit.Text = "Изменить";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);

            // btnDelete
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(204, 0, 0);
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(228, 510);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(100, 35);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "Удалить";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            // btnRefresh
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(0, 122, 204);
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(336, 510);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 35);
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.Text = "Обновить";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);

            // btnRent
            this.btnRent.BackColor = System.Drawing.Color.FromArgb(0, 150, 136);
            this.btnRent.FlatAppearance.BorderSize = 0;
            this.btnRent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRent.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRent.ForeColor = System.Drawing.Color.White;
            this.btnRent.Location = new System.Drawing.Point(444, 510);
            this.btnRent.Name = "btnRent";
            this.btnRent.Size = new System.Drawing.Size(117, 35);
            this.btnRent.TabIndex = 6;
            this.btnRent.Text = "Арендовать";
            this.btnRent.UseVisualStyleBackColor = false;
            this.btnRent.Click += new System.EventHandler(this.btnRent_Click);
            this.btnRent.Visible = false;

            // btnMyOrders
            this.btnMyOrders.BackColor = System.Drawing.Color.FromArgb(0, 150, 136);
            this.btnMyOrders.FlatAppearance.BorderSize = 0;
            this.btnMyOrders.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMyOrders.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnMyOrders.ForeColor = System.Drawing.Color.White;
            this.btnMyOrders.Location = new System.Drawing.Point(567, 510);
            this.btnMyOrders.Name = "btnMyOrders";
            this.btnMyOrders.Size = new System.Drawing.Size(140, 35);
            this.btnMyOrders.TabIndex = 7;
            this.btnMyOrders.Text = "Мои заказы";
            this.btnMyOrders.UseVisualStyleBackColor = false;
            this.btnMyOrders.Click += new System.EventHandler(this.btnMyOrders_Click);
            this.btnMyOrders.Visible = false;

            // btnUsers
            this.btnUsers.BackColor = System.Drawing.Color.FromArgb(0, 122, 204);
            this.btnUsers.FlatAppearance.BorderSize = 0;
            this.btnUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUsers.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnUsers.ForeColor = System.Drawing.Color.White;
            this.btnUsers.Location = new System.Drawing.Point(673, 510);
            this.btnUsers.Name = "btnUsers";
            this.btnUsers.Size = new System.Drawing.Size(120, 35);
            this.btnUsers.TabIndex = 8;
            this.btnUsers.Text = "Пользователи";
            this.btnUsers.UseVisualStyleBackColor = false;
            this.btnUsers.Click += new System.EventHandler(this.btnUsers_Click);

            // btnLogout
            this.btnLogout.BackColor = System.Drawing.Color.FromArgb(100, 100, 100);
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Location = new System.Drawing.Point(972, 510);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(100, 35);
            this.btnLogout.TabIndex = 5;
            this.btnLogout.Text = "Выход";
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);

            // lblUserInfo
            this.lblUserInfo.AutoSize = true;
            this.lblUserInfo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblUserInfo.ForeColor = System.Drawing.Color.FromArgb(0, 122, 204);
            this.lblUserInfo.Location = new System.Drawing.Point(12, 15);
            this.lblUserInfo.Name = "lblUserInfo";
            this.lblUserInfo.Size = new System.Drawing.Size(0, 19);
            this.lblUserInfo.TabIndex = 6;

            // lblStatus (новая строка состояния)
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblStatus.ForeColor = System.Drawing.Color.DarkGray;
            this.lblStatus.Location = new System.Drawing.Point(12, 455);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 15);
            this.lblStatus.TabIndex = 13;

            // lblEventLog (лог событий)
            this.lblEventLog.AutoSize = true;
            this.lblEventLog.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            this.lblEventLog.ForeColor = System.Drawing.Color.Gray;
            this.lblEventLog.Location = new System.Drawing.Point(12, 475);
            this.lblEventLog.Name = "lblEventLog";
            this.lblEventLog.Size = new System.Drawing.Size(0, 13);
            this.lblEventLog.TabIndex = 14;

            // lblHotkeys
            this.lblHotkeys.AutoSize = true;
            this.lblHotkeys.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            this.lblHotkeys.ForeColor = System.Drawing.Color.Gray;
            this.lblHotkeys.Location = new System.Drawing.Point(12, 550);
            this.lblHotkeys.Name = "lblHotkeys";
            this.lblHotkeys.Size = new System.Drawing.Size(0, 13);
            this.lblHotkeys.TabIndex = 12;
            this.lblHotkeys.Text = "Горячие клавиши: F1-Добавить | F2-Изменить | F3-Обновить | F4-Графики | F5-Заказы | Ctrl+Q-Выход";

            // MainForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1084, 600);
            this.Controls.Add(this.lblEventLog);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblHotkeys);
            this.Controls.Add(this.lblUserInfo);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.btnUsers);
            this.Controls.Add(this.btnMyOrders);
            this.Controls.Add(this.btnRent);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnExportVehicles);
            this.Controls.Add(this.btnAnalytics);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.dgvVehicles);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Каршеринг транспортных средств";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVehicles)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}