from concurrent import futures
import time
import grpc
import seventiny_cloud_faas_proto_pb2
import seventiny_cloud_faas_pb2_grpc
from grpc.beta import implementations

# 实现 proto 文件中定义的 GreeterServicer


class DynamicScriptExecutor(seventiny_cloud_faas_pb2_grpc.DynamicScriptExecutorServicer):
    # 实现 proto 文件中定义的 rpc 调用
    def CheckScript(self, request, context):
        print(str(request))
        return seventiny_cloud_faas_proto_pb2.DynamicScriptExecuteResult(Message='this is return message 7tiny...')

    def Execute(self, request, context):
        print(str(request))
        return seventiny_cloud_faas_proto_pb2.DynamicScriptExecuteResult(Message='this is return message 7tiny...')


def serve():
    # 启动 rpc 服务
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    seventiny_cloud_faas_pb2_grpc.add_DynamicScriptExecutorServicer_to_server(
        DynamicScriptExecutor(), server)

    # 下面这段是处理TSL用的 TSL >>>
    with open('./ca/server.pem', 'rb') as f:
        private_key = f.read()
    with open('./ca/server.crt', 'rb') as f:
        certificate_chain = f.read()
    with open('./ca/ca.crt', 'rb') as f:
        root_certificates = f.read()

    # server_credentials = grpc.ssl_server_credentials(
    #     ((private_key, certificate_chain),), root_certificates, True)
    # server.add_secure_port('[::]:5001', server_credentials)

    # 22
    server_credentials = grpc.ssl_server_credentials(
        ((private_key, certificate_chain),), None, False)
    server.add_secure_port('[::]:5001', server_credentials)
    # 处理TSL结束 TSL <<<

    # server.add_insecure_port('[::]:5001')

    server.start()
    print('server start...!')
    try:
        while True:
            time.sleep(60*60*24)  # one day in seconds
    except KeyboardInterrupt:
        server.stop(0)


if __name__ == '__main__':
    serve()
