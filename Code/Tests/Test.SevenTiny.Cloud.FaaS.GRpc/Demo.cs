using Grpc.Net.Client;
using SevenTiny.Cloud.FaaS.GRpc;
using System;
using Xunit;

namespace Test.SevenTiny.Cloud.FaaS.GRpc
{
    public class Demo
    {
        [Fact]
        public void Test1()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new DynamicScriptExecutor.DynamicScriptExecutorClient(channel);
            var reply = client.Test(new DynamicScript
            {
                ClassFullName = "123123"
            });
        }

        [Fact]
        public void Test2()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var reply = client.SayHello(new HelloRequest
            {
                Name = "7tiny"
            });
        }
    }
}
