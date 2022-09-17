using Bamboo.ScriptEngine;
using Microsoft.Extensions.Logging;
using System;

namespace Chameleon.Faas.CSharp.Service
{
    public interface ICSharpScriptService
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
        /// <summary>
        /// 异步执行脚本
        /// </summary>
        /// <param name="scripeId"></param>
        /// <returns></returns>
        ExecutionResult<object> RunAsync(Guid scripeId);
    }

    public class CSharpScriptService : ICSharpScriptService
    {
        private readonly IScriptEngine _scriptEngine;
        private readonly ILogger _logger;

        public CSharpScriptService(IScriptEngine scriptEngine, ILogger logger)
        {
            _scriptEngine = scriptEngine;
            _logger = logger;
        }

        public ExecutionResult Check(Guid scriptId)
        {
            //脚本的信息从库里获取
            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
            using System;
            using Chameleon.Common.Context;
            using Newtonsoft.Json;

            public class Test
            {
                public string Method()
                {
                    var query = ChameleonContext.Current.Get(""FaasRequestQuery"");
                    return (JsonConvert.SerializeObject(query));
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "Method";
            return _scriptEngine.CheckScript(script);
        }

        public ExecutionResult<object> Run(Guid scripeId)
        {
            //脚本的信息从库里获取
            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
            using System;
            using Chameleon.Common.Context;
            using Newtonsoft.Json;

            public class Test
            {
                public string Method()
                {
                    var query = ChameleonContext.Current.Get(""FaasRequestQuery"");
                    return (JsonConvert.SerializeObject(query));
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "Method";

            return _scriptEngine.Execute<object>(script);
        }

        public ExecutionResult<object> RunAsync(Guid scripeId)
        {
            throw new NotImplementedException();
        }
    }
}
