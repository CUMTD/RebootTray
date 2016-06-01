using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RebootTray.App
{
    internal class CustomApplicationContext : ApplicationContext
    {
        private NotifyIcon NotifyIcon { get; set; }

        public CustomApplicationContext()
        {
            var components = new Container();
            NotifyIcon = new NotifyIcon(components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = new Icon("reboot.ico"),
                Text = "Copies run command to reboot at 4AM to clipboard.",
                Visible = true
            };
            NotifyIcon.MouseUp += notifyIcon_MouseUp;
        }

        private static void notifyIcon_MouseUp(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            var then = GetNext4AM(now);
            var timespan = then - now;
            var seconds = Math.Round(timespan.TotalSeconds);
            Clipboard.SetText($"shutdown /r /t {seconds}");
        }

        private static DateTime GetNext4AM(DateTime now)
        {
            if (now.Hour < 4)
            {
                return new DateTime(now.Year, now.Month, now.Day, 4, 0, 0);
            }
            var tomorrow = now.AddDays(1);
            return new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 4, 0, 0);
        }

        protected override void Dispose(bool disposing)
        {
            NotifyIcon.Dispose();
            NotifyIcon = null;
            base.Dispose(disposing);
        }
    }
}