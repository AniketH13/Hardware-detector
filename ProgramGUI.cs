using System;
using System.Windows.Forms;

namespace DetectIt
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Enable per-monitor DPI awareness (Windows 10+)
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
