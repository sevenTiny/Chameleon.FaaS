using SevenTiny.Cloud.ScriptEngine;

namespace SevenTiny.Cloud.FaaS
{
    public class DynamicScriptExecuteService : IDynamicScriptExecuteService
    {
        public DynamicScriptExecuteResult CheckScript(DynamicScript dynamicScript)
        {
            return DynamicScriptEngineSelector.GetScriptEngine(dynamicScript.Language).CheckScript(dynamicScript);
        }

        public DynamicScriptExecuteResult<object> Execute(DynamicScript dynamicScript)
        {
            return DynamicScriptEngineSelector.GetScriptEngine(dynamicScript.Language).Execute<object>(dynamicScript);
        }
    }
}
