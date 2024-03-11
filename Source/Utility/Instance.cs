#pragma warning disable IDE0305
using System;
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
        /// <param name="application"> Title of the application in process list. </param>
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
        
        /// <summary> Return the number of instances of multiple applications executed by the operating system with specified application titles. </summary>
        /// <remarks> Process names are truncated to 15 characters in some operating systems. </remarks>
        /// <param name="applicationList"> Array containing titles names of the application in process list. </param>
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
        
        /// <summary> Return process information of application with specified window titles. </summary>
        /// <remarks> Process names are truncated to 15 characters in some operating systems. </remarks>
        /// <param name="applicationList"> Array containing titles names of the application in process list. </param>
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
    }
}
