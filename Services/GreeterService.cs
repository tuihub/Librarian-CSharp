using Grpc.Core;
using Librarian_CSharp;

namespace Librarian_CSharp.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            Task.Delay(2000).Wait();
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override Task<HelloReply2> SayHello2(HelloRequest2 request, ServerCallContext context)
        {
            Task.Delay(3000).Wait();
            return Task.FromResult(new HelloReply2
            {
                Message = "Hello2 " + request.Name
            });
        }
    }
}