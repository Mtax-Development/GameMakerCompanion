#pragma warning disable IDE0047, IDE0057
using System;
using System.Runtime.InteropServices;
using Avalonia.Interactivity;
using GameMakerCompanion.Interface.WindowType;
using GameMakerCompanion.Utility;

namespace GameMakerCompanion.Component
{
    /// <summary> External application launching component. </summary>
    /// <remarks> Handles launching the IDE through the Tray Menu. </remarks>
    internal static class Launcher
    {
        /// <summary> Operate configured launch protocol or begin its configuration if it has not been performed yet. </summary>
        internal static void OperateLaunch(object? sender = null, EventArgs? e = null)
        {
            if (Application.Configuration.Launcher.Path == null)
            {
                new Prompt(UserText.Window.Prompt.Launcher.Explanation.PromptText, UserText.Window.Prompt.Launcher.Explanation.ButtonText,
                           PromptPathSetting).Show();
            }
            else
            {
                Application.OperatingSystem.OpenApplication(Application.Configuration.Launcher.Path);
            }
        }
        
        /// <summary> Create a <see cref="Prompt"/> with user instructions for setting a launcher path and operating such procedure. </summary>
        internal static void PromptPathSetting(object? sender = null, EventArgs? e = null)
        {
            string promptText = string.Empty;
            string? buttonText = "Retry";
            EventHandler<RoutedEventArgs>? buttonEvent = PromptPathSetting;
            
            string[] path = Instance.GetPaths(Tracker.TrackedApplication.Title.PrimaryProcess);
            switch (path.Length)
            {
                case 0:
                    promptText = UserText.Window.Prompt.Launcher.PathSetting.NoInstanceDetected;
                break;
                case 1:
                    buttonText = null;
                    buttonEvent = null;
                    
                    Application.Configuration.Launcher.Path = (((Application.OperatingSystem.Platform == OSPlatform.OSX)
                                                               && (path[0].Contains(".app/")))
                                                               ? path[0].Substring(0, (path[0].LastIndexOf(".app/") + 4)) : path[0]);
                    Application.Configuration.SaveToFile();
                    
                    bool pathUsesSteam = PathUsesSteam(Application.Configuration.Launcher.Path);
                    bool forceSteamProtocol = false;
                    
                    if ((Application.OperatingSystem.Platform == OSPlatform.Windows) && (pathUsesSteam))
                    {
                        forceSteamProtocol = (GetFileDirectory(Application.Configuration.Launcher.Path) == @"gamemaker_studio");
                    }
                    
                    if ((pathUsesSteam) && (!forceSteamProtocol))
                    {
                        promptText = UserText.Window.Prompt.Launcher.PathSetting.SteamConversionPossible.PromptText;
                        buttonText = UserText.Window.Prompt.Launcher.PathSetting.SteamConversionPossible.ButtonText;
                        buttonEvent = ConvertPathToSteamProtocol;
                    }
                    else
                    {
                        if (forceSteamProtocol)
                        {
                            ConvertPathToSteamProtocol();
                        }
                        
                        promptText = UserText.Window.Prompt.Launcher.PathSetting.DirectPathSettingSuccess;
                    }
                break;
                default:
                    promptText = UserText.Window.Prompt.Launcher.PathSetting.MultipleInstancesDetected;
                break;
            }
            
            new Prompt(promptText, buttonText, buttonEvent).Show();
        }
        
        /// <summary> Return the last directory a file at specific path is in. </summary>
        /// <param name="path"> Path to a file. </param>
        /// <returns> Name of a directory or an empty string in case of an error. </returns>
        private static string GetFileDirectory(string? path)
        {
            string result = string.Empty;
            
            if (path != null)
            {
                string[] part = path.Split([@"/", @"\"], StringSplitOptions.RemoveEmptyEntries);
                
                if (part.Length >= 2)
                {
                    result = part[(part.Length - 2)];
                }
            }
            
            if (result == string.Empty)
            {
                Console.WriteLine(UserText.Error.Launcher.FileDirectoryParsingFailure);
            }
            
            return result;
        }
        
        /// <summary> Parse launcher path to check if the IDE is an application installed by Steam and if it is, overwrite launcher path
        ///           to a launch protocol causing Steam to operate its launch. </summary>
        /// <remarks> The conversion is performed according to a list of application identifiers managed by Steam for each edition
        ///           of the IDE that can be installed through it. </remarks>
        private static void ConvertPathToSteamProtocol(object? sender = null, EventArgs? e = null)
        {
            string? launchProtocolID = null;
            string directory = GetFileDirectory(Application.Configuration.Launcher.Path);
            switch (directory)
            {
                case @"gamemaker_studio": launchProtocolID = @"214850"; break;
                case @"GameMaker Studio 2": launchProtocolID = @"1670460"; break;
                case @"GameMaker Studio 2 Desktop": launchProtocolID = @"585410"; break;
                case @"GameMaker Studio 2 Web": launchProtocolID = @"585600"; break;
                case @"GameMaker Studio 2 UWP": launchProtocolID = @"585610"; break;
                case @"GameMaker Studio 2 Mobile": launchProtocolID = @"585620"; break;
                default:
                    Console.WriteLine(UserText.Error.Launcher.PathConversion.Steam.PathParsingFailure);
                break;
            }
            
            if (launchProtocolID != null)
            {
                Application.Configuration.Launcher.Path = (@"steam://rungameid/" + launchProtocolID);
                Application.Configuration.SaveToFile();
            }
            else
            {
                new Prompt(UserText.Error.Launcher.PathConversion.Steam.ApplicationIdentifierNotRecognized, UserText.Error.OpenRepositoryIssues,
                           delegate
                {
                    Application.OperatingSystem.OpenURL(Path.Remote.RepositoryIssues);
                }).Show();
            }
        }
        
        /// <summary> Check if the specified path refers to an application installed by Steam. </summary>
        /// <param name="path"> File or directory path. </param>
        /// <returns> Whether or not Steam application path was recognized. </returns>
        private static bool PathUsesSteam(string path)
        {
            return ((path.Contains(@"steamapps\common\")) || (path.Contains(@"steamapps/common/")));
        }
    }
}
