namespace GMS2_RPC.Element
{
    /// <summary> Class that contains strings displayed to the user. </summary>
    internal static class UserText
    {
        /// <summary> Text used in various contexts. </summary>
        internal struct General
        {
            internal static readonly string message_title = "GameMaker Studio 2 Rich Presence";
            internal static readonly string message_proceed = "Proceed?";
        }

        /// <summary> Error notification text. </summary>
        internal struct Error
        {
            internal static readonly string multipleInstances = "An instance of the application is already running.";
        }

        /// <summary> Text displayed solely on the Discord end. </summary>
        internal struct RichPresence
        {
            internal static readonly string details_startPage = "On the Start Page";
            internal static readonly string details_projectOpen = "Working on";
            internal static readonly string details_multipleProjectsOpen = " projects:";

            internal static readonly string status_ide = "In main workspace";
            internal static readonly string status_game = "Running the application";

            internal static readonly string presence_imageTitle = "GameMaker Studio 2 IDE";
        }

        /// <summary> Text displayed by the tray icon and some of its elements. </summary>
        internal struct TrayIcon
        {
            internal static readonly string hover_text = "GameMaker Studio 2 Rich Presence Module";
            internal static readonly string notification_title = General.message_title;

            internal static readonly string menuItem_boot = "Launch on boot";
            internal static readonly string menuItem_removeTray = "Remove tray icon";
            internal static readonly string menuItem_about = "About";
            internal static readonly string menuItem_exit = "Exit";

            internal static readonly string uptime_title = "GameMaker Studio 2 Uptime: ";

            internal static readonly string message_content_removeTray_warning = "This option will permamently hide the tray icon used to control the application. It can be " +
                                                                                 "restored by setting the \"" + INI_Config.Key.hideTrayIcon + "\" key in the " + Path.fileName_config +
                                                                                 " file found in the application's folder to \"False\".\r\n\r\n";

            internal static readonly string message_content_removeTray_bootInfo = "The application is currently not configured to start automatically.\r\n\r\n";
        }

        /// <summary> Text displayed in the "About" window. </summary>
        internal struct About
        {
            internal static readonly string window_title = "About";
            internal static readonly string head_text = "Discord Rich Presence Module for GameMaker Studio 2";
            internal static readonly string repository_text = "GitHub Repository";
            internal static readonly string legalClause_text = "GameMaker Studio 2 and its logo are a property of YoYo Games." +
                                                               "\r\nDiscord is a property of Discord, Inc." +
                                                               "\r\nThis is a third-party project not affiliated with either of them.";
        }
    }
}
