using Grpc.Core;
using SevenTiny.Bantina.Validation;
using SevenTiny.Cloud.FaaS.GRpc.Helpers;
using System.Threading.Tasks;

namespace SevenTiny.Cloud.FaaS.GRpc
{
    public class DynamicScriptExecutorService : DynamicScriptExecutor.DynamicScriptExecutorBase
    {
        private IDynamicScriptExecuteService _dynamicScriptExecutorService;
        public DynamicScriptExecutorService(IDynamicScriptExecuteService dynamicScriptExecutorService)
        {
            _dynamicScriptExecutorService = dynamicScriptExecutorService;
        }

        private void CheckRequiredArguments(DynamicScript request)
        {
            Ensure.ArgumentNotNullOrEmpty(request.ClassFullName, nameof(request.ClassFullName));
            Ensure.ArgumentNotNullOrEmpty(request.FunctionName, nameof(request.FunctionName));
            Ensure.ArgumentNotNullOrEmpty(request.Script, nameof(request.Script));
        }

        public override Task<DynamicScriptExecuteResult> CheckScript(DynamicScript request, ServerCallContext context)
        {
            try
            {
                CheckRequiredArguments(request);
                return Task.FromResult(_dynamicScriptExecutorService.CheckScript(request.ToScriptEngineDynamicScript()).ToGRpcDynamicScriptExecuteResult());
            }
            catch (System.Exception ex)
            {
                return Task.FromResult(new DynamicScriptExecuteResult { IsSuccess = false, Message = ex.ToString() });
            }
        }

        public override Task<DynamicScriptExecuteResult> Execute(DynamicScript request, ServerCallContext context)
        {
            try
            {
                CheckRequiredArguments(request);
                return Task.FromResult(_dynamicScriptExecutorService.Execute(request.ToScriptEngineDynamicScript()).ToGRpcDynamicScriptExecuteResult());
            }
            catch (System.Exception ex)
            {
                return Task.FromResult(new DynamicScriptExecuteResult { IsSuccess = false, Message = ex.ToString() });
            }
        }
    }
}