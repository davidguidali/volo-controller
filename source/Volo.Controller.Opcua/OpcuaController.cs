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
        public async Task StartListening()
        {
            Console.WriteLine("Connecting to Server...");
            Channel channel = new Channel("volo-opcua-server:50051", ChannelCredentials.Insecure);
            var client = new DatapointService.DatapointServiceClient(channel);
            Console.WriteLine("Connected!");

            Console.WriteLine("Connecting to DB...");
            var db = new DB("opcuadata", host: "mongodb", port: 27017);
            Console.WriteLine("Connected!");

            Datapoint dp = new Datapoint() { Identifier = "choco", Value = 50 };
            dp.Save();

            Timer timer = new Timer(5000);

            async void OnTimerElapsed(object sender, ElapsedEventArgs e)
            {
                var datapoints = db.Queryable<Datapoint>().ToList();

                Console.WriteLine($"Number of datapoints: {datapoints.Count}");

                foreach (var datapoint in datapoints)
                {
                    var reply = await client.SetDatapointAsync(new DatapointMessage() { Identifier = datapoint.Identifier, Value = datapoint.Value }, new CallOptions().WithWaitForReady(true));
                    Console.WriteLine($"id: {datapoint.Identifier}, value:{datapoint.Value}, message:{reply.Message}");
                }
            }

            timer.Elapsed += OnTimerElapsed;

            timer.Start();

            while (true) { await Task.Delay(1000); };
        }
    }
}