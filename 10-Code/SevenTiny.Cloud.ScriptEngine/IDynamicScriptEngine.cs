using SevenTiny.Bantina;

namespace SevenTiny.Cloud.ScriptEngine
{
    public interface IDynamicScriptEngine
    {
        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dynamicScript"></param>
        /// <returns></returns>
        DynamicScriptExecuteResult<T> Run<T>(DynamicScript dynamicScript);
        /// <summary>
        /// 校验脚本
        /// </summary>
        /// <param name="dynamicScript"></param>
        /// <returns></returns>
        DynamicScriptExecuteResult CheckScript(DynamicScript dynamicScript);
    }
}
