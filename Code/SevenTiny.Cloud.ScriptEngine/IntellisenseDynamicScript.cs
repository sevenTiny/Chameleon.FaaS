namespace SevenTiny.Cloud.ScriptEngine
{
    /// <summary>
    /// 智能提示动态脚本对象
    /// </summary>
    public class IntellisenseDynamicScript
    {
        /// <summary>
        /// 脚本所属的租户Id，默认0为全局脚本
        /// </summary>
        public int TenantId { get; set; } = 0;
        /// <summary>
        /// 脚本内容
        /// </summary>
        public string Script { get; set; }
        /// <summary>
        /// 类的最全限定名
        /// python和go等语言可以写脚本名
        /// </summary>
        public string ClassFullName { get; set; }
        /// <summary>
        /// 方法名
        /// </summary>
        public string FunctionName { get; set; }
        /// <summary>
        /// 脚本语言
        /// </summary>
        public DynamicScriptLanguage Language { get; set; }
        /// <summary>
        /// 上一语句，作为下一语句分析来源
        /// </summary>
        public string LastStatement { get; set; }
    }
}
