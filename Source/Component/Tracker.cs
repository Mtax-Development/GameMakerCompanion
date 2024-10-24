#pragma warning disable IDE0047
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using DiscordRPC;
using DiscordRPC.Message;
using GameMakerCompanion.Utility;

namespace GameMakerCompanion.Component
{
    /// <summary> External application tracking component and Rich Presence operator. </summary>
    /// <remarks> Handles multiple application types at once. </remarks>
    internal static class Tracker
    {
        internal readonly struct TrackedApplication
        {
            /// <summary> Total uptime of the primary executables during the runtime of this application. </summary>
            internal static readonly Stopwatch Uptime = new();
            
            /// <summary> Application ID on Discord Developer Portal. </summary>
            /// <remarks> Used to display the application name and its assets. </remarks>
            internal static readonly string[] ID = ["634416362898325534", "990567886772903986"];
            
            /// <summary> Identified titles within tracked application types. </summary>
            /// <remarks> Used to trace their current state. </remarks>
            internal readonly struct Title
            {
                internal static readonly string[][] PrimaryProcess =
                [
                    [
                        "GameMaker",
                        "GameMaker-LTS",
                        "GameMaker-Beta",
                        "GameMaker Studio 2",
                        "GameMaker Studio 2-Beta",
                        "GameMakerStudio",
                        "GameMakerStudio.exe",
                        "GameMaker.exe"
                    ],
                    [
                        "GameMaker-Studio"
                    ]
                ];
                
                internal static readonly string SecondaryProcess = "Runner";
                
                internal readonly struct Window
                {
                    internal static readonly string[] StartPage = ["Start Page - GameMaker", "<new game>  -"];
                }
            }
        }
        
        /// <summary> Assets displayed solely on Rich Presence end. </summary>
        internal readonly struct RichPresenceAsset
        {
            internal static readonly Assets[] Icon =
            [
                new Assets()
                {
                    LargeImageKey = Path.Remote.DiscordDeveloperPortal.RichPresenceImage[0],
                    LargeImageText = UserText.RichPresence.ImageDescription[0]
                },
                
                new Assets()
                {
                    LargeImageKey = Path.Remote.DiscordDeveloperPortal.RichPresenceImage[1],
                    LargeImageText = UserText.RichPresence.ImageDescription[1]
                }
            ];
        }
        
        /// <summary> Time in milliseconds between checks for instances of the primary executable. </summary>
        private static readonly int ApplicationCheckDelay = Math.Max(200, Application.Configuration.Tracking.CheckDelay);
        
        /// <summary> Time in millisecond after which Rich Presence is removed when the application is no longer detected. </summary>
        /// <remarks> Extends Rich Presence to prevent non-Rich Presence display. </remarks>
        private static readonly int ApplicationExitDelay = Math.Max(200, Application.Configuration.Tracking.ExitDelay);
        
        /// <summary> Entry function for this class. </summary>
        /// <remarks> Handles preparations and launches an endless loop that checks for appropriate applications and operates Rich Presence. </remarks>
        internal static void Start()
        {
            Console.WriteLine(UserText.Information.TrackerStart);
            
            //|External application checking loop. 
            // Continuously checks for whether the target application is running. If it is, a different loop will be applied inside of it.
            // If the target application stops executing, performs a cleanup and returns to this loop.
            while (true) 
            {
                if (Instance.IsRunning(TrackedApplication.Title.PrimaryProcess))
                {
                    OperateDiscordRichPresence();
                }
                
                Thread.Sleep(ApplicationCheckDelay);
            }
        }
        
        /// <summary> Cut the project name from the obtained window title of the IDE. </summary>
        /// <returns> The title of currently active project. </returns>
        /// <param name="text"> Window title to extract the title from. </param>
        private static string GetProjectTitle(this string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                int projectTitleHeaderPosition = Math.Max(text.LastIndexOf(" - GameMaker", StringComparison.Ordinal),
                                                          text.LastIndexOf(".project.gmx  -", StringComparison.Ordinal));
                
                if (projectTitleHeaderPosition > 0)
                {
                    return text.Substring(0, projectTitleHeaderPosition);
                }
            }
            
