import grpc
import seventiny_cloud_faas_proto_pb2
import seventiny_cloud_faas_pb2_grpc
from grpc.beta import implementations


def run():

    # TSL连接方式 >>>
    with open('G:\DotNet\SevenTiny.Cloud.FaaS\Code\Python\SevenTiny.Cloud.FaaS.GRpc\ca\client.pem', 'rb') as f:
        pem = f.read()

    creds = implementations.ssl_channel_credentials(
        pem, None, None)

    channel = implementations.secure_channel('localhost', 5001, creds)
    # TSL连接方式 <<<

    # 连接 rpc 服务器
    # channel = grpc.insecure_channel('localhost:5001')
    # 调用 rpc 服务
    stub = seventiny_cloud_faas_pb2_grpc.DynamicScriptExecutorStub(channel)
    response = stub.CheckScript(seventiny_cloud_faas_proto_pb2.DynamicScript(
        TenantId=100000, Script='123123123'))
    print("CheckScript client received: " + response.Message)

    response = stub.Execute(seventiny_cloud_faas_proto_pb2.DynamicScript(
        TenantId=100000, Script='123123123'))
    print("Execute client received: " + response.Message)


if __name__ == '__main__':
    run()
