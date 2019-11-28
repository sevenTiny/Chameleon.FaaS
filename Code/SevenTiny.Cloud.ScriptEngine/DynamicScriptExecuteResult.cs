namespace SevenTiny.Cloud.ScriptEngine
{
    public struct DynamicScriptExecuteResult
    {
        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool IsSuccess { get; private set; }
        /// <summary>
        /// 执行消息
        /// </summary>
        public string Message { get; set; }

        public static DynamicScriptExecuteResult Error(string message = null)
        {
            return new DynamicScriptExecuteResult() { IsSuccess = false, Message = message };
        }
        public static DynamicScriptExecuteResult Success(string message = null)
        {
            return new DynamicScriptExecuteResult { IsSuccess = true, Message = message };
        }
    }

    public struct DynamicScriptExecuteResult<T>
    {
        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool IsSuccess { get; private set; }
        /// <summary>
        /// 执行信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 返回结果
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// 执行耗时
        /// </summary>
        public long ProcessorTime { get; set; }
        /// <summary>
        /// 内存占用
        /// </summary>
        public long TotalMemoryAllocated { get; set; }

        public static DynamicScriptExecuteResult<T> Error(string message = null)
        {
            return new DynamicScriptExecuteResult<T> { IsSuccess = false, Message = message };
        }
        public static DynamicScriptExecuteResult<T> Success(string message = null, T data = default)
        {
            return new DynamicScriptExecuteResult<T> { IsSuccess = true, Message = message, Data = data };
        }
    }
}
