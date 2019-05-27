namespace Seventiny.Cloud.ScriptEngine
{
    /// <summary>
    /// 动态脚本对象
    /// </summary>
    public class DynamicScript
    {
        /// <summary>
        /// 脚本所属的租户Id，默认0为全局脚本
        /// </summary>
        public int TenantId { get; set; } = 0;
        /// <summary>
        /// 项目名称,可不填
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 脚本内容
        /// </summary>
        public string Script { get; set; }
        /// <summary>
        /// 方法名
        /// </summary>
        public string FunctionName { get; set; }
        /// <summary>
        /// 执行参数
        /// </summary>
        public object[] Parameters { get; set; }
        /// <summary>
        /// 脚本语言
        /// </summary>
        public DynamicScriptLanguage Language { get; set; }
    }
}
