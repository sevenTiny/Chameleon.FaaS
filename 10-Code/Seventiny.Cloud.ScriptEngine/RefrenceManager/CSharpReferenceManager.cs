using Microsoft.CodeAnalysis;
using Seventiny.Cloud.ScriptEngine.Configs;
using Seventiny.Cloud.ScriptEngine.Toolkit;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Seventiny.Cloud.ScriptEngine.RefrenceManager
{
    internal class CSharpReferenceManager
    {
        public static void InitMetadataReferences(AdvancedCache<string, List<MetadataReference>> metadataReferences)
        {
            if (!metadataReferences.ContainsKey(AppSettingsConfigHelper.GetAppName()))
                metadataReferences.Insert(AppSettingsConfigHelper.GetAppName(), new List<MetadataReference>(), CacheStrategy.Permanent);

            ReferenceNecessaryAssembly(metadataReferences);
            ReferenceSystemAssembly(metadataReferences);
            ReferenceExternalAssembly(metadataReferences);
        }

        private static void ReferenceNecessaryAssembly(AdvancedCache<string, List<MetadataReference>> metadataReferences)
        {
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            var references = new[]
            {
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, Const.RefMscorlib)),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, Const.RefSystem)),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, Const.RefSystemCore)),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, Const.RefMicrosoftCSharp)),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, Const.RefCollections)),
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
            var systemAssemblyNames = AssemblyReferenceConfig.GetByAppName(appName);
            if (systemAssemblyNames != null && systemAssemblyNames.Any())
            {
                var assemblyPath = Path.GetDirectoryName(Const.ScriptEngine_AssemblyReferenceConfig_ReferenceDllDirectoryName);
                foreach (var assemblyName in systemAssemblyNames)
                {
                    if (!Directory.Exists(assemblyName))
                        throw new DirectoryNotFoundException($"directory [{assemblyPath}] not found.");

                    string filePath = Path.Combine(assemblyPath, assemblyName + ".dll");

                    if (!File.Exists(filePath))
                        throw new FileNotFoundException($"reference file [{filePath}] in config not found in directory");

                    metadataReferences[AppSettingsConfigHelper.GetAppName()].Add(MetadataReference.CreateFromFile(filePath));
                }
            }
        }
    }
}
