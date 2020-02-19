using Newtonsoft.Json;
using SevenTiny.Bantina.Configuration;
using SevenTiny.Cloud.ScriptEngine.Exceptions;

namespace SevenTiny.Cloud.ScriptEngine.Configs
{
    [ConfigName("appsettings")]
    internal class AppSettingsConfig : JsonConfigBase<AppSettingsConfig>
    {
        [JsonProperty("ConnectionStrings")]
        public ConnectionStrings ConnectionStrings { get; private set; }
        [JsonProperty("SevenTinyCloud")]
        public SevenTinyCloud SevenTinyCloud { get; private set; }
    }

    internal class SevenTinyCloud
    {
        [JsonProperty]
        public string AppName { get; private set; }
    }

    internal class ConnectionStrings
    {
        [JsonProperty]
        public string SevenTinyConfig { get; private set; }
    }

    public static class AppSettingsConfigHelper
    {
        public static string GetCurrentAppName()
        { 
            return AppSettingsConfig.Instance?.SevenTinyCloud?.AppName ?? throw new ConfigNodeNotFoundException("appsettings.SevenTinyCloud.AppName", "appsettings.json -> SevenTinyCloud(object) -> AppName(key) node notfound int config appsettins.json");
        }
    }
}
