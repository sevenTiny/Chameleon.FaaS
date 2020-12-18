import grpc
import helloworld_pb2
import helloworld_pb2_grpc
from grpc.beta import implementations

def run():
    # 连接 rpc 服务器
    # TSL连接方式 >>>
    with open('G:\\DotNet\\SevenTiny.Cloud.FaaS\\Code\\Python\\SevenTiny.Cloud.FaaS.GRpc\\ca\\client.pem', 'rb') as f:
        pem = f.read()

    creds = implementations.ssl_channel_credentials(
        pem, None, None)

    channel = implementations.secure_channel('localhost', 5001, creds)
    # TSL连接方式 <<<
    # channel = grpc.insecure_channel('localhost:39901')
    # 调用 rpc 服务
    stub = helloworld_pb2_grpc.GreeterStub(channel)
    response = stub.SayHello(helloworld_pb2.HelloRequest(name='czl'))
    print("Greeter client received: " + response.message)
    response = stub.SayHelloAgain(helloworld_pb2.HelloRequest(name='daydaygo'))
    print("Greeter client received: " + response.message)

if __name__ == '__main__':
    run()