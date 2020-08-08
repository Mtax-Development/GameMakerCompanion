using Microsoft.Win32;

namespace GMS2_RPC.Element
{
    /// <summary> Class that handles all operations on the system's registry. </summary>
    internal static class RegistryHandler
    {
        /// <summary> Name of the value in the automatic startup key. </summary>
        private static readonly string valueName_automaticStartup = Program.projectName;

        /// <summary> Create or delete a value in the automatic startup key of the registry, which causes the application to be run on system startup. </summary>
        /// <param name="value">If true, the value will be created. If false, the value will be deleted.</param>
        internal static void SetAutomaticStartup(bool value)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(Path.registry_startup, true);

            if (registryKey != null)
            {
                if (value)
                {
                    registryKey.SetValue(valueName_automaticStartup, Path.file_executable);
                }
                else
                {
                    registryKey.DeleteValue(valueName_automaticStartup, false);
                }
            }
        }
    }
}
