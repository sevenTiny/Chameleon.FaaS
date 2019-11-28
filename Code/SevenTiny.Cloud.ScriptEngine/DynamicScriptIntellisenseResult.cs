using System.Collections.Generic;

namespace SevenTiny.Cloud.ScriptEngine
{
    /// <summary>
    /// 智能提示结果集
    /// </summary>
    public class DynamicScriptIntellisenseResult
    {
        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool IsSuccess { get; private set; }
        /// <summary>
        /// 执行消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string SourceCode { get; set; }
        /// <summary>
        /// 智能提示列表
        /// </summary>
        public List<Intellisense> Intellisenses { get; set; }
    }

    /// <summary>
    /// 智能提示
    /// </summary>
    public class Intellisense
    {
        /// <summary>
        /// 提示项
        /// </summary>
        public int Item { get; set; }
        /// <summary>
        /// 详情
        /// </summary>
        public string Detail { get; set; }
    }
}
