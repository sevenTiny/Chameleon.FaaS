using Newtonsoft.Json;
using Seventiny.Cloud.ScriptEngine;
using System;
using Xunit;

namespace Test.Seventiny.Cloud.ScriptEngine
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            IScriptEngineProvider scriptEngineProvider = new ScriptEngineProvider();

            DynamicScript script = new DynamicScript();
            script.TenantId = 0;
            script.Language = DynamicScriptLanguage.CSharp;
            script.ProjectName = "TestApp";
            script.Script =
@"
using System;

//EndUsing

public int GetA(int a)
{
    return a;
}
";
            script.FunctionName = "GetA";
            script.Parameters = new object[] { 111 };

            for (int i = 0; i < 10000; i++)
            {
                var result = scriptEngineProvider.RunScript<int>(script);
            }

            script.Script =
                @"
using System;

//EndUsing

public int GetB(int a)
{
    return a;
}
";
            script.FunctionName = "GetB";
            script.Parameters = new object[] { 99999999 };

            for (int i = 0; i < 10000; i++)
            {
                var result = scriptEngineProvider.RunScript<int>(script);
            }
        }

        [Fact]
        public void CheckScript()
        {
            IScriptEngineProvider scriptEngineProvider = new ScriptEngineProvider();
            DynamicScript script = new DynamicScript();
            script.TenantId = 0;
            script.Language = DynamicScriptLanguage.CSharp;
            script.ProjectName = "TestApp";
            script.Script =
@"
using System;

//EndUsing

public int GetA(int a)
{
    return a;int 
}
";
            script.FunctionName = "GetA";
            script.Parameters = new object[] { 111 };

            var result = scriptEngineProvider.CheckScript(script);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void Reference()
        {
            IScriptEngineProvider scriptEngineProvider = new ScriptEngineProvider();
            DynamicScript script = new DynamicScript();
            script.TenantId = 0;
            script.Language = DynamicScriptLanguage.CSharp;
            script.ProjectName = "TestApp";
            script.Script =
@"
using System;
using Newtonsoft.Json;

//EndUsing

public string GetA(int a)
{
    var b = JsonConvert.SerializeObject(a);
    return b;
}
";
            script.FunctionName = "GetA";
            script.Parameters = new object[] { 111 };
            var type = typeof(JsonConvert);
            var ss = type.Assembly;
            var result = scriptEngineProvider.CheckScript(script);
        }
    }
}
