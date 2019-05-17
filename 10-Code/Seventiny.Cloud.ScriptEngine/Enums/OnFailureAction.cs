using System;
using System.Collections.Generic;
using System.Text;

namespace Seventiny.Cloud.ScriptEngine
{
    /// <summary>
    /// 脚本执行错误后执行动作
    /// </summary>
    public enum OnFailureAction
    {
        Continue = 1,
        Break = 2
    }
}
