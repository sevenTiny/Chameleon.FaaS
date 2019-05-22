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
            ScriptEngineProvider scriptEngineProvider = new ScriptEngineProvider();

            DynamicScript script = new DynamicScript();
            script.TenantId = 0;
            script.Language = DynamicScriptLanguage.CSharp;
            script.OnFailureAction = OnFailureAction.Continue;
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
    }
}
