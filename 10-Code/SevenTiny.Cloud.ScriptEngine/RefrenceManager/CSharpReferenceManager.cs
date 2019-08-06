using Microsoft.CodeAnalysis;
using SevenTiny.Cloud.ScriptEngine.Configs;
using SevenTiny.Cloud.ScriptEngine.Toolkit;
using SevenTiny.Bantina.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SevenTiny.Cloud.ScriptEngine.RefrenceManager
{
    internal class CSharpReferenceManager
    {
        private static readonly ILog _logger = new LogManager();

        public static void InitMetadataReferences(AdvancedCache<string, List<MetadataReference>> metadataReferences)
        {
            if (!metadataReferences.ContainsKey(AppSettingsConfigHelper.GetAppName()))
                metadataReferences.Insert(AppSettingsConfigHelper.GetAppName(), new List<MetadataReference>(), CacheStrategy.Permanent);

            ReferenceNecessaryAssembly(metadataReferences);
            ReferenceSystemAssembly(metadataReferences);
            ReferenceExternalAssembly(metadataReferences);

            var referenceArrayJson = Newtonsoft.Json.JsonConvert.SerializeObject(metadataReferences[AppSettingsConfigHelper.GetAppName()]?.Select(t => t.Display)?.ToArray());
            _logger.Debug($"dll加载完毕,加载信息如下：{referenceArrayJson}");
        }

        private static void ReferenceNecessaryAssembly(AdvancedCache<string, List<MetadataReference>> metadataReferences)
        {
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            var references = new[]
            {
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "mscorlib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "netstandard.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "Microsoft.CSharp.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Collections.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Linq.dll")),
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Xml.XmlReader).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Net.HttpWebRequest).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Net.Http.HttpClient).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.Serialization.DataContractSerializer).Assembly.Location),
            };
            metadataReferences[AppSettingsConfigHelper.GetAppName()].AddRange(references);
        }

        /// <summary>
        /// 公共引用包，在配置文件中配置引用key=System
        /// </summary>
        /// <param name="metadataReferences"></param>
        private static void ReferenceSystemAssembly(AdvancedCache<string, List<MetadataReference>> metadataReferences)
        {
            ReferenceAssemblyByAppName(metadataReferences, Const.ScriptEngine_AssemblyReferenceConfig_SystemAssemblyKey);
        }

        private static void ReferenceExternalAssembly(AdvancedCache<string, List<MetadataReference>> metadataReferences)
        {
            ReferenceAssemblyByAppName(metadataReferences, AppSettingsConfigHelper.GetAppName());
        }

        /// <summary>
        /// 引用第三方包，配置文件中配置引用key=对应服务的应用名
        /// </summary>
        /// <param name="metadataReferences"></param>
        /// <param name="appName"></param>
        private static void ReferenceAssemblyByAppName(AdvancedCache<string, List<MetadataReference>> metadataReferences, string appName)
        {
            //这里配置的程序集引用要写文件全名，包括路径和后缀
            var assemblyInfos = AssemblyReferenceConfig.GetAssemblyInfoByAppName(appName);

            //为不为空都把默认执行路径加上扫描
            var dirs = SettingsConfigHelper.GetReferenceDirs();
            if (dirs == null || !dirs.Any())
                dirs = new List<string>();

            dirs.Insert(0, AppContext.BaseDirectory);

            if (assemblyInfos != null && assemblyInfos.Any())
            {
                foreach (var assemblyInfo in assemblyInfos)
                {
                    foreach (var dir in dirs)
                    {
                        var fileFullPath = Path.Combine(dir, assemblyInfo.Assembly);
                        if (!File.Exists(fileFullPath))
                        {
                            _logger.Debug($"reference file [{fileFullPath}] in config not found.");
                            continue;
                        }
                        //如果文件存在，则加载完成后跳过后续扫描
                        metadataReferences[AppSettingsConfigHelper.GetAppName()].Add(MetadataReference.CreateFromFile(fileFullPath));
                        break;
                    }
                }
            }
        }
    }
}
