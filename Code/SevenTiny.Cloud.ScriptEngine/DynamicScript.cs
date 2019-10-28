using Newtonsoft.Json;

namespace SevenTiny.Cloud.ScriptEngine
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
        /// 执行参数
        /// </summary>
        [JsonIgnore]
        public object[] Parameters { get; set; }
        /// <summary>
        /// 脚本语言
        /// </summary>
        public DynamicScriptLanguage Language { get; set; }
        /// <summary>
        /// 是否收集执行统计信息
        /// 默认False：统计非常耗时且会带来更多GC开销，正常运行过程请关闭！
        /// </summary>
        public bool IsExecutionStatistics { get; set; } = false;
        /// <summary>
        /// 是否执行信任的脚本（默认是）
        /// </summary>
        public bool IsTrustedScript { get; set; } = true;
        /// <summary>
        /// 执行不信任的脚本的超时时间
        /// </summary>
        public int MillisecondsTimeout { get; set; }
    }
}
