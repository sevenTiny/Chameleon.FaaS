using Newtonsoft.Json;
using SevenTiny.Bantina.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seventiny.Cloud.ScriptEngine.Configs
{
    [ConfigName("appsettings")]
    internal class AppSettingsConfig : JsonConfigBase<AppSettingsConfig>
    {
        public static AppSettingsConfig Instance = new AppSettingsConfig();

        [JsonProperty("ConnectionStrings")]
        public ConnectionStrings ConnectionStrings { get; set; }
        [JsonProperty("SevenTinyCloud")]
        public SevenTinyCloud SevenTinyCloud { get; set; }
    }

    public class SevenTinyCloud
    {
        [JsonProperty]
        public string AppName { get; set; }
    }

    public class ConnectionStrings
    {
        [JsonProperty]
        public string SevenTinyConfig { get; set; }
    }
}
