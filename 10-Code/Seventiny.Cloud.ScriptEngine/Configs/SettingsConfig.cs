using SevenTiny.Cloud.ScriptEngine.Toolkit;
using SevenTiny.Bantina.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SevenTiny.Cloud.ScriptEngine.Configs
{
    [ConfigName("ScriptEngine_Settings")]
    internal class SettingsConfig : MySqlColumnConfigBase<SettingsConfig>
    {
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
            return ValueTranslator.TrueFalse(SettingsConfig.Instance?.IsDebug ?? 0);
        }
        public static bool IsOutPutFiles()
        {
            return ValueTranslator.TrueFalse(SettingsConfig.Instance?.IsOutPutFiles ?? 1);
        }
        public static bool IsOutPutAllFiles()
        {
            return ValueTranslator.TrueFalse(SettingsConfig.Instance?.IsOutPutAllFiles ?? 1);
        }
        public static bool IsCachePermanent()
        {
            return ValueTranslator.TrueFalse(SettingsConfig.Instance.IsCachePermanent);
        }
        /// <summary>
        /// 是否单机部署模式
        /// </summary>
        /// <returns></returns>
        public static bool IsStandAloneDeployMode()
        {
            return SettingsConfig.Instance?.DeployMode?.Equals("StandAlone") ?? true;
        }
        /// <summary>
        /// 获取引用路径
        /// </summary>
        /// <returns></returns>
        public static List<string> GetReferenceDirs()
        {
            //默认当前用户.nuget目录，这个目录不需要配置
            List<string> referenceDirs = new List<string>() { $"C:\\Users\\{Environment.UserName}\\.nuget\\packages" };
            var configDirs = SettingsConfig.Instance?.ReferenceDirs?.Split(',').ToList();
            if (configDirs != null && configDirs.Any())
                referenceDirs.AddRange(configDirs);
            return referenceDirs;
        }
    }
}
