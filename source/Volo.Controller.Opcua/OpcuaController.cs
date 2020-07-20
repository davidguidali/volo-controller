using Grpc.Core;
using MongoDB.Driver;
using MongoDB.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Volo.Controller.Opcua;

namespace Volo.Controller
{
    public class OpcuaController
    {
        public void StartListening()
        {
            Channel channel = new Channel("volo-opcua-server:50051", ChannelCredentials.Insecure);
            var client = new DatapointService.DatapointServiceClient(channel);

            var db = new DB("opcuadata", host: "mongodb", port: 27017);

            Timer timer = new Timer(5000);

            async void OnTimerElapsed(object sender, ElapsedEventArgs e)
            {
                var datapoints = db.Queryable<Datapoint>().ToList();

                foreach (var datapoint in datapoints)
                {
                    var reply = await client.SetDatapointAsync(new DatapointMessage() { Identifier = datapoint.Identifier, Value = datapoint.Value }, new CallOptions().WithWaitForReady(true));
                    Console.WriteLine($"id: {datapoint.Identifier}, value:{datapoint.Value}, message:{reply.Message}");
                }
            }

            timer.Elapsed += OnTimerElapsed;

            timer.Start();

            while (true) ;
        }
    }
}