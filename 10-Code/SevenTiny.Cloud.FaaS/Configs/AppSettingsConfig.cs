using Newtonsoft.Json;
using SevenTiny.Bantina.Configuration;
using SevenTiny.Cloud.FaaS.Exceptions;

namespace SevenTiny.Cloud.FaaS.Configs
{
    [ConfigName("appsettings")]
    public class AppSettingsConfig : JsonConfigBase<AppSettingsConfig>
    {
        [JsonProperty("ConnectionStrings")]
        public ConnectionStrings ConnectionStrings { get; private set; }
        [JsonProperty("SevenTinyCloud")]
        public SevenTinyCloud SevenTinyCloud { get; private set; }

        public string CurrentAppName => Instance?.SevenTinyCloud?.AppName ?? throw new ConfigNodeNotFoundException("appsettings.SevenTinyCloud.AppName", "appsettings.json -> SevenTinyCloud(object) -> AppName(key) node notfound int config appsettins.json");
    }

    public class SevenTinyCloud
    {
        [JsonProperty]
        public string AppName { get; private set; }
    }

    public class ConnectionStrings
    {
        [JsonProperty]
        public string SevenTinyConfig { get; private set; }
    }
}
