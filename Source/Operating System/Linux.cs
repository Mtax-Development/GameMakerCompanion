#if Linux
#pragma warning disable CS8600, CS8602, CS8604, IDE0047
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using GameMakerCompanion.Interface.WindowType;
using GameMakerCompanion.Utility;
using Path = GameMakerCompanion.Utility.Path;

namespace GameMakerCompanion.OperatingSystem
{
    internal partial class Linux : IOperatingSystem
    {
        OSPlatform IOperatingSystem.Platform {get => OSPlatform.Linux;}
        HorizontalAlignment IOperatingSystem.WindowControlAlignment {get => HorizontalAlignment.Right;}
        
        bool IOperatingSystem.SetupDependencies()
        {
            try
            {
                ProcessStartInfo terminalCommand = new ProcessStartInfo
                {
                    FileName = "which",
                    Arguments = "xdotool",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                
                Process? terminalProcess = Process.Start(terminalCommand);
                
                if (terminalProcess != null)
                {
                    terminalProcess.WaitForExit();
                    
                    string dependencyPath = WhitespaceRegex().Replace(terminalProcess.StandardOutput.ReadToEnd(), string.Empty);
                    
                    if (dependencyPath.Length > 0)
                    {
                        return true;
                    }
                    else
                    {
                        Application.ReportLaunchFailure(UserText.Error.Launch.Dependency.Initialization);
                    }
                }
            }
            catch
            {
                Application.ReportLaunchFailure(UserText.Error.Launch.Dependency.UnexpectedError);
            }
            
            return false;
        }
        
        string IOperatingSystem.GetWindowTitle(Process process)
        {
            try
            {
                // Run a command line process to scan for X Window System windows with the specified process ID and get its name.
                ProcessStartInfo terminalCommand = new ProcessStartInfo
                {
                    FileName = "xdotool",
                    Arguments = ("search --onlyvisible --pid " + process.Id),
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                
                Process? terminalProcess = Process.Start(terminalCommand);
                
                if (terminalProcess != null)
                {
                    terminalProcess.WaitForExit();
                    
                    string windowID = WhitespaceRegex().Replace(terminalProcess.StandardOutput.ReadToEnd(), string.Empty);
                    
                    if (windowID.Length > 0)
                    { 
                        terminalCommand.Arguments = ("getwindowname " + windowID);
                        terminalProcess = Process.Start(terminalCommand);
                        
                        if (terminalProcess != null)
                        {
                            terminalProcess.WaitForExit();
                            
                            return WhitespaceRegex().Replace(terminalProcess.StandardOutput.ReadToEnd(), string.Empty);
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine(UserText.Error.Launch.Dependency.Ongoing);
            }
            
            return string.Empty;
        }
        
        void IOperatingSystem.OpenApplication(string launchProtocol)
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
        
        void IOperatingSystem.OpenURL(params string[] URL)
        {
            try
            {
                foreach (string address in URL)
                {
                    Process.Start("xdg-open", address);
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
            try
            {
                Process.Start("notify-send", ("\"" + text + "\" " + "--app-name=\"" + Application.Name + "\""));
            }
            catch
            {
                new Prompt(text).Show();
            }
        }
        
        void IOperatingSystem.SetAutomaticStartup(bool enable)
        {
            try
            {
                bool autostartDirectoryExists = Directory.Exists(Path.OperatingSystem.AutoStart);
                
                if (enable)
                {
                    string logSuccess = UserText.Information.AutostartEntrySaved;
                    
                    if (!autostartDirectoryExists)
                    {
                        Directory.CreateDirectory(Path.OperatingSystem.AutoStart);
                    }
                    
                    if (File.Exists(Path.File.AutoStart))
                    {
                        try
                        {
                            File.Delete(Path.File.AutoStart);
                            
                            logSuccess = UserText.Information.AutostartEntryOverwritten;
                        }
                        catch (Exception exception)
                        {
                            Application.LogException(exception, new Prompt(UserText.Error.Autostart.OverwriteFailure));
                        }
                    }
                    
                    if (!File.Exists(Path.File.AutoStart))
                    {
                        using (FileStream fileStream = File.Create(Path.File.AutoStart))
                        {
                            string content =
                                ("[Desktop Entry]" + Environment.NewLine +
                                 "Type=Application" + Environment.NewLine +
                                 "Name=" + Application.Name + Environment.NewLine +
                                 "Exec=" + @"'" + Path.File.Executable + @"'" + Environment.NewLine +
                                 "StartupNotify=false" + Environment.NewLine +
                                 "Terminal=false" + Environment.NewLine);
                            byte[] buffer = new UTF8Encoding(true).GetBytes(content);
                            
                            fileStream.Write(buffer, 0, buffer.Length);
                        }
                        
                        Console.WriteLine(logSuccess);
                    }
                }
                else if (autostartDirectoryExists && ValidateAutomaticStartup())
                {
                    try
                    {
                        File.Delete(Path.File.AutoStart);
                        
                        Console.WriteLine(UserText.Information.AutostartEntryRemoved);
                    }
                    catch (Exception exception)
                    {
                        Application.LogException(exception, new Prompt(UserText.Error.Autostart.RemovalFailure));
                    }
                }
            }
            catch (Exception exception)
            {
                Application.LogException(exception, new Prompt(((enable) ? UserText.Error.Autostart.SetupFailure
                                                                         : UserText.Error.Autostart.RemovalFailure)));
            }
        }
        
        public bool ValidateAutomaticStartup()
        {
            try
            {
                foreach (string line in File.ReadAllLines(Path.File.AutoStart))
                {
                   if (line.Contains("Exec") && line.Contains('=') && line.Contains(Path.File.Executable))
                   {
                        return true;
                   }
                }
            }
            catch (Exception exception)
            {
                Application.LogException(exception, UserText.Error.Autostart.ValidationFailure);
            }
            
            return false;
        }
        
        char IOperatingSystem.GetStateSymbol(bool enabled)
        {
            return ((enabled) ? '✅' : '❌');
        }
        
        [GeneratedRegex(@"\t|\n|\r")]
        private static partial Regex WhitespaceRegex();
    }
}
#endif
