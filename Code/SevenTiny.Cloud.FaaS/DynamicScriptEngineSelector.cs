using SevenTiny.Cloud.ScriptEngine;
using SevenTiny.Cloud.ScriptEngine.CSharp;
using System.Collections.Generic;

namespace SevenTiny.Cloud.FaaS
{
    /// <summary>
    /// 动态脚本引擎执行选择器
    /// </summary>
    internal class DynamicScriptEngineSelector
    {
        private static Dictionary<DynamicScriptLanguage, IDynamicScriptEngine> _scriptEngineCache;

        static DynamicScriptEngineSelector()
        {
            _scriptEngineCache = new Dictionary<DynamicScriptLanguage, IDynamicScriptEngine>();
            //add dynamic script engine of each language
            _scriptEngineCache.Add(DynamicScriptLanguage.CSharp, new CSharpDynamicScriptEngine());
        }

        public static IDynamicScriptEngine GetScriptEngine(DynamicScriptLanguage dynamicScriptLanguage)
        {
            return _scriptEngineCache[dynamicScriptLanguage];
        }
    }
}
