using System.Windows.Forms;

namespace SanatoriumStaro
{
    partial class ServiceEditForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.TextBox txtPrice;
        private System.Windows.Forms.TextBox txtDuration;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.CheckBox chkActive;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;

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
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtPrice = new System.Windows.Forms.TextBox();
            this.txtDuration = new System.Windows.Forms.TextBox();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            this.StartPosition = FormStartPosition.CenterScreen;
            int y = 20;
            int labelX = 20;
            int textX = 120;
            int width = 200;

            Label lblName = new Label(); lblName.Text = "Название*:"; lblName.Location = new System.Drawing.Point(labelX, y + 3);
            this.txtName.Location = new System.Drawing.Point(textX, y); this.txtName.Size = new System.Drawing.Size(width, 20);
            y += 30;

            Label lblDesc = new Label(); lblDesc.Text = "Описание:"; lblDesc.Location = new System.Drawing.Point(labelX, y + 3);
            this.txtDescription.Location = new System.Drawing.Point(textX, y); this.txtDescription.Size = new System.Drawing.Size(width, 40);
            this.txtDescription.Multiline = true;
            y += 50;

            Label lblPrice = new Label(); lblPrice.Text = "Цена*:"; lblPrice.Location = new System.Drawing.Point(labelX, y + 3);
            this.txtPrice.Location = new System.Drawing.Point(textX, y); this.txtPrice.Size = new System.Drawing.Size(width, 20);
            y += 30;

            Label lblDuration = new Label(); lblDuration.Text = "Длительность (мин):"; lblDuration.Location = new System.Drawing.Point(labelX, y + 3);
            this.txtDuration.Location = new System.Drawing.Point(textX, y); this.txtDuration.Size = new System.Drawing.Size(width, 20);
            y += 30;

            Label lblCategory = new Label(); lblCategory.Text = "Категория:"; lblCategory.Location = new System.Drawing.Point(labelX, y + 3);
            this.cmbCategory.Location = new System.Drawing.Point(textX, y); this.cmbCategory.Size = new System.Drawing.Size(width, 20);
            this.cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            y += 30;
            //
            // chkActive
            //
            this.chkActive.Text = "Услуга активна";
            this.chkActive.Location = new System.Drawing.Point(textX, y);
            this.chkActive.Checked = true;
            y += 30;
            //
            // btnSave
            //
            this.btnSave.Text = "Сохранить";
            this.btnSave.Location = new System.Drawing.Point(textX, y);
            this.btnSave.Size = new System.Drawing.Size(90, 30);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnCancel
            //
            this.btnCancel.Text = "Отмена";
            this.btnCancel.Location = new System.Drawing.Point(textX + 100, y);
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            this.ClientSize = new System.Drawing.Size(400, y + 50);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblName, this.txtName,
                lblDesc, this.txtDescription,
                lblPrice, this.txtPrice,
                lblDuration, this.txtDuration,
                lblCategory, this.cmbCategory,
                this.chkActive,
                this.btnSave, this.btnCancel
            });
            this.Text = service?.Id == 0 ? "Добавление услуги" : "Редактирование услуги";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}