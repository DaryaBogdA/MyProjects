namespace Avto.Forms
{
    partial class AnalyticsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartByType;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartOrders;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartAvailability;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClose;

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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.chartByType = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartOrders = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartAvailability = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chartByType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartOrders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartAvailability)).BeginInit();
            this.SuspendLayout();

            // chartByType
            chartArea1.Name = "ChartArea1";
            this.chartByType.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartByType.Legends.Add(legend1);
            this.chartByType.Location = new System.Drawing.Point(12, 12);
            this.chartByType.Name = "chartByType";
            this.chartByType.Size = new System.Drawing.Size(450, 250);
            this.chartByType.TabIndex = 0;

            // chartOrders
            chartArea2.Name = "ChartArea1";
            this.chartOrders.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chartOrders.Legends.Add(legend2);
            this.chartOrders.Location = new System.Drawing.Point(480, 12);
            this.chartOrders.Name = "chartOrders";
            this.chartOrders.Size = new System.Drawing.Size(400, 250);
            this.chartOrders.TabIndex = 1;

            // chartAvailability
            chartArea3.Name = "ChartArea1";
            this.chartAvailability.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.chartAvailability.Legends.Add(legend3);
            this.chartAvailability.Location = new System.Drawing.Point(12, 280);
            this.chartAvailability.Name = "chartAvailability";
            this.chartAvailability.Size = new System.Drawing.Size(450, 250);
            this.chartAvailability.TabIndex = 2;

            // btnRefresh
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(0, 122, 204);
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(480, 490);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(120, 40);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Обновить";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);

            // btnClose
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(100, 100, 100);
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(760, 490);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(120, 40);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Закрыть";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            // AnalyticsForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(900, 550);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.chartAvailability);
            this.Controls.Add(this.chartOrders);
            this.Controls.Add(this.chartByType);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "AnalyticsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Аналитика и графики";
            this.Load += new System.EventHandler(this.AnalyticsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chartByType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartOrders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartAvailability)).EndInit();
            this.ResumeLayout(false);
        }
    }
}