using Grpc.Core;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace Volo.Controller
{
    public class OpcuaController
    {
        public async Task SeedRandom()
        {
            Channel channel = new Channel("volo-opcua-server:50051", ChannelCredentials.Insecure);
            var client = new DatapointService.DatapointServiceClient(channel);

            Console.WriteLine("Adding datapoint");
            var reply = await client.AddDatapointAsync(new DatapointMessage() { Identifier = "drizzt", Value = DateTime.Now.Millisecond }, new CallOptions().WithWaitForReady(true));
            Console.WriteLine(reply.Message);

            var timer = new Timer(1000);
            timer.Elapsed += async (sender, e) =>
            {
                var res2 = await client.UpdateDatapointAsync(new DatapointMessage() { Identifier = "drizzt", Value = DateTime.Now.Millisecond }, new CallOptions().WithWaitForReady(true));
                Console.WriteLine(res2);
            };

            timer.Start();
        }
    }
}