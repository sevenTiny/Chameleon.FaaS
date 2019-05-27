using SevenTiny.Bantina;

namespace Seventiny.Cloud.ScriptEngine.DynamicScriptEngine
{
    internal interface IDynamicScriptEngine
    {
        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dynamicScript"></param>
        /// <returns></returns>
        Result<T> Run<T>(DynamicScript dynamicScript);
        /// <summary>
        /// 校验脚本
        /// </summary>
        /// <param name="dynamicScript"></param>
        /// <returns></returns>
        Result CheckScript(DynamicScript dynamicScript);
    }
}
