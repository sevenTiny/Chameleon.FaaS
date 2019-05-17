using Seventiny.Cloud.ScriptEngine.DynamicScriptEngine;
using SevenTiny.Bantina;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seventiny.Cloud.ScriptEngine
{
    public class ScriptEngineProvider : IScriptEngineProvider
    {
        public Result<T> RunScript<T>(DynamicScript dynamicScript, string functionName, params object[] parameters)
        {
            return new CSharpDynamicScript().Run<T>(dynamicScript, functionName, parameters);
        }
    }
}
