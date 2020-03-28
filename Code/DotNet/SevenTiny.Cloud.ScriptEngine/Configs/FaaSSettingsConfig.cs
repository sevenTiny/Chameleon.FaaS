using Microsoft.Extensions.Logging;
using SevenTiny.Bantina.Configuration;
using SevenTiny.Bantina.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SevenTiny.Cloud.ScriptEngine.Configs
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
        private static readonly ILogger _logger = new LogManager();

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
            List<string> referenceDirs = new List<string>()
            {
                //默认扫描当前用户.nuget目录，这个目录不需要配置
                $"C:\\Users\\{Environment.UserName}\\.nuget\\packages"
            };

            if (FaaSSettingsConfig.Instance == null)
            {
                _logger.LogError("get FaaSSettingsConfig faild");
                return referenceDirs;
            }

            var configDirs = FaaSSettingsConfig.Instance.ReferenceDirs?.Split(',');

            if (configDirs != null && configDirs.Any())
                referenceDirs.AddRange(configDirs);

            return referenceDirs;
        }
    }
}
