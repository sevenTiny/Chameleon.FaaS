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
using Demo;
using Grpc.Auth;
using System.IO;
using System.Net.Http;

namespace Test.SevenTiny.Cloud.FaaS.GRpc
{
    public class DemoTest
    {
        [Fact]
        public void Execute()
        {
            //var channel = GrpcChannel.ForAddress("http://localhost:39901", new GrpcChannelOptions { Credentials = ChannelCredentials.Insecure });
            //var client = new Greeter.GreeterClient(channel);

            var credentials = new System.Security.Cryptography.X509Certificates.X509Certificate(@"G:\DotNet\SevenTiny.Cloud.FaaS\Code\Python\SevenTiny.Cloud.FaaS.GRpc\ca\client.crt");

            // Add client cert to the handler
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(credentials);

            var channel = GrpcChannel.ForAddress("https://localhost:39901", new GrpcChannelOptions { HttpClient = new HttpClient(handler) });

            var client = new Greeter.GreeterClient(channel);

            var reply = client.SayHello(new HelloRequest { Name="test name -- 7tiny"});

            Assert.True(true);
        }
    }
}

//研究备注：当前通过证书的方式没有跑通...2020年2月26日