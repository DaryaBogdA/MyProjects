using System;
using System.Drawing;
using System.Windows.Forms;

namespace shop.Controls
{
    partial class ProductCardControl
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblName;
        private Label lblPrice;
        private Label lblSize;
        private Label lblColor;
        private Label lblStock;
        private Button btnAddToCart;
        private PictureBox pictureBox;

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
            this.pictureBox = new PictureBox();
            this.lblName = new Label();
            this.lblPrice = new Label();
            this.lblSize = new Label();
            this.lblColor = new Label();
            this.lblStock = new Label();
            this.btnAddToCart = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();

            // pictureBox
            this.pictureBox.Location = new Point(10, 10);
            this.pictureBox.Size = new Size(160, 100);
            this.pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox.BorderStyle = BorderStyle.None;

            // lblName
            this.lblName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblName.ForeColor = Color.Black;
            this.lblName.Location = new Point(10, 120);
            this.lblName.Size = new Size(160, 20);
            this.lblName.Text = "Название";

            // lblPrice
            this.lblPrice.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.lblPrice.ForeColor = Color.DarkGreen;
            this.lblPrice.Location = new Point(10, 145);
            this.lblPrice.Size = new Size(160, 20);
            this.lblPrice.Text = "Цена";

            // lblSize
            this.lblSize.Font = new Font("Segoe UI", 8F);
            this.lblSize.ForeColor = Color.Gray;
            this.lblSize.Location = new Point(10, 170);
            this.lblSize.Size = new Size(160, 15);
            this.lblSize.Text = "Размер";

            // lblColor
            this.lblColor.Font = new Font("Segoe UI", 8F);
            this.lblColor.ForeColor = Color.Gray;
            this.lblColor.Location = new Point(10, 190);
            this.lblColor.Size = new Size(160, 15);
            this.lblColor.Text = "Цвет";

            // lblStock
            this.lblStock.Font = new Font("Segoe UI", 8F);
            this.lblStock.ForeColor = Color.Blue;
            this.lblStock.Location = new Point(10, 210);
            this.lblStock.Size = new Size(160, 15);
            this.lblStock.Text = "В наличии";

            // btnAddToCart
            this.btnAddToCart.BackColor = Color.Black;
            this.btnAddToCart.FlatStyle = FlatStyle.Flat;
            this.btnAddToCart.ForeColor = Color.White;
            this.btnAddToCart.Location = new Point(10, 235);
            this.btnAddToCart.Size = new Size(160, 30);
            this.btnAddToCart.Text = "В корзину";
            this.btnAddToCart.UseVisualStyleBackColor = false;
            this.btnAddToCart.Click += new EventHandler(this.btnAddToCart_Click);

            // ProductCardControl
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblPrice);
            this.Controls.Add(this.lblSize);
            this.Controls.Add(this.lblColor);
            this.Controls.Add(this.lblStock);
            this.Controls.Add(this.btnAddToCart);
            this.Size = new Size(180, 280);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
        }
    }
}