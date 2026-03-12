using System;
using System.Drawing;
using System.Windows.Forms;

namespace shop.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private MenuStrip menuStrip;
        private ToolStripMenuItem каталогToolStripMenuItem;
        private ToolStripMenuItem корзинаToolStripMenuItem;
        private ToolStripMenuItem моиЗаказыToolStripMenuItem;
        private ToolStripMenuItem выходToolStripMenuItem;
        private FlowLayoutPanel flowLayoutPanel;

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
            this.menuStrip = new MenuStrip();
            this.каталогToolStripMenuItem = new ToolStripMenuItem();
            this.корзинаToolStripMenuItem = new ToolStripMenuItem();
            this.моиЗаказыToolStripMenuItem = new ToolStripMenuItem();
            this.выходToolStripMenuItem = new ToolStripMenuItem();
            this.flowLayoutPanel = new FlowLayoutPanel();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();

            // menuStrip
            this.menuStrip.Items.AddRange(new ToolStripItem[] {
                this.каталогToolStripMenuItem,
                this.корзинаToolStripMenuItem,
                this.моиЗаказыToolStripMenuItem,
                this.выходToolStripMenuItem});
            this.menuStrip.Location = new Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new Size(900, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";

            // каталогToolStripMenuItem
            this.каталогToolStripMenuItem.Name = "каталогToolStripMenuItem";
            this.каталогToolStripMenuItem.Size = new Size(63, 20);
            this.каталогToolStripMenuItem.Text = "Каталог";
            this.каталогToolStripMenuItem.Click += new EventHandler(this.каталогToolStripMenuItem_Click);

            // корзинаToolStripMenuItem
            this.корзинаToolStripMenuItem.Name = "корзинаToolStripMenuItem";
            this.корзинаToolStripMenuItem.Size = new Size(61, 20);
            this.корзинаToolStripMenuItem.Text = "Корзина";
            this.корзинаToolStripMenuItem.Click += new EventHandler(this.корзинаToolStripMenuItem_Click);

            // моиЗаказыToolStripMenuItem
            this.моиЗаказыToolStripMenuItem.Name = "моиЗаказыToolStripMenuItem";
            this.моиЗаказыToolStripMenuItem.Size = new Size(80, 20);
            this.моиЗаказыToolStripMenuItem.Text = "Мои заказы";
            this.моиЗаказыToolStripMenuItem.Click += new EventHandler(this.моиЗаказыToolStripMenuItem_Click);

            // выходToolStripMenuItem
            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            this.выходToolStripMenuItem.Size = new Size(54, 20);
            this.выходToolStripMenuItem.Text = "Выход";
            this.выходToolStripMenuItem.Click += new EventHandler(this.выходToolStripMenuItem_Click);

            // flowLayoutPanel
            this.flowLayoutPanel.AutoScroll = true;
            this.flowLayoutPanel.BackColor = Color.White;
            this.flowLayoutPanel.Dock = DockStyle.Fill;
            this.flowLayoutPanel.Location = new Point(0, 24);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Padding = new Padding(10);
            this.flowLayoutPanel.Size = new Size(900, 526);
            this.flowLayoutPanel.TabIndex = 1;

            // MainForm
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(900, 550);
            this.Controls.Add(this.flowLayoutPanel);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "ILNI Shop - Магазин одежды";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}