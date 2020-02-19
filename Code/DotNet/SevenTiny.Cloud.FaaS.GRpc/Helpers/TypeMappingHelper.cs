using Google.Protobuf;
using System;
using System.Text.Json;

namespace SevenTiny.Cloud.FaaS.GRpc.Helpers
{
    internal static class TypeMappingHelper
    {
        public static ScriptEngine.DynamicScriptLanguage ToScriptEngineDynamicScriptLanguage(this DynamicScriptLanguage dynamicScriptLanguage)
        {
            switch (dynamicScriptLanguage)
            {
                case DynamicScriptLanguage.Csharp:
                    return ScriptEngine.DynamicScriptLanguage.Csharp;
                default:
                    throw new InvalidCastException("no mapping item with type DynamicScriptLanguage");
            }
        }

        public static ScriptEngine.DynamicScript ToScriptEngineDynamicScript(this DynamicScript dynamicScript)
        {
            return new ScriptEngine.DynamicScript
            {
                ClassFullName = dynamicScript.ClassFullName,
                FunctionName = dynamicScript.FunctionName,
                IsExecutionStatistics = dynamicScript.IsExecutionStatistics,
                Script = dynamicScript.Script,
                IsTrustedScript = dynamicScript.IsTrustedScript,
                Language = dynamicScript.Language.ToScriptEngineDynamicScriptLanguage(),
                MillisecondsTimeout = dynamicScript.MillisecondsTimeout,
                Parameters = Newtonsoft.Json.JsonConvert.DeserializeObject<object[]>(dynamicScript.Parameters),
                TenantId = dynamicScript.TenantId
            };
        }

        public static DynamicScriptExecuteResult ToGRpcDynamicScriptExecuteResult(this ScriptEngine.DynamicScriptExecuteResult<object> executeResult)
        {
            return new DynamicScriptExecuteResult
            {
                IsSuccess = executeResult.IsSuccess,
                Message = executeResult.Message ?? string.Empty,
                Data = executeResult.Data == null ? string.Empty : Newtonsoft.Json.JsonConvert.SerializeObject(executeResult.Data),
                ProcessorTime = executeResult.ProcessorTime,
                TotalMemoryAllocated = executeResult.TotalMemoryAllocated
            };
        }

        public static DynamicScriptExecuteResult ToGRpcDynamicScriptExecuteResult(this ScriptEngine.DynamicScriptExecuteResult executeResult)
        {
            return new DynamicScriptExecuteResult
            {
                IsSuccess = executeResult.IsSuccess,
                Message = executeResult.Message,
            };
        }
    }
}
