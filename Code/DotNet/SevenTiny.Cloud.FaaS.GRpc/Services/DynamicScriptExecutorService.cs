using Grpc.Core;
using Microsoft.Extensions.Logging;
using SevenTiny.Bantina.Validation;
using SevenTiny.Cloud.FaaS.GRpc.Helpers;
using SevenTiny.Cloud.ScriptEngine;
using System;
using System.Threading.Tasks;

namespace SevenTiny.Cloud.FaaS.GRpc
{
    public class DynamicScriptExecutorService : DynamicScriptExecutor.DynamicScriptExecutorBase
    {
        private IDynamicScriptEngine _dynamicScriptEngine;
        private ILogger _logger;

        public DynamicScriptExecutorService(IDynamicScriptEngine dynamicScriptEngine, ILogger logger)
        {
            _dynamicScriptEngine = dynamicScriptEngine;
            _logger = logger;
        }

        private void CheckRequiredArguments(DynamicScript request)
        {
            Ensure.ArgumentNotNullOrEmpty(request.ClassFullName, "ClassFullName");
            Ensure.ArgumentNotNullOrEmpty(request.FunctionName, "FunctionName");
            Ensure.ArgumentNotNullOrEmpty(request.Script, "Script");
            if (request.Language <= 0)
                throw new ArgumentException("Language must be provide");
            if (request.Language != DynamicScriptLanguage.Csharp)
                throw new ArgumentException("Language is not csharp, please check your code or language argument.");
        }

        public override Task<DynamicScriptExecuteResult> CheckScript(DynamicScript request, ServerCallContext context)
        {
            try
            {
                CheckRequiredArguments(request);
                return Task.FromResult(_dynamicScriptEngine.CheckScript(request.ToScriptEngineDynamicScript()).ToGRpcDynamicScriptExecuteResult());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CheckScript Error");
                return Task.FromResult(new DynamicScriptExecuteResult { IsSuccess = false, Message = ex.InnerException?.ToString() ?? ex.ToString() });
            }
        }

        public override Task<DynamicScriptExecuteResult> Execute(DynamicScript request, ServerCallContext context)
        {
            try
            {
                CheckRequiredArguments(request);
                return Task.FromResult(_dynamicScriptEngine.Execute<object>(request.ToScriptEngineDynamicScript()).ToGRpcDynamicScriptExecuteResult());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Execute Error");
                return Task.FromResult(new DynamicScriptExecuteResult { IsSuccess = false, Message = ex.InnerException?.ToString() ?? ex.ToString() });
            }
        }
    }
}