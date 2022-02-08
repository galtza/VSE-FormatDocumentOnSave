using EditorConfig.Core;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace Elders.VSE_FormatDocumentOnSave.Configurations
{
    public sealed class EditorConfigConfiguration : IConfiguration
    {
        private readonly bool enable = true;
        private readonly string allowed = ".*";
        private readonly string denied = "";
        private readonly string excludedPaths = "";
        private readonly string command = "Edit.FormatDocument";
        private readonly bool enableInDebug = false;
        private readonly string basePath = "";

        public EditorConfigConfiguration(string formatConfigFile)
        {
            var parser = new EditorConfigParser(formatConfigFile);
            FileConfiguration configFile = parser.Parse(formatConfigFile).First();

            if (configFile.Properties.TryGetValue("enable", out string enableAsString) && bool.TryParse(enableAsString, out bool enableParsed))
                enable = enableParsed;

            if (configFile.Properties.ContainsKey("allowed_extensions"))
                configFile.Properties.TryGetValue("allowed_extensions", out allowed);

            if (configFile.Properties.ContainsKey("denied_extensions"))
                configFile.Properties.TryGetValue("denied_extensions", out denied);

            if (configFile.Properties.ContainsKey("excluded_paths"))
                configFile.Properties.TryGetValue("excluded_paths", out excludedPaths);

            if (configFile.Properties.ContainsKey("command"))
                configFile.Properties.TryGetValue("command", out command);

            if (configFile.Properties.TryGetValue("enable_in_debug", out string enableInDebugAsString) && bool.TryParse(enableInDebugAsString, out bool enableInDebugParsed))
                enableInDebug = enableInDebugParsed;

            basePath = Path.GetDirectoryName(formatConfigFile);
        }

        public bool IsEnable => enable;

        IEnumerable<string> IConfiguration.Allowed => allowed.Split(' ');

        IEnumerable<string> IConfiguration.Denied => denied.Split(' ');

        IEnumerable<string> IConfiguration.ExcludedPaths => excludedPaths.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries).Select(s => Path.Combine(basePath, s.Replace("/", "\\")));

        public string Commands => command;

        public bool EnableInDebug => enableInDebug;
    }
}
