namespace SevenTiny.Cloud.ScriptEngine
{
    public interface IDynamicScriptEngine
    {
        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="dynamicScript">动态脚本</param>
        /// <returns></returns>
        DynamicScriptExecuteResult<T> Execute<T>(DynamicScript dynamicScript);
        /// <summary>
        /// 校验脚本
        /// </summary>
        /// <param name="dynamicScript">动态脚本</param>
        /// <returns></returns>
        DynamicScriptExecuteResult CheckScript(DynamicScript dynamicScript);
        /// <summary>
        /// 智能提示
        /// </summary>
        /// <param name="intellisenseDynamicScript">动态脚本分析</param>
        /// <returns></returns>
        DynamicScriptIntellisenseResult Intellisense(IntellisenseDynamicScript intellisenseDynamicScript);
    }
}
