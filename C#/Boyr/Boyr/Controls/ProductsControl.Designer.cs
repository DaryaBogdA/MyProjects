using System.Drawing;
using System.Windows.Forms;

namespace Boyr.Controls
{
    partial class ProductsControl
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridView dataGridViewProducts;
        private Panel panelProductFields;
        private Panel panelProductButtons;
        private Button btnAddProduct;
        private Button btnUpdateProduct;
        private Button btnDeleteProduct;

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
            this.dataGridViewProducts = new System.Windows.Forms.DataGridView();
            this.panelProductFields = new System.Windows.Forms.Panel();
            this.tlpProduct = new System.Windows.Forms.TableLayoutPanel();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblCategory = new System.Windows.Forms.Label();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.lblPrice = new System.Windows.Forms.Label();
            this.nudPrice = new System.Windows.Forms.NumericUpDown();
            this.lblMetal = new System.Windows.Forms.Label();
            this.txtMetal = new System.Windows.Forms.TextBox();
            this.lblPurity = new System.Windows.Forms.Label();
            this.nudPurity = new System.Windows.Forms.NumericUpDown();
            this.lblWeight = new System.Windows.Forms.Label();
            this.nudWeight = new System.Windows.Forms.NumericUpDown();
            this.lblGemstone = new System.Windows.Forms.Label();
            this.txtGemstone = new System.Windows.Forms.TextBox();
            this.lblGemCharacteristics = new System.Windows.Forms.Label();
            this.txtGemCharacteristics = new System.Windows.Forms.TextBox();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.nudQuantity = new System.Windows.Forms.NumericUpDown();
            this.panelProductButtons = new System.Windows.Forms.Panel();
            this.btnAddProduct = new System.Windows.Forms.Button();
            this.btnUpdateProduct = new System.Windows.Forms.Button();
            this.btnDeleteProduct = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewProducts)).BeginInit();
            this.panelProductFields.SuspendLayout();
            this.tlpProduct.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPurity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudQuantity)).BeginInit();
            this.panelProductButtons.SuspendLayout();
            this.SuspendLayout();

            // dataGridViewProducts
            this.dataGridViewProducts.AllowUserToAddRows = false;
            this.dataGridViewProducts.AllowUserToDeleteRows = false;
            this.dataGridViewProducts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewProducts.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewProducts.Dock = System.Windows.Forms.DockStyle.Fill; // ВАЖНО
            this.dataGridViewProducts.Location = new System.Drawing.Point(0, 222); // будет переопределено Dock'ом, можно оставить 0,0
            this.dataGridViewProducts.MultiSelect = false;
            this.dataGridViewProducts.Name = "dataGridViewProducts";
            this.dataGridViewProducts.ReadOnly = true;
            this.dataGridViewProducts.RowHeadersVisible = false;
            this.dataGridViewProducts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewProducts.Size = new System.Drawing.Size(686, 212);
            this.dataGridViewProducts.TabIndex = 0;
            this.dataGridViewProducts.SelectionChanged += new System.EventHandler(this.dataGridViewProducts_SelectionChanged);
            // 
            // panelProductFields
            // 
            this.panelProductFields.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelProductFields.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelProductFields.Controls.Add(this.tlpProduct);
            this.panelProductFields.Dock = System.Windows.Forms.DockStyle.Top; // ВАЖНО
            this.panelProductFields.Location = new System.Drawing.Point(0, 0);
            this.panelProductFields.Name = "panelProductFields";
            this.panelProductFields.Size = new System.Drawing.Size(686, 222);
            this.panelProductFields.TabIndex = 1;
            // 
            // panelProductButtons
            // 
            this.panelProductButtons.BackColor = System.Drawing.Color.LightGray;
            this.panelProductButtons.Controls.Add(this.btnAddProduct);
            this.panelProductButtons.Controls.Add(this.btnUpdateProduct);
            this.panelProductButtons.Controls.Add(this.btnDeleteProduct);
            this.panelProductButtons.Dock = System.Windows.Forms.DockStyle.Bottom; // ВАЖНО
            this.panelProductButtons.Location = new System.Drawing.Point(0, 434);
            this.panelProductButtons.Name = "panelProductButtons";
            this.panelProductButtons.Size = new System.Drawing.Size(686, 43);
            this.panelProductButtons.TabIndex = 2;
            // 
            // btnAddProduct
            // 
            this.btnAddProduct.BackColor = System.Drawing.Color.Teal;
            this.btnAddProduct.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddProduct.ForeColor = System.Drawing.Color.White;
            this.btnAddProduct.Location = new System.Drawing.Point(9, 9);
            this.btnAddProduct.Name = "btnAddProduct";
            this.btnAddProduct.Size = new System.Drawing.Size(86, 26);
            this.btnAddProduct.TabIndex = 0;
            this.btnAddProduct.Text = "Добавить";
            this.btnAddProduct.UseVisualStyleBackColor = false;
            this.btnAddProduct.Click += new System.EventHandler(this.btnAddProduct_Click);
            // 
            // btnUpdateProduct
            // 
            this.btnUpdateProduct.BackColor = System.Drawing.Color.Teal;
            this.btnUpdateProduct.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateProduct.ForeColor = System.Drawing.Color.White;
            this.btnUpdateProduct.Location = new System.Drawing.Point(103, 9);
            this.btnUpdateProduct.Name = "btnUpdateProduct";
            this.btnUpdateProduct.Size = new System.Drawing.Size(86, 26);
            this.btnUpdateProduct.TabIndex = 1;
            this.btnUpdateProduct.Text = "Обновить";
            this.btnUpdateProduct.UseVisualStyleBackColor = false;
            this.btnUpdateProduct.Click += new System.EventHandler(this.btnUpdateProduct_Click);
            // 
            // btnDeleteProduct
            // 
            this.btnDeleteProduct.BackColor = System.Drawing.Color.Teal;
            this.btnDeleteProduct.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteProduct.ForeColor = System.Drawing.Color.White;
            this.btnDeleteProduct.Location = new System.Drawing.Point(197, 9);
            this.btnDeleteProduct.Name = "btnDeleteProduct";
            this.btnDeleteProduct.Size = new System.Drawing.Size(86, 26);
            this.btnDeleteProduct.TabIndex = 2;
            this.btnDeleteProduct.Text = "Удалить";
            this.btnDeleteProduct.UseVisualStyleBackColor = false;
            this.btnDeleteProduct.Click += new System.EventHandler(this.btnDeleteProduct_Click);
            // 
            // tlpProduct
            // 
            this.tlpProduct.ColumnCount = 2;
            this.tlpProduct.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tlpProduct.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpProduct.Controls.Add(this.lblName, 0, 0);
            this.tlpProduct.Controls.Add(this.txtName, 1, 0);
            this.tlpProduct.Controls.Add(this.lblCategory, 0, 1);
            this.tlpProduct.Controls.Add(this.cmbCategory, 1, 1);
            this.tlpProduct.Controls.Add(this.lblPrice, 0, 2);
            this.tlpProduct.Controls.Add(this.nudPrice, 1, 2);
            this.tlpProduct.Controls.Add(this.lblMetal, 0, 3);
            this.tlpProduct.Controls.Add(this.txtMetal, 1, 3);
            this.tlpProduct.Controls.Add(this.lblPurity, 0, 4);
            this.tlpProduct.Controls.Add(this.nudPurity, 1, 4);
            this.tlpProduct.Controls.Add(this.lblWeight, 0, 5);
            this.tlpProduct.Controls.Add(this.nudWeight, 1, 5);
            this.tlpProduct.Controls.Add(this.lblGemstone, 0, 6);
            this.tlpProduct.Controls.Add(this.txtGemstone, 1, 6);
            this.tlpProduct.Controls.Add(this.lblGemCharacteristics, 0, 7);
            this.tlpProduct.Controls.Add(this.txtGemCharacteristics, 1, 7);
            this.tlpProduct.Controls.Add(this.lblQuantity, 0, 8);
            this.tlpProduct.Controls.Add(this.nudQuantity, 1, 8);
            this.tlpProduct.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpProduct.Location = new System.Drawing.Point(0, 0);
            this.tlpProduct.Name = "tlpProduct";
            this.tlpProduct.Padding = new System.Windows.Forms.Padding(3);
            this.tlpProduct.RowCount = 9;
            this.tlpProduct.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tlpProduct.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tlpProduct.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tlpProduct.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tlpProduct.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tlpProduct.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tlpProduct.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tlpProduct.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tlpProduct.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tlpProduct.Size = new System.Drawing.Size(684, 220);
            this.tlpProduct.TabIndex = 1;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblName.Location = new System.Drawing.Point(6, 3);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(68, 23);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Название:";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtName
            // 
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Location = new System.Drawing.Point(80, 6);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(598, 20);
            this.txtName.TabIndex = 1;
            // 
            // lblCategory
            // 
            this.lblCategory.AutoSize = true;
            this.lblCategory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCategory.Location = new System.Drawing.Point(6, 26);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(68, 23);
            this.lblCategory.TabIndex = 2;
            this.lblCategory.Text = "Категория:";
            this.lblCategory.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbCategory
            // 
            this.cmbCategory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Location = new System.Drawing.Point(80, 29);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(598, 21);
            this.cmbCategory.TabIndex = 3;
            // 
            // lblPrice
            // 
            this.lblPrice.AutoSize = true;
            this.lblPrice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPrice.Location = new System.Drawing.Point(6, 49);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(68, 23);
            this.lblPrice.TabIndex = 4;
            this.lblPrice.Text = "Цена:";
            this.lblPrice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nudPrice
            // 
            this.nudPrice.DecimalPlaces = 2;
            this.nudPrice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nudPrice.Location = new System.Drawing.Point(80, 52);
            this.nudPrice.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudPrice.Name = "nudPrice";
            this.nudPrice.Size = new System.Drawing.Size(598, 20);
            this.nudPrice.TabIndex = 5;
            // 
            // lblMetal
            // 
            this.lblMetal.AutoSize = true;
            this.lblMetal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMetal.Location = new System.Drawing.Point(6, 72);
            this.lblMetal.Name = "lblMetal";
            this.lblMetal.Size = new System.Drawing.Size(68, 23);
            this.lblMetal.TabIndex = 6;
            this.lblMetal.Text = "Металл:";
            this.lblMetal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtMetal
            // 
            this.txtMetal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMetal.Location = new System.Drawing.Point(80, 75);
            this.txtMetal.Name = "txtMetal";
            this.txtMetal.Size = new System.Drawing.Size(598, 20);
            this.txtMetal.TabIndex = 7;
            // 
            // lblPurity
            // 
            this.lblPurity.AutoSize = true;
            this.lblPurity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPurity.Location = new System.Drawing.Point(6, 95);
            this.lblPurity.Name = "lblPurity";
            this.lblPurity.Size = new System.Drawing.Size(68, 23);
            this.lblPurity.TabIndex = 8;
            this.lblPurity.Text = "Проба:";
            this.lblPurity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nudPurity
            // 
            this.nudPurity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nudPurity.Location = new System.Drawing.Point(80, 98);
            this.nudPurity.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudPurity.Name = "nudPurity";
            this.nudPurity.Size = new System.Drawing.Size(598, 20);
            this.nudPurity.TabIndex = 9;
            // 
            // lblWeight
            // 
            this.lblWeight.AutoSize = true;
            this.lblWeight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblWeight.Location = new System.Drawing.Point(6, 118);
            this.lblWeight.Name = "lblWeight";
            this.lblWeight.Size = new System.Drawing.Size(68, 23);
            this.lblWeight.TabIndex = 10;
            this.lblWeight.Text = "Вес (г):";
            this.lblWeight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nudWeight
            // 
            this.nudWeight.DecimalPlaces = 2;
            this.nudWeight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nudWeight.Location = new System.Drawing.Point(80, 121);
            this.nudWeight.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudWeight.Name = "nudWeight";
            this.nudWeight.Size = new System.Drawing.Size(598, 20);
            this.nudWeight.TabIndex = 11;
            // 
            // lblGemstone
            // 
            this.lblGemstone.AutoSize = true;
            this.lblGemstone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblGemstone.Location = new System.Drawing.Point(6, 141);
            this.lblGemstone.Name = "lblGemstone";
            this.lblGemstone.Size = new System.Drawing.Size(68, 23);
            this.lblGemstone.TabIndex = 12;
            this.lblGemstone.Text = "Камень:";
            this.lblGemstone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtGemstone
            // 
            this.txtGemstone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtGemstone.Location = new System.Drawing.Point(80, 144);
            this.txtGemstone.Name = "txtGemstone";
            this.txtGemstone.Size = new System.Drawing.Size(598, 20);
            this.txtGemstone.TabIndex = 13;
            // 
            // lblGemCharacteristics
            // 
            this.lblGemCharacteristics.AutoSize = true;
            this.lblGemCharacteristics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblGemCharacteristics.Location = new System.Drawing.Point(6, 164);
            this.lblGemCharacteristics.Name = "lblGemCharacteristics";
            this.lblGemCharacteristics.Size = new System.Drawing.Size(68, 23);
            this.lblGemCharacteristics.TabIndex = 14;
            this.lblGemCharacteristics.Text = "Хар-ки камня:";
            this.lblGemCharacteristics.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtGemCharacteristics
            // 
            this.txtGemCharacteristics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtGemCharacteristics.Location = new System.Drawing.Point(80, 167);
            this.txtGemCharacteristics.Name = "txtGemCharacteristics";
            this.txtGemCharacteristics.Size = new System.Drawing.Size(598, 20);
            this.txtGemCharacteristics.TabIndex = 15;
            // 
            // lblQuantity
            // 
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblQuantity.Location = new System.Drawing.Point(6, 187);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(68, 30);
            this.lblQuantity.TabIndex = 16;
            this.lblQuantity.Text = "Количество:";
            this.lblQuantity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nudQuantity
            // 
            this.nudQuantity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nudQuantity.Location = new System.Drawing.Point(80, 190);
            this.nudQuantity.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudQuantity.Name = "nudQuantity";
            this.nudQuantity.Size = new System.Drawing.Size(598, 20);
            this.nudQuantity.TabIndex = 17;
            // 
            // ProductsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewProducts);
            this.Controls.Add(this.panelProductButtons);
            this.Controls.Add(this.panelProductFields);
            this.Name = "ProductsControl";
            this.Size = new System.Drawing.Size(686, 477);

            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewProducts)).EndInit();
            this.panelProductFields.ResumeLayout(false);
            this.tlpProduct.ResumeLayout(false);
            this.tlpProduct.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPurity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudQuantity)).EndInit();
            this.panelProductButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private TableLayoutPanel tlpProduct;
        private Label lblName;
        private TextBox txtName;
        private Label lblCategory;
        private ComboBox cmbCategory;
        private Label lblPrice;
        private NumericUpDown nudPrice;
        private Label lblMetal;
        private TextBox txtMetal;
        private Label lblPurity;
        private NumericUpDown nudPurity;
        private Label lblWeight;
        private NumericUpDown nudWeight;
        private Label lblGemstone;
        private TextBox txtGemstone;
        private Label lblGemCharacteristics;
        private TextBox txtGemCharacteristics;
        private Label lblQuantity;
        private NumericUpDown nudQuantity;
    }
}