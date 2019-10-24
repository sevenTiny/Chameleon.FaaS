using SevenTiny.Bantina.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace SevenTiny.Cloud.FaaS.Configs
{
    [ConfigName("ScriptEngine_AssemblyReference")]
    internal class AssemblyReferenceConfig : MySqlRowConfigBase<AssemblyReferenceConfig>
    {
        [ConfigProperty]
        public string AppName { get; set; }
        [ConfigProperty]
        public string Assembly { get; set; }

        public static List<AssemblyInfo> GetAssemblyInfoByAppName(string appName)
        {
            return Instance?.Where(t => t.AppName.Equals(appName))?.Select(t => new AssemblyInfo { AppName = t.AppName, Assembly = t.Assembly})?.ToList();
        }
    }

    internal class AssemblyInfo
    {
        public string AppName { get; set; }
        public string Assembly { get; set; }
    }
}
