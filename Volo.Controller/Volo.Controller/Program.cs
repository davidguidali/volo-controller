using Grpc.Core;

namespace Volo.Controller
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
            var client = new DatapointService.DatapointServiceClient(channel);
            var reply = client.AddDatapoint(new DatapointMessage() { Identifier = "test", Value = 20 });
        }
    }
}