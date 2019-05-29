using Seventiny.Cloud.ScriptEngine.Toolkit;
using SevenTiny.Bantina.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Seventiny.Cloud.ScriptEngine.Configs
{
    [ConfigName("ScriptEngine_Settings")]
    internal class SettingsConfig : MySqlColumnConfigBase<SettingsConfig>
    {
        public static SettingsConfig Instance = new SettingsConfig();
        protected override string _ConnectionString => GetConnectionStringFromAppSettings("SevenTinyConfig");

        [ConfigProperty]
        public int IsDebug { get; set; }
        [ConfigProperty]
        public int IsOutPutFiles { get; set; }
        [ConfigProperty]
        public int IsOutPutAllFiles { get; set; }
        [ConfigProperty]
        public int IsCachePermanent { get; set; }
        /// <summary>
        /// 服务部署模式
        /// StandAlone=单机模式，Cluster=集群模式
        /// </summary>
        [ConfigProperty]
        public string DeployMode { get; set; }
        /// <summary>
        /// 引用路径，多个逗号分隔
        /// </summary>
        [ConfigProperty]
        public string ReferenceDirs { get; set; }
    }

    internal static class SettingsConfigHelper
    {
        public static bool IsDebug()
        {
            return ValueTranslator.TrueFalse(SettingsConfig.Instance?.Config?.IsDebug ?? 0);
        }
        public static bool IsOutPutFiles()
        {
            return ValueTranslator.TrueFalse(SettingsConfig.Instance?.Config?.IsOutPutFiles ?? 1);
        }
        public static bool IsOutPutAllFiles()
        {
            return ValueTranslator.TrueFalse(SettingsConfig.Instance?.Config?.IsOutPutAllFiles ?? 1);
        }
        public static bool IsCachePermanent()
        {
            return ValueTranslator.TrueFalse(SettingsConfig.Instance.Config.IsCachePermanent);
        }
        /// <summary>
        /// 是否单机部署模式
        /// </summary>
        /// <returns></returns>
        public static bool IsStandAloneDeployMode()
        {
            return SettingsConfig.Instance?.Config?.DeployMode?.Equals("StandAlone") ?? true;
        }
        /// <summary>
        /// 获取引用路径
        /// </summary>
        /// <returns></returns>
        public static List<string> GetReferenceDirs()
        {
            return SettingsConfig.Instance?.Config?.ReferenceDirs?.Split(',').ToList();
        }
    }
}
