namespace shop.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem каталогToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem корзинаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem моиЗаказыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem выходToolStripMenuItem;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartSales;
        private System.Windows.Forms.Label lblChartTitle;

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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.каталогToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.корзинаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.моиЗаказыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelChart = new System.Windows.Forms.Panel();
            this.lblChartTitle = new System.Windows.Forms.Label();
            this.chartSales = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.menuStrip.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartSales)).BeginInit();
            this.SuspendLayout();

            // menuStrip
            this.menuStrip.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
            this.menuStrip.ForeColor = System.Drawing.Color.White;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.каталогToolStripMenuItem,
                this.корзинаToolStripMenuItem,
                this.моиЗаказыToolStripMenuItem,
                this.выходToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(900, 24);
            this.menuStrip.TabIndex = 0;

            // каталогToolStripMenuItem
            this.каталогToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.каталогToolStripMenuItem.Name = "каталогToolStripMenuItem";
            this.каталогToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+K";
            this.каталогToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.каталогToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.каталогToolStripMenuItem.Text = "Каталог";
            this.каталогToolStripMenuItem.Click += new System.EventHandler(this.каталогToolStripMenuItem_Click);

            // корзинаToolStripMenuItem
            this.корзинаToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.корзинаToolStripMenuItem.Name = "корзинаToolStripMenuItem";
            this.корзинаToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+C";
            this.корзинаToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.корзинаToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.корзинаToolStripMenuItem.Text = "Корзина";
            this.корзинаToolStripMenuItem.Click += new System.EventHandler(this.корзинаToolStripMenuItem_Click);

            // моиЗаказыToolStripMenuItem
            this.моиЗаказыToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.моиЗаказыToolStripMenuItem.Name = "моиЗаказыToolStripMenuItem";
            this.моиЗаказыToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+O";
            this.моиЗаказыToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.моиЗаказыToolStripMenuItem.Size = new System.Drawing.Size(89, 20);
            this.моиЗаказыToolStripMenuItem.Text = "Мои заказы";
            this.моиЗаказыToolStripMenuItem.Click += new System.EventHandler(this.моиЗаказыToolStripMenuItem_Click);

            // выходToolStripMenuItem
            this.выходToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            this.выходToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Q";
            this.выходToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.выходToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.выходToolStripMenuItem.Text = "Выход";
            this.выходToolStripMenuItem.Click += new System.EventHandler(this.выходToolStripMenuItem_Click);

            // panelTop
            this.panelTop.Controls.Add(this.panelChart);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 24);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(900, 220);
            this.panelTop.TabIndex = 2;

            // panelChart
            this.panelChart.BackColor = System.Drawing.Color.White;
            this.panelChart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelChart.Controls.Add(this.chartSales);
            this.panelChart.Controls.Add(this.lblChartTitle);
            this.panelChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelChart.Location = new System.Drawing.Point(0, 0);
            this.panelChart.Name = "panelChart";
            this.panelChart.Padding = new System.Windows.Forms.Padding(10);
            this.panelChart.Size = new System.Drawing.Size(900, 220);
            this.panelChart.TabIndex = 0;

            // lblChartTitle
            this.lblChartTitle.AutoSize = true;
            this.lblChartTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblChartTitle.Location = new System.Drawing.Point(10, 10);
            this.lblChartTitle.Name = "lblChartTitle";
            this.lblChartTitle.Size = new System.Drawing.Size(235, 19);
            this.lblChartTitle.TabIndex = 0;
            this.lblChartTitle.Text = "Продажи за последние 7 дней (BYN)";

            // chartSales
            this.chartSales.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartSales.Location = new System.Drawing.Point(10, 29);
            this.chartSales.Name = "chartSales";
            this.chartSales.Size = new System.Drawing.Size(878, 179);
            this.chartSales.TabIndex = 1;

            // ChartArea
            var chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            chartArea.AxisX.Title = "Дата";
            chartArea.AxisX.TitleFont = new System.Drawing.Font("Segoe UI", 8F);
            chartArea.AxisY.Title = "Сумма (BYN)";
            chartArea.AxisY.TitleFont = new System.Drawing.Font("Segoe UI", 8F);
            chartArea.BackColor = System.Drawing.Color.White;
            this.chartSales.ChartAreas.Add(chartArea);

            // Series
            var series = new System.Windows.Forms.DataVisualization.Charting.Series();
            series.Name = "Продажи";
            series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
            series.Color = System.Drawing.Color.FromArgb(59, 130, 246);
            series.BorderWidth = 0;
            series.Font = new System.Drawing.Font("Segoe UI", 7F);
            this.chartSales.Series.Add(series);

            // Title
            var title = new System.Windows.Forms.DataVisualization.Charting.Title();
            title.Text = "Динамика продаж";
            title.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            title.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            this.chartSales.Titles.Add(title);

            // flowLayoutPanel
            this.flowLayoutPanel.AutoScroll = true;
            this.flowLayoutPanel.BackColor = System.Drawing.Color.White;
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel.Location = new System.Drawing.Point(0, 244);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Padding = new System.Windows.Forms.Padding(10);
            this.flowLayoutPanel.Size = new System.Drawing.Size(900, 306);
            this.flowLayoutPanel.TabIndex = 1;

            // MainForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(900, 550);
            this.Controls.Add(this.flowLayoutPanel);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.menuStrip);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ILNI Shop — Интернет-магазин одежды";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelChart.ResumeLayout(false);
            this.panelChart.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartSales)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}