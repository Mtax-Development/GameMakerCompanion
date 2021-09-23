using IniParser; //|https://github.com/rickyah/ini-parser
using IniParser.Model;
using System.IO;

namespace GMS2_RPC.Element
{
    /// <summary> Class that handles all operations on INI-based configuration. </summary>
    /// <remarks> The properties can be returned or set (as strings). The INI file will be operated automatically after that.
    /// In case the config file does not exist, it will be created with default values. </remarks>
    internal static class INI_Config
    {
        /// <summary> Contains all possible sections in the file. </summary>
        internal struct Section
        {
            internal static readonly string richPresence = "RichPresence";
            internal static readonly string preferences = "Preferences";
        }

        /// <summary> Contains all possible keys in the file. </summary>
        internal struct Key
        {
            //|[RichPresence]
            internal static readonly string applicationCheckDelay = "ApplicationCheckDelay";
            internal static readonly string applicationExitDelay = "ApplicationExitDelay";

            //|[Preferences]
            internal static readonly string startOnBoot = "StartOnBoot";
            internal static readonly string hideTrayIcon = "HideTrayIcon";
            internal static readonly string showProjectTitles = "ShowProjectTitles";
        }

        /// <summary> Contains the default values for all possible keys in the file. </summary>
        internal struct DefaultData
        {
            //|[RichPresence]
            internal static readonly string applicationCheckDelay = "5000";
            internal static readonly string applicationExitDelay = "7000";

            //|[Preferences]
            internal static readonly string startOnBoot = false.ToString();
            internal static readonly string hideTrayIcon = false.ToString();
            internal static readonly string showProjectTitles = true.ToString();
        }
        
        internal static string ApplicationCheckDelay
        {
            get { return GetValue(Section.richPresence, Key.applicationCheckDelay); }
            set { SetValue(Section.richPresence, Key.applicationCheckDelay, value); }
        }

        internal static string ApplicationExitDelay
        {
            get { return GetValue(Section.richPresence, Key.applicationExitDelay); }
            set { SetValue(Section.richPresence, Key.applicationExitDelay, value); }
        }
        
        internal static string StartOnBoot
        {
            get { return GetValue(Section.preferences, Key.startOnBoot); }
            set { SetValue(Section.preferences, Key.startOnBoot, value); }
        }

        internal static string ShowProjectTitles
        {
            get { return GetValue(Section.preferences, Key.showProjectTitles); }
            set { SetValue(Section.preferences, Key.showProjectTitles, value); }
        }

        internal static string HideTrayIcon
        {
            get { return GetValue(Section.preferences, Key.hideTrayIcon); }
            set { SetValue(Section.preferences, Key.hideTrayIcon, value); }
        }

        internal static readonly FileIniDataParser parser = new FileIniDataParser();

        /// <summary> Create the config file with default values. </summary>
        internal static void CreateDefault()
        {
            IniData defaultData = new IniData();

            defaultData.Sections.AddSection(Section.richPresence);
            defaultData[Section.richPresence].AddKey(Key.applicationCheckDelay, DefaultData.applicationCheckDelay);
            defaultData[Section.richPresence].AddKey(Key.applicationExitDelay, DefaultData.applicationExitDelay);

            defaultData.Sections.AddSection(Section.preferences);
            defaultData[Section.preferences].AddKey(Key.startOnBoot, DefaultData.startOnBoot);
            defaultData[Section.preferences].AddKey(Key.hideTrayIcon, DefaultData.hideTrayIcon);
            defaultData[Section.preferences].AddKey(Key.showProjectTitles, DefaultData.showProjectTitles);

            parser.WriteFile(Path.file_config, defaultData);
        }

        /// <summary> Read key value from the config file. </summary>
        /// <remarks> The file will be created with default values if it does not exist. </remarks>
        internal static string GetValue(string section, string key)
        {
            if (!File.Exists(Path.file_config))
            {
                CreateDefault();
            }

            IniData data = parser.ReadFile(Path.file_config);

            return data[section][key];
        }

        /// <summary> Save key value to the config file. </summary>
        /// <remarks> The file will be created with default values if it does not exist. </remarks>
        internal static void SetValue(string section, string key, string value)
        {
            if (!File.Exists(Path.file_config))
            {
                CreateDefault();
            }

            IniData data = parser.ReadFile(Path.file_config);
            data[section][key] = value;

            parser.WriteFile(Path.file_config, data);
        }
    }
}
