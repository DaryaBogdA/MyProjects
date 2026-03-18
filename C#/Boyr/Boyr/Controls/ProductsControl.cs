using Boyr.DataAccess;
using Boyr.Entities;
using Boyr.UI;
using System;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Boyr.Controls
{
    public partial class ProductsControl : UserControl
    {
        private ProductRepository productRepo = new ProductRepository();
        private CategoryRepository categoryRepo = new CategoryRepository();

        public ProductsControl()
        {
            InitializeComponent();
            UiTheme.ApplyToControlTree(this);
            LoadCategories();
            LoadProducts();
        }

        private void LoadCategories()
        {
            cmbCategory.DataSource = categoryRepo.GetAll();
            cmbCategory.DisplayMember = "Name";
            cmbCategory.ValueMember = "Id";
            cmbCategory.SelectedIndex = -1;
        }

        private void LoadProducts()
        {
            try
            {
                var products = productRepo.GetAll();

                dataGridViewProducts.AutoGenerateColumns = false;
                dataGridViewProducts.Columns.Clear();

                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Name = "Id", Visible = false });
                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CategoryId", HeaderText = "CategoryId", Name = "CategoryId", Visible = false });

                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Название", Name = "Name" });
                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CategoryName", HeaderText = "Категория", Name = "CategoryName" });
                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Price", HeaderText = "Цена", Name = "Price" });
                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Metal", HeaderText = "Металл", Name = "Metal" });
                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Purity", HeaderText = "Проба", Name = "Purity" });
                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Weight", HeaderText = "Вес (г)", Name = "Weight" });
                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Gemstone", HeaderText = "Камень", Name = "Gemstone" });
                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "GemCharacteristics", HeaderText = "Хар-ки камня", Name = "GemCharacteristics" });
                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "Количество", Name = "Quantity" });

                dataGridViewProducts.DataSource = products;
                dataGridViewProducts.Refresh();
                dataGridViewProducts.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки товаров: " + ex.Message);
                // Очищаем DataGridView, чтобы избежать ошибок с колонками
                dataGridViewProducts.DataSource = null;
                dataGridViewProducts.Columns.Clear();
            }
        }

        private void dataGridViewProducts_SelectionChanged(object sender, EventArgs e)
        {
            // Проверка наличия строк и колонок
            if (dataGridViewProducts.CurrentRow == null || dataGridViewProducts.Rows.Count == 0)
                return;

            // Защита от отсутствия необходимых колонок
            if (dataGridViewProducts.Columns.Count == 0 || !dataGridViewProducts.Columns.Contains("Name"))
                return;

            try
            {
                var row = dataGridViewProducts.CurrentRow;

                txtName.Text = row.Cells["Name"].Value?.ToString() ?? "";

                var categoryId = row.Cells["CategoryId"].Value;
                if (categoryId != null && categoryId != DBNull.Value)
                {
                    cmbCategory.SelectedValue = categoryId;
                }
                else
                {
                    cmbCategory.SelectedIndex = -1;
                }

                nudPrice.Value = row.Cells["Price"].Value != null ? Convert.ToDecimal(row.Cells["Price"].Value) : 0;
                txtMetal.Text = row.Cells["Metal"].Value?.ToString() ?? "";
                nudPurity.Value = row.Cells["Purity"].Value != null ? Convert.ToInt32(row.Cells["Purity"].Value) : 0;
                nudWeight.Value = row.Cells["Weight"].Value != null ? Convert.ToDecimal(row.Cells["Weight"].Value) : 0;
                txtGemstone.Text = row.Cells["Gemstone"].Value?.ToString() ?? "";
                txtGemCharacteristics.Text = row.Cells["GemCharacteristics"].Value?.ToString() ?? "";
                nudQuantity.Value = row.Cells["Quantity"].Value != null ? Convert.ToInt32(row.Cells["Quantity"].Value) : 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            if (cmbCategory.SelectedValue == null)
            {
                MessageBox.Show("Выберите категорию!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название товара!");
                return;
            }

            var product = new Product
            {
                Name = txtName.Text,
                CategoryId = (int)cmbCategory.SelectedValue,
                Price = nudPrice.Value,
                Metal = txtMetal.Text,
                Purity = (int)nudPurity.Value,
                Weight = nudWeight.Value,
                Gemstone = txtGemstone.Text,
                GemCharacteristics = txtGemCharacteristics.Text,
                Quantity = (int)nudQuantity.Value
            };

            productRepo.Add(product);
            LoadProducts();
            ClearProductFields();
            MessageBox.Show("Товар добавлен!");
        }

        private void btnUpdateProduct_Click(object sender, EventArgs e)
        {
            if (dataGridViewProducts.CurrentRow == null)
            {
                MessageBox.Show("Выберите товар для редактирования!");
                return;
            }

            if (cmbCategory.SelectedValue == null)
            {
                MessageBox.Show("Выберите категорию!");
                return;
            }

            int id = (int)dataGridViewProducts.CurrentRow.Cells["Id"].Value;

            var product = productRepo.GetById(id);
            if (product == null)
            {
                MessageBox.Show("Товар не найден!");
                return;
            }

            product.Name = txtName.Text;
            product.CategoryId = (int)cmbCategory.SelectedValue;
            product.Price = nudPrice.Value;
            product.Metal = txtMetal.Text;
            product.Purity = (int)nudPurity.Value;
            product.Weight = nudWeight.Value;
            product.Gemstone = txtGemstone.Text;
            product.GemCharacteristics = txtGemCharacteristics.Text;
            product.Quantity = (int)nudQuantity.Value;

            productRepo.Update(product);
            LoadProducts();
            MessageBox.Show("Товар обновлён!");
        }

        private void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            if (dataGridViewProducts.CurrentRow == null)
            {
                MessageBox.Show("Выберите товар для удаления!");
                return;
            }

            int id = (int)dataGridViewProducts.CurrentRow.Cells["Id"].Value;

            if (MessageBox.Show("Удалить товар?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                productRepo.Delete(id);
                LoadProducts();
                ClearProductFields();
                MessageBox.Show("Товар удалён!");
            }
        }

        private void ClearProductFields()
        {
            txtName.Clear();
            cmbCategory.SelectedIndex = -1;
            nudPrice.Value = 0;
            txtMetal.Clear();
            nudPurity.Value = 0;
            nudWeight.Value = 0;
            txtGemstone.Clear();
            txtGemCharacteristics.Clear();
            nudQuantity.Value = 0;
        }
    }
}