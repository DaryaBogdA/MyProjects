using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SanatoriumStaro
{
    partial class StatisticsForm
    {
        private System.ComponentModel.IContainer components = null;
        private Chart chartByService;
        private Chart chartByDay;
        private Label lblTitle;
        private Button btnRefresh;
        private Button btnClose;
        private ComboBox cmbPeriod;
        private Label lblPeriod;

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
            this.chartByService = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartByDay = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.cmbPeriod = new System.Windows.Forms.ComboBox();
            this.lblPeriod = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chartByService)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartByDay)).BeginInit();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.DarkSlateBlue;
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Size = new System.Drawing.Size(287, 25);
            this.lblTitle.Text = "Статистика записей санатория";

            // lblPeriod
            this.lblPeriod.AutoSize = true;
            this.lblPeriod.Location = new System.Drawing.Point(12, 44);
            this.lblPeriod.Text = "Период:";
            this.lblPeriod.Font = new System.Drawing.Font("Segoe UI", 9F);

            // cmbPeriod
            this.cmbPeriod.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbPeriod.Location = new System.Drawing.Point(70, 41);
            this.cmbPeriod.Size = new System.Drawing.Size(150, 23);
            this.cmbPeriod.Items.AddRange(new object[] { "Все время", "Последние 7 дней", "Последние 30 дней", "Этот месяц", "Прошлый месяц" });
            this.cmbPeriod.SelectedIndex = 0;
            this.cmbPeriod.SelectedIndexChanged += new System.EventHandler(this.cmbPeriod_SelectedIndexChanged);

            // btnRefresh
            this.btnRefresh.FlatStyle = FlatStyle.Flat;
            this.btnRefresh.BackColor = System.Drawing.Color.SteelBlue;
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.Location = new System.Drawing.Point(240, 39);
            this.btnRefresh.Size = new System.Drawing.Size(100, 28);
            this.btnRefresh.Text = "Обновить";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);

            // btnClose
            this.btnClose.FlatStyle = FlatStyle.Flat;
            this.btnClose.BackColor = System.Drawing.Color.IndianRed;
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.Location = new System.Drawing.Point(650, 39);
            this.btnClose.Size = new System.Drawing.Size(100, 28);
            this.btnClose.Text = "Закрыть";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            // chartByService
            this.chartByService.Location = new System.Drawing.Point(12, 80);
            this.chartByService.Name = "chartByService";
            this.chartByService.Size = new System.Drawing.Size(400, 300);
            this.chartByService.TabIndex = 0;
            this.chartByService.Text = "Записи по услугам";
            this.chartByService.BackColor = System.Drawing.Color.White;
            this.chartByService.BackGradientStyle = GradientStyle.None;
            this.chartByService.BorderlineColor = System.Drawing.Color.LightGray;
            ChartArea area1 = new ChartArea();
            area1.Name = "ChartArea1";
            this.chartByService.ChartAreas.Add(area1);

            // chartByDay
            this.chartByDay.Location = new System.Drawing.Point(420, 80);
            this.chartByDay.Name = "chartByDay";
            this.chartByDay.Size = new System.Drawing.Size(450, 300);
            this.chartByDay.TabIndex = 1;
            this.chartByDay.Text = "Записи по дням";
            this.chartByDay.BackColor = System.Drawing.Color.White;
            ChartArea area2 = new ChartArea();
            area2.Name = "ChartArea1";
            this.chartByDay.ChartAreas.Add(area2);

            // StatisticsForm
            this.BackColor = System.Drawing.Color.FromArgb(255, 255, 224);
            this.ClientSize = new System.Drawing.Size(884, 400);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblPeriod);
            this.Controls.Add(this.cmbPeriod);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.chartByService);
            this.Controls.Add(this.chartByDay);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "StatisticsForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Статистика санатория Staro";
            ((System.ComponentModel.ISupportInitialize)(this.chartByService)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartByDay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}