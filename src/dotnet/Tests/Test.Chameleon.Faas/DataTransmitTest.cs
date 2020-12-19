using Bamboo.ScriptEngine;
using Bamboo.ScriptEngine.CSharp;
using Chameleon.Faas.Common;
using System.Diagnostics;
using Xunit;

namespace Test.Chameleon.Faas
{
    public class DataTransmitTest
    {
        [Trait("desc", "对象跨脚本传递")]
        [Fact]
        public void DownloadPackage()
        {
            IScriptEngine scriptEngineProvider = new CSharpScriptEngine();

            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
            using System;
            using Chameleon.Faas.Common;

            public class Test
            {
                public string Method()
                {
                    string stringProp = FaasContext.Current.Get(""StrKey"");
                    int intProp = FaasContext.Current.Get<int>(""IntKey"");
                    return (stringProp+intProp);
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "Method";
            script.IsExecutionInSandbox = false;

            FaasContext.Current.Put("StrKey", "888");
            FaasContext.Current.Put("IntKey", 999);

            var result = scriptEngineProvider.Execute<string>(script);

            Assert.True(result.IsSuccess);
            Assert.Equal("888999", result.Data);
        }
    }
}
