using SevenTiny.Cloud.ScriptEngine;

namespace SevenTiny.Cloud.FaaS
{
    public interface IDynamicScriptExecuteService
    {
        DynamicScriptExecuteResult CheckScript(DynamicScript dynamicScript);
        DynamicScriptExecuteResult<object> Execute(DynamicScript dynamicScript);
    }
}
