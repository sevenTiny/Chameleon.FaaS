using Fasterflect;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using SevenTiny.Bantina;
using SevenTiny.Bantina.Logging;
using SevenTiny.Bantina.Security;
using SevenTiny.Bantina.Validation;
using SevenTiny.Cloud.ScriptEngine.Configs;
using SevenTiny.Cloud.ScriptEngine.CSharp;
using SevenTiny.Cloud.ScriptEngine.CSharp.RefrenceManager;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SevenTiny.Cloud.ScriptEngine.CSharp
{
    /// <summary>
    ///  .NET Compiler Platform ("Roslyn")
    /// </summary>
    public class CSharpDynamicScriptEngine : IDynamicScriptEngine
    {
        private int _tenantId;
        private string _scriptHash;
        private readonly string _path;
        private static readonly object _lock = new object();
        private static IDictionary<string, Type> _scriptTypeDict = new ConcurrentDictionary<string, Type>();
        private static readonly ILog _logger = new LogManager();
        private readonly static string _currentAppName = AppSettingsConfigHelper.GetCurrentAppName();

        static CSharpDynamicScriptEngine()
        {
            //初始化引用
            CSharpReferenceManager.InitMetadataReferences();
        }

        public CSharpDynamicScriptEngine()
        {
            _path = Path.Combine(AppContext.BaseDirectory, Consts.DefaultOutPutDllPath);
        }

        public DynamicScriptExecuteResult<T> Run<T>(DynamicScript dynamicScript)
        {
            DynamicScriptPropertiesCheck(dynamicScript);
            return RunningDynamicScript<T>(dynamicScript);
        }

        public DynamicScriptExecuteResult CheckScript(DynamicScript dynamicScript)
        {
            DynamicScriptPropertiesCheck(dynamicScript);
            return BuildDynamicScript(dynamicScript, out string errorMsg) ? DynamicScriptExecuteResult.Success() : DynamicScriptExecuteResult.Error(errorMsg);
        }

        private void DynamicScriptPropertiesCheck(DynamicScript dynamicScript)
        {
            _tenantId = dynamicScript.TenantId;
            dynamicScript.Script.CheckNullOrEmpty("script can not be null.");
            dynamicScript.ClassFullName.CheckNullOrEmpty("classFullName cannot be null.");
            dynamicScript.FunctionName.CheckNullOrEmpty("FunctionName can not be null.");
        }

        private DynamicScriptExecuteResult<T> RunningDynamicScript<T>(DynamicScript dynamicScript)
        {
            //检查编译
            if (!BuildDynamicScript(dynamicScript, out string errorMessage))
            {
                _logger.Error($"Build Script Error ! Script Info:{JsonConvert.SerializeObject(dynamicScript)}");
                return DynamicScriptExecuteResult<T>.Error(errorMessage);
            }

            try
            {
                //是否开启执行分析,统计非常耗时且会带来更多GC开销，正常运行过程请关闭！
                if (dynamicScript.ExecutionStatistics)
                {
                    Stopwatch stopwatch = new Stopwatch();  //程序执行时间
                    var startMemory = GC.GetTotalMemory(true);  //方法调用内存占用
                    stopwatch.Start();

                    var result = CallFunction<T>(dynamicScript.FunctionName, dynamicScript.Parameters);

                    stopwatch.Stop();
                    result.TotalMemoryAllocated = GC.GetTotalMemory(true) - startMemory;
                    result.ProcessorTime = stopwatch.Elapsed;
                    return result;
                }

                return CallFunction<T>(dynamicScript.FunctionName, dynamicScript.Parameters);
            }
            catch (MissingMethodException missingMethod)
            {
                _logger.Error(
                    new MissingMethodException(
                        String.Format("TenantId:{0},FunctionName:{1},Language:{2},AppName:{3},ScriptHash:{4},ParameterCount:{5},ErrorMsg: {6}",
                             _tenantId, dynamicScript.FunctionName, "CSharp", _currentAppName, _scriptHash, dynamicScript.Parameters?.Length, missingMethod.Message)));
                return DynamicScriptExecuteResult<T>.Error($"function name can not be null.");
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message + ",innerEx:" + ex.InnerException?.Message;
                string errorMsgContext = string.Format("Script objectId:{0},tenantId:{1},appName:{2},functionName:{3},errorMsg:{4}", null, dynamicScript.TenantId, dynamicScript.AppName, dynamicScript.FunctionName, ex.Message);
                _logger.Error(errorMsgContext, ex);
                return DynamicScriptExecuteResult<T>.Error(errorMsg);
            }
        }

        private bool BuildDynamicScript(DynamicScript dynamicScript, out string errorMessage)
        {
            _scriptHash = GetScriptKeyHash(dynamicScript.Script);

            errorMessage = string.Empty;
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

                    var asm = CreateAsmExecutor(dynamicScript.Script, out errorMessage);
                    if (asm != null)
                    {
                        if (!_scriptTypeDict.ContainsKey(_scriptHash))
                        {
                            var type = asm.GetType(dynamicScript.ClassFullName);
                            if (type == null)
                            {
                                errorMessage = $"type [{dynamicScript.ClassFullName}] not found in the assembly [{asm.FullName}].";
                                return false;
                            }
                            _scriptTypeDict.Add(_scriptHash, type);
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.ToString();
                _logger.Error(ex);
                return false;
            }
        }

        private string GetScriptKeyHash(string script)
        {
            return String.Format(Consts.AssemblyScriptKey, DynamicScriptLanguage.CSharp, _tenantId, _currentAppName, MD5Helper.GetMd5Hash(script));
        }

        #region Build and Create Assembly and output files
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

            var references = CSharpReferenceManager.GetMetaDataReferences()[_currentAppName];

            var compilation = CSharpCompilation.Create(assemblyName,
                new[] { sourceTree }, references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithOptimizationLevel(FaaSSettingsConfigHelper.IsDebugMode() ? OptimizationLevel.Debug : OptimizationLevel.Release));

            Assembly assembly = null;
            using (var assemblyStream = new MemoryStream())
            {
                using (var pdbStream = new MemoryStream())
                {
                    var emitDynamicScriptExecuteResult = compilation.Emit(assemblyStream, pdbStream);

                    if (emitDynamicScriptExecuteResult.Success)
                    {
                        var assemblyBytes = assemblyStream.GetBuffer();
                        var pdbBytes = pdbStream.GetBuffer();
                        assembly = Assembly.Load(assemblyBytes, pdbBytes);
                        //output files
                        if (FaaSSettingsConfigHelper.IsOutPutFiles())
                            OutputDynamicScriptAllFile(script, assemblyName, assemblyBytes, pdbBytes);
                    }
                    else
                    {
                        var msgs = new StringBuilder();
                        foreach (var msg in emitDynamicScriptExecuteResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => string.Format("[{0}]:{1}({2})", d.Id, d.GetMessage(), d.Location.GetLineSpan().StartLinePosition)))
                            msgs.AppendLine(msg);

                        if (FaaSSettingsConfigHelper.IsOutPutFiles())
                            WriteDynamicScriptCs(Path.Combine(EnsureOutputPath(), assemblyName + ".cs"), script);

                        errorMsg = msgs.ToString();
                        _logger.Error(String.Format("{0}：{1}：{2}：{3}：{4}", _tenantId, "CSharp", _currentAppName, errorMsg, _scriptHash));
                    }
                }
            }
            _logger.Debug($"CreateAsmExecutor -> _context:{_tenantId},{"CSharp"}, {_currentAppName},{_scriptHash} _scriptTypeDict:{_scriptTypeDict?.Count} _metadataReferences:{ CSharpReferenceManager.GetMetaDataReferences()[_currentAppName]?.Count}");
            return assembly;
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

        #region Call script function
        private DynamicScriptExecuteResult<T> CallFunction<T>(string functionName, params object[] parameters)
        {
            if (functionName.IsNullOrEmpty())
                return DynamicScriptExecuteResult<T>.Error($"function name can not be null.");

            if (_scriptHash.IsNullOrEmpty() || !_scriptTypeDict.ContainsKey(_scriptHash))
                return DynamicScriptExecuteResult<T>.Error($"type not found.");

            var type = _scriptTypeDict[_scriptHash];

            var methodInfo = type.Method(functionName);

            if (methodInfo == null)
                return DynamicScriptExecuteResult<T>.Error($"function name can not be null.");

            if (type.Method(functionName).IsStatic)
            {
                return DynamicScriptExecuteResult<T>.Success(data: ExecuteTrustedCodeStaticMethod<T>(type, methodInfo, functionName, parameters));
            }
            else
            {
                return DynamicScriptExecuteResult<T>.Success(data: ExecuteTrustedCodeMethod<T>(type, methodInfo, functionName, parameters));
            }

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
        }
        private T ExecuteTrustedCodeMethod<T>(Type type, MethodInfo methodInfo, string functionName, object[] parameters)
        {
            var obj = Activator.CreateInstance(type);
            var parms = methodInfo.GetParameters();
            var DynamicScriptExecuteResult = obj.TryCallMethod(functionName, true, parms.Select(t => t.Name).ToArray(), parms.Select(t => t.ParameterType).ToArray(), parameters);
            return (T)DynamicScriptExecuteResult;
        }
        private T ExecuteTrustedCodeStaticMethod<T>(Type type, MethodInfo methodInfo, string functionName, object[] parameters)
        {
            var parms = methodInfo.GetParameters();
            var DynamicScriptExecuteResult = type.TryCallMethod(functionName, true, parms.Select(t => t.Name).ToArray(), parms.Select(t => t.ParameterType).ToArray(), parameters);
            return (T)DynamicScriptExecuteResult;
        }
        #endregion
    }
}
