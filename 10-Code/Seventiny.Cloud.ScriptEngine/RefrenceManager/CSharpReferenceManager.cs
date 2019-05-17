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
            if (!metadataReferences.ContainsKey(AppSettingsConfig.Instance.Config.SevenTinyCloud.AppName))
                metadataReferences.Insert(AppSettingsConfig.Instance.Config.SevenTinyCloud.AppName, new List<MetadataReference>(), ValueTranslator.TrueFalse(ScriptEngine_SettingsConfig.Instance.Config.IsCachePermanent) ? CacheStrategy.Permanent : CacheStrategy.Temporary);

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
            metadataReferences[AppSettingsConfig.Instance.Config.SevenTinyCloud.AppName].AddRange(references);
        }

        private static void ReferenceSystemAssembly(AdvancedCache<string, List<MetadataReference>> metadataReferences)
        {
            ReferenceAssemblyByAppName(metadataReferences, Const.ScriptEngine_AssemblyReferenceConfig_SystemAssemblyKey);
        }

        private static void ReferenceExternalAssembly(AdvancedCache<string, List<MetadataReference>> metadataReferences)
        {
            ReferenceAssemblyByAppName(metadataReferences, AppSettingsConfig.Instance.Config.SevenTinyCloud.AppName);
        }

        private static void ReferenceAssemblyByAppName(AdvancedCache<string, List<MetadataReference>> metadataReferences, string appName)
        {
            var systemAssemblyNames = ScriptEngine_AssemblyReferenceConfig.GetByAppName(appName);
            if (systemAssemblyNames != null && systemAssemblyNames.Any())
            {
                var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
                foreach (var assemblyName in systemAssemblyNames)
                {
                    metadataReferences[AppSettingsConfig.Instance.Config.SevenTinyCloud.AppName].Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, assemblyName + ".dll")));
                }
            }
        }
    }
}
