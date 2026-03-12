using System.Drawing;
using System.Windows.Forms;

namespace paseka.Helpers
{
    public static class TextBoxWatermark
    {
        public static void Set(TextBox textBox, string watermarkText)
        {
            Color originalColor = textBox.ForeColor;

            textBox.Text = watermarkText;
            textBox.ForeColor = Color.Gray;

            textBox.Enter += (sender, e) =>
            {
                if (textBox.Text == watermarkText)
                {
                    textBox.Text = "";
                    textBox.ForeColor = originalColor;
                }
            };

            textBox.Leave += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = watermarkText;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }
    }
}