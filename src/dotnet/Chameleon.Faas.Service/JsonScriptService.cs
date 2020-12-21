using Bamboo.ScriptEngine;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Chameleon.Faas.CSharp.Api.Services
{
    public interface IJsonScriptService
    {
        /// <summary>
        /// 校验脚本
        /// </summary>
        /// <param name="scriptId"></param>
        ExecutionResult Check(Guid scriptId);
        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="scripeId"></param>
        /// <returns></returns>
        ExecutionResult<object> Run(Guid scripeId);
    }

    public class JsonScriptService : IJsonScriptService
    {
        private readonly IScriptEngine _scriptEngine;
        private readonly ILogger _logger;

        public JsonScriptService(IScriptEngine scriptEngine, ILogger logger)
        {
            _scriptEngine = scriptEngine;
            _logger = logger;
        }

        public ExecutionResult Check(Guid scriptId)
        {
            //脚本的信息从库里获取
            var json = "";

            try
            {
                JObject.Parse(json);
                return ExecutionResult.Success();
            }
            catch (Exception ex)
            {
                return ExecutionResult.Error(ex.Message);
            }
        }

        public ExecutionResult<object> Run(Guid scripeId)
        {
            //脚本的信息从库里获取
            var json = "";

            try
            {
                //先尝试获取对象
                return ExecutionResult<object>.Success(data: JObject.Parse(json));
            }
            catch
            {
                try
                {
                    //降级尝试获取数组
                    return ExecutionResult<object>.Success(data: JArray.Parse(json));
                }
                catch (Exception ex)
                {
                    return ExecutionResult<object>.Error(ex.Message);
                }
            }
        }
    }
}
