using System.Drawing;
using System.IO;

namespace GMS2_RPC.Element
{
    /// <summary> Class that handles contains information about asset files. </summary>
    internal static class Resource
    {
        internal static readonly string trayIcon = "GMS2_RPC.ico";
        internal static readonly string presence_image_main = "gms2";

        internal static Icon projectIcon = null;

        internal static void InitializeResources()
        {
            if (File.Exists(Path.file_asset_projectIcon))
            {
                projectIcon = new Icon(Path.file_asset_projectIcon);
            }
        }
    }
}
