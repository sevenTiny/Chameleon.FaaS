using Chameleon.Common.Models;
using Chameleon.Faas.CSharp.Service;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Chameleon.Faas.CSharp.Api.Controllers
{
    [Route("api/faas/json/v1")]
    [ApiController]
    public class JsonV1Controller : ControllerBase
    {
        private readonly IJsonScriptService _jsonScriptService;

        public JsonV1Controller(IJsonScriptService jsonScriptService)
        {
            _jsonScriptService = jsonScriptService;
        }

        [Route("check")]
        [HttpPost]
        public IActionResult Check([FromQuery] string scriptId)
        {
            if (string.IsNullOrEmpty(scriptId) || !Guid.TryParse(scriptId, out Guid id))
                return Ok(ResponseModel.Error(50005, "scriptId 参数不正确"));

            var result = _jsonScriptService.Check(id);

            if (!result.IsSuccess)
                return Ok(ResponseModel.Error(50006, "检查脚本失败：" + result.Message));

            return Ok(ResponseModel.Success("操作成功"));
        }

        [Route("run")]
        [HttpPost]
        public IActionResult Run([FromQuery] string scriptId, [FromBody] object argument)
        {
            if (string.IsNullOrEmpty(scriptId) || !Guid.TryParse(scriptId, out Guid id))
                return Ok(ResponseModel.Error(50007, "scriptId 参数不正确"));

            var result = _jsonScriptService.Run(id);

            if (!result.IsSuccess)
                return Ok(ResponseModel.Error(50008, "执行脚本失败：" + result.Message));

            return Ok(ResponseModel.Success("操作成功", result.Data));
        }
    }
}
