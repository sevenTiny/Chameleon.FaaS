using Fasterflect;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using SevenTiny.Cloud.ScriptEngine.Configs;
using SevenTiny.Cloud.ScriptEngine.RefrenceManager;
using SevenTiny.Cloud.ScriptEngine.Toolkit;
using SevenTiny.Bantina;
using SevenTiny.Bantina.Logging;
using SevenTiny.Bantina.Security;
using SevenTiny.Bantina.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SevenTiny.Cloud.ScriptEngine.DynamicScriptEngine
{
    /// <summary>
    ///  .NET Compiler Platform ("Roslyn")
    /// </summary>
    internal class CSharpDynamicScriptEngine : IDynamicScriptEngine
    {
        private int _tenantId;
        private string _scriptHash;
        private string _projectName;
        private readonly string _path;
        private static object _lock = new object();
        private static AdvancedCache<string, Type> _scriptTypeDict = new AdvancedCache<string, Type>();
        private static AdvancedCache<string, List<MetadataReference>> _metadataReferences = new AdvancedCache<string, List<MetadataReference>>();
        private readonly ILog _logger = new LogManager();

        static CSharpDynamicScriptEngine()
        {
            //监控项目路径
            AssemblyResolver.Instance.Init();
            //初始化引用
            CSharpReferenceManager.InitMetadataReferences(_metadataReferences);
        }

        public CSharpDynamicScriptEngine()
        {
            _projectName = Const.DefaultProjectName;
            _path = Path.Combine(AppContext.BaseDirectory, Const.DefaultOutPutDllPath);
        }

        private void ArgumentCheckSet(DynamicScript dynamicScript)
        {
            _tenantId = dynamicScript.TenantId;
            dynamicScript.Script.CheckNullOrEmpty("script can not be null");
            dynamicScript.FunctionName.CheckNullOrEmpty("function name can not be null");
            if (!string.IsNullOrEmpty(dynamicScript.ProjectName))
                _projectName = dynamicScript.ProjectName;
        }

        public Result<T> Run<T>(DynamicScript dynamicScript)
        {
            ArgumentCheckSet(dynamicScript);
            return RunningDynamicScript<T>(dynamicScript);
        }

        public Result CheckScript(DynamicScript dynamicScript)
        {
            dynamicScript.FunctionName = "_Function";
            ArgumentCheckSet(dynamicScript);
            return BuildDynamicScript(dynamicScript.Script, out string errorMsg) ? Result.Success() : Result.Error(errorMsg);
        }

        private Result<T> RunningDynamicScript<T>(DynamicScript dynamicScript)
        {
            var dynamicScriptResult = BuildDynamicScript(dynamicScript.Script, out string errorMessage);
            if (!dynamicScriptResult)
            {
                _logger.Error($"Build Script Error ! Script Info:{JsonConvert.SerializeObject(dynamicScript)}");
                return Result<T>.Error(errorMessage);
            }

            try
            {
                var scriptResult = CallFunction<T>(dynamicScript.FunctionName, dynamicScript.Parameters);
                return Result<T>.Success(data: scriptResult);
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message + ",innerEx:" + ex.InnerException?.Message;
                string errorMsgContext = string.Format("Script objectId:{0},tenantId:{1},appName:{2},functionName:{3},errorMsg:{4}", null, dynamicScript.TenantId, dynamicScript.ProjectName, dynamicScript.FunctionName, ex.Message);
                _logger.Error(errorMsgContext, ex);
                return Result<T>.Error(errorMsg);
            }
        }

        private bool BuildDynamicScript(string dynamicScript, out string errorMessage)
        {
            //deal with script to runable script
            string script = GetScriptForRun(dynamicScript);
            //get script hash
            _scriptHash = GetScriptKeyHash(script);
            return BuildDynamicScriptAndCreateType(script, out errorMessage);
        }

        private string GetScriptForRun(string script)
        {
            var commonUsingTag = "[_common_using_]";
            var usingTag = "[_using_]";
            var namespacesTag = "[_namespaces_]";
            var tenantIdTag = "[_tenantId_]";
            var commonCodeTag = "[_common_code_]";
            var codeTag = "[_code_]";

            string usingNameSpaces = string.Empty;
            string code = script;

            if (script.Contains(Const.EndUsing))
            {
                string[] scriptArray = Regex.Split(script, Const.EndUsing, RegexOptions.IgnoreCase);
                usingNameSpaces = scriptArray[0];
                code = scriptArray[1];
            }

            return Const.CsharpScriptTemplate
                .Replace(commonUsingTag, Const.CSharpCommonUsing)
                .Replace(usingTag, usingNameSpaces)
                .Replace(namespacesTag, string.Empty)
                .Replace(tenantIdTag, _tenantId.ToString())
                .Replace(commonCodeTag, Const.CSharpCommonCode)
                .Replace(codeTag, code)
                .ClearScript()
                .RemoveEmptyLines();
        }

        private string GetScriptKeyHash(string script)
        {
            return String.Format(Const.AssemblyScriptKey, DynamicScriptLanguage.CSharp, _tenantId, _projectName, MD5Helper.GetMd5Hash(script));
        }

        private bool BuildDynamicScriptAndCreateType(string script, out string errorMsg)
        {
            string typeName = String.Format(Const.MethodTypeName, _tenantId);
            errorMsg = string.Empty;
            try
            {
                if (_scriptTypeDict.ContainsKey(_scriptHash))
                {
                    return true;
                }

                lock (_lock)
                {
                    if (_scriptTypeDict.ContainsKey(_scriptHash))
                        return true;

                    var asm = CreateAsmExecutor(script, out errorMsg);
                    if (asm != null)
                    {
                        if (!_scriptTypeDict.ContainsKey(_scriptHash))
                        {
                            var type = asm.GetType(typeName);
                            _scriptTypeDict.Insert(_scriptHash, type, CacheStrategy.Permanent);
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                errorMsg = ex.ToString();
                _logger.Error(ex);
                return false;
            }
        }

        #region Create Assembly and output files
        /// <summary>
        /// Create Assembly whick will run
        /// </summary>
        /// <param name="script"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private Assembly CreateAsmExecutor(string script, out string errorMsg)
        {
            errorMsg = null;
            var assemblyName = _scriptHash;

            var sourceTree = CSharpSyntaxTree.ParseText(script, path: assemblyName + ".cs", encoding: Encoding.UTF8);

            var references = _metadataReferences[AppSettingsConfigHelper.GetAppName()];

            var compilation = CSharpCompilation.Create(assemblyName,
                new[] { sourceTree }, references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithOptimizationLevel(SettingsConfigHelper.IsDebug() ? OptimizationLevel.Debug : OptimizationLevel.Release));

            Assembly assembly;
            using (var assemblyStream = new MemoryStream())
            {
                using (var pdbStream = new MemoryStream())
                {
                    var emitResult = compilation.Emit(assemblyStream, pdbStream);

                    if (emitResult.Success)
                    {
                        var assemblyBytes = assemblyStream.GetBuffer();
                        var pdbBytes = pdbStream.GetBuffer();
                        assembly = Assembly.Load(assemblyBytes, pdbBytes);
                        //output files
                        if (SettingsConfigHelper.IsOutPutFiles())
                        {
                            if (SettingsConfigHelper.IsOutPutAllFiles())
                                OutputDynamicScriptAllFile(script, assemblyName, assemblyBytes, pdbBytes);
                            else
                                OutputDynamicScriptDllFile(assemblyName, assemblyBytes);
                        }
                    }
                    else
                    {
                        var msgs = new StringBuilder();
                        foreach (var msg in emitResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => string.Format("[{0}]:{1}({2})", d.Id, d.GetMessage(), d.Location.GetLineSpan().StartLinePosition)))
                        {
                            msgs.AppendLine(msg);
                            if (SettingsConfigHelper.IsOutPutFiles())
                                WriteDynamicScriptCs(Path.Combine(EnsureOutputPath(), assemblyName + ".cs"), script);
                            _logger.Error(String.Format("{0}：{1}：{2}：{3}：{4}", _tenantId, "CSharp", _projectName, msg, _scriptHash));
                        }
                        errorMsg = msgs.ToString();
                        return null;
                    }
                }
            }
            _logger.Debug($"CreateAsmExecutor->_context:{_tenantId},{"CSharp"}, {_projectName},{_scriptHash}   _scriptTypeDict:{_scriptTypeDict?.Count}  _metadataReferences:{ _metadataReferences[_projectName]?.Count}");
            return assembly;
        }

        private void OutputDynamicScriptDllFile(string assemblyName, byte[] assemblyBytes)
        {
            string path = EnsureOutputPath();
            WriteDynamicScriptFile(Path.Combine(path, assemblyName + ".dll"), assemblyBytes);
        }
        private void OutputDynamicScriptAllFile(string script, string assemblyName, byte[] assemblyBytes, byte[] pdbBytes)
        {
            string path = EnsureOutputPath();
            WriteDynamicScriptFile(Path.Combine(path, assemblyName + ".dll"), assemblyBytes);
            WriteDynamicScriptFile(Path.Combine(path, assemblyName + ".pdb"), pdbBytes);
            WriteDynamicScriptCs(Path.Combine(path, assemblyName + ".cs"), script);
        }
        private string EnsureOutputPath()
        {
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
            return _path;
        }
        private void WriteDynamicScriptFile(string filePathName, byte[] bytes)
        {
            try
            {
                if (!File.Exists(filePathName))
                    File.WriteAllBytes(filePathName, bytes);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
        private void WriteDynamicScriptCs(string filePathName, string script)
        {
            try
            {
                if (!File.Exists(filePathName))
                    File.WriteAllText(filePathName, script, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
        #endregion

        private T ExecuteTrustedCode<T>(Type type, string functionName, object[] parameters)
        {
            var obj = Activator.CreateInstance(type);
            try
            {
                var parms = type.GetMethod(functionName).GetParameters();
                var result = obj.TryCallMethod(functionName, true, parms.Select(t => t.Name).ToArray(), parms.Select(t => t.ParameterType).ToArray(), parameters);
                return (T)result;
            }
            catch (MissingMethodException missingMethod)
            {
                _logger.Error(
                    new MissingMethodException(
                        String.Format("TenantId:{0},FunctionName:{1},Language:{2},AppName:{3},ScriptHash:{4},ParameterCount:{5},ErrorMsg: {6}",
                             _tenantId, functionName, "CSharp", _projectName, _scriptHash, parameters.Length, missingMethod.Message)));
                return default(T);
            }
        }
        /// <summary>
        /// Call script function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="functionName">name of function</param>
        /// <param name="parameters">parameters of function</param>
        /// <returns></returns>
        private T CallFunction<T>(string functionName, params object[] parameters)
        {
            ArgumentChecker.NotNullOrEmpty(functionName, nameof(functionName));
            if (!string.IsNullOrEmpty(_scriptHash) && _scriptTypeDict.ContainsKey(_scriptHash))
            {
                var type = _scriptTypeDict[_scriptHash];
                //Note:.NET Core 3.0 Preview 5 start support
                //暂时不支持沙箱环境
                //if (SettingsConfig.Instance.SandboxEnable)
                //{
                //    object obj = null;
                //    var sandBoxer = new SandBoxer();
                //    obj = sandBoxer.ExecuteUntrustedCode(type, functionName, 0, parameters);
                //    sandBoxer.UnloadSandBoxer();
                //    return (T)obj;
                //}
                return ExecuteTrustedCode<T>(type, functionName, parameters);
            }
            return default(T);
        }
    }
}
