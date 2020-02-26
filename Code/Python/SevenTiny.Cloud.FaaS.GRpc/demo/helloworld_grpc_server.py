from concurrent import futures
import time
import grpc
import helloworld_pb2
import helloworld_pb2_grpc

# 实现 proto 文件中定义的 GreeterServicer
class Greeter(helloworld_pb2_grpc.GreeterServicer):
    # 实现 proto 文件中定义的 rpc 调用
    def SayHello(self, request, context):
        print(str(request))
        return helloworld_pb2.HelloReply(message = 'hello {msg}'.format(msg = request.name))

    def SayHelloAgain(self, request, context):
        print(str(request))
        return helloworld_pb2.HelloReply(message='hello {msg}'.format(msg = request.name))

def serve():
    # 启动 rpc 服务
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    helloworld_pb2_grpc.add_GreeterServicer_to_server(Greeter(), server)

    # 下面这段是处理TSL用的 TSL >>>
    with open('./ca/server.pem', 'rb') as f:
        private_key = f.read()
    with open('./ca/server.crt', 'rb') as f:
        certificate_chain = f.read()
    with open('./ca/ca.crt', 'rb') as f:
        root_certificates = f.read()

    server_credentials = grpc.ssl_server_credentials(
        ((private_key, certificate_chain),), root_certificates, True)
    server.add_secure_port('[::]:39901', server_credentials)

    # 处理TSL结束 TSL <<<
    
    # server.add_insecure_port('[::]:39901')
    server.start()
    print('grpc server start...!')
    try:
        while True:
            time.sleep(60*60*24) # one day in seconds
    except KeyboardInterrupt:
        server.stop(0)

if __name__ == '__main__':
    serve()