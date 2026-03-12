using System.Drawing;
using System.Windows.Forms;

namespace shop.UI
{
    internal static class UiTheme
    {
        private static readonly Font AppFont = new Font("Segoe UI", 9F, FontStyle.Regular);
        private static readonly Font AppFontStrong = new Font("Segoe UI", 9F, FontStyle.Bold);

        public static void Apply(Form form)
        {
            if (form == null) return;

            form.Font = AppFont;
            if (form.BackColor == Control.DefaultBackColor)
                form.BackColor = Color.White;

            form.AutoScaleMode = AutoScaleMode.Font;
            ApplyToControlTree(form);
        }

        public static void ApplyToControlTree(Control root)
        {
            if (root == null) return;

            foreach (Control c in root.Controls)
            {
                ApplyToControl(c);
                if (c.HasChildren)
                    ApplyToControlTree(c);
            }
        }

        private static void ApplyToControl(Control c)
        {
            switch (c)
            {
                case Button b:
                    StyleButton(b);
                    break;
                case DataGridView dgv:
                    StyleGrid(dgv);
                    break;
                case TabControl tabs:
                    StyleTabs(tabs);
                    break;
                case MenuStrip menu:
                    StyleMenu(menu);
                    break;
                case ToolStrip toolStrip:
                    StyleToolStrip(toolStrip);
                    break;
                case TextBox tb:
                    StyleTextBox(tb);
                    break;
                case ComboBox cb:
                    StyleComboBox(cb);
                    break;
                case NumericUpDown nud:
                    nud.BorderStyle = BorderStyle.FixedSingle;
                    break;
                case GroupBox gb:
                    gb.Font = AppFontStrong;
                    break;
            }
        }

        private static void StyleButton(Button b)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            var baseColor = b.BackColor;
            bool isDark = baseColor.R <= 32 && baseColor.G <= 32 && baseColor.B <= 32;
            b.FlatAppearance.MouseOverBackColor = isDark ? Color.FromArgb(30, 30, 30) : Darken(baseColor, 0.10f);
            b.FlatAppearance.MouseDownBackColor = isDark ? Color.FromArgb(20, 20, 20) : Darken(baseColor, 0.20f);
            b.UseVisualStyleBackColor = false;
            b.Cursor = Cursors.Hand;

            if (b.Height < 30)
                b.Height = 30;
        }

        private static void StyleTextBox(TextBox tb)
        {
            tb.BorderStyle = BorderStyle.FixedSingle;
        }

        private static void StyleComboBox(ComboBox cb)
        {
            cb.FlatStyle = FlatStyle.Flat;
        }

        private static void StyleTabs(TabControl tabs)
        {
            tabs.Padding = new Point(14, 6);
        }

        private static void StyleMenu(MenuStrip menu)
        {
            menu.RenderMode = ToolStripRenderMode.System;
        }

        private static void StyleToolStrip(ToolStrip ts)
        {
            ts.RenderMode = ToolStripRenderMode.System;
        }

        private static void StyleGrid(DataGridView dgv)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.BorderStyle = BorderStyle.None;
            dgv.GridColor = Color.Gainsboro;

            dgv.BackgroundColor = Color.White;
            dgv.RowTemplate.Height = 30;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(64, 64, 64);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = AppFontStrong;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersHeight = 34;

            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 30, 30);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.DefaultCellStyle.Font = AppFont;
            dgv.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
        }

        private static Color Darken(Color color, float amount)
        {
            if (color.A == 0) color = Color.Black;

            amount = amount < 0 ? 0 : (amount > 1 ? 1 : amount);
            int r = Clamp((int)(color.R * (1f - amount)));
            int g = Clamp((int)(color.G * (1f - amount)));
            int b = Clamp((int)(color.B * (1f - amount)));
            return Color.FromArgb(color.A, r, g, b);
        }

        private static int Clamp(int v) => v < 0 ? 0 : (v > 255 ? 255 : v);
    }
}

