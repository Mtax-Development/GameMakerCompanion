#pragma warning disable CS8600, CS8603
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.Layout;
using GameMakerCompanion.Utility;

namespace GameMakerCompanion.OperatingSystem
{
    public interface IOperatingSystem
    {
        /// <summary> Operating system type identifier for the current runtime. </summary>
        OSPlatform Platform {get;}
        
        /// <summary> Whether operating system executing the runtime places window close control on the left or right side. </summary>
        HorizontalAlignment WindowControlAlignment {get;}
        
        /// <summary> Time in milliseconds of a delay applied between openings of multiple calls to open the default browser. </summary>
        /// <remarks> Prevents the operating system from treating simultaneous calls as error. </remarks>
        static readonly int URLOpeningDelay = 200;
        
        /// <summary> Initialize platform constructor. </summary>
        /// <returns> Instance of a member for this interface specific to the platform of current runtime. </returns>
        /// <exception cref="PlatformNotSupportedException"></exception>
        internal static IOperatingSystem SetupPlatform()
        {
            IOperatingSystem? operatingSystem = null;

            #if Windows
                operatingSystem = (IOperatingSystem)Activator.CreateInstance(typeof(Windows));
            #elif macOS
                operatingSystem = (IOperatingSystem)Activator.CreateInstance(typeof(macOS));
            #elif Linux
                operatingSystem = (IOperatingSystem)Activator.CreateInstance(typeof(Linux));
            #endif
            
            if ((operatingSystem != null) && (RuntimeInformation.IsOSPlatform(operatingSystem.Platform)))
            {
                Console.WriteLine(UserText.Boot.Platform + operatingSystem.GetType().Name);
                
                return operatingSystem;
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }
        
        /// <summary> Check if setup specific to each platform is present and prevent this application from launching further if not. </summary>
        /// <returns> Whether required setup is present or <c>true</c> by default if not required on current runtime platform. </returns>
        internal bool SetupDependencies();
        
        /// <summary> Return the Unicode symbol marking the ON and OFF state, automatically formatted by each operating system. </summary>
        /// <param name="enabled"> Decision on which symbol to return. </param>
        /// <returns> Check or cross symbol automatically formatted by the operating system. </returns>
        internal char GetStateSymbol(bool enabled);
        
        /// <summary> Return the window name of the specified process by means specific to the current runtime platform. </summary>
        /// <param name="process"> Process to obtain the window title of. </param>
        /// <returns> Window title on success. Empty string on failure. </returns>
        internal string GetWindowTitle(Process process);
        
        /// <summary> Forward the specified remote link to opening call in the current runtime platform. </summary>
        /// <param name="URL"> One or multiple links to operate. </param>
        internal void OpenURL(params string[] URL);
        
        /// <summary> Cause the operating system to show a notification with the specified text by means specific to each runtime platform. </summary>
        /// <param name="text"> Text shown to the user, contained in the notification. </param>
        internal void ShowNotification(string text);
        
        /// <summary> Attempt to create an entry specific to each runtime platform, causing this application to launch on current user login. </summary>
        /// <param name="enable"> Whether the entry should be created/replaced or removed. </param>
        internal void SetAutomaticStartup(bool enable);
        
        /// <summary> Check contents of an existing autostart entry to confirm it points to the current executable. </summary>
        /// <remarks> Accuracy of this check varies between each runtime platform. </remarks>
        /// <returns> Whether the entry is assumed to be valid. </returns>
        public bool ValidateAutomaticStartup();
    }
}
