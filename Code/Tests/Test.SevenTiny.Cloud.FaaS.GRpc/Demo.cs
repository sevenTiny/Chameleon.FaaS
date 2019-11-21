using Grpc.Net.Client;
using SevenTiny.Cloud.FaaS.GRpc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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
        public void Test11()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new DynamicScriptExecutor.DynamicScriptExecutorClient(channel);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (var item in new int[1000])
            {
                Task.Factory.StartNew(() =>
                {
                    var reply = client.Test(new DynamicScript
                    {
                        ClassFullName = "123123"
                    });
                });
            }
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ElapsedMilliseconds / 1000);
        }
    }
}
