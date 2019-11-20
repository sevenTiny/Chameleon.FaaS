using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace SevenTiny.Cloud.FaaS.GRpc
{
    public class DynamicScriptExecutorService : DynamicScriptExecutor.DynamicScriptExecutorBase
    {
        public override Task<DynamicScriptExecuteResult> Test(DynamicScript request, ServerCallContext context)
        {
            Console.WriteLine("request:" + request.ClassFullName);

            return Task.FromResult(new DynamicScriptExecuteResult
            {
                IsSuccess = true,
                Message = "this is return value --7tiny,time:" + DateTime.Now
            });
        }
    }
}