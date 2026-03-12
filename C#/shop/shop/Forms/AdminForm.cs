using shop.DataAccess;
using shop.Entities;
using shop.Services;
using shop.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace shop.Forms
{
    public partial class AdminForm : Form
    {
        private ProductRepository productRepo = new ProductRepository();
        private CategoryRepository categoryRepo = new CategoryRepository();
        private UserRepository userRepo = new UserRepository();
        private SaleRepository saleRepo = new SaleRepository();

        public AdminForm()
        {
            InitializeComponent();
            UiTheme.Apply(this);
            LoadCategories();
            LoadProducts();
            LoadUsers();
            LoadSales();
        }

        // ----- Вкладка Товары -----
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

                // Отключаем авто-генерацию и создаём колонки вручную
                dataGridViewProducts.AutoGenerateColumns = false;
                dataGridViewProducts.Columns.Clear();

                // Скрытая колонка ID
                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Id",
                    HeaderText = "ID",
                    Name = "Id",
                    Visible = false
                });

                // Скрытая колонка CategoryId (нужна для редактирования)
                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "CategoryId",
                    HeaderText = "CategoryId",
                    Name = "CategoryId",
                    Visible = false
                });

                // Видимые колонки
                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Name",
                    HeaderText = "Название",
                    Name = "Name"
                });

                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "CategoryName",
                    HeaderText = "Категория",
                    Name = "CategoryName"
                });

                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Price",
                    HeaderText = "Цена",
                    Name = "Price"
                });

                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Size",
                    HeaderText = "Размер",
                    Name = "Size"
                });

                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Color",
                    HeaderText = "Цвет",
                    Name = "Color"
                });

                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Quantity",
                    HeaderText = "Количество",
                    Name = "Quantity"
                });

                dataGridViewProducts.DataSource = products;
                dataGridViewProducts.Refresh();

                dataGridViewProducts.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки товаров: " + ex.Message);
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
                Size = txtSize.Text,
                Color = txtColor.Text,
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
            product.Size = txtSize.Text;
            product.Color = txtColor.Text;
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

        private void dataGridViewProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewProducts.CurrentRow == null || dataGridViewProducts.Rows.Count == 0)
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

                nudPrice.Value = row.Cells["Price"].Value != null
                    ? Convert.ToDecimal(row.Cells["Price"].Value)
                    : 0;

                txtSize.Text = row.Cells["Size"].Value?.ToString() ?? "";
                txtColor.Text = row.Cells["Color"].Value?.ToString() ?? "";

                nudQuantity.Value = row.Cells["Quantity"].Value != null
                    ? Convert.ToInt32(row.Cells["Quantity"].Value)
                    : 0;
            }
            catch (Exception ex)
            {
                // Игнорируем ошибки при выборе строки
            }
        }

        private void ClearProductFields()
        {
            txtName.Clear();
            cmbCategory.SelectedIndex = -1;
            nudPrice.Value = 0;
            txtSize.Clear();
            txtColor.Clear();
            nudQuantity.Value = 0;
        }

        // ----- Вкладка Пользователи -----
        private void LoadUsers()
        {
            try
            {
                var users = userRepo.GetAll();

                // Настройка DataGridView для пользователей
                dataGridViewUsers.AutoGenerateColumns = false;
                dataGridViewUsers.Columns.Clear();

                dataGridViewUsers.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Id",
                    HeaderText = "ID",
                    Name = "Id",
                    Visible = false
                });

                dataGridViewUsers.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Login",
                    HeaderText = "Логин",
                    Name = "Login"
                });

                dataGridViewUsers.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Role",
                    HeaderText = "Роль",
                    Name = "Role"
                });

                // Скрываем пароль
                dataGridViewUsers.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Password",
                    HeaderText = "Пароль",
                    Name = "Password",
                    Visible = false
                });

                dataGridViewUsers.DataSource = users;
                dataGridViewUsers.Refresh();

                dataGridViewUsers.ClearSelection();
                cmbUserRole.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки пользователей: " + ex.Message);
            }
        }

        private void dataGridViewUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewUsers.CurrentRow == null || dataGridViewUsers.Rows.Count == 0)
                return;

            try
            {
                var row = dataGridViewUsers.CurrentRow;
                string role = row.Cells["Role"].Value?.ToString();
                if (role == "admin")
                    cmbUserRole.SelectedItem = "admin";
                else
                    cmbUserRole.SelectedItem = "user";
            }
            catch
            {
                // игнорируем
            }
        }

        private void btnChangeRole_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsers.CurrentRow == null)
            {
                MessageBox.Show("Выберите пользователя!");
                return;
            }

            if (cmbUserRole.SelectedItem == null)
            {
                MessageBox.Show("Выберите новую роль!");
                return;
            }

            int userId = (int)dataGridViewUsers.CurrentRow.Cells["Id"].Value;
            var user = userRepo.GetById(userId);
            if (user == null)
            {
                MessageBox.Show("Пользователь не найден!");
                return;
            }

            // Получаем выбранную роль из ComboBox
            string selectedRole = cmbUserRole.SelectedItem.ToString();
            user.Role = selectedRole == "admin" ? UserRole.admin : UserRole.user;

            userRepo.Update(user);
            LoadUsers();
            MessageBox.Show("Роль пользователя изменена!");
        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsers.CurrentRow == null)
            {
                MessageBox.Show("Выберите пользователя!");
                return;
            }

            int userId = (int)dataGridViewUsers.CurrentRow.Cells["Id"].Value;

            // Нельзя удалить самого себя
            if (AuthService.CurrentUser != null && AuthService.CurrentUser.Id == userId)
            {
                MessageBox.Show("Вы не можете удалить себя!");
                return;
            }

            if (MessageBox.Show("Удалить пользователя?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                userRepo.Delete(userId);
                LoadUsers();
                MessageBox.Show("Пользователь удалён!");
            }
        }

        // ----- Вкладка Продажи -----
        private void LoadSales()
        {
            try
            {
                dataGridViewSales.AutoGenerateColumns = true;
                dataGridViewSales.DataSource = null;
                dataGridViewSales.DataSource = saleRepo.GetAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки продаж: " + ex.Message);
            }
        }
    }
}