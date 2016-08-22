using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace RebootTray.App
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
#if DEBUG
            StartApp();
#else
    // Only start if it is not already running.
            var appProcessName = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
            var runningProcesses = Process.GetProcessesByName(appProcessName);
            if (runningProcesses.Length == 1)
            {
                StartApp();
            }
#endif
        }

        private static void StartApp()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationContext applicationContext = new RebootTrayApplicationContext();
            Application.Run(applicationContext);
        }
    }
}