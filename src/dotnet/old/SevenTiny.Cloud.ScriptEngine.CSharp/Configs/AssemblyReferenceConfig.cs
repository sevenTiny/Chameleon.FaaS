using SevenTiny.Bantina.Configuration;
using SevenTiny.Cloud.ScriptEngine.Configs;
using System.Collections.Generic;
using System.Linq;

namespace SevenTiny.Cloud.ScriptEngine.CSharp.Configs
{
    [ConfigName("FaaS_CSharpAssemblyReference")]
    internal class AssemblyReferenceConfig : MySqlRowConfigBase<AssemblyReferenceConfig>
    {
        [ConfigProperty]
        public string AppName { get; set; }
        [ConfigProperty]
        public string Assembly { get; set; }

        /// <summary>
        /// 默认的全局应用名
        /// </summary>
        private const string DefaultAppName = "SYSTEM";

        public static List<AssemblyInfo> GetCurrentAppAssemblyReferenceInfos()
        {
            return Instance?.Where(t => t.AppName.ToUpper().Equals(DefaultAppName) || t.AppName.Equals(AppSettingsConfigHelper.GetAppName()))?.Select(t => new AssemblyInfo { AppName = t.AppName, Assembly = t.Assembly })?.ToList();
        }
    }

    internal class AssemblyInfo
    {
        public string AppName { get; set; }
        public string Assembly { get; set; }
    }
}
