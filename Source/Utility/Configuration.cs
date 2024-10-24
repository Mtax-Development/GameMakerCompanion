#pragma warning disable CA1507, CS8601, CS8602, CS8618, IDE0047
using System;
using System.IO;
using System.Text.Json;
using GameMakerCompanion.Interface.WindowType;

namespace GameMakerCompanion.Utility
{
    /// <summary> JSON-based user preferences manager. </summary>
    [Serializable]
    public class Configuration
    {
        /// <summary> Options used when saving the configuration to a JSON file. </summary>
        static readonly JsonSerializerOptions SerializerOptions = new() {WriteIndented = true};
        
        /// <summary> Customization of primary features. </summary>
        public ApplicationConfiguration Application {get; set;}
        
        /// <summary> Customization of Rich Presence communication. </summary>
        public RichPresenceConfiguration RichPresence {get; set;}
        
        /// <summary> Customization of external application detection. </summary>
        public TrackingConfiguration Tracking {get; set;}
        
        /// <summary> Customization of external application launching. </summary>
        public LauncherConfiguration Launcher {get; set;}
        
        public record ApplicationConfiguration
        {
            /// <summary> Whether the application should attempt to start on user login. </summary>
            public bool StartOnBoot {get; set;}
            
            /// <summary> Whether the interface icon should appear in system Tray. </summary>
            public bool HideTrayIcon {get; set;}
            
            /// <summary> Time in milliseconds between clicks to activate actions requiring two mouse clicks. </summary>
            /// <remarks> Not applicable on macOS. </remarks>
            public int DoubleClickTimeWindow {get; set;}
            
            /// <summary> Time in milliseconds before a notification will be shown after displaying previous one. </summary>
            public int RepeatedNotificationCooldown {get; set;}
        }
        
        public record RichPresenceConfiguration
        {
            /// <summary> Whether Discord Rich Presence should be operated. </summary>
            public bool EnableForDiscord {get; set;}
            
            /// <summary> Whether GameMaker project titles should be public in Rich Presence. </summary>
            public bool IncludeProjectTitles {get; set;}
        }
        
        public record TrackingConfiguration
        {
            /// <summary> Time in milliseconds between each checks of target applications. </summary>
            public int CheckDelay {get; set;}
            
            /// <summary> Time in milliseconds after which Rich Presence is removed when no longer applicable. </summary>
            /// <remarks> Extends Rich Presence to prevent non-Rich Presence display. </remarks>
            public int ExitDelay {get; set;}
        }

        public record LauncherConfiguration
        {
            /// <summary> Path or a launch protocol used to launch an external application. </summary>
            public string? Path {get; set;}
        }

        /// <summary> Create or overwrite the configuration file in the same directory as this application. </summary>
        public void SaveToFile()
        {
            try
            {
                File.WriteAllText(Path.File.Configuration,
                                  (JsonSerializer.Serialize(this, SerializerOptions)
                                                 .Replace(" {", (Environment.NewLine + "  {"))
                                                 .Replace("},", ("}," + Environment.NewLine)) +
                                   Environment.NewLine));
                
                Console.WriteLine(UserText.Information.ConfigurationSaved);
            }
            catch (Exception exception)
            {
                GameMakerCompanion.Application.LogException(exception, new Prompt(UserText.Error.ConfigurationSavingFailure));
            }
        }
        
        /// <summary> Create initial configuration with prepared values and save it to file. </summary>
        public void CreateDefault()
        {
            Application = new()
            {
                StartOnBoot = false,
                HideTrayIcon = false,
                DoubleClickTimeWindow = 900,
                RepeatedNotificationCooldown = 5000
            };
            
            RichPresence = new()
            {
                EnableForDiscord = true,
                IncludeProjectTitles = true
            };
            
            Tracking = new()
            {
                CheckDelay = 5000,
                ExitDelay = 7000
            };
            
            Launcher = new()
            {
                Path = null
            };
            
            SaveToFile();
        }
        
        public Configuration()
        {
            if (File.Exists(Path.File.Configuration))
            {
                try
                {
                    JsonElement JSON = JsonDocument.Parse(File.ReadAllText(Path.File.Configuration)).RootElement;
                    Application = JSON.GetProperty("Application").Deserialize<ApplicationConfiguration>();
                    RichPresence = JSON.GetProperty("RichPresence").Deserialize<RichPresenceConfiguration>();
                    Tracking = JSON.GetProperty("Tracking").Deserialize<TrackingConfiguration>();
                    Launcher = JSON.GetProperty("Launcher").Deserialize<LauncherConfiguration>();
                    
                    Application.StartOnBoot = (Application.StartOnBoot && GameMakerCompanion.Application.OperatingSystem
                                                                                            .ValidateAutomaticStartup());
                }
                catch
                {
                    Console.WriteLine(UserText.Error.Launch.MalformedConfiguration);
                    
                    CreateDefault();
                }
            }
            else
            {
                CreateDefault();
            }
        }
    }
}
