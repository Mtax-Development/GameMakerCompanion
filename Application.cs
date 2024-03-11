using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using GameMakerCompanion.Interface;
using GameMakerCompanion.Interface.WindowType;
using GameMakerCompanion.OperatingSystem;
using GameMakerCompanion.Utility;

namespace GameMakerCompanion
{
    /// <summary> Entry class of the program. </summary>
    public static class Application
    {
        /// <summary> Title and executable file name of this application. </summary>
        public static readonly string Name = LogApplicationStart();
        
        /// <summary> Methods and properties operated differently for each platform. </summary>
        public readonly static IOperatingSystem OperatingSystem = IOperatingSystem.SetupPlatform();
        
        /// <summary> Serialized application preferences. </summary>
        internal static Configuration Configuration = new();
        
        /// <summary> Main interaction interface operated through icon menu. </summary>
        /// <remarks> Can be removed for this application to operate in background. </remarks>
        internal static TrayIcon? TrayIcon = null;
        
        /// <summary> Window with information produced by this application as a result of user interaction. </summary>
        /// <remarks> Only one window can be visible at a time. New windows replace existing ones. </remarks>
        internal static Window? PrimaryWindow = null;
        
        /// <summary> Entry function of the program. </summary>
        /// <remarks> This application is restricted to only one instance. Subsequent instances will automatically quit after showing a prompt. </remarks>
        public static void Main()
        {
            AppBuilder.Configure<Tray>()
                      .UsePlatformDetect()
                     #if macOS
                      .With(new MacOSPlatformOptions() {ShowInDock = false})
                     #endif
                      .StartWithClassicDesktopLifetime([], ShutdownMode.OnExplicitShutdown);
            
            Close();
        }
        
        /// <summary> Output the first message of the application on declaration of the first static value. </summary>
        /// <returns> Name of this application. </returns>
        private static string LogApplicationStart()
        {
            string name = "GameMaker Companion";
            Console.WriteLine(name + UserText.Boot.Start);
            
            return name;
        }
        
        /// <summary> Output error information about caught exception to command line. </summary>
        /// <param name="exception"> Error data information. </param>
        /// <param name="explanation"> Message specific to the error. </param>
        internal static void LogException(Exception exception, string explanation)
        {
            Console.WriteLine(UserText.Error.Header + exception.GetType().Name + " (" + exception.Message + ")");
            Console.WriteLine(explanation);
        }
        
        /// <summary> Output error information about caught exception to command line and show a prompt to the user. </summary>
        /// <param name="exception"> Error data information. </param>
        /// <param name="prompt"> Prompt to show. </param>
        internal static void LogException(Exception exception, Prompt prompt)
        {
            Console.WriteLine(UserText.Error.Header + exception.GetType().Name + " (" + exception.Message + ")");
            prompt.Show();
        }
        
        /// <summary> Show a prompt to notify the user about a critical problem at application launch. </summary>
        /// <remarks> Interaction with this prompt will close the application. </remarks>
        /// <param name="explanation"> Message specific to the error. </param>
        internal static void ReportLaunchFailure(string explanation)
        {
            Prompt prompt = new(explanation, UserText.Error.Launch.Exit, Close);
            prompt.ExitButton.Click += Close;
            prompt.Show();
        }
        
        /// <summary> Close the application after outputting information about it finishing. </summary>
        public static void Close(object? sender = null, RoutedEventArgs? e = null)
        {
            Console.WriteLine(Name + UserText.Boot.End);
            Environment.Exit(0);
        }
    }
}
