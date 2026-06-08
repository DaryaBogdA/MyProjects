using System.Drawing;
using System.Windows.Forms;

namespace TattooYou.Helpers
{
    public static class UiTheme
    {
        public static readonly Color Accent = Color.FromArgb(155, 89, 182);
        public static readonly Color AccentLight = Color.FromArgb(235, 220, 245);
        public static readonly Color AccentDark = Color.FromArgb(120, 60, 150);
        public static readonly Color Background = Color.White;
        public static readonly Color TextPrimary = Color.Black;
        public static readonly Color TextMuted = Color.FromArgb(90, 90, 90);
        public static readonly Color HeaderBg = Color.FromArgb(248, 245, 252);
        public static readonly Color GridHeader = Color.FromArgb(230, 230, 230);
        public static readonly Color GridAltRow = Color.FromArgb(250, 247, 253);
        public static readonly Color CardBorder = Color.FromArgb(220, 210, 235);
        public static readonly Color LoginDark = Color.FromArgb(44, 44, 44);
        public static readonly Color InputDark = Color.FromArgb(64, 64, 64);

        public static void ApplyPrimaryButton(Button btn)
        {
            btn.BackColor = Accent;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        }

        public static void ApplySecondaryButton(Button btn)
        {
            btn.BackColor = AccentLight;
            btn.ForeColor = AccentDark;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        }

        public static void ApplyDataGrid(DataGridView dgv)
        {
            dgv.BackgroundColor = Background;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = CardBorder;
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 36;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = GridHeader;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextPrimary;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(4);
            dgv.DefaultCellStyle.BackColor = Background;
            dgv.DefaultCellStyle.ForeColor = TextPrimary;
            dgv.DefaultCellStyle.SelectionBackColor = AccentLight;
            dgv.DefaultCellStyle.SelectionForeColor = TextPrimary;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = GridAltRow;
            dgv.RowTemplate.Height = 32;
        }

        public static Panel CreateStatCard(string title, Label valueLabel, int width = 180, int height = 88)
        {
            var card = new Panel
            {
                Size = new Size(width, height),
                BackColor = Background,
                Margin = new Padding(8)
            };
            card.Paint += (s, e) =>
            {
                var g = e.Graphics;
                using (var pen = new Pen(CardBorder, 1))
                using (var accent = new Pen(Accent, 3))
                {
                    g.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
                    g.DrawLine(accent, 0, 0, card.Width, 0);
                }
            };

            var lblTitle = new Label
            {
                Text = title,
                ForeColor = TextMuted,
                Font = new Font("Segoe UI", 8.5F),
                Location = new Point(12, 14),
                AutoSize = true
            };
            valueLabel.Location = new Point(12, 38);
            valueLabel.AutoSize = true;
            valueLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            valueLabel.ForeColor = AccentDark;

            card.Controls.Add(lblTitle);
            card.Controls.Add(valueLabel);
            return card;
        }
    }
}
