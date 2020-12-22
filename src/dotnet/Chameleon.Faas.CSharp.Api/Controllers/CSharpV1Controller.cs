using Chameleon.Common.Context;
using Chameleon.Common.Models;
using Chameleon.Faas.CSharp.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Chameleon.Faas.CSharp.Api.Controllers
{
    [Route("api/faas/csharp/v1")]
    [ApiController]
    public class CSharpV1Controller : ControllerBase
    {
        private readonly ICSharpScriptService _cSharpScriptService;

        public CSharpV1Controller(ICSharpScriptService cSharpScriptService)
        {
            _cSharpScriptService = cSharpScriptService;
        }

        [Route("check")]
        [HttpPost]
        public IActionResult Check([FromQuery] string scriptId, [FromBody] object argument)
        {
            if (string.IsNullOrEmpty(scriptId) || !Guid.TryParse(scriptId, out Guid id))
                return Ok(ResponseModel.Error(50001, "scriptId 参数不正确"));

            var result = _cSharpScriptService.Check(id);

            if (!result.IsSuccess)
                return Ok(ResponseModel.Error(50002, "检查脚本失败：" + result.Message));

            return Ok(ResponseModel.Success("操作成功"));
        }

        [Route("run")]
        [HttpPost]
        public IActionResult Run([FromQuery] string scriptId, [FromBody] object argument)
        {
            if (string.IsNullOrEmpty(scriptId) || !Guid.TryParse(scriptId, out Guid id))
                return Ok(ResponseModel.Error(50003, "scriptId 参数不正确"));

            //赋值上下文
            ChameleonContext.Current.Put(ChameleonContext.Const.RequestQuery, HttpContext.Request.Query.ToDictionary(k => k.Key, v => v.Value.ToString()));
            ChameleonContext.Current.Put(ChameleonContext.Const.RequestBody, argument);

            var result = _cSharpScriptService.Run(id);

            if (!result.IsSuccess)
                return Ok(ResponseModel.Error(50004, "执行脚本失败：" + result.Message));

            return Ok(ResponseModel.Success("操作成功", result.Data));
        }
    }
}
