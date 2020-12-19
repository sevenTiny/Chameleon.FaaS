using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chameleon.Faas.CSharp.Api.Services
{
    public interface ICSharpScriptService
    {
        /// <summary>
        /// 校验脚本
        /// </summary>
        /// <param name="scriptId"></param>
        void Check(Guid scriptId);
        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="scripeId"></param>
        /// <returns></returns>
        object Run(Guid scripeId);
        /// <summary>
        /// 异步执行脚本
        /// </summary>
        /// <param name="scripeId"></param>
        /// <returns></returns>
        object RunAsync(Guid scripeId);
    }

    public class CSharpScriptService : ICSharpScriptService
    {
        private readonly ILogger<CSharpScriptService> _logger;

        public CSharpScriptService(ILogger<CSharpScriptService> logger)
        {
            _logger = logger;
        }

        public void Check(Guid scriptId)
        {
            throw new NotImplementedException();
        }

        public object Run(Guid scripeId)
        {
            throw new NotImplementedException();
        }

        public object RunAsync(Guid scripeId)
        {
            throw new NotImplementedException();
        }
    }
}
