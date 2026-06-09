using shop.Controls;
using shop.DataAccess;
using shop.Entities;
using shop.Services;
using shop.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace shop.Forms
{
    // Объявление делегата
    public delegate void LoadDataDelegate(List<Product> products);

    public partial class MainForm : Form
    {
        private ProductRepository productRepo = new ProductRepository();

        // Объявление события 
        public event LoadDataDelegate DataLoaded;

        // Поток 
        private Thread loadThread;

        private ProgressBar progressBar;
        private Label lblLoading;
        private Panel loadingPanel;

        public MainForm()
        {
            InitializeComponent();
            UiTheme.Apply(this);
            ConfigureMenu();

            CreateLoadingPanel();

            ShowLoading(true);

            this.DataLoaded += OnDataLoaded;

            // Запускаем поток 
            loadThread = new Thread(LoadProductsAsync);
            loadThread.IsBackground = true;
            loadThread.Start();

            SetupShortcuts();

            if (AuthService.IsAdmin)
            {
                LoadChartData();
                panelChart.Visible = true;
                panelTop.Height = 220;
            }
            else
            {
                panelChart.Visible = false;
                panelTop.Height = 0;
            }
        }

        private void CreateLoadingPanel()
        {
            loadingPanel = new Panel();
            loadingPanel.Size = new Size(300, 80);
            loadingPanel.BackColor = Color.White;
            loadingPanel.BorderStyle = BorderStyle.FixedSingle;

            lblLoading = new Label();
            lblLoading.Text = "Загрузка товаров...";
            lblLoading.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblLoading.Size = new Size(280, 25);
            lblLoading.TextAlign = ContentAlignment.MiddleCenter;
            lblLoading.Location = new Point(10, 10);

            progressBar = new ProgressBar();
            progressBar.Size = new Size(280, 25);
            progressBar.Location = new Point(10, 40);
            progressBar.Style = ProgressBarStyle.Marquee; 

            loadingPanel.Controls.Add(lblLoading);
            loadingPanel.Controls.Add(progressBar);

            loadingPanel.Location = new Point(
                (this.ClientSize.Width - loadingPanel.Width) / 2,
                (this.ClientSize.Height - loadingPanel.Height) / 2
            );
            loadingPanel.Anchor = AnchorStyles.None;

            this.Controls.Add(loadingPanel);
            loadingPanel.BringToFront();
        }

        private void ShowLoading(bool show)
        {
            if (loadingPanel != null)
            {
                loadingPanel.Visible = show;
                if (show)
                    loadingPanel.BringToFront();
            }
        }

        private void LoadProductsAsync()
        {
            Thread.Sleep(1500);

            var products = productRepo.GetAll();

            if (DataLoaded != null)
            {
                this.Invoke((Action)(() => DataLoaded(products)));
            }
        }

        private void OnDataLoaded(List<Product> products)
        {
            ShowLoading(false);

            flowLayoutPanel.Controls.Clear();
            foreach (var product in products)
            {
                var card = new ProductCardControl(product);
                card.AddToCartClicked += (s, e) =>
                {
                    if (product.Quantity <= 0)
                    {
                        MessageBox.Show("Товара нет в наличии.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (CartService.AddToCart(product, 1))
                    {
                        MessageBox.Show($"Товар \"{product.Name}\" добавлен в корзину.", "Корзина", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Нельзя добавить больше, чем есть в наличии. Доступно: {product.Quantity}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                };
                flowLayoutPanel.Controls.Add(card);
            }
        }

        private void SetupShortcuts()
        {
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.K)
            {
                btnCatalog_Click(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                btnCart_Click(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.O)
            {
                btnOrders_Click(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.A && AuthService.IsAdmin)
            {
                btnAdmin_Click(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.Q)
            {
                btnExit_Click(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.F5)
            {
                RefreshCatalog();
                e.SuppressKeyPress = true;
            }
        }

        private void RefreshCatalog()
        {
            ShowLoading(true);

            if (loadThread != null && loadThread.IsAlive)
            {
                loadThread.Abort();
            }

            loadThread = new Thread(LoadProductsAsync);
            loadThread.IsBackground = true;
            loadThread.Start();
        }

        private void LoadChartData()
        {
            var sales = new SaleRepository().GetAll();
            var last7Days = new Dictionary<DateTime, decimal>();
            for (int i = 6; i >= 0; i--)
                last7Days[DateTime.Today.AddDays(-i)] = 0;

            foreach (var sale in sales)
                if (last7Days.ContainsKey(sale.SaleDate.Date))
                    last7Days[sale.SaleDate.Date] += sale.TotalAmount;

            chartSales.Series["Продажи"].Points.Clear();
            foreach (var kvp in last7Days)
                chartSales.Series["Продажи"].Points.AddXY(kvp.Key.ToShortDateString(), kvp.Value);
        }

        private void btnCatalog_Click(object sender, EventArgs e)
        {
            RefreshCatalog();
        }

        private void btnCart_Click(object sender, EventArgs e)
        {
            CartForm cart = new CartForm();
            cart.ShowDialog();
            LoadProducts();
        }

        private void btnOrders_Click(object sender, EventArgs e)
        {
            if (!AuthService.IsAuthenticated)
            {
                MessageBox.Show("Необходимо войти в систему.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            OrderHistoryForm history = new OrderHistoryForm();
            history.ShowDialog();
        }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            if (!AuthService.IsAdmin)
            {
                MessageBox.Show("Доступ запрещён.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            AdminForm admin = new AdminForm();
            admin.ShowDialog();
            RefreshCatalog();

            if (AuthService.IsAdmin)
            {
                LoadChartData();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (loadThread != null && loadThread.IsAlive)
            {
                loadThread.Abort();
            }
            AuthService.Logout();
            Application.Restart();
        }

        private void ConfigureMenu()
        {
            menuStrip.BackColor = Color.FromArgb(64, 64, 64);
            menuStrip.ForeColor = Color.White;
            foreach (ToolStripMenuItem item in menuStrip.Items)
            {
                item.ForeColor = Color.White;
            }

            if (AuthService.IsAdmin)
            {
                var adminItem = new ToolStripMenuItem("Администрирование");
                adminItem.Click += (s, e) => OpenAdminForm();
                menuStrip.Items.Add(adminItem);
            }
        }

        private void OpenAdminForm()
        {
            AdminForm admin = new AdminForm();
            admin.ShowDialog();
        }

        public void LoadProducts()
        {
            var products = productRepo.GetAll();
            flowLayoutPanel.Controls.Clear();
            foreach (var product in products)
            {
                var card = new ProductCardControl(product);
                card.AddToCartClicked += (s, e) =>
                {
                    if (product.Quantity <= 0)
                    {
                        MessageBox.Show("Товара нет в наличии.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (CartService.AddToCart(product, 1))
                    {
                        MessageBox.Show($"Товар \"{product.Name}\" добавлен в корзину.", "Корзина", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Нельзя добавить больше, чем есть в наличии. Доступно: {product.Quantity}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                };
                flowLayoutPanel.Controls.Add(card);
            }
        }

        private void каталогToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshCatalog();
        }

        private void корзинаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CartForm cart = new CartForm();
            cart.ShowDialog();
            LoadProducts();
        }

        private void моиЗаказыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrderHistoryForm history = new OrderHistoryForm();
            history.ShowDialog();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (loadThread != null && loadThread.IsAlive)
            {
                loadThread.Abort();
            }
            AuthService.Logout();
            Application.Restart();
        }
    }
}