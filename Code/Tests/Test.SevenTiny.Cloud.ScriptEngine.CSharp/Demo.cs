using SevenTiny.Cloud.ScriptEngine;
using SevenTiny.Cloud.ScriptEngine.CSharp;
using System.Diagnostics;
using Xunit;

namespace Test.SevenTiny.Cloud.ScriptEngine.CSharp
{
    public class Demo
    {
        [Trait("desc","执行受信任的脚本 execute trasted code")]
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

            var result = scriptEngineProvider.Execute<int>(script);

            Assert.True(result.IsSuccess);
            Assert.Equal(111, result.Data);
        }

        [Trait("desc","执行不受信任的脚本 execute untrasted code")]
        [Fact]
        public void ExecuteUntrastedCode()
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
                public int GetC(int a)
                {
                    int c = 0;
                    for(int b = 1; b < 100000000; b++)
                    {
                           c += b * a;
                    }
                    return c;
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetC";
            script.Parameters = new object[] { 1 };
            script.IsTrustedScript = false;     //是否受信任的脚本
            script.MillisecondsTimeout = 100;   //执行超时时间

            var result = scriptEngineProvider.Execute<int>(script);

            Assert.True(result.IsSuccess);
            Assert.Equal(111, result.Data);
        }
    }
}