            return string.Empty;
        }
        
        /// <summary> Check whether the title of the window signifies the IDE being on its start page. </summary>
        /// <param name="text"> The title to check. </param>
        /// <returns> Whether the title contains the start page title. </returns>
        private static bool IsStartPageTitle(this string text)
        {
            foreach (string windowTitle in TrackedApplication.Title.Window.StartPage)
            {
                if (text.Contains(windowTitle))
                {
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary> Loop handling Rich Presence creation, updates and removal. </summary>
        private static void OperateDiscordRichPresence()
        {
            /// <summary> Construct and prepare Rich Presence client, ready to perform API connections. </summary>
            /// <remarks> The client must be disposed after it is no longer being used. </remarks>
            /// <param name="applicationID"> Application ID on Discord Developer Portal of application to operate. </param>
            /// <returns> Discord Rich Presence client. </returns>
            static DiscordRpcClient CreateRichPresenceClient(string applicationID)
            {
                DiscordRpcClient client = new(applicationID);
                
                client.OnReady += delegate (object sender, ReadyMessage message)
                {
                    Console.WriteLine((UserText.Information.TrackerConnected + message.User.Username));
                };
                
                client.OnPresenceUpdate += delegate
                {
                    Console.WriteLine(UserText.Information.TrackerUpdate);
                };
                
                client.Initialize();
                
                return client;
            }
            
            TrackedApplication.Uptime.Start();
            
            List<DiscordRpcClient?> richPresenceClient = [null, null];
            Timestamps timeSinceStart = Timestamps.Now;
            int dominatingPresence = 0;
            int dominatingPresencePrevious = -1;
            
            while (true)
            {
                if (Instance.IsRunning(TrackedApplication.Title.PrimaryProcess))
                {
                    RichPresence richPresence = CreateDiscordRichPresence(timeSinceStart);
                    dominatingPresence = Array.IndexOf(RichPresenceAsset.Icon, richPresence.Assets);
                    
                    if (dominatingPresence != dominatingPresencePrevious)
                    {
                        if (dominatingPresencePrevious >= 0)
                        {
                            richPresenceClient[dominatingPresencePrevious]?.Deinitialize();
                        }
                        
                        richPresenceClient[dominatingPresence] = CreateRichPresenceClient(TrackedApplication.ID[dominatingPresence]);
                    }
                    
                    richPresenceClient[dominatingPresence]?.SetPresence(richPresence);
                    
                    dominatingPresencePrevious = dominatingPresence;
                }
                else
                {
                    break;
                }
                
                Thread.Sleep(ApplicationCheckDelay);
            }
            
            Console.WriteLine(UserText.Information.TrackerRemovalPending);
            
            richPresenceClient.ForEach(delegate (DiscordRpcClient? client)
            {
                client?.Dispose();
            });
            
            Thread.Sleep(ApplicationExitDelay);
            TrackedApplication.Uptime.Stop();
            
            Console.WriteLine(UserText.Information.TrackerRemovalComplete);
        }
        
        /// <summary> Obtain information from process list to return Rich Presence data appropriate to the current state of target application. </summary>
        /// <param name="timeSinceStart"> Starting timestamp of the target application to include in Rich Presence. </param>
        /// <returns> Rich Presence data. </returns>
        private static RichPresence CreateDiscordRichPresence(Timestamps timeSinceStart)
        {
            /// <summary> Iterate through specified processes to return extracted process titles for each of application types. </summary>
            /// <param name="processList"> Nested lists with arrays separated for each application type. </param>
            /// <returns> List containing subsidiary lists with results, separated for each application type. </returns>
            static List<List<string>> GetProjectTitleList(List<Process[]> processList)
            {
                List<List<string>> list = [];
                int i = 0;
                foreach (Process[] subsidiaryProcessList in processList)
                {
                    List<string> subsidiaryTitleList = [];
                    foreach (Process process in subsidiaryProcessList)
                    {
                        string windowTitle = Application.OperatingSystem.GetWindowTitle(process);
                        
                        if (!windowTitle.IsStartPageTitle())
                        {
                            string projectTitle = windowTitle.GetProjectTitle();
                            
                            if (projectTitle.Length > 0)
                            {
                                subsidiaryTitleList.Add(projectTitle);
                            }
                        }
                    }
                    
                    list.Add(subsidiaryTitleList);
                    
                    ++i;
                }
                
                return list;
            }
            
            /// <summary> Check if any of the specified processes among all types is currently on its starting page. </summary>
            /// <param name="processList"> Nested lists with arrays separated for each application type. </param>
            /// <returns> Whether starting page exists in any of target processes. </returns>
            static bool StartPageExists(List<Process[]> processList)
            {
                int i = 0;
                foreach (Process[] subsidiaryProcessList in processList)
                {
                    foreach (Process process in subsidiaryProcessList)
                    {
                        if (Application.OperatingSystem.GetWindowTitle(process).Contains(TrackedApplication.Title.Window.StartPage[i]))
                        {
                            return true;
                        }
                    }
                    
                    ++i;
                }
                
                return false;
            }
            
            string details = string.Empty;
            string state = string.Empty;
            List<Process[]> processList = [];
            int dominatingPresence = 0;
            
            int[] processCount = new int[TrackedApplication.Title.PrimaryProcess.Length];
            int processSum = 0;
            int i = 0;
            foreach (string[] processTitleList in TrackedApplication.Title.PrimaryProcess)
            {
                Process[] instance = Instance.GetProcessesByName(processTitleList);
                processList.Add(instance);
                processCount[i] = instance.Length;
                processSum += instance.Length;
                
                ++i;
            }
            
            if (processSum > 0)
            {
                List<List<string>> projectTitleList = GetProjectTitleList(processList);
                HashSet<string> allProjectTitles = [];
                
                foreach (List<string> element in projectTitleList)
                {
                    allProjectTitles.UnionWith(element);
                }
                
                switch (allProjectTitles.Count)
                {
                    case 0:
                        if (StartPageExists(processList))
                        {
                            details = UserText.RichPresence.Details.StartPage;
                        }
                        
                        dominatingPresence = Convert.ToInt32((!(processList[0].Length >= processList[1].Length)));
                    break;
                    
                    case 1:
                        details = (UserText.RichPresence.Details.ProjectOpen +
                                   (((Application.Configuration.RichPresence.IncludeProjectTitles) ? (": \"" + allProjectTitles.First() + "\"")
                                                                                                   : UserText.RichPresence.Details.ProjectTitleNotShown)));
                        state = ((Instance.GetNumber(TrackedApplication.Title.SecondaryProcess) > 0) ? UserText.RichPresence.Status.SecondaryProcess
                                                                                                     : UserText.RichPresence.Status.PrimaryProcess);
                        dominatingPresence = Convert.ToInt32((!(projectTitleList[0].Count >= projectTitleList[1].Count)));
                    break;
                    
                    default:
                        if (Application.Configuration.RichPresence.IncludeProjectTitles)
                        {
                            string projectListing = string.Empty;
                            
                            foreach (string projectTitle in allProjectTitles)
                            {
                                projectListing += (((projectListing == string.Empty) ? string.Empty : ", ") + "\"" + projectTitle + "\"");
                            }
                            
                            details = (UserText.RichPresence.Details.ProjectOpen + UserText.RichPresence.Details.MultipleProjectsOpen + ":");
                            state = projectListing;
                        }
                        else
                        {
                            details = (UserText.RichPresence.Details.ProjectOpen + " " + allProjectTitles.Count.ToString() +
                                       UserText.RichPresence.Details.MultipleProjectsOpen);
                            state = ((Instance.GetNumber(TrackedApplication.Title.SecondaryProcess) > 0) ? UserText.RichPresence.Status.SecondaryProcess
                                                                                                         : UserText.RichPresence.Status.PrimaryProcess);
                        }
                        
                        dominatingPresence = Convert.ToInt32((!(projectTitleList[0].Count >= projectTitleList[1].Count)));
                    break;
                }
            }
            
            return new RichPresence()
            {
                Details = details,
                State = state,
                Timestamps = timeSinceStart,
                Assets = RichPresenceAsset.Icon[dominatingPresence]
            };
        }
    }
}
