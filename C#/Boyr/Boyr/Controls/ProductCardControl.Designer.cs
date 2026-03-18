using System;
using System.Drawing;
using System.Windows.Forms;

namespace Boyr.Controls
{
    partial class ProductCardControl
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblName;
        private Label lblPrice;
        private Label lblMetal;
        private Label lblWeight;
        private Label lblGemstone;
        private Label lblGemCharacteristics;
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
            this.lblMetal = new Label();
            this.lblWeight = new Label();
            this.lblGemstone = new Label();
            this.lblGemCharacteristics = new Label();
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

            // lblMetal
            this.lblMetal.Font = new Font("Segoe UI", 8F);
            this.lblMetal.ForeColor = Color.Gray;
            this.lblMetal.Location = new Point(10, 170);
            this.lblMetal.Size = new Size(160, 15);
            this.lblMetal.Text = "Металл";

            // lblWeight
            this.lblWeight.Font = new Font("Segoe UI", 8F);
            this.lblWeight.ForeColor = Color.Gray;
            this.lblWeight.Location = new Point(10, 185);
            this.lblWeight.Size = new Size(160, 15);
            this.lblWeight.Text = "Вес";

            // lblGemstone
            this.lblGemstone.Font = new Font("Segoe UI", 8F);
            this.lblGemstone.ForeColor = Color.Gray;
            this.lblGemstone.Location = new Point(10, 200);
            this.lblGemstone.Size = new Size(160, 15);
            this.lblGemstone.Text = "Камень";

            // lblGemCharacteristics
            this.lblGemCharacteristics.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            this.lblGemCharacteristics.ForeColor = Color.Gray;
            this.lblGemCharacteristics.Location = new Point(10, 215);
            this.lblGemCharacteristics.Size = new Size(160, 15);
            this.lblGemCharacteristics.Text = "Характеристики";

            // lblStock
            this.lblStock.Font = new Font("Segoe UI", 8F);
            this.lblStock.ForeColor = Color.Blue;
            this.lblStock.Location = new Point(10, 230);
            this.lblStock.Size = new Size(160, 15);
            this.lblStock.Text = "В наличии";

            // btnAddToCart
            this.btnAddToCart.BackColor = Color.Black;
            this.btnAddToCart.FlatStyle = FlatStyle.Flat;
            this.btnAddToCart.ForeColor = Color.White;
            this.btnAddToCart.Location = new Point(10, 255);
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
            this.Controls.Add(this.lblMetal);
            this.Controls.Add(this.lblWeight);
            this.Controls.Add(this.lblGemstone);
            this.Controls.Add(this.lblGemCharacteristics);
            this.Controls.Add(this.lblStock);
            this.Controls.Add(this.btnAddToCart);
            this.Size = new Size(180, 300);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
        }
    }
}