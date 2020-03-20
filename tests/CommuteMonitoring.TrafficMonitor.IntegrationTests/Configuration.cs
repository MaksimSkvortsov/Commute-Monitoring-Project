using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace CommuteMonitoring.TrafficMonitor.IntegrationTests
{
    internal class LocalSettings
    {
        public Dictionary<string, string> Values { get; set; }
    }

    internal class Configuration
    {
        public static void InitializeEnvironmentVariables()
        {
            var basePath = Environment.CurrentDirectory;
            var settingsFilePath = Path.Combine(basePath, "local.settings.json");
            var settings = JsonConvert.DeserializeObject<LocalSettings>(File.ReadAllText(settingsFilePath));

            foreach (var setting in settings.Values)
            {
                Environment.SetEnvironmentVariable(setting.Key, setting.Value);
            }
        }
    }
}
