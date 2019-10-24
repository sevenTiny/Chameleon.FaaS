using SevenTiny.Bantina.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SevenTiny.Cloud.FaaS.Configs
{
    [ConfigName("FaaS_Settings")]
    internal class FaaSSettingsConfig : MySqlColumnConfigBase<FaaSSettingsConfig>
    {
        [ConfigProperty]
        public int IsDebugMode { get; set; }
        [ConfigProperty]
        public int IsOutPutFiles { get; set; }
        /// <summary>
        /// 引用路径，多个逗号分隔
        /// </summary>
        [ConfigProperty]
        public string ReferenceDirs { get; set; }
    }

    public static class FaaSSettingsConfigHelper
    {
        public static bool IsDebugMode()
        {
            return FaaSSettingsConfig.Instance?.IsDebugMode == 1;
        }
        public static bool IsOutPutFiles()
        {
            return FaaSSettingsConfig.Instance?.IsOutPutFiles == 1;
        }
        /// <summary>
        /// 获取引用的dll路径
        /// </summary>
        /// <returns></returns>
        public static List<string> GetReferenceDirs()
        {
            //默认扫描当前用户.nuget目录，这个目录不需要配置
            List<string> referenceDirs = new List<string>() { $"C:\\Users\\{Environment.UserName}\\.nuget\\packages" };
            var configDirs = FaaSSettingsConfig.Instance?.ReferenceDirs?.Split(',').ToList();
            if (configDirs != null && configDirs.Any())
                referenceDirs.AddRange(configDirs);
            return referenceDirs;
        }
    }
}
