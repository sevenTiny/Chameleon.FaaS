using Grpc.Core;
using SevenTiny.Bantina.Logging;
using SevenTiny.Bantina.Validation;
using SevenTiny.Cloud.FaaS.GRpc.Helpers;
using System;
using System.Threading.Tasks;

namespace SevenTiny.Cloud.FaaS.GRpc
{
    public class DynamicScriptExecutorService : DynamicScriptExecutor.DynamicScriptExecutorBase
    {
        private IDynamicScriptExecuteService _dynamicScriptExecutorService;
        private ILog _logger;

        public DynamicScriptExecutorService(IDynamicScriptExecuteService dynamicScriptExecutorService, ILog logger)
        {
            _dynamicScriptExecutorService = dynamicScriptExecutorService;
            _logger = logger;
        }

        private void CheckRequiredArguments(DynamicScript request)
        {
            Ensure.ArgumentNotNullOrEmpty(request.ClassFullName, "ClassFullName");
            Ensure.ArgumentNotNullOrEmpty(request.FunctionName, "FunctionName");
            Ensure.ArgumentNotNullOrEmpty(request.Script, "Script");
            if (request.Language <= 0)
                throw new ArgumentException("Language must be provide");
        }

        public override Task<DynamicScriptExecuteResult> CheckScript(DynamicScript request, ServerCallContext context)
        {
            try
            {
                CheckRequiredArguments(request);
                return Task.FromResult(_dynamicScriptExecutorService.CheckScript(request.ToScriptEngineDynamicScript()).ToGRpcDynamicScriptExecuteResult());
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return Task.FromResult(new DynamicScriptExecuteResult { IsSuccess = false, Message = ex.InnerException?.ToString() ?? ex.ToString() });
            }
        }

        public override Task<DynamicScriptExecuteResult> Execute(DynamicScript request, ServerCallContext context)
        {
            try
            {
                CheckRequiredArguments(request);
                return Task.FromResult(_dynamicScriptExecutorService.Execute(request.ToScriptEngineDynamicScript()).ToGRpcDynamicScriptExecuteResult());
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return Task.FromResult(new DynamicScriptExecuteResult { IsSuccess = false, Message = ex.InnerException?.ToString() ?? ex.ToString() });
            }
        }
    }
}