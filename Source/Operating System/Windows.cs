#if Windows
#pragma warning disable CA1416, CS8600, CS8602, CS8604, IDE0047
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;
using Microsoft.Toolkit.Uwp.Notifications;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using GameMakerCompanion.Interface.WindowType;
using GameMakerCompanion.Utility;
using File = System.IO.File;

namespace GameMakerCompanion.OperatingSystem
{
    internal class Windows : IOperatingSystem
    {
        OSPlatform IOperatingSystem.Platform {get => OSPlatform.Windows;}
        HorizontalAlignment IOperatingSystem.WindowControlAlignment {get => HorizontalAlignment.Right;}
        
        bool IOperatingSystem.SetupDependencies() => true;
        
        string IOperatingSystem.GetWindowTitle(Process process) => process.MainWindowTitle;
        
        void IOperatingSystem.OpenApplication(string launchProtocol)
        {
            if ((launchProtocol.EndsWith(".exe")) && (!File.Exists(launchProtocol)))
            {
                new Prompt(UserText.Error.Launcher.TargetFileNonexistent).Show();
            }
            else
            {
                try
                {
                    Process.Start(new ProcessStartInfo(launchProtocol) {UseShellExecute = true});
                }
                catch (Exception exception)
                {
                    Application.LogException(exception, new Prompt(UserText.Error.Launcher.LaunchProtocolFailure));
                }
            }
        }
        
        void IOperatingSystem.OpenURL(params string[] URL)
        {
            try
            {
                foreach (string address in URL)
                {
                    Process.Start(new ProcessStartInfo(address) {UseShellExecute = true});
                    Thread.Sleep(IOperatingSystem.URLOpeningDelay);
                }
            }
            catch
            {
                new Prompt(UserText.Error.URLOpeningFailure.Explanation, UserText.Error.URLOpeningFailure.ButtonText,
                           delegate (object? sender, RoutedEventArgs args)
                {
                    ((Prompt)((Button)sender).Parent.Parent).ClipboardHandle.SetTextAsync(URL[0]);
                }).Show();
            }
        }
        
        void IOperatingSystem.ShowNotification(string text)
        {
            new ToastContentBuilder()
                .AddText(text)
                .Show();
        }
        
        void IOperatingSystem.SetAutomaticStartup(bool enable)
        {
            RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey(Path.OperatingSystem.AutoStart, true);
            
            if (registryKey != null)
            {
                if (enable)
                {
                    string logSuccess = ((registryKey.GetValue(Application.Name) == null)
                                         ? UserText.Information.AutostartEntrySaved : UserText.Information.AutostartEntryOverwritten);
                    
                    registryKey.SetValue(Application.Name, ("\"" + Path.File.Executable + "\""), RegistryValueKind.String);
                    
                    Console.WriteLine(logSuccess);
                }
                else
                {
                    registryKey.DeleteValue(Application.Name, false);
                    
                    Console.WriteLine(UserText.Information.AutostartEntryRemoved);
                }
                
                registryKey.Close();
            }
            else
            {
                Console.WriteLine(UserText.Error.Autostart.SetupFailure);
            }
        }
        
        bool IOperatingSystem.ValidateAutomaticStartup()
        {
            RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey(Path.OperatingSystem.AutoStart, false);
            
            return ((registryKey != null) && (registryKey.GetValue(Application.Name).Equals("\"" + Path.File.Executable + "\"")));
        }
        
        char IOperatingSystem.GetStateSymbol(bool enabled)
        {
            return ((enabled) ? '✔' : '❌');
        }
    }
}
#endif
