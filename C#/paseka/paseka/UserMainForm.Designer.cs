namespace paseka
{
    partial class UserMainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnViewServices;
        private System.Windows.Forms.Button btnMyBookings;
        private System.Windows.Forms.Button btnBookService;
        private System.Windows.Forms.Button btnLogout;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.btnViewServices = new System.Windows.Forms.Button();
            this.btnMyBookings = new System.Windows.Forms.Button();
            this.btnBookService = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnViewServices
            // 
            this.btnViewServices.Location = new System.Drawing.Point(50, 30);
            this.btnViewServices.Name = "btnViewServices";
            this.btnViewServices.Size = new System.Drawing.Size(200, 40);
            this.btnViewServices.TabIndex = 0;
            this.btnViewServices.Text = "Просмотреть услуги";
            this.btnViewServices.UseVisualStyleBackColor = true;
            this.btnViewServices.Click += new System.EventHandler(this.btnViewServices_Click);
            // 
            // btnMyBookings
            // 
            this.btnMyBookings.Location = new System.Drawing.Point(50, 80);
            this.btnMyBookings.Name = "btnMyBookings";
            this.btnMyBookings.Size = new System.Drawing.Size(200, 40);
            this.btnMyBookings.TabIndex = 1;
            this.btnMyBookings.Text = "Мои бронирования";
            this.btnMyBookings.UseVisualStyleBackColor = true;
            this.btnMyBookings.Click += new System.EventHandler(this.btnMyBookings_Click);
            // 
            // btnBookService
            // 
            this.btnBookService.Location = new System.Drawing.Point(50, 130);
            this.btnBookService.Name = "btnBookService";
            this.btnBookService.Size = new System.Drawing.Size(200, 40);
            this.btnBookService.TabIndex = 2;
            this.btnBookService.Text = "Забронировать услугу";
            this.btnBookService.UseVisualStyleBackColor = true;
            this.btnBookService.Click += new System.EventHandler(this.btnBookService_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(50, 180);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(200, 40);
            this.btnLogout.TabIndex = 3;
            this.btnLogout.Text = "Выход";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // UserMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 250);
            this.Controls.Add(this.btnViewServices);
            this.Controls.Add(this.btnMyBookings);
            this.Controls.Add(this.btnBookService);
            this.Controls.Add(this.btnLogout);
            this.Name = "UserMainForm";
            this.Text = "Пользователь";
            this.ResumeLayout(false);
        }

        #endregion
    }
}