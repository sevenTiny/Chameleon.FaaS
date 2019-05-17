using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seventiny.Cloud.ScriptEngine.Context
{
    class CSharpDynamicScriptContext : DynamicScriptContext
    {
        /// <summary>
        /// 元数据引用
        /// </summary>
        public HashSet<MetadataReference> MetadataReferences { get; set; }
    }
}
