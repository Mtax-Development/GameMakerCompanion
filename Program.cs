using System;
using System.IO;
using System.Windows.Forms;
using GMS2_RPC.Main;
using GMS2_RPC.Element;

namespace GMS2_RPC
{
    /// <summary> Entry class for the program. </summary>
    public static class Program
    {
        /// <summary> Name of the application's executable file. </summary>
        internal static readonly string projectName = "GMS2_RPC";

        /// <summary> Configuration for application's automatic startup. </summary>
        internal static bool startOnBoot = false;

        /// <summary> Configuration for display of project titles in Rich Presence text. </summary>
        internal static bool showProjectTitles = true;

        /// <summary> Configuration for display of application's tray icon. </summary>
        internal static bool hideTrayIcon = false;

        /// <summary> Entry function for the program. </summary>
        /// <remarks> The application is restricted to only one instance. All subsequent ones will automatically quit after showing a message box. </remarks>
        public static void Main()
        {
            Console.WriteLine(projectName + " start.");

            if (ApplicationInstance.GetNumber(projectName) <= 1)
            {
                //|Create the default config file if it does not exist.
                if (!File.Exists(GMS2_RPC.Element.Path.file_config))
                {
                    INI_Config.CreateDefault();
                }

                //|Read the configuration from the config file.
                startOnBoot = ((INI_Config.StartOnBoot.ToLower() == "true") ? true : false);
                showProjectTitles = ((INI_Config.ShowProjectTitles.ToLower() == "true") ? true : false);
                hideTrayIcon = ((INI_Config.HideTrayIcon.ToLower() == "false") ? false : true);

                //|Handle the registry key for automatic startup of the application, based on the configuration.
                RegistryHandler.SetAutomaticStartup(startOnBoot);

                Resource.InitializeResources();

                //|Create a tray icon if the config did not specify it being disabled.
                if (!hideTrayIcon)
                {
                    TrayIcon.Create();
                }

                //|Run a handler for the Rich Presence. This uses an endless loop and should be called last on a thread.
                RPC_Handler.Start();
            }
            else
            {
                //|If another instance of the application is running, notify the user about that and exit the duplicate.
                Console.WriteLine("An instance of " + projectName + " already exists. Exiting.");

                DialogResult dialogResult = MessageBox.Show(UserText.Error.multipleInstances, UserText.General.message_title, MessageBoxButtons.OK);

                Environment.Exit(0);
            }
        }
    }
}
