namespace paseka
{
    partial class AdminMainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnServices;
        private System.Windows.Forms.Button btnUsers;
        private System.Windows.Forms.Button btnInventory;
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
            this.btnServices = new System.Windows.Forms.Button();
            this.btnUsers = new System.Windows.Forms.Button();
            this.btnInventory = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnServices
            // 
            this.btnServices.Location = new System.Drawing.Point(50, 30);
            this.btnServices.Name = "btnServices";
            this.btnServices.Size = new System.Drawing.Size(200, 40);
            this.btnServices.TabIndex = 0;
            this.btnServices.Text = "Управление услугами";
            this.btnServices.UseVisualStyleBackColor = true;
            this.btnServices.Click += new System.EventHandler(this.btnServices_Click);
            // 
            // btnUsers
            // 
            this.btnUsers.Location = new System.Drawing.Point(50, 80);
            this.btnUsers.Name = "btnUsers";
            this.btnUsers.Size = new System.Drawing.Size(200, 40);
            this.btnUsers.TabIndex = 1;
            this.btnUsers.Text = "Просмотр пользователей";
            this.btnUsers.UseVisualStyleBackColor = true;
            this.btnUsers.Click += new System.EventHandler(this.btnUsers_Click);
            // 
            // btnInventory
            // 
            this.btnInventory.Location = new System.Drawing.Point(50, 130);
            this.btnInventory.Name = "btnInventory";
            this.btnInventory.Size = new System.Drawing.Size(200, 40);
            this.btnInventory.TabIndex = 2;
            this.btnInventory.Text = "Управление инвентарём";
            this.btnInventory.UseVisualStyleBackColor = true;
            this.btnInventory.Click += new System.EventHandler(this.btnInventory_Click);
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
            // AdminMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 250);
            this.Controls.Add(this.btnServices);
            this.Controls.Add(this.btnUsers);
            this.Controls.Add(this.btnInventory);
            this.Controls.Add(this.btnLogout);
            this.Name = "AdminMainForm";
            this.Text = "Администратор";
            this.ResumeLayout(false);
        }

        #endregion
    }
}