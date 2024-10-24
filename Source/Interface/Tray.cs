#pragma warning disable CS8600, CS8602, CS8604, IDE0001, IDE0002, IDE0047, IDE0220
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using GameMakerCompanion.Component;
using GameMakerCompanion.Interface.WindowType;
using GameMakerCompanion.Utility;

namespace GameMakerCompanion.Interface
{
    /// <summary> Primary interface operator focused on interactions through menus produced by an icon in system tray. </summary>
    public class Tray : Avalonia.Application
    {
        static readonly TimeSpan DoubleClickTimeWindow = new(0, 0, 0, 0, Math.Max(300, Application.Configuration.Application.DoubleClickTimeWindow));
        static readonly TimeSpan UptimeNotificationCooldown = new(0, 0, 0, 0, Math.Max(1, Application.Configuration.Application
                                                                                                     .RepeatedNotificationCooldown));
        static TimeOnly? PreviousClickTime = null;
        static TimeOnly PreviousUptimeNotificationTime = TimeOnly.FromDateTime(DateTime.Now);
        
        public override void Initialize()
        {
            Name = Application.Name;
            Styles.Add(new FluentTheme());
            RequestedThemeVariant = ThemeVariant.Dark;
            
            if (Instance.GetNumber(Name) <= 1)
            {
                if (!Application.Configuration.Application.HideTrayIcon)
                {
                    ConstructTrayIcon();
                    PreloadRender();
                }
                
                if (Application.OperatingSystem.SetupDependencies())
                {
                    new Thread(Tracker.Start).Start();
                }
            }
            else
            {
                Application.ReportLaunchFailure(UserText.Error.Launch.DuplicateInstance);
            }
        }
        
        /// <summary> Create a tray icon and configure all of its elements. </summary>
        public static void ConstructTrayIcon()
        {
            Console.WriteLine(UserText.Information.TrayIconCreation);
            
            /// <summary> Switch the state of the specified menu entry ON and OFF if it can be toggled. </summary>
            /// <param name="item"> Menu entry to change the header of. </param>
            /// <returns> New state of the specified menu entry. </returns>
            static bool Toggle(NativeMenuItem item)
            {
                bool state = false;
                
                if ((item != null) && (item.Header != null))
                {
                    char symbolDisabled = Application.OperatingSystem.GetStateSymbol(false);
                    char symbolEnabled = Application.OperatingSystem.GetStateSymbol(true);
                    char symbolFirst = item.Header[0];
                    
                    if (symbolFirst == symbolDisabled)
                    {
                        item.Header = item.Header.Replace(symbolDisabled, symbolEnabled);
                        
                        state = true;
                    }
                    else if (symbolFirst == symbolEnabled)
                    {
                        item.Header = item.Header.Replace(symbolEnabled, symbolDisabled);
                    }
                }
                
                return state;
            }

            /// <summary> Save the time of a click and return the state of current double click. </summary>
            /// <returns> Whether this click is a second click performed within the time window of a double click. </returns>
            static bool RegisterDoubleClick()
            {
                if ((Application.OperatingSystem.Platform == OSPlatform.Windows) || (Application.OperatingSystem.Platform == OSPlatform.OSX))
                {
                    return true;
                }
                
                TimeOnly clickTime = TimeOnly.FromDateTime(DateTime.Now);
                
                if (PreviousClickTime == null)
                {
                    PreviousClickTime = clickTime;
                }
                else
                {
                    TimeSpan timeBetweenClicks = (clickTime - (TimeOnly)PreviousClickTime);
                    PreviousClickTime = clickTime;
                    
                    if (timeBetweenClicks <= DoubleClickTimeWindow)
                    {
                        PreviousClickTime = null;
                        
                        return true;
                    }
                }
                
                return false;
            }
            
            /// <summary> Click event which creates a notification displaying the uptime saved by external application tracking component. </summary>
            /// <remarks> Displays total time the target application has been operating for during the runtime of this application. </remarks>
            static void Click_ShowUptimeNotification(object? sender, EventArgs e)
            {
                TimeOnly clickTime = TimeOnly.FromDateTime(DateTime.Now);
                
                if (((clickTime - PreviousUptimeNotificationTime) >= UptimeNotificationCooldown) && RegisterDoubleClick())
                {
                    TimeSpan uptime = Tracker.TrackedApplication.Uptime.Elapsed;
                    string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}", uptime.Hours, uptime.Minutes, uptime.Seconds);
                    
                    Application.OperatingSystem.ShowNotification((UserText.TrayIcon.Notification.TrackedUptime + elapsedTime));
                    PreviousUptimeNotificationTime = clickTime;
                }
            }
            
            Application.TrayIcon = new()
            {
                Icon = new WindowIcon(AssetLoader.Open(Path.File.Icon)),
                ToolTipText = Application.Name,
                Menu =
                [
                    CreateMenuEntry(UserText.TrayIcon.Menu.OpenGameMaker, Launcher.OperateLaunch),

                    CreateMenuEntry(UserText.TrayIcon.Menu.RichPresence.Title,
                    [
                        CreateMenuEntry((Application.OperatingSystem.GetStateSymbol(Application.Configuration.RichPresence.EnableForDiscord) +
                                         UserText.TrayIcon.Menu.RichPresence.EnableForDiscord), delegate (object? sender, EventArgs e)
                                         {
                                             Application.Configuration.RichPresence.EnableForDiscord = Toggle((NativeMenuItem)sender);
                                             Application.Configuration.SaveToFile();
                                         }),
                        
                        CreateMenuEntry((Application.OperatingSystem.GetStateSymbol(Application.Configuration.RichPresence.IncludeProjectTitles) +
                                         UserText.TrayIcon.Menu.RichPresence.IncludeProjectTitles), delegate (object? sender, EventArgs e)
                                         {
                                             Application.Configuration.RichPresence.IncludeProjectTitles = Toggle((NativeMenuItem)sender);
                                             Application.Configuration.SaveToFile();
                                         })
                    ]),
                    
                    CreateMenuEntry(UserText.TrayIcon.Menu.GameMakerWeb.Title,
                    [
                        CreateMenuEntry(UserText.TrayIcon.Menu.GameMakerWeb.Homepage, delegate
                        {
                            Application.OperatingSystem.OpenURL(Path.Remote.GameMakerWeb.Homepage);
                        }),
                        
                        CreateMenuEntry(UserText.TrayIcon.Menu.GameMakerWeb.Manual,
                        [
                            CreateMenuEntry(UserText.TrayIcon.Menu.GameMakerWeb.Branch.Stable, delegate
                            {
                                Application.OperatingSystem.OpenURL(Path.Remote.GameMakerWeb.Manual.Stable);
                            }),
                            
                            CreateMenuEntry(UserText.TrayIcon.Menu.GameMakerWeb.Branch.LTS, delegate
                            {
                                Application.OperatingSystem.OpenURL(Path.Remote.GameMakerWeb.Manual.LTS);
                            }),
                            
                            CreateMenuEntry(UserText.TrayIcon.Menu.GameMakerWeb.Branch.Beta, delegate
                            {
                                Application.OperatingSystem.OpenURL(Path.Remote.GameMakerWeb.Manual.Beta);
                            })
                        ]),
                        
                        CreateMenuEntry(UserText.TrayIcon.Menu.GameMakerWeb.ReleaseNotes, delegate
                        {
                            Application.OperatingSystem.OpenURL(Path.Remote.GameMakerWeb.ReleaseNotes);
                        }),
                        
                        CreateMenuEntry(UserText.TrayIcon.Menu.GameMakerWeb.Forum, delegate
                        {
                            Application.OperatingSystem.OpenURL(Path.Remote.GameMakerWeb.Forum);
                        }),
                        
                        CreateMenuEntry(UserText.TrayIcon.Menu.GameMakerWeb.Marketplace, delegate
                        {
                            Application.OperatingSystem.OpenURL(Path.Remote.GameMakerWeb.Marketplace);
                        }),
                        CreateMenuEntry(UserText.TrayIcon.Menu.GameMakerWeb.UserAccount, delegate
                        {
                            Application.OperatingSystem.OpenURL(Path.Remote.GameMakerWeb.UserAccount);
                        }),
                        
                        CreateMenuEntry(UserText.TrayIcon.Menu.GameMakerWeb.HelpCenter, delegate
                        {
                            Application.OperatingSystem.OpenURL(Path.Remote.GameMakerWeb.HelpCenter);
                        }),
                        
                        CreateMenuEntry(UserText.TrayIcon.Menu.GameMakerWeb.Repositories, delegate
                        {
                            Application.OperatingSystem.OpenURL(Path.Remote.GameMakerWeb.Repositories);
                        })
                    ]),
                    
                    CreateMenuEntry(UserText.TrayIcon.Menu.Configuration.Title,
                    [
                        CreateMenuEntry((Application.OperatingSystem.GetStateSymbol(Application.Configuration.Application.StartOnBoot) +
                                        UserText.TrayIcon.Menu.Configuration.StartOnBoot), delegate (object? sender, EventArgs e)
                                        {
                                            Application.Configuration.Application.StartOnBoot = Toggle((NativeMenuItem)sender);
                                            Application.OperatingSystem.SetAutomaticStartup(Application.Configuration.Application.StartOnBoot);
                                            Application.Configuration.SaveToFile();
                                        }),
                        
                        CreateMenuEntry(UserText.TrayIcon.Menu.Configuration.RemoveTrayIcon, delegate (object? sender, EventArgs e)
                        {
                            new Prompt(string.Format(UserText.Window.Prompt.RemoveTrayIcon.Explanation[0],
                                                     UserText.Window.Prompt.RemoveTrayIcon
                                                             .Explanation[(1 + Convert.ToInt32(Application.Configuration.Application.StartOnBoot))]),
                                       UserText.Window.Prompt.RemoveTrayIcon.ButtonText, delegate
                                       {
                                           Console.WriteLine(UserText.Information.TrayIconRemoval);
                                           
                                           if (Application.TrayIcon != null)
                                           {
                                               Application.Configuration.Application.HideTrayIcon = true;
                                               Application.Configuration.SaveToFile();
                                               
                                               if (Application.OperatingSystem.Platform == OSPlatform.Linux)
                                               {
                                                   new Prompt(UserText.Window.Prompt.RemoveTrayIcon.RestartRequired).Show();
                                               }
                                               else
                                               {
                                                   Application.TrayIcon.Dispose();
                                                   Application.TrayIcon = null;
                                               }
                                           }
                                       }).Show();
                        })
                    ]),
                    
                    CreateMenuEntry(UserText.TrayIcon.Menu.About, delegate
                    {
                        new WindowType.About().Show();
                    }),
                    
                    CreateMenuEntry(UserText.TrayIcon.Menu.Exit, delegate
                    {
                        Application.TrayIcon?.Dispose();
                        Application.TrayIcon = null;
                        Application.Close();
                    })
                ]
            };
            
            Application.TrayIcon.Clicked += Click_ShowUptimeNotification;
            

            if (Application.OperatingSystem.Platform != OSPlatform.Windows)
            {
                foreach (NativeMenuItem item in Application.TrayIcon.Menu)
                {
                    if (item.Menu?.Items.Count > 0)
                    {
                        item.Click += Click_ShowUptimeNotification;
                    }
                }
            }

            Application.TrayIcon.IsVisible = true;
        }
        
        /// <summary> Create an entry of the tray icon menu displaying the specified header and executing the specified event on being clicked. </summary>
        /// <param name="header"> Text to display in menu entry. </param>
        /// <param name="clickEvent"> Event to execute on click. </param>
        /// <returns> Single element of tray icon menu. </returns>
        public static NativeMenuItem CreateMenuEntry(string header, EventHandler clickEvent)
        {
            NativeMenuItem item = new(header);
            item.Click += clickEvent;
            
            return item;
        }
        
        /// <summary> Create an entry of the tray icon menu displaying the specified header to contain other subsidiary elements. </summary>
        /// <param name="header"> Text to display in menu entry. </param>
        /// <param name="menu"> Subsidiary menu to add to this entry. </param>
        /// <returns> Single element of tray icon menu. </returns>
        public static NativeMenuItem CreateMenuEntry(string header, NativeMenu? menu)
        {
            return new NativeMenuItem(header) {Menu = menu};
        }

        /// <summary> Initialize rendering pipeline with a transparent render, preventing such procedure from taking time when demanded. </summary>
        async internal static void PreloadRender()
        {
            Canvas canvas = new();
            canvas.Children.Add(new Button()
            {
                Content = string.Empty,
                Foreground = Brushes.Transparent,
                Background = Brushes.Transparent,
                Width = 0,
                Height = 0
            });
            
            Window window = new()
            {
                Background = Brushes.Transparent,
                SystemDecorations = SystemDecorations.None,
                WindowStartupLocation = WindowStartupLocation.Manual,
                Position = new PixelPoint(-1, -1),
                CanResize = false,
                ShowInTaskbar = false,
                Width = 0,
                Height = 0,
                Content = canvas,
            };
            
            window.Show();
            await Task.Delay(100);
            window.Close();
        }
    }
}
