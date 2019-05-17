using SevenTiny.Bantina.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seventiny.Cloud.ScriptEngine.Configs
{
    [ConfigName("ScriptEngine_Settings")]
    internal class ScriptEngine_SettingsConfig : MySqlColumnConfigBase<ScriptEngine_SettingsConfig>
    {
        public static ScriptEngine_SettingsConfig Instance = new ScriptEngine_SettingsConfig();
        protected override string _ConnectionString => GetConnectionStringFromAppSettings("SevenTinyConfig");

        [ConfigProperty]
        public int IsDebug { get; set; }
        [ConfigProperty]
        public int IsOutPutAllFiles { get; set; }
        [ConfigProperty]
        public int IsCachePermanent { get; set; }
    }
}
