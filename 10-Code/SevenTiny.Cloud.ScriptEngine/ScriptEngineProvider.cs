using SevenTiny.Cloud.ScriptEngine.DynamicScriptEngine;
using SevenTiny.Bantina;
using SevenTiny.Bantina.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SevenTiny.Cloud.ScriptEngine
{
    public class ScriptEngineProvider : IScriptEngineProvider
    {
        public Result CheckScript(DynamicScript dynamicScript)
        {
            dynamicScript.CheckNull("dynamicScript can not be null");
            return SwitchDynamicScriptEngine(dynamicScript.Language).CheckScript(dynamicScript);
        }

        public Result<T> RunScript<T>(DynamicScript dynamicScript)
        {
            dynamicScript.CheckNull("dynamicScript can not be null");
            return SwitchDynamicScriptEngine(dynamicScript.Language).Run<T>(dynamicScript);
        }
        private IDynamicScriptEngine SwitchDynamicScriptEngine(DynamicScriptLanguage language)
        {
            switch (language)
            {
                case DynamicScriptLanguage.CSharp:
                    return new CSharpDynamicScriptEngine();
                default:
                    break;
            }
            throw new KeyNotFoundException($"script engine not found with choice language {Enum.GetName(typeof(DynamicScriptLanguage), language)}");
        }
    }
}
