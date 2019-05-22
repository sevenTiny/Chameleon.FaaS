using SevenTiny.Bantina;
using System.Collections.Generic;

namespace Seventiny.Cloud.ScriptEngine.DynamicScriptEngine
{
    internal interface IDynamicScriptEngine
    {
        Result<T> Run<T>(DynamicScript dynamicScript);
        Result<T> Run<T>(List<DynamicScript> dynamicScripts);
    }
}
