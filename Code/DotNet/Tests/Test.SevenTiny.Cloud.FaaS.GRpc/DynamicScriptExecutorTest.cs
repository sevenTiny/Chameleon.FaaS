using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using SevenTiny.Cloud.FaaS.GRpc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Test.SevenTiny.Cloud.FaaS.GRpc
{
    public class DynamicScriptExecutorTest
    {
        [Fact]
        public void Execute()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5001", new GrpcChannelOptions { Credentials = ChannelCredentials.Insecure });
            var client = new DynamicScriptExecutor.DynamicScriptExecutorClient(channel);

            DynamicScript script = new DynamicScript();
            script.TenantId = 0;
            script.Language = DynamicScriptLanguage.Csharp;
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
            script.IsTrustedScript = true;
            script.Parameters = JsonSerializer.Serialize(new object[] { 111 });

            var reply = client.Execute(script);

            Assert.Equal("111", reply.Data);
        }
    }
}
