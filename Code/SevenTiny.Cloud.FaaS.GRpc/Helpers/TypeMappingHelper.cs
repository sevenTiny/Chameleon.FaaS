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
                    return ScriptEngine.DynamicScriptLanguage.CSharp;
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
                Parameters = JsonSerializer.Deserialize<object[]>(dynamicScript.Parameters.Span),
                TenantId = dynamicScript.TenantId
            };
        }

        public static DynamicScriptExecuteResult ToGRpcDynamicScriptExecuteResult(this ScriptEngine.DynamicScriptExecuteResult<object> executeResult)
        {
            return new DynamicScriptExecuteResult
            {
                IsSuccess = executeResult.IsSuccess,
                Message = executeResult.Message,
                Data = ByteString.CopyFrom(JsonSerializer.SerializeToUtf8Bytes(executeResult.Data)),
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
