using SevenTiny.Cloud.ScriptEngine;
using SevenTiny.Cloud.ScriptEngine.CSharp;
using System.Diagnostics;
using Xunit;

namespace Test.SevenTiny.Cloud.ScriptEngine.CSharp
{
    public class CSharpDynamicScriptEngineTest
    {
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
            //script.ExecutionStatistics = true;//�������ִ�к�ʱ���ڴ�ռ��

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //������
            int sum = 0;
            for (int i = 0; i <= 10000; i++)
            {
                var result = scriptEngineProvider.Run<int>(script);
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

        [Fact]
        public void RepeatClassExecute()
        {
            IDynamicScriptEngine scriptEngineProvider = new CSharpDynamicScriptEngine();

            DynamicScript script = new DynamicScript();
            script.TenantId = 0;
            script.Language = DynamicScriptLanguage.CSharp;

            //�ȱ���Aִ��A
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

            var result1 = scriptEngineProvider.Run<int>(script);

            //����Bִ��B
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

            var result2 = scriptEngineProvider.Run<int>(script);

            //��ִ��A������Ǵ�B�Ľű���Ӧ��Hashֵȥ��Test���ͣ����沢û��A�����Ա���û���ҵ�����A
            //Ҳ����˵����B�Ľű�ȥ����A�Ǵ�����÷����������������һ���ģ�����ʵ����һ����
            script.ClassFullName = "Test";
            script.FunctionName = "GetA";
            script.Parameters = new object[] { 333 };

            var result3 = scriptEngineProvider.Run<int>(script);
        }
    }
}
