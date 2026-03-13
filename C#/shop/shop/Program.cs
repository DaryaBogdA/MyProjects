using shop.Forms;
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace shop
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            CultureInfo belarusianCulture = new CultureInfo("be-BY");
            Thread.CurrentThread.CurrentCulture = belarusianCulture;
            Thread.CurrentThread.CurrentUICulture = belarusianCulture;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
    }
}