using SevenTiny.Cloud.ScriptEngine;
using SevenTiny.Cloud.ScriptEngine.CSharp;
using System.Diagnostics;
using Xunit;

namespace Test.SevenTiny.Cloud.ScriptEngine.CSharp
{
    public class Demo
    {
        /// <summary>
        /// 动态执行CSharp脚本的Demo
        /// </summary>
        [Fact]
        public void Execute()
        {
            IDynamicScriptEngine scriptEngineProvider = new CSharpDynamicScriptEngine();

            DynamicScript script = new DynamicScript();
            script.TenantId = 0;
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
            using System;

            public class Test
            {
                public int GetA(int a)
                {
                    return a;
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetA";
            script.Parameters = new object[] { 111 };

            var result = scriptEngineProvider.Run<int>(script);

            Assert.True(result.IsSuccess);
            Assert.Equal(111, result.Data);
        }
    }
}
