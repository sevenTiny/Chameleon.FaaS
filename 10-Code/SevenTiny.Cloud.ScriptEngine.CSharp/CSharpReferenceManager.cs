using Microsoft.CodeAnalysis;
using SevenTiny.Bantina.Logging;
using SevenTiny.Cloud.ScriptEngine.Configs;
using SevenTiny.Cloud.ScriptEngine.CSharp.Configs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SevenTiny.Cloud.ScriptEngine.CSharp.RefrenceManager
{
    internal static class CSharpReferenceManager
    {
        private static readonly ILog _logger = new LogManager();

        private static string _currentAppName => AppSettingsConfigHelper.GetCurrentAppName();

        private static ConcurrentDictionary<string, List<MetadataReference>> _metadataReferences = new ConcurrentDictionary<string, List<MetadataReference>>();

        public static IDictionary<string, List<MetadataReference>> GetMetaDataReferences() => _metadataReferences;

        /// <summary>
        /// 初始化元数据引用
        /// </summary>
        public static void InitMetadataReferences()
        {
            _metadataReferences.TryAdd(_currentAppName, new List<MetadataReference>());

            ReferenceNecessaryAssembly();
            ReferenceAssembly();

            var referenceArrayJson = Newtonsoft.Json.JsonConvert.SerializeObject(_metadataReferences[_currentAppName]?.Select(t => t.Display)?.ToArray());
            _logger.Debug($"dll load finished,load detail：{referenceArrayJson}");
        }

        private static void ReferenceNecessaryAssembly()
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
            _metadataReferences[_currentAppName].AddRange(references);
        }

        /// <summary>
        /// 引用第三方包，配置文件中配置引用key=对应服务的应用名
        /// </summary>
        private static void ReferenceAssembly()
        {
            //这里配置的程序集引用要写文件全名，包括路径和后缀
            var assemblyInfos = AssemblyReferenceConfig.GetCurrentAppAssemblyReferenceInfos();

            //为不为空都把默认执行路径加上扫描
            var dirs = FaaSSettingsConfigHelper.GetReferenceDirs();

            if (dirs == null)
                dirs = new List<string>();

            dirs.Insert(0, AppContext.BaseDirectory);

            _logger.Debug($"scan config dirs: [{string.Join(",", dirs)}].");

            if (assemblyInfos != null && assemblyInfos.Any())
            {
                foreach (var assemblyInfo in assemblyInfos)
                {
                    foreach (var dir in dirs)
                    {
                        var fileFullPath = Path.Combine(dir, assemblyInfo.Assembly);
                        if (!File.Exists(fileFullPath))
                        {
                            continue;
                        }
                        //如果文件存在，则加载完成后跳过后续扫描
                        _metadataReferences[_currentAppName].Add(MetadataReference.CreateFromFile(fileFullPath));
                        break;
                    }
                }
            }
        }
    }
}
