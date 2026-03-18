using System;
using System.Drawing;
using System.Windows.Forms;

namespace TattooYou.Forms
{
    public partial class SplashForm : Form
    {
        public SplashForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.BackgroundImage = Properties.Resources.logo;
        }

        private void SplashForm_Load(object sender, EventArgs e)
        {
            Timer timer = new Timer();
            timer.Interval = 3000;
            timer.Tick += (s, ev) =>
            {
                timer.Stop();
                this.Close();
            };
            timer.Start();
        }
    }
}