using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using DiscordRPC; //|https://github.com/Lachee/discord-rpc-csharp
using GMS2_RPC.Element;

namespace GMS2_RPC.Main
{
    /// <summary> Class that handles all operations to Discord Rich Presence. </summary>
    internal static class RPC_Handler
    {
        /// <summary> ID of the application on Discord's Developer site. </summary>
        /// <remarks> Required for usage of assets hosted on Discord's Developer site. </remarks>
        private static readonly string clientID_GMS2 = "634416362898325534";

        /// <summary> Name variations of the primary executable. </summary>
        private static readonly string[] applicationList_IDE = {"GameMakerStudio", "GameMakerStudio-Beta"};

        /// <summary> Name of the executable that runs the developed application. </summary>
        private static readonly string application_runner = "Runner";

        /// <summary> Unique window title that the primary executable will have while on its start page.</summary>
        internal static readonly string windowTitle_startPage = "Start Page - GameMaker Studio 2";

        /// <summary> Time between checks for instances of primary executable. </summary>
        private static int applicationCheckDelay = Int32.Parse(INI_Config.DefaultData.applicationCheckDelay);

        /// <summary> Time after which RPC status is removed after the application is no longer detected. 
        /// <remarks> Attempts to prevent non-RPC display. </remarks>
        private static int applicationExitDelay = Int32.Parse(INI_Config.DefaultData.applicationExitDelay);

        /// <summary> Minimum clamp of the delay time. </summary>
        /// <remarks> Used to ignore low user configuration to prevent CPU overhead. </remarks>
        private static readonly int delay_minimum = 200;

        /// <summary> Total uptime of the primary executable during the runtime of this application. </summary>
        internal static readonly Stopwatch application_main_uptime = new Stopwatch();

        /// <summary> Details of the application image in Rich Presence on Discord. </summary>
        private static readonly Assets RPC_icon = new Assets()
        {
            LargeImageKey = Resource.presence_image_main,
            LargeImageText = UserText.RichPresence.presence_imageTitle
        };

        /// <summary> Entry function for the class. </summary>
        /// <remarks> Handles preparations and launches an endless loop that checks for appriopate applications and enables Discord Rich Presence. </remarks>
        internal static void Start()
        {
            //|Read the config file for delay setup and apply either it or the minimum delay clamp.
            int _applicationCheckDelay = applicationCheckDelay;
            int _applicationExitDelay = applicationExitDelay;

            if (Int32.TryParse(INI_Config.ApplicationCheckDelay, out _applicationCheckDelay))
            {
                if (_applicationCheckDelay >= delay_minimum)
                {
                    applicationCheckDelay = _applicationCheckDelay;
                }
            }

            if (Int32.TryParse(INI_Config.ApplicationExitDelay, out _applicationExitDelay))
            {
                if (_applicationExitDelay >= delay_minimum)
                {
                    applicationExitDelay = _applicationExitDelay;
                }
            }

            //|IDE-checking loop. 
            // Continously checks for whether the IDE is running. If it is, a different loop will be applied inside of it.
            // If the IDE stops running, it will clean itself up and return to this loop.
            while (true)
            {
                if (ApplicationInstance.GetNumber(applicationList_IDE) > 0)
                {
                    application_main_uptime.Start(); //|Start the total IDE uptime stopwatch.
                    SetPresence_GMS2();
                }

                Thread.Sleep(applicationCheckDelay); //|Since this is an infinite loop, a delay is applied to not cause a CPU overhead.
            }
        }

        /// <summary> Cut the project name from the obtained window title of the IDE. </summary>
        /// <returns> The title of currently active project. </returns>
        /// <param name="text"> Window title of the IDE. </param>
        private static string GetProjectTitle(this string text)
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(" - GameMaker Studio 2", StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return String.Empty;
        }

        /// <summary> Build an updated Rich Presence for currently running GMS2 executables. </summary>
        /// <remarks> Detects the number of IDE instances, also if there are multiple projects open across multiple IDE instances.
        /// In that case, the status display will be occupied with name of these projects, instead of detailed IDE status.
        /// <para> The information about status of each IDE instance is based on its window title. </para></remarks>
        /// <param name="timestamp_start"> Timestamp displayed in the Rich Presence. </param>
        /// <returns> Rich Presence data. </returns>
        private static RichPresence PresenceUpdate_GMS2(Timestamps timestamp_start)
        {
            string windowTitle;

            int openProjects = 0;
            int lastProject = 0;
            int singleProjectID = 0;

            string projectNames = String.Empty;
            string details = String.Empty;
            string state = String.Empty;

            //|Get the list of IDE processes.
            Process[] processList = {};

            for (int i = 0; i < applicationList_IDE.Length; i++)
            {
                processList = processList.Concat(Process.GetProcessesByName(applicationList_IDE[i])).ToArray();
            }

            int instanceNumber_IDE = processList.Length;

            //|In case there's more than one IDE that has a project open, get all names of the projects.
            if (instanceNumber_IDE > 1)
            {
                for (int i = 0; i < instanceNumber_IDE; i++)
                {
                    windowTitle = processList[i].MainWindowTitle;
                    
                    string projectTitle = windowTitle.GetProjectTitle();

                    //|Check if IDE is useable and has projects open.
                    if ((windowTitle != String.Empty) && (!windowTitle.Contains(windowTitle_startPage)))
                    {
                        //|Check if there are multiple instances that run the same project, so the duplicates can be ignored.
                        bool projectDuplicate = false;

                        for (int j = 0; j < i; j++)
                        {
                            if (processList[j].MainWindowTitle.GetProjectTitle() == projectTitle)
                            {
                                projectDuplicate = true;
                                break;
                            }
                        }

                        if (!projectDuplicate)
                        {
                            string separator = ", ";

                            if (projectNames == String.Empty)
                            {
                                separator = String.Empty;
                            }

                            projectNames += separator + "\"" + projectTitle + "\"";

                            //|Count the number or projects open and mark the last IDE with open project,
                            // so it will be focused on in case there's only one of them.
                            lastProject = i;
                            openProjects++;
                        }
                    }
                }
            }

            //|If there are multiple, create a string for them and set it to Rich Presence.
            // Otherwise prepare the code for handling the single project with more detail.
            if (openProjects > 1)
            {
                details = UserText.RichPresence.details_projectOpen + UserText.RichPresence.details_multipleProjectsOpen;
                state = projectNames;
            }
            else
            {
                singleProjectID = lastProject;
            }

            //|If there's only one IDE that runs a project, create a detailed status/details string for it and set it to Rich Presence.
            if ((instanceNumber_IDE == 1) || (openProjects == 1))
            {
                windowTitle = processList[singleProjectID].MainWindowTitle;

                if (windowTitle != String.Empty)
                {
                    if (windowTitle.Contains(windowTitle_startPage))
                    {
                        details = UserText.RichPresence.details_startPage;
                    }
                    else
                    {
                        details = UserText.RichPresence.details_projectOpen + ": \"" + windowTitle.GetProjectTitle() + "\"";

                        state = (ApplicationInstance.GetNumber(application_runner) > 0) ? UserText.RichPresence.status_game : UserText.RichPresence.status_ide;
                    }
                }
            }

            //|Form the Rich Presence data with the prepared information.
            RichPresence presence_GMS2 = new RichPresence()
            {
                Details = details,
                State = state,
                Timestamps = timestamp_start,
                Assets = RPC_icon
            };

            return presence_GMS2;
        }

        /// <summary> Loop that appropriately handles Rich Presence creation, update and disposal. </summary>
        private static void SetPresence_GMS2()
        {
            DiscordRpcClient RPC_client = new DiscordRpcClient(clientID_GMS2);

            RPC_client.OnReady += (sender, message) =>
            {
                Console.WriteLine("Connected to Discord as user: " + message.User.Username);
            };

            RPC_client.OnPresenceUpdate += (sender, message) =>
            {
                Console.WriteLine("Rich Presence has been updated.");
            };

            RPC_client.Initialize();

            Timestamps timestamp_start = Timestamps.Now;

            while (true)
            {
                if (ApplicationInstance.GetNumber(applicationList_IDE) > 0)
                {
                    RPC_client.SetPresence(PresenceUpdate_GMS2(timestamp_start));
                }
                else
                {
                    //|If the IDE is not running, the loop will break, triggering clean-up and then it will
                    // return to the application's main IDE-checking loop.
                    break;
                }

                Thread.Sleep(applicationCheckDelay);
            }

            //|Clean-up after IDE is no longer running.
            Console.WriteLine("Rich Presence no longer applicable. Disposing of it...");
            Thread.Sleep(applicationExitDelay);
            RPC_client.Dispose();
            application_main_uptime.Stop();
            Console.WriteLine("Disposed of the Rich Presence.");
        }
    }
}
