namespace SevenTiny.Cloud.ScriptEngine
{
    public struct DynamicScriptExecuteResult
    {
        public bool IsSuccess { get; private set; }
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
        public bool IsSuccess { get; private set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public long ProcessorTime { get; set; }
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
