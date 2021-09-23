using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using GMS2_RPC.Element;

namespace GMS2_RPC.Main
{
    /// <summary> Class that handles creation and operations of the program's tray icon. </summary>
    internal static class TrayIcon
    {
        private static readonly NotifyIcon trayIcon = new NotifyIcon();

        private static readonly ContextMenu menu = new ContextMenu();
        private static readonly MenuItem menuItem_boot = new MenuItem(UserText.TrayIcon.menuItem_boot, Event.Menu_MarkBoot);
        private static readonly MenuItem menuItem_showTitles = new MenuItem(UserText.TrayIcon.menuItem_showTitles, Event.Menu_MarkTitles);
        private static readonly MenuItem menuItem_removeTray = new MenuItem(UserText.TrayIcon.menuItem_removeTray, Event.Menu_RemoveTray);
        private static readonly MenuItem menuItem_about = new MenuItem(UserText.TrayIcon.menuItem_about, Event.Menu_About);
        private static readonly MenuItem menuItem_exit = new MenuItem(UserText.TrayIcon.menuItem_exit, Event.Menu_ApplicationExit);

        /// <summary> Structure containing all events that the tray icon can trigger. </summary>
        internal readonly struct Event
        {
            /// <summary> Event that triggers on the left click of the tray icon. </summary>
            /// <remarks> A system notification will be created with the data about total uptime of the IDE during this application's runtime. </remarks>
            internal static void Click(object sender, EventArgs e)
            {
                MouseEventArgs m = (MouseEventArgs)e;

                switch (m.Button)
                {
                    case MouseButtons.Left:
                        TimeSpan ts = RPC_Handler.application_main_uptime.Elapsed;

                        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
                        string text_uptime = UserText.TrayIcon.uptime_title + elapsedTime;

                        trayIcon.ShowBalloonTip(10000, UserText.TrayIcon.notification_title, text_uptime, ToolTipIcon.None);
                    break;
                }
            }

            /// <summary> Event that triggers on click of a tray menu option related to changing the automatic startup configuration. </summary>
            /// <remarks> The status of the menu item being checked will switch. An appriopate information about automatic startup will be written
            /// to the INI config and the registry. </remarks>
            internal static void Menu_MarkBoot(object sender, EventArgs e)
            {
                menuItem_boot.Checked = !menuItem_boot.Checked;

                Program.startOnBoot = menuItem_boot.Checked;
                INI_Config.StartOnBoot = Program.startOnBoot.ToString();

                RegistryHandler.SetAutomaticStartup(menuItem_boot.Checked);
            }

            /// <summary> Event that triggers on click of a tray menu option related to changing the option of including project titles in Rich Presence text. </summary>
            /// <remarks> The status of the menu item being checked will switch. An appriopate information about including project titles in Rich Presence text will be
            /// written to the INI config and the registry. </remarks>
            internal static void Menu_MarkTitles(object sender, EventArgs e)
            {
                menuItem_showTitles.Checked = !menuItem_showTitles.Checked;

                Program.showProjectTitles = menuItem_showTitles.Checked;
                INI_Config.ShowProjectTitles = Program.showProjectTitles.ToString();
            }

            /// <summary> Event that triggers on click of a tray menu option related to permamently disabling the tray icon. </summary>
            /// <remarks> A "Yes/No" message box will be shown with a warning about that the tray icon removal is permament. Upon receiving "Yes",
            /// the information about tray removal will be written to the INI config and the tray icon will exit. </remarks>
            internal static void Menu_RemoveTray(object sender, EventArgs e)
            {
                string message = UserText.TrayIcon.message_content_removeTray_warning;

                if (!Program.startOnBoot)
                {
                    //|Add a note if the this application is not configured to start automatically.
                    message += UserText.TrayIcon.message_content_removeTray_bootInfo;
                }

                message += UserText.General.message_proceed;

                DialogResult dialogResult = MessageBox.Show(message, UserText.General.message_title, MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    Program.hideTrayIcon = true;
                    INI_Config.HideTrayIcon = Program.hideTrayIcon.ToString();

                    trayIcon.Dispose();
                    Application.Exit();
                }
            }

            /// <summary> Event that triggers on click of a tray menu option relating to opening a window with details about the application. </summary>
            /// <remarks> A customized window will be shown with information about the application. </remarks>
            internal static void Menu_About(object sender, EventArgs e)
            {
                Window.About window = new Window.About();

                window.Show();
            }

            /// <summary> Event that triggers on click of a tray menu option related to exiting the entire application. </summary>
            /// <remarks> All elements of the application will be closed. </remarks>
            internal static void Menu_ApplicationExit(object sender, EventArgs e)
            {
                Console.WriteLine("Exiting the application...");

                trayIcon.Dispose();
                Environment.Exit(1);
            }
        }

        /// <summary> Create the tray icon and run it. </summary>
        internal static void Create()
        {
            menu.MenuItems.Add(menuItem_boot);
            menu.MenuItems.Add(menuItem_showTitles);
            menu.MenuItems.Add(menuItem_removeTray);
            menu.MenuItems.Add(menuItem_about);
            menu.MenuItems.Add(menuItem_exit);

            menuItem_boot.Checked = Program.startOnBoot;
            menuItem_showTitles.Checked = Program.showProjectTitles;

            Console.WriteLine("Creating the Tray Icon.");

            Thread thread = new Thread
            (
                delegate ()
                {
                    trayIcon.Text = UserText.TrayIcon.hover_text;
                    trayIcon.Icon = Resource.projectIcon;
                    trayIcon.Visible = true;
                    trayIcon.Click += Event.Click;
                    trayIcon.ContextMenu = menu;

                    Application.Run();
                }
            );

            thread.Start();
        }
    }
}
