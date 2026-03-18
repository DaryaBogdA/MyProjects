using System.Drawing;
using System.Windows.Forms;

namespace SanatoriumStaro
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Button btnProfile;
        private System.Windows.Forms.Button btnServices;
        private System.Windows.Forms.Button btnMyAppointments;
        private System.Windows.Forms.Button btnManageServices;
        private System.Windows.Forms.Button btnAllAppointments;
        private System.Windows.Forms.Button btnLogout;

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
            this.lblWelcome = new System.Windows.Forms.Label();
            this.btnProfile = new System.Windows.Forms.Button();
            this.btnServices = new System.Windows.Forms.Button();
            this.btnMyAppointments = new System.Windows.Forms.Button();
            this.btnManageServices = new System.Windows.Forms.Button();
            this.btnAllAppointments = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.SuspendLayout();
            this.BackColor = Color.FromArgb(255, 255, 224);
            this.Font = new Font("Segoe UI", 9F);
            this.StartPosition = FormStartPosition.CenterScreen;
            //
            // lblWelcome
            //
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.Location = new System.Drawing.Point(20, 20);
            this.lblWelcome.Size = new System.Drawing.Size(300, 20);
            this.lblWelcome.Text = "Добро пожаловать!";
            this.lblWelcome.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblWelcome.ForeColor = Color.DarkSlateBlue;
            //
            // btnProfile
            //
            this.btnProfile.Location = new System.Drawing.Point(20, 60);
            this.btnProfile.Size = new System.Drawing.Size(150, 30);
            this.btnProfile.Text = "Личный кабинет";
            this.btnProfile.Click += new System.EventHandler(this.btnProfile_Click);
            //
            // btnServices
            //
            this.btnServices.Location = new System.Drawing.Point(20, 100);
            this.btnServices.Size = new System.Drawing.Size(150, 30);
            this.btnServices.Text = "Услуги";
            this.btnServices.Click += new System.EventHandler(this.btnServices_Click);
            //
            // btnMyAppointments
            //
            this.btnMyAppointments.Location = new System.Drawing.Point(20, 140);
            this.btnMyAppointments.Size = new System.Drawing.Size(150, 30);
            this.btnMyAppointments.Text = "Мои записи";
            this.btnMyAppointments.Click += new System.EventHandler(this.btnMyAppointments_Click);
            //
            // btnManageServices
            //
            this.btnManageServices.Location = new System.Drawing.Point(20, 180);
            this.btnManageServices.Size = new System.Drawing.Size(150, 30);
            this.btnManageServices.Text = "Управление услугами";
            this.btnManageServices.Click += new System.EventHandler(this.btnManageServices_Click);
            //
            // btnAllAppointments
            //
            this.btnAllAppointments.Location = new System.Drawing.Point(20, 220);
            this.btnAllAppointments.Size = new System.Drawing.Size(150, 30);
            this.btnAllAppointments.Text = "Все записи";
            this.btnAllAppointments.Click += new System.EventHandler(this.btnAllAppointments_Click);
            btnLogout.BackColor = Color.IndianRed;
            //
            // btnLogout
            //
            this.btnLogout.Location = new System.Drawing.Point(20, 280);
            this.btnLogout.Size = new System.Drawing.Size(150, 30);
            this.btnLogout.Text = "Выход";
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            foreach (Button btn in new[] { btnProfile, btnServices, btnMyAppointments,
                                    btnManageServices, btnAllAppointments, btnLogout })
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.BackColor = Color.SteelBlue;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderSize = 0;
                btn.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                btn.Height = 35;
            }
            //
            // MainForm
            //
            this.ClientSize = new System.Drawing.Size(400, 350);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblWelcome,
                this.btnProfile,
                this.btnServices,
                this.btnMyAppointments,
                this.btnManageServices,
                this.btnAllAppointments,
                this.btnLogout
            });
            this.Text = "Санаторий Staro";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}