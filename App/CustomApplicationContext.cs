using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RebootTray.App
{
    internal class CustomApplicationContext : ApplicationContext
    {
        private NotifyIcon NotifyIcon { get; set; }
        private ContextMenuStrip ContextMenu { get; set; }

        public CustomApplicationContext()
        {
            var components = new Container();

            // create a new context menu
            ContextMenu = new ContextMenuStrip();

            // add an item for closing the application
            var closeMenuItem = new ToolStripMenuItem("Exit")
            {
                Name = "CloseMenuItem",
                Size = new Size(100, 22),
                Text = "Exit"
            };
            closeMenuItem.Click += CloseMenuItem_Click;

            // add close to the context menu
            ContextMenu.SuspendLayout();
            ContextMenu.Items.AddRange(new ToolStripItem[] { closeMenuItem });
            ContextMenu.Name = "ContextMenu";
            ContextMenu.Size = new Size(110, 70);
            ContextMenu.ResumeLayout(false);

            NotifyIcon = new NotifyIcon(components)
            {
                ContextMenuStrip = ContextMenu,
                Icon = new Icon("reboot.ico"),
                Text = "Click to copy a command to reboot at 4AM.",
                Visible = true
            };
            NotifyIcon.MouseUp += notifyIcon_MouseUp;
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

        #region Event Handlers

        private static void CloseMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void notifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var now = DateTime.Now;
                var then = GetNext4AM(now);
                var timespan = then - now;
                var seconds = Math.Round(timespan.TotalSeconds);
                var text = $"shutdown /r /t {seconds}";
                Clipboard.SetText(text);
                NotifyIcon.ShowBalloonTip(1000,
                    "Shutdown Command Coppied",
                    $"The shutdown command \"{text}\" was coppied to your clipboard. When run, this will reboot your computer at 4:00 AM.",
                    ToolTipIcon.Info);
            }
        }

        #endregion Event Handlers

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            ContextMenu.Dispose();
            ContextMenu = null;
            NotifyIcon.Dispose();
            NotifyIcon = null;
            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}