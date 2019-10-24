using SevenTiny.Bantina;

namespace SevenTiny.Cloud.FaaS.DynamicScriptEngine
{
    internal interface IDynamicScriptEngine
    {
        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dynamicScript"></param>
        /// <returns></returns>
        Result<T> Run<T>(DynamicScriptBase dynamicScript);
        /// <summary>
        /// 校验脚本
        /// </summary>
        /// <param name="dynamicScript"></param>
        /// <returns></returns>
        Result CheckScript(DynamicScriptBase dynamicScript);
    }
}
