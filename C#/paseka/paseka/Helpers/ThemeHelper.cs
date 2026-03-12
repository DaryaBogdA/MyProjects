using System.Drawing;
using System.Windows.Forms;

namespace paseka.Helpers
{
    public static class ThemeHelper
    {
        public static void ApplyDarkTheme(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is Button btn)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.BackColor = Color.FromArgb(255, 68, 68);
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderSize = 0;
                }
                else if (c is Label lbl)
                {
                    lbl.ForeColor = Color.White;
                    lbl.BackColor = Color.Transparent;
                }
                else if (c is TextBox tb)
                {
                    tb.BackColor = Color.FromArgb(64, 64, 64); 
                    tb.ForeColor = Color.White;
                    tb.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (c is ComboBox cmb)
                {
                    cmb.BackColor = Color.FromArgb(64, 64, 64);
                    cmb.ForeColor = Color.White;
                    cmb.FlatStyle = FlatStyle.Flat;
                }
                else if (c is DateTimePicker dtp)
                {
                    dtp.BackColor = Color.FromArgb(64, 64, 64);
                    dtp.ForeColor = Color.White;
                }
                else if (c is CheckBox chk)
                {
                    chk.ForeColor = Color.White;
                }
                else if (c is DataGridView dgv)
                {
                    dgv.BackgroundColor = Color.Black;
                    dgv.GridColor = Color.DarkRed;
                    dgv.DefaultCellStyle.BackColor = Color.Black;
                    dgv.DefaultCellStyle.ForeColor = Color.White;
                    dgv.DefaultCellStyle.SelectionBackColor = Color.Red;
                    dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkRed;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    dgv.EnableHeadersVisualStyles = false;
                }
                if (c.HasChildren)
                    ApplyDarkTheme(c);
            }

            parent.BackColor = Color.FromArgb(30, 30, 30);
        }
    }
}