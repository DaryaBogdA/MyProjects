using System.Windows.Forms;

namespace SanatoriumStaro
{
    partial class AppointmentsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvAppointments;
        private System.Windows.Forms.Button btnChangeStatus;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.Button btnSave;

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
            this.dgvAppointments = new System.Windows.Forms.DataGridView();
            this.btnChangeStatus = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAppointments)).BeginInit();
            this.SuspendLayout();
            this.StartPosition = FormStartPosition.CenterScreen;
            //
            // dgvAppointments
            //
            this.dgvAppointments.AllowUserToAddRows = false;
            this.dgvAppointments.AllowUserToDeleteRows = false;
            this.dgvAppointments.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAppointments.Location = new System.Drawing.Point(12, 12);
            this.dgvAppointments.MultiSelect = false;
            this.dgvAppointments.ReadOnly = true;
            this.dgvAppointments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvAppointments.Size = new System.Drawing.Size(760, 300);
            this.dgvAppointments.TabIndex = 0;
            this.dgvAppointments.SelectionChanged += new System.EventHandler(this.dgvAppointments_SelectionChanged);
            //
            // cmbStatus
            //
            this.cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatus.Location = new System.Drawing.Point(12, 320);
            this.cmbStatus.Size = new System.Drawing.Size(150, 21);
            this.cmbStatus.Visible = false;
            //
            // btnChangeStatus
            //
            this.btnChangeStatus.Location = new System.Drawing.Point(170, 318);
            this.btnChangeStatus.Size = new System.Drawing.Size(120, 23);
            this.btnChangeStatus.Text = "Изменить статус";
            this.btnChangeStatus.Visible = false;
            this.btnChangeStatus.Click += new System.EventHandler(this.btnChangeStatus_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(300, 318);
            this.btnCancel.Size = new System.Drawing.Size(100, 23);
            this.btnCancel.Text = "Отменить запись";
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(420, 318); // пример координат
            this.btnSave.Size = new System.Drawing.Size(120, 23);
            this.btnSave.Text = "Сохранить в файл";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnClose
            //
            this.btnClose.Location = new System.Drawing.Point(672, 318);
            this.btnClose.Size = new System.Drawing.Size(100, 23);
            this.btnClose.Text = "Закрыть";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // AppointmentsForm
            //
            this.ClientSize = new System.Drawing.Size(784, 361);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.dgvAppointments,
                this.cmbStatus,
                this.btnSave,
                this.btnChangeStatus,
                this.btnCancel,
                this.btnClose
            });
            this.Text = "Записи";
            ((System.ComponentModel.ISupportInitialize)(this.dgvAppointments)).EndInit();
            this.ResumeLayout(false);
        }
    }
}