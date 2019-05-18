using Seventiny.Cloud.ScriptEngine.DynamicScriptEngine;
using SevenTiny.Bantina;
using SevenTiny.Bantina.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Seventiny.Cloud.ScriptEngine
{
    public class ScriptEngineProvider : IScriptEngineProvider
    {
        public Result<T> RunScript<T>(DynamicScript dynamicScript)
        {
            dynamicScript.CheckNull("dynamicScript can not be null");
            return SwitchDynamicScriptEngine(dynamicScript.Language).Run<T>(dynamicScript);
        }
        public Result<T> RunScript<T>(List<DynamicScript> dynamicScripts)
        {
            if (dynamicScripts == null || !dynamicScripts.Any())
                throw new ArgumentNullException(nameof(dynamicScripts), $"dynamicScript can not be null");

            if (dynamicScripts.Select(t => t.Language).Distinct().Count() > 1)
            {
                throw new NotSupportedException("current version only support single type scripts");
            }

            return SwitchDynamicScriptEngine(dynamicScripts.FirstOrDefault().Language).Run<T>(dynamicScripts);
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
