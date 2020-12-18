using Newtonsoft.Json;
using SevenTiny.Bantina.Configuration;

namespace SevenTiny.Cloud.ScriptEngine.Configs
{
    [ConfigName("appsettings")]
    internal class AppSettingsConfig : JsonConfigBase<AppSettingsConfig>
    {
        [JsonProperty("ConnectionStrings")]
        public ConnectionStrings ConnectionStrings { get; private set; }

        [JsonProperty("AppName")]
        public string AppName { get; set; }
    }

    internal class ConnectionStrings
    {
        [JsonProperty]
        public string SevenTinyConfig { get; set; }
    }

    public static class AppSettingsConfigHelper
    {
        public static string GetAppName()
        {
            return AppSettingsConfig.Instance?.AppName ?? "SevenTinyCloud";
        }
    }
}
