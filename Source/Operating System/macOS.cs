#if macOS
#pragma warning disable CS8600, CS8602, CS8604, IDE0047, IDE0063, IDE1006
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using GameMakerCompanion.Interface.WindowType;
using GameMakerCompanion.Utility;
using static GameMakerCompanion.OperatingSystem.UnmanagedCode.CoreGraphics;
using static GameMakerCompanion.OperatingSystem.UnmanagedCode.CoreFoundation;
using File = System.IO.File;
using Path = GameMakerCompanion.Utility.Path;

namespace GameMakerCompanion.OperatingSystem
{
    internal unsafe class macOS : IOperatingSystem
    {
        OSPlatform IOperatingSystem.Platform {get => OSPlatform.OSX;}
        HorizontalAlignment IOperatingSystem.WindowControlAlignment {get => HorizontalAlignment.Left;}
        
        bool IOperatingSystem.SetupDependencies()
        {
            try
            {
                if (CGPreflightScreenCaptureAccess())
                {
                    return true;
                }
                else
                {
                    CGRequestScreenCaptureAccess();
                    
                    Application.ReportLaunchFailure(UserText.Error.Launch.Dependency.Initialization);
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
            string result = string.Empty;
            
            try
            {
                //|Iterate through the list of X Window System windows to return the title of a window with matched process ID.
                // This operation requires Screen Reading permission.
                CFStringRef kCGWindowOwnerPID = CFString("kCGWindowOwnerPID");
                CFStringRef kCGWindowName = CFString("kCGWindowName");
                CFArrayRef windowIdentifier = CGWindowListCreate(kCGWindowListOptionAll, kCGNullWindowID);
                CFArrayRef windowDictionary = CGWindowListCreateDescriptionFromArray(windowIdentifier);
                CFIndex windowCount = CFArrayGetCount(windowDictionary);
                for (int i = 0; (i < windowCount); ++i)
                {
                    CFDictionaryRef dictionary = CFArrayGetValueAtIndex(windowDictionary, i);
                    int? windowOwnerPID = DictionaryReadCInt(dictionary, kCGWindowOwnerPID);
                    string? windowName = DictionaryReadString(dictionary, kCGWindowName);
                    
                    if ((process.Id == windowOwnerPID) && (windowName?.Length > 0))
                    {
                        result = windowName;
                        
                        break;
                    }
                }
                
                CFRelease(kCGWindowOwnerPID);
                CFRelease(kCGWindowName);
                CFRelease(windowIdentifier);
                CFRelease(windowDictionary);
            }
            catch (Exception exception)
            {
                Application.LogException(exception, UserText.Error.Launch.Dependency.Ongoing);
            }
            
            return result;
        }
        
        void IOperatingSystem.OpenApplication(string launchProtocol)
        {
            try
            {
                using (Process process = new())
                {
                    process.StartInfo.FileName = "open";
                    process.StartInfo.ArgumentList.Add(launchProtocol);
                    process.Start();
                    process.WaitForExit();
                }
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
                    Process.Start("open", address);
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
            using (Process process = new())
            {
                process.StartInfo.FileName = "osascript";
                process.StartInfo.ArgumentList.Add("-e");
                process.StartInfo.ArgumentList.Add("display notification \"" + text + "\" with title \"" + Application.Name + "\"");
                process.Start();
                process.WaitForExit();
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
                            string fileHeader = ("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine +
                                                 "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" " +
                                                 "\"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">" + Environment.NewLine +
                                                 "<plist version=\"1.0\">" + Environment.NewLine +
                                                 "<dict>" + Environment.NewLine +
                                                 "	<key>Label</key>" + Environment.NewLine +
                                                 "	 <string>" + Application.Name + "</string>" + Environment.NewLine +
                                                 "	" + Environment.NewLine);
                            string fileFooter = (Environment.NewLine + "	" + Environment.NewLine +
                                                 "	<key>RunAtLoad</key>" + Environment.NewLine +
                                                 "	 <true/>" + Environment.NewLine +
                                                 "</dict>" + Environment.NewLine +
                                                 "</plist>" + Environment.NewLine);
                            string content = string.Empty;
                            
                            if (Path.Directory.ApplicationRoot.Contains(Path.Directory.Subsidiary.ApplicationBundleBinary))
                            {
                                //|Case for being run through Application Bundle.
                                content = ("	<key>ProgramArguments</key>" + Environment.NewLine +
                                           "	 <array>" + Environment.NewLine +
                                           "	  <string>" + "open" + "</string>" + Environment.NewLine +
                                           "	  <string>" + "-a" + "</string>" + Environment.NewLine +
                                           "	  <string>" + "" + Path.Directory.ApplicationBundle + "" + "</string>" +
                                           Environment.NewLine + "	 </array>");
                            }
                            else
                            {
                                //|Case for being run through Terminal.
                                content = ("	<key>Program</key>" + Environment.NewLine +
                                           "	 <string>" + Path.File.Executable + "</string>");
                            }
                            
                            byte[] buffer = new UTF8Encoding(true).GetBytes(fileHeader + content + fileFooter);
                            
                            fileStream.Write(buffer, 0, buffer.Length);
                        }
                        
                        Console.WriteLine(logSuccess);
                    }
                }
                else
                {
                    if (autostartDirectoryExists && ValidateAutomaticStartup())
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
                string[] lineSet = File.ReadAllLines(Path.File.AutoStart);
                string targetPath = ((Path.Directory.ApplicationRoot.Contains(Path.Directory.Subsidiary.ApplicationBundleBinary))
                                     ? Path.Directory.ApplicationBundle : Path.File.Executable);
                int? lineIndexHeader = null;
                int? lineIndexProgram = null;
                int i = 0;
                foreach (string line in lineSet)
                {
                    if ((lineIndexHeader == null) && (line.Contains("plist")))
                    {
                        lineIndexHeader = i; 
                    }
                    
                    if (lineIndexHeader != null)
                    {
                        if ((lineIndexProgram == null) && (line.Contains("Program")))
                        {
                            lineIndexProgram = i;
                        }
                        
                        if ((lineIndexProgram != null) && (lineIndexProgram >= lineIndexHeader) && (line.Contains(targetPath)))
                        {
                            return true;
                        }
                    }
                    
                    ++i;
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
    }
}
#endif
