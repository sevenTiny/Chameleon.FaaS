using SevenTiny.Bantina;

namespace Seventiny.Cloud.ScriptEngine
{
    public interface IScriptEngineProvider
    {
        Result<T> RunScript<T>(DynamicScript dynamicScript);
        Result CheckScript(DynamicScript dynamicScript);
    }
}
