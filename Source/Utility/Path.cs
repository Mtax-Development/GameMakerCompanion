#pragma warning disable CS8604, IDE0047
using System;
using System.Runtime.InteropServices;

namespace GameMakerCompanion.Utility
{
    /// <summary> Container listing all used file, directory and remote locations. </summary>
    /// <remarks> Nullable entries are subject to platform differences. </remarks>
    internal static class Path
    {
        internal readonly struct OperatingSystem()
        {
            internal readonly static string? AutoStart =
                ((Application.OperatingSystem.Platform == OSPlatform.Windows) ? @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run" :
                 (Application.OperatingSystem.Platform == OSPlatform.OSX) ? (@"/Users/" + Environment.UserName + @"/Library/LaunchAgents/") :
                 (Application.OperatingSystem.Platform == OSPlatform.Linux) ? @"/home/user/.config/autostart/" : null);
        }
        
        internal readonly struct Folder()
        {
            internal readonly static string ApplicationRoot = AppDomain.CurrentDomain.BaseDirectory;
            internal readonly static string? ApplicationBundle = ((Application.OperatingSystem.Platform == OSPlatform.OSX)
                                                                  ? AppDomain.CurrentDomain.BaseDirectory
                                                                             .Replace((SubFolder.ApplicationBundleBinary), @".app")
                                                                  : null);
        }
        
        internal readonly struct SubFolder()
        {
            internal readonly static string ApplicationAssets = @"/Asset/";
            internal readonly static string? ApplicationBundleBinary = ((Application.OperatingSystem.Platform == OSPlatform.OSX) ? @".app/Contents/MacOS/"
                                                                                                                                 : null);
        }
        
        internal readonly struct File()
        {
            internal readonly static string Executable = (Folder.ApplicationRoot + Application.Name +
                                                          ((Application.OperatingSystem.Platform == OSPlatform.Windows) ? ".exe" : string.Empty));
            internal readonly static string Configuration = (Folder.ApplicationRoot + FileName.Configuration);
            internal readonly static string? AutoStart = (((Application.OperatingSystem.Platform == OSPlatform.OSX) ||
                                                           (Application.OperatingSystem.Platform == OSPlatform.Linux))
                                                          ? (OperatingSystem.AutoStart + FileName.AutoStart) : null);
            internal static Uri Icon {get => new Uri(@"avares://" + Application.Name + SubFolder.ApplicationAssets + FileName.Icon);}
            internal static Uri Logo {get => new Uri(@"avares://" + Application.Name + SubFolder.ApplicationAssets + FileName.Logo);}
        }
        
        internal readonly struct FileName()
        {
            internal readonly static string Configuration = @"configuration.json";
            internal readonly static string? AutoStart = ((Application.OperatingSystem.Platform == OSPlatform.OSX) ? (Application.Name + @".plist") :
                                                          (Application.OperatingSystem.Platform == OSPlatform.Linux) ? (Application.Name + @".desktop")
                                                                                                                     : null);
            internal readonly static string Icon = (Application.Name + @".ico");
            internal readonly static string Logo = (Application.Name + @" Logo.png");
        }
        
        internal readonly struct Remote()
        {
            internal static readonly string ProjectRepository = @"https://github.com/Mtax-Development/GameMakerCompanion";
            internal static readonly string InitializationInstructions = @"https://github.com/Mtax-Development/GameMakerCompanion#Initialization";
            
            internal readonly struct GameMakerWeb()
            {
                internal static readonly string Homepage = @"https://gamemaker.io";
                internal static readonly string ReleaseNotes = @"https://releases.gamemaker.io";
                internal static readonly string Forum = @"https://forum.gamemaker.io";
                internal static readonly string Marketplace = @"https://marketplace.gamemaker.io";
                internal static readonly string UserAccount = @"https://id.gamemaker.io";
                internal static readonly string HelpCenter = @"https://help.gamemaker.io";
                internal static readonly string Repositories = @"https://github.com/YoYoGames";  
                
                internal readonly struct Manual()
                {
                    internal static readonly string Stable = @"https://manual.gamemaker.io/monthly";
                    internal static readonly string LTS = @"https://manual.gamemaker.io/lts";
                    internal static readonly string Beta = @"https://manual.gamemaker.io/beta";
                }
            }
            
            internal readonly struct DiscordDeveloperPortal()
            {
                internal static readonly string[] RichPresenceImage = [@"gamemaker_logo", @"gamemakerstudio_logo"];
            }
        }
    }
}
