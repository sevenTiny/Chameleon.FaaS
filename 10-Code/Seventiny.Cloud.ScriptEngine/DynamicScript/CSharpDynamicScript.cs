using Fasterflect;
using Microsoft.CodeAnalysis;
using Seventiny.Cloud.ScriptEngine.Toolkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Seventiny.Cloud.ScriptEngine.DynamicScript
{
    public class CSharpDynamicScript : IDynamicScript
    {
        #region Fields
        private string _scriptHash;
        private static object _lock = new object();
        private static AdvancedCache<string, Type> _scriptTypeDict = new AdvancedCache<string, Type>();
        private static AdvancedCache<string, List<MetadataReference>> _metadataReferences = new AdvancedCache<string, List<MetadataReference>>();
        #endregion

        #region Properties

        public static string CloudAppName
        {
            get
            {
                string cloudAppName = ConfigurationManager.AppSettings[DynamicScriptConst.BeisenCloudAppName];
                return string.IsNullOrEmpty(cloudAppName) ? DynamicScriptConst.Common : cloudAppName;
            }
        }

        #endregion

        static CSharpDynamicScript()
        {
            AssemblyResolver.Instance.Init();
            InitMetadataReference();
        }

        private static void InitMetadataReference()
        {
            _metadataReferences.Insert(CloudAppName, new List<MetadataReference>(),
                DynamicScriptEngineSettings.Instance.CachePermanent ? CacheStrategy.Permanent : CacheStrategy.Temporary);
            ReferenceNecessaryAssembly();
            ReferenceSystemAssembly();
            ReferenceExternalAssembly();
        }

        public T CallFunction<T>(string functionName, params object[] parameters)
        {
            ArgumentChecker.NotNullOrEmpty(functionName, nameof(functionName));
            if (_scriptHash != null && _scriptTypeDict.ContainsKey(_scriptHash))
            {
                var type = _scriptTypeDict[_scriptHash];
                return ExecuteTrustedCode<T>(type, functionName, parameters);
            }
            return default(T);
        }

        private T ExecuteTrustedCode<T>(Type type, string functionName, object[] parameters)
        {
            var obj = Activator.CreateInstance(type);
            try
            {
                var result = obj.TryCallMethodWithValues(functionName, parameters);
                return (T)result;
            }
            catch (MissingMethodException missingMethod)
            {
                //_logger.Error(
                //    new ScriptFunctionNotFoundException(
                //        String.Format("TenantId:{0},FunctionName:{1},Language:{2},AppName:{3},ScriptHash:{4},ParameterCount:{5},ErrorMsg: {6}",
                //             _tenantId, functionName, Language, _applicationName, _scriptHash, parameters.Length, missingMethod.Message)));
                return default(T);
            }
        }


        #region Private Method

        private static void ReferenceNecessaryAssembly()
        {
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            var references = new[]
            {
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, Const.RefMscorlib)),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, Const.RefSystem)),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, Const.RefSystemCore)),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, Const.RefMicrosoftCSharp)),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, Const.RefCollections)),
                MetadataReference.CreateFromAssembly(typeof (object).Assembly),
                MetadataReference.CreateFromAssembly(typeof (System.Xml.XmlReader).Assembly),
                MetadataReference.CreateFromAssembly(typeof (System.Web.HttpRuntime).Assembly),
                MetadataReference.CreateFromAssembly(typeof (System.Net.HttpWebRequest).Assembly),
                MetadataReference.CreateFromAssembly(typeof (System.Net.Http.HttpClient).Assembly),
                MetadataReference.CreateFromAssembly(typeof (Enumerable).Assembly),
                MetadataReference.CreateFromAssembly(typeof (WebGetAttribute).Assembly),
                MetadataReference.CreateFromAssembly(typeof (OperationContractAttribute).Assembly),
                MetadataReference.CreateFromAssembly(typeof (InternalContext).Assembly),
                MetadataReference.CreateFromAssembly(typeof (LogWrapper).Assembly),
                MetadataReference.CreateFromAssembly(typeof (MethodDynamicScriptAction).Assembly),
                MetadataReference.CreateFromAssembly(typeof (CsCuttingEdgeDynamicScript).Assembly),
                MetadataReference.CreateFromAssembly(typeof (System.Runtime.Serialization.DataContractSerializer).Assembly),
            };
            _metadataReferences[CloudAppName].AddRange(references);
        }

        private static void ReferenceSystemAssembly()
        {
            var applications = AppFrameworkConfig.Instance.Applications.FirstOrDefault(t => t.ApplicationName.Equals(CloudAppName, StringComparison.OrdinalIgnoreCase));
            if (applications != null && applications.ReferenceSystemAssemblies != null)
            {
                var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
                foreach (var assembly in applications.ReferenceSystemAssemblies)
                {
                    _logger.Debug("Reference the system assembly : " + assembly.Name);
                    _metadataReferences[CloudAppName].Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, assembly.Name + ".dll")));
                }
            }
        }

        private static void ReferenceExternalAssembly()
        {
            var assembliesPath = GetAssemblies(new string[] { BaseLibPath, AppLibPath, });
            foreach (var assemblyPath in assembliesPath)
            {
                string assemblyName = string.Empty;
                if (IsManagedAssembly(assemblyPath, out assemblyName) &&
                    !_metadataReferences[CloudAppName].Exists(f => f.Display.EndsWith(assemblyName + ".dll")))
                {
                    _metadataReferences[CloudAppName].Add(MetadataReference.CreateFromFile(assemblyPath));
                }
            }
        }


        #endregion
    }
}
