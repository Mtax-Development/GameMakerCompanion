#pragma warning disable IDE0305
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GameMakerCompanion.Utility
{
    /// <summary> Collection of methods operating on process instances of applications. </summary>
    internal static class Instance
    {
        /// <summary> Check if any application with specified titles is currently executed by the operating system. </summary>
        /// <param name="applicationLists"> Arrays with nested application titles for multiple application types. </param>
        /// <returns> Whether any instance was found. </returns>
        internal static bool IsRunning(string[][] applicationLists)
        {
            foreach (string[] applicationList in applicationLists)
            {
                if (GetNumber(applicationList) > 0)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary> Return the number of instances of application executed by the operating system with specified application title. </summary>
        /// <remarks> Process names are truncated to 15 characters in some operating systems. </remarks>
        /// <param name="application"> Title of the application in the process list. </param>
        /// <returns> Number of instances found. </returns>
        internal static int GetNumber(string application)
        {
            int processCount = Process.GetProcessesByName(application).Length;
            
            if (processCount > 0)
            {
                return processCount;
            }
            else
            {
                //|Case for truncated titles.
                return Process.GetProcessesByName(application.Substring(0, Math.Min(application.Length, 15))).Length;
            }
        }
        
        /// <summary> Return the number of instances of multiple applications executed by the operating system with specified application
        ///           titles. </summary>
        /// <remarks> Process names are truncated to 15 characters in some operating systems. </remarks>
        /// <param name="applicationList"> Array containing title names of applications in the process list. </param>
        internal static int GetNumber(string[] applicationList)
        {
            int result = 0;
            foreach (string application in applicationList)
            {
                int processCount = Process.GetProcessesByName(application).Length;
                
                if (processCount > 0)
                {
                    result += processCount;
                }
                else
                {
                    //|Case for truncated titles.
                    result += Process.GetProcessesByName(application.Substring(0, Math.Min(application.Length, 15))).Length;
                }
            }
            
            return result;
        }
        
        /// <summary> Return the number of instances of multiple applications executed by the operating system that are on the specified
        ///           application lists. </summary>
        /// <remarks> Process names are truncated to 15 characters in some operating systems. </remarks>
        /// <param name="applicationLists"> Array containing nested arrays with title names of applications in the process list. </param>
        internal static int GetNumber(string[][] applicationLists)
        {
            int count = 0;
            foreach (string[] applicationList in applicationLists)
            {
                count += GetNumber(applicationList);
            }
            
            return count;
        }
        
        /// <summary> Return process information of application with specified window titles. </summary>
        /// <remarks> Process names are truncated to 15 characters in some operating systems. </remarks>
        /// <param name="applicationList"> Array containing title names of the application in the process list. </param>
        /// <returns> Array with processes of similar titles to specified ones. </returns>
        internal static Process[] GetProcessesByName(string[] applicationList)
        {
            Process[] processList = [];
            
            foreach (string applicationName in applicationList)
            {
                processList = processList.Concat(Process.GetProcessesByName(applicationName)).ToArray();
            }
            
            if (processList.Length < 1)
            {
                foreach (string applicationName in applicationList)
                {
                    //|Case for truncated titles.
                    processList = processList.Concat(Process.GetProcessesByName(applicationName.Substring(0, Math.Min(applicationName.Length, 15))))
                                             .ToArray();
                }
            }
            
            return processList;
        }
        
        /// <summary> Return file paths of currently running instances of applications in specified application list. </summary>
        /// <param name="applicationList"> Array containing title names of the application in the process list. </param>
        /// <returns> Array with paths of processes of similar titles to specified ones. </returns>
        internal static string[] GetPaths(string[] applicationList)
        {
            List<string> result = [];
            foreach (string application in applicationList)
            {
                Process[] process = Process.GetProcessesByName(application);
                
                if (process.Length > 0)
                {
                    ProcessModuleCollection module = process[0].Modules;
                    
                    if (module.Count > 0)
                    {
                        result.Add(module[0].FileName);
                    }
                }
            }
            
            return result.ToArray();
        }
        
        /// <summary> Return file paths of currently running instances of applications in specified application lists. </summary>
        /// <param name="applicationList"> Array containing nested arrays with title names of the application in the process list. </param>
        /// <returns> Array with paths of processes of similar titles to specified ones. </returns>
        internal static string[] GetPaths(string[][] applicationLists)
        {
            string[] result = [];
            int i = 0;
            foreach (string[] applicationList in applicationLists)
            {
                result = result.Concat(GetPaths(applicationLists[i])).ToArray();
                
                ++i;
            }
            
            return result;
        }
    }
}
