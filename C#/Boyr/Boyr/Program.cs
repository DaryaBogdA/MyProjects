using Boyr.Forms;
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Boyr
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            CultureInfo belarusCulture = new CultureInfo("be-BY");
            Thread.CurrentThread.CurrentCulture = belarusCulture;
            Thread.CurrentThread.CurrentUICulture = belarusCulture;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
    }
}