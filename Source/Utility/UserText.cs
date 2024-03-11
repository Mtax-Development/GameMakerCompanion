#pragma warning disable IDE0047
using System;
using System.Runtime.InteropServices;

namespace GameMakerCompanion.Utility
{
    /// <summary> Container listing strings that can be visible to the user. </summary>
    internal static class UserText
    {
        internal static readonly string ApplicationName = "GameMaker Companion";
        
        /// <summary> Logging messages displayed upon application entry and exit. </summary>
        /// <remarks> This text struct is initialized earliest and it cannot refer to other classes. </remarks>
        internal readonly struct Boot
        {
            internal static readonly string Start = " start.";
            internal static readonly string End = " end.";
            internal static readonly string Platform = "Platform: ";
        }
        
        /// <summary> Logging messages displayed upon successful operation of primary functionalities. </summary>
        internal readonly struct Information
        {
            internal static readonly string TrayIconCreation = "Creating a Tray icon.";
            internal static readonly string TrayIconRemoval = "Removing the Tray icon.";
            internal static readonly string TrackerStart = "Rich Presence tracker start.";
            internal static readonly string TrackerConnected = "Connected to Discord as user: ";
            internal static readonly string TrackerUpdate = "Discord Rich Presence has been updated.";
            internal static readonly string TrackerRemovalPending = "Discord Rich Presence no longer applicable. Removing it...";
            internal static readonly string TrackerRemovalComplete = "Discord Rich Presence removed.";
            internal static readonly string ConfigurationSaved = ("Configuration saved at: " + Path.File.Configuration);
            internal static readonly string AutostartEntrySaved = ("Autostart entry saved at" +
                                                                   ((Application.OperatingSystem.Platform == OSPlatform.Windows)
                                                                    ? " registry key" : string.Empty) + ": " + Path.OperatingSystem.AutoStart);
            internal static readonly string AutostartEntryOverwritten = ("Autostart entry overwritten at" +
                                                                         ((Application.OperatingSystem.Platform == OSPlatform.Windows)
                                                                         ? " registry key" : string.Empty) + ": " + Path.OperatingSystem.AutoStart);
            internal static readonly string AutostartEntryRemoved = ("Autostart entry removed from" +
                                                                     ((Application.OperatingSystem.Platform == OSPlatform.Windows)
                                                                     ? " registry at" : string.Empty) + ": " + Path.OperatingSystem.AutoStart);
        }
        
        /// <summary> Text displayed by interface windows. </summary>
        internal readonly struct Window
        {
            internal readonly struct About
            {
                internal static readonly string Title = ("About " + Application.Name);
                
                internal readonly struct Label
                {
                    internal static readonly string ApplicationDescription =
                        ("Companion application and Discord Rich Presence module designed for GameMaker series of software." + " " +
                         "Created and maintained by Mtax." + Environment.NewLine + Environment.NewLine +
                         "GameMaker series of software, their logos and online resources are properties of YoYo Games." + Environment.NewLine +
                         "Discord is a property of Discord Inc." + Environment.NewLine +
                         "This is a free and open-source third-party project, not affiliated with either of them.");
                }
                
                internal readonly struct Button
                {
                    internal static readonly string OpenApplicationDirectory = "Open Application Directory";
                    internal static readonly string OpenApplicationRepository = "Open Application Repository";
                }
            }
            
            internal readonly struct Prompt
            {
                internal static readonly string DefaultButtonText = "Close";
                
                internal readonly struct RemoveTrayIcon
                {
                    internal static readonly string[] Explanation = (["Icon removal is intended for use along autostart, which currently {0} set up. " +
                                                                      "It can be restored by changing the " + Path.FileName.Configuration + " file in " +
                                                                      "application folder. Continue?", "is not", "is"]);
                    internal static readonly string RestartRequired= ("The change will take effect the next time " + Application.Name + " starts.");
                    internal static readonly string ButtonText = "Proceed";
                }
            }
        }
        
        /// <summary> Text displayed during handling of predicted errors. </summary>
        internal readonly struct Error
        {
            internal static readonly string Header = "ERROR: ";
            internal static readonly string ConfigurationSavingFailure = ("Failed writing to the \"" + Path.FileName.Configuration + "\" file. " +
                                                                          Instruction.EnsureFileAccess);
            
            private readonly struct Instruction
            {
                internal static readonly string EnsureFileAccess = ("Please ensure " + Application.Name + " has file access permissions.");
            }
            
            internal readonly struct Launch
            {
                internal static readonly string Exit = "Exit";
                internal static readonly string DuplicateInstance = (Application.Name + " is already running." + Environment.NewLine +
                                                                     "Close other instances before launching a new one.");
                internal static readonly string MalformedConfiguration = ("Incorrect \"" + Path.FileName.Configuration + "\" formatting. Replacing it " +
                                                                          "with default configuration.");
                
                internal readonly struct Dependency
                {
                    internal static readonly string UnexpectedError = ("Unexpected error during dependency check." + Environment.NewLine +
                                                                       Application.Name + " is unable to start.");
                    internal static readonly string? Initialization = ((Application.OperatingSystem.Platform == OSPlatform.OSX)
                                                                       ? (Application.Name + " requires access to Screen Recording permissions in " +
                                                                          "order to function. Please provide them in System Preferences and try again.")
                                                                       : ((Application.OperatingSystem.Platform == OSPlatform.Linux)
                                                                         ? ("Dependency \"xdotool\" was not found on the system. Please install it " +
                                                                            "before proceeding.") : null)) ;
                    internal static readonly string? Ongoing = ((Application.OperatingSystem.Platform == OSPlatform.OSX)
                                                                ? "Critical error during when attempting to read title of a window."
                                                                : ((Application.OperatingSystem.Platform == OSPlatform.Linux)
                                                                  ? "Dependency \"xdotool\" stopped working." : null));
                }
            }
            
            internal readonly struct Autostart
            {
                internal static readonly string? SetupFailure = ((Application.OperatingSystem.Platform == OSPlatform.Windows) ?
                                                                 "Failed to manage automatic startup registry key." : null);
                internal static readonly string? RemovalFailure = ((Application.OperatingSystem.Platform == OSPlatform.Linux) ?
                                                                   ("Failed to delete an existing autostart file. " +
                                                                    Instruction.EnsureFileAccess) : null);
                internal static readonly string? OverwriteFailure = ((Application.OperatingSystem.Platform == OSPlatform.Linux) ?
                                                                     ("Failed to delete an existing autostart file while attempting to replace it. " +
                                                                      Instruction.EnsureFileAccess) : null);
                internal static readonly string ValidationFailure = "Failed to read the autostart entry.";
            }
            
            internal readonly struct URLOpeningFailure
            {
                internal static readonly string Explanation = "Failed to open the URL in default browser.";
                internal static readonly string ButtonText = "Copy URL to Clipboard";
            }
        }
        
        /// <summary> Text displayed during interaction with the Tray icon. </summary>
        internal readonly struct TrayIcon
        {
            internal readonly struct Menu
            {
                internal static readonly string About = "About";
                internal static readonly string Exit = "Exit";
                
                internal readonly struct RichPresence
                {
                    internal static readonly string Title = "Rich Presence";
                    internal static readonly string EnableForDiscord = " Enable for Discord";
                    internal static readonly string IncludeProjectTitles = " Include project titles";
                }
                
                internal readonly struct GameMakerWeb
                {
                    internal static readonly string Title = "GameMaker Web";
                    internal static readonly string Homepage = "Homepage";
                    internal static readonly string Manual = "Manual";
                    internal static readonly string ReleaseNotes = "Release Notes";
                    internal static readonly string Forum = "Forum";
                    internal static readonly string Marketplace = "Marketplace";
                    internal static readonly string UserAccount = "User Account";
                    internal static readonly string HelpCenter = "Help Center";
                    internal static readonly string Repositories = "Repositories";
                    
                    internal readonly struct Branch
                    {
                        internal static readonly string Stable = "Stable";
                        internal static readonly string LTS = "LTS";
                        internal static readonly string Beta = "Beta";
                    }
                }
                
                internal readonly struct Configuration
                {
                    internal static readonly string Title = "Configuration";
                    internal static readonly string StartOnBoot = " Start on boot";
                    internal static readonly string RemoveTrayIcon = " Remove Tray icon";
                }
            }
            
            internal readonly struct Notification
            {
                internal static readonly string TrackedUptime = "GameMaker uptime: ";
            }
        }
        
        /// <summary> Text displayed solely on the Discord end. </summary>
        internal readonly struct RichPresence
        {
            internal static readonly string[] ImageDescription = ["GameMaker", "GameMaker: Studio"];
            
            internal readonly struct Details
            {
                internal static readonly string StartPage = "On the Start Page";
                internal static readonly string ProjectOpen = "Working on";
                internal static readonly string ProjectTitleNotShown = " a project";
                internal static readonly string MultipleProjectsOpen = " projects";
            }
            
            internal readonly struct Status
            {
                internal static readonly string PrimaryProcess = "In main workspace";
                internal static readonly string SecondaryProcess = "Running the application";
            }
        }
    }
}
