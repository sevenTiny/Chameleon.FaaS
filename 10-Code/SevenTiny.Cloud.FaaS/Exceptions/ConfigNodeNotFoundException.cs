using System;

namespace SevenTiny.Cloud.FaaS.Exceptions
{
    /// <summary>
    /// 配置文件节点没有找到
    /// </summary>
    public class ConfigNodeNotFoundException : Exception
    {
        public string NodeName { get; set; }
        public ConfigNodeNotFoundException(string nodeName, string message) : base(message)
        {
            this.NodeName = nodeName;
        }
    }
}
