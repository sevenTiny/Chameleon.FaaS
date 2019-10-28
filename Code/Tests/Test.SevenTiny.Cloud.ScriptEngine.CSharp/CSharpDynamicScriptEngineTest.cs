using SevenTiny.Cloud.ScriptEngine;
using SevenTiny.Cloud.ScriptEngine.CSharp;
using System.Diagnostics;
using Xunit;

namespace Test.SevenTiny.Cloud.ScriptEngine.CSharp
{
    public class CSharpDynamicScriptEngineTest
    {
        [Trait("desc","多次执行")]
        [Fact]
        public void MultiExecute()
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
            script.Parameters = new object[] { 1 };
            //script.ExecutionStatistics = true;//可以输出执行耗时，内存占用

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //结果相加
            int sum = 0;
            for (int i = 0; i <= 10000; i++)
            {
                var result = scriptEngineProvider.Execute<int>(script);
                //Trace.WriteLine($"Execute{i} -> IsSuccess:{result.IsSuccess},Data={result.Data},Message={result.Message},TotalMemoryAllocated={result.TotalMemoryAllocated},ProcessorTime={result.ProcessorTime.TotalSeconds}");
                if (result.IsSuccess)
                {
                    sum += result.Data;
                }
            }
            stopwatch.Stop();
            var cos = stopwatch.ElapsedMilliseconds;

            Assert.Equal(10000, sum);
        }

        [Trait("desc","执行同名不同类的不同方法")]
        [Fact]
        public void RepeatClassExecute()
        {
            IDynamicScriptEngine scriptEngineProvider = new CSharpDynamicScriptEngine();

            DynamicScript script = new DynamicScript();
            script.TenantId = 0;
            script.Language = DynamicScriptLanguage.CSharp;

            //先编译A执行A
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

            var result1 = scriptEngineProvider.Execute<int>(script);

            //编译B执行B
            script.Script =
         @"
        using System;

        public class Test
        {
            public int GetB(int a)
            {
                return a;
            }
        }
        ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetB";
            script.Parameters = new object[] { 99999999 };

            var result2 = scriptEngineProvider.Execute<int>(script);

            //再执行A，这次是从B的脚本对应的Hash值去找Test类型，里面并没有A，所以报错没有找到方法A
            //也就是说，用B的脚本去调用A是错误的用法，即便类的名称是一样的，但其实不是一个类
            script.ClassFullName = "Test";
            script.FunctionName = "GetA";
            script.Parameters = new object[] { 333 };

            var result3 = scriptEngineProvider.Execute<int>(script);
        }
    }
}
