using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RebootTray.App
{
    /// <summary>
    /// A custom <see cref="ApplicationContext" /> for the reboot tray app.
    /// </summary>
    internal class RebootTrayApplicationContext : ApplicationContext
    {
        /// <summary>
        /// The tray icon.
        /// </summary>
        private NotifyIcon NotifyIcon { get; set; }

        /// <summary>
        /// The context menu for the tray.
        /// </summary>
        private ContextMenuStrip ContextMenu { get; set; }

        /// <summary>
        /// Create a new instance of <see cref="RebootTrayApplicationContext" />
        /// </summary>
        public RebootTrayApplicationContext()
        {
            InitializeComponents();
        }

        /// <summary>
        /// Get a DateTime representing the next time it will be 4AM.
        /// </summary>
        /// <param name="now">
        /// The current time.
        /// </param>
        /// <returns>
        /// A DateTime representing the next time it will be 4AM.
        /// </returns>
        private static DateTime GetNext4AM(DateTime now)
        {
            if (now.Hour < 4)
            {
                return new DateTime(now.Year, now.Month, now.Day, 4, 0, 0);
            }
            var tomorrow = now.AddDays(1);
            return new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 4, 0, 0);
        }

        #region Initialization

        /// <summary>
        /// Initialize all components.
        /// </summary>
        private void InitializeComponents()
        {
            InitializeContextMenu();
            InitializeNotifyIcon();
        }

        /// <summary>
        /// Initialize the context menu for the NotifyIcon.
        /// </summary>
        private void InitializeContextMenu()
        {
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
        }

        /// <summary>
        /// Initialize the NotifyIcon (Tray Icon)
        /// </summary>
        private void InitializeNotifyIcon()
        {
            var components = new Container();
            NotifyIcon = new NotifyIcon(components)
            {
                ContextMenuStrip = ContextMenu,
                Icon = new Icon("reboot.ico"),
                Text = "Click to copy a command to reboot at 4AM.",
                Visible = true
            };
            NotifyIcon.MouseUp += notifyIcon_MouseUp;
        }

        #endregion Initialization

        #region Event Handlers

        /// <summary>
        /// Handles close clicks by closing the application.
        /// </summary>
        /// <param name="sender">
        /// The event sender.
        /// </param>
        /// <param name="e">
        /// The event.
        /// </param>
        private static void CloseMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Handles clicks on the notify icon.
        /// </summary>
        /// <param name="sender">
        /// The event sender.
        /// </param>
        /// <param name="e">
        /// The click event.
        /// </param>
        /// <remarks>
        /// Will only respond to left clicks.
        /// </remarks>
        private void notifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var now = DateTime.Now;
                var then = GetNext4AM(now);
                var timespan = then - now;
                var seconds = Math.Round(timespan.TotalSeconds);
                var text = $"shutdown /r /t {seconds}";
                try
                {
                    Clipboard.SetText(text);
                    NotifyIcon.ShowBalloonTip(1000,
                        "Shutdown Command Coppied",
                        $"The shutdown command \"{text}\" was coppied to your clipboard. When run, this will reboot your computer at 4:00 AM.",
                        ToolTipIcon.Info);
                }
                catch (Exception ex)
                {
                    NotifyIcon.ShowBalloonTip(1000, "Unable to access clipboard.", ex.Message, ToolTipIcon.Error);
                }
            }
        }

        #endregion Event Handlers

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ContextMenu.Dispose();
                ContextMenu = null;
                NotifyIcon.Dispose();
                NotifyIcon = null;
            }
            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}