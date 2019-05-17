using Fasterflect;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using Seventiny.Cloud.ScriptEngine.Configs;
using Seventiny.Cloud.ScriptEngine.Context;
using Seventiny.Cloud.ScriptEngine.RefrenceManager;
using Seventiny.Cloud.ScriptEngine.Toolkit;
using SevenTiny.Bantina;
using SevenTiny.Bantina.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Seventiny.Cloud.ScriptEngine.DynamicScriptEngine
{
    /// <summary>
    ///  .NET Compiler Platform ("Roslyn")
    /// </summary>
    internal class CSharpDynamicScript : IDynamicScript
    {
        private CSharpDynamicScriptContext _context;
        private int _tenantId;
        private string _projectName;
        private string _scriptHash;
        private string path = string.Empty;
        private static object _lock = new object();
        private static AdvancedCache<string, Type> _scriptTypeDict = new AdvancedCache<string, Type>();
        private static AdvancedCache<string, List<MetadataReference>> _metadataReferences = new AdvancedCache<string, List<MetadataReference>>();

        //CSharpDynamicScript(CSharpDynamicScriptContext context)
        //{
        //    _context = context;
        //}

        static CSharpDynamicScript()
        {
            //监控项目路径
            AssemblyResolver.Instance.Init();
            //初始化引用
            CSharpReferenceManager.InitMetadataReferences(_metadataReferences);
        }

        //public object RunDynamicScript(int businessCode, int tenantId, string applicationName, DynamicScriptAction action,
        //    string type = null, string keyName = null, string keyCode = null, ScriptRange scriptRange = ScriptRange.SysAndCustom,
        //    string functionName = null, params object[] parameters)
        //{
        //    return RunDynamicScriptList(businessCode, tenantId, applicationName, action, type, keyName, keyCode, scriptRange, functionName, parameters);
        //}

        //private object RunDynamicScriptList(int businessCode, int tenantId, string applicationName, DynamicScriptAction action,
        //                                    string type, string keyName, string keyCode, ScriptRange scriptRange, string functionName = null, params object[] parameters)
        //{
        //    //ArgumentHelper.AssertPositive(businessCode, "BusinessCode must greater than 0");
        //    //ArgumentHelper.AssertPositive(tenantId, "TenantID must greater than 0");
        //    //ArgumentHelper.AssertNotEmpty(applicationName, "ApplicationName is null or empty");
        //    //ArgumentHelper.AssertIsTrue(!(type == null && keyName == null && keyCode == null), "type、keyname、keyCode at least one can't be empty");
        //    //AssertBusinessCode(businessCode);

        //    var dynamicScripts = DynamicScriptAccessorProvider.Instance.GetDynamicScriptForRun(businessCode, tenantId, applicationName, action, type, keyName, keyCode, scriptRange);
        //    var scriptKey = ConcatScriptKey(String.Join("|", dynamicScripts.Select(o => o.ObjectID)), businessCode, tenantId, applicationName, action, type, keyName, keyCode);
        //    return RunningDynamicScript(scriptKey, dynamicScripts, functionName, parameters);
        //}

        //这只是一个Test，回头重构掉
        public Result<T> Run<T>(DynamicScript dynamicScript, string functionName, params object[] parameters)
        {
            _tenantId = dynamicScript.TenantId;
            _projectName = dynamicScript.ProjectName;
            return RunningDynamicScript<T>(new List<DynamicScript> { dynamicScript }, functionName, parameters);
        }


        private Result<T> RunningDynamicScript<T>(List<DynamicScript> dynamicScripts, string functionName = null, params object[] parameters)
        {
            T scriptResult = default(T);
            Result<T> runResult = Result<T>.Success();
            foreach (var script in dynamicScripts)
            {
                runResult = RunningDynamicScript<T>(script, functionName, parameters, ref scriptResult);
                if (!runResult.IsSuccess && script.OnFailureAction == OnFailureAction.Break)
                {
                    break;
                }
            }
            runResult.Data = scriptResult;
            return runResult;
        }

        internal Result<T> RunningDynamicScript<T>(DynamicScript dynamicScript, string functionName, object[] parameters, ref T scriptResult)
        {
            //var watch = new Stopwatch();
            //var beginTime = DateTime.Now;
            //watch.Restart();
            var dynamicScriptResult = CreateInstance(dynamicScript);
            if (!dynamicScriptResult.IsSuccess)
            {
                //logger.Error(new ScriptCompileErrorException(JsonConvert.SerializeObject(dynamicScript)) + "|" + JsonConvert.SerializeObject(parameters));
                return Result<T>.Error(dynamicScriptResult.Message);
            }

            try
            {
                scriptResult = CallFunction<T>(functionName, parameters);
                //watch.Stop();
                //dynamicScriptResult.Dispose();
                //AddScriptTrackerLog(dynamicScript, beginTime, watch.ElapsedMilliseconds);
                return Result<T>.Success(data: scriptResult);
            }
            catch (Exception ex)
            {
                //watch.Stop();
                string errorMsg = ex.Message + ",innerEx:" + (ex.InnerException == null ? "" : ex.InnerException.Message);
                //string errorMsgContext = string.Format("Script objectId:{0},tenantId:{1},appName:{2},functionName:{3},errorMsg:{4}", script.ObjectID, script.TenantId, script.ApplicationName, functionName, ex.Message);
                //AddScriptTrackerLog(script, beginTime, watch.ElapsedMilliseconds, parameters, errorMsg);
                //WriteErrMsgToContext(errorMsgContext);
                //WriteErrToLog(errorMsg, ex);
                return Result<T>.Error(errorMsg);
            }
        }

        private Result CreateInstance(DynamicScript dynamicScript)
        {
            return BuildDynamicScript(dynamicScript);
        }

        private Result BuildDynamicScript(DynamicScript dynamicScript)
        {
            string script = GetScript(dynamicScript.Script);
            _scriptHash = GetScriptKeyHash(script);
            if (!BuildingDynamicScript(script, out string errorMsg))
            {
                return Result.Error(errorMsg);
            }
            return Result.Success();
        }

        private string GetScript(string script)
        {
            var tenantIdTag = "[_tenantId_]";
            var codeTag = "[_code_]";
            var namespacesTag = "[_namespaces_]";

            string usingNameSpaces = string.Empty;
            string code = string.Empty;

            if (script.Contains(Const.EndUsing))
            {
                string[] scriptArray = Regex.Split(script, Const.EndUsing, RegexOptions.IgnoreCase);
                usingNameSpaces = scriptArray[0];
                code = scriptArray[1];
            }
            else
            {
                code = script;
            }

            return Const.CsharpScriptTemplate
                .Replace(tenantIdTag, _tenantId.ToString())
                .Replace(namespacesTag, usingNameSpaces)
                .Replace(codeTag, code)
                .ClearScript()
                .RemoveEmptyLines();
        }

        private string GetScriptKeyHash(string script)
        {
            return String.Format(Const.AssemblyScriptKey, DynamicScriptLanguage.CSharp, _tenantId, _projectName, MD5Helper.GetMd5Hash(script), "");
        }

        private bool BuildingDynamicScript(string script, out string errorMsg)
        {
            string typeName = String.Format(Const.MethodTypeName, _tenantId);
            errorMsg = string.Empty;
            try
            {
                if (_scriptTypeDict.ContainsKey(_scriptHash))
                {
                    //SetContextDataRepository(_scriptTypeDict[_scriptHash]);
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
                            _scriptTypeDict.Insert(_scriptHash, type, ValueTranslator.TrueFalse(ScriptEngine_SettingsConfig.Instance.Config.IsCachePermanent) ? CacheStrategy.Permanent : CacheStrategy.Temporary);
                            //SetContextDataRepository(type);
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                //_logger.Error(ex);
                return false;
            }
        }

        private Assembly CreateAsmExecutor(string script, out string errorMsg)
        {

            errorMsg = null;
            var assemblyName = _scriptHash;

            var sourceTree = CSharpSyntaxTree.ParseText(script, path: assemblyName + ".cs", encoding: Encoding.UTF8);

            var references = _metadataReferences[AppSettingsConfig.Instance.Config.SevenTinyCloud.AppName];

            var compilation = CSharpCompilation.Create(assemblyName,
                new[] { sourceTree }, references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithOptimizationLevel(ValueTranslator.TrueFalse(ScriptEngine_SettingsConfig.Instance.Config.IsDebug) ? OptimizationLevel.Debug : OptimizationLevel.Release));

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
                        if (ValueTranslator.TrueFalse(ScriptEngine_SettingsConfig.Instance.Config.IsOutPutAllFiles))
                            OutputDynamicScriptAllFile(script, assemblyName, assemblyBytes, pdbBytes);
                        else
                            OutputDynamicScriptDllFile(assemblyName, assemblyBytes);
                    }
                    else
                    {
                        var msgs = new StringBuilder();
                        foreach (var msg in emitResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => string.Format("[{0}]:{1}({2})", d.Id, d.GetMessage(), d.Location.GetLineSpan().StartLinePosition)))
                        {
                            msgs.AppendLine(msg);
                            //if (IsOutputDynamicScriptFile())
                            //    WriteDynamicScriptCs(Path.Combine(EnsureOutputPath(true), assemblyName + ".cs"), script);
                            //_logger.Error(new CompileErrorException(String.Format("{0}：{1}：{2}：{3}：{4}", _tenantId, msg, Language, _applicationName, _scriptHash)));
                        }
                        errorMsg = msgs.ToString();
                        return null;
                    }
                }
            }
            //_logger.Debug($"CreateAsmExecutor->_context:{_tenantId},{Language}, {_applicationName},{_scriptHash}   _scriptTypeDict:{_scriptTypeDict.Count}  _metadataReferences:{ _metadataReferences[CloudAppName].Count}");
            return assembly;
        }

        private void OutputDynamicScriptDllFile(string assemblyName, byte[] assemblyBytes)
        {
            //string path = EnsureOutputPath(true);
            WriteDynamicScriptFile(Path.Combine(path, assemblyName + ".dll"), assemblyBytes);
        }

        private void OutputDynamicScriptAllFile(string script, string assemblyName, byte[] assemblyBytes, byte[] pdbBytes)
        {
            //string path = EnsureOutputPath(true);
            WriteDynamicScriptFile(Path.Combine(path, assemblyName + ".dll"), assemblyBytes);
            WriteDynamicScriptFile(Path.Combine(path, assemblyName + ".pdb"), pdbBytes);
            WriteDynamicScriptCs(Path.Combine(path, assemblyName + ".cs"), script);
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
                //_logger.Error(ex);
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
                //_logger.Error(ex);
            }
        }

        private T CallFunction<T>(string functionName, params object[] parameters)
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
    }
}
