using System;
using System.Windows.Forms;
using TattooYou.Forms;

namespace TattooYou
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var splash = new SplashForm())
            {
                splash.ShowDialog();
            }

            Application.Run(new FormLogin());
        }
    }
}