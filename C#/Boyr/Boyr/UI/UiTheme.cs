using System.Drawing;
using System.Windows.Forms;

namespace Boyr.UI
{
    internal static class UiTheme
    {
        private static readonly Font AppFont = new Font("Segoe UI", 9F, FontStyle.Regular);
        private static readonly Font AppFontStrong = new Font("Segoe UI", 9F, FontStyle.Bold);
        private static readonly Font HeaderFont = new Font("Segoe UI", 10F, FontStyle.Bold);
        private static readonly Color AccentColor = Color.Teal;
        private static readonly Color LightBg = Color.FromArgb(245, 245, 250);
        private static readonly Color FieldBg = Color.LightCyan;
        private static readonly Color TextColor = Color.Black;

        public static void Apply(Form form)
        {
            if (form == null) return;

            form.Font = AppFont;
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
                case ToolStrip ts:
                    StyleToolStrip(ts);
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
                case Label lbl:
                    if (lbl.Font.Bold)
                        lbl.ForeColor = AccentColor;
                    break;
            }
        }

        private static void StyleButton(Button b)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.BackColor = AccentColor;
            b.ForeColor = Color.White;
            b.FlatAppearance.MouseOverBackColor = Color.DarkCyan;
            b.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 80, 80);
            b.UseVisualStyleBackColor = false;
            b.Cursor = Cursors.Hand;
            if (b.Height < 32) b.Height = 32;
        }

        private static void StyleTextBox(TextBox tb)
        {
            tb.BorderStyle = BorderStyle.FixedSingle;
            tb.BackColor = Color.White;
            tb.ForeColor = TextColor;
            tb.Enter += (s, e) => tb.BackColor = FieldBg;
            tb.Leave += (s, e) => tb.BackColor = Color.White;
        }

        private static void StyleComboBox(ComboBox cb)
        {
            cb.FlatStyle = FlatStyle.Flat;
            cb.BackColor = Color.White;
        }

        private static void StyleTabs(TabControl tabs)
        {
            tabs.Padding = new Point(14, 6);
            tabs.BackColor = Color.White;
        }

        private static void StyleMenu(MenuStrip menu)
        {
            menu.Renderer = new ToolStripProfessionalRenderer(new LightMenuColorTable());
            menu.BackColor = Color.WhiteSmoke;
            menu.ForeColor = AccentColor;
            foreach (ToolStripMenuItem item in menu.Items)
            {
                item.ForeColor = AccentColor;
                item.Font = AppFontStrong;
            }
        }

        private static void StyleToolStrip(ToolStrip ts)
        {
            ts.RenderMode = ToolStripRenderMode.Professional;
        }

        private static void StyleGrid(DataGridView dgv)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.BorderStyle = BorderStyle.None;
            dgv.GridColor = Color.LightGray;

            dgv.BackgroundColor = Color.White;
            dgv.RowTemplate.Height = 30;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = AccentColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = AppFontStrong;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersHeight = 36;

            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = TextColor;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 230, 230);
            dgv.DefaultCellStyle.SelectionForeColor = AccentColor;
            dgv.DefaultCellStyle.Font = AppFont;
            dgv.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);

            dgv.AlternatingRowsDefaultCellStyle.BackColor = LightBg;
        }

        private class LightMenuColorTable : ProfessionalColorTable
        {
            public override Color MenuItemSelected => Color.LightCyan;
            public override Color MenuItemSelectedGradientBegin => Color.LightCyan;
            public override Color MenuItemSelectedGradientEnd => Color.LightCyan;
            public override Color MenuItemBorder => Color.Teal;
            public override Color ToolStripDropDownBackground => Color.White;
            public override Color ImageMarginGradientBegin => Color.White;
            public override Color ImageMarginGradientMiddle => Color.White;
            public override Color ImageMarginGradientEnd => Color.White;
        }
    }
}