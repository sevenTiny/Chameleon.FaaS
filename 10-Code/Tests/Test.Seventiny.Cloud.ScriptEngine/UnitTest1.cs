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
            script.TenantId = 10000;
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
            for (int i = 0; i < 10; i++)
            {
                var result = scriptEngineProvider.RunScript<int>(script, "GetA", 111);
            }

            script.Script=
                @"
using System;

//EndUsing

public int GetB(int a)
{
    return a;
}
";

            for (int i = 0; i < 10; i++)
            {
                var result = scriptEngineProvider.RunScript<int>(script, "GetB", 99999999);
            }
        }
    }
}
