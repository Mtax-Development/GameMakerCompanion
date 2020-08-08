using System;

namespace GMS2_RPC.Element
{
    /// <summary> Class that contains various location information. </summary>
    internal static class Path
    {
        internal static readonly string folder_app = AppDomain.CurrentDomain.BaseDirectory;
        internal static readonly string folder_assets = folder_app + @"\assets\";
        internal static readonly string registry_startup = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        internal static readonly string fileName_config = "config.ini";
        internal static readonly string file_executable = folder_app + Program.projectName + ".exe";
        internal static readonly string file_config = folder_app + fileName_config;
        internal static readonly string file_asset_projectIcon = folder_assets + Resource.trayIcon;
        internal static readonly string url_projectRepository = "https://github.com/Git-Mtax/GMS2_RPC";
    }
}
