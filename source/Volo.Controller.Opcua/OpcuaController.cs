using Grpc.Core;
using MongoDB.Driver;
using MongoDB.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Volo.Controller.Opcua;
using Volo.Controller.Shared;

namespace Volo.Controller
{
    public class OpcuaController
    {
        private readonly AppSettings _settings;
        private DB _database;
        private OpcuaServerService.OpcuaServerServiceClient _client;

        public OpcuaController(AppSettings settings)
        {
            _settings = settings;
        }

        public async Task Start()
        {
            var channel = CreateChannel();
            _client = new OpcuaServerService.OpcuaServerServiceClient(channel);

            Console.WriteLine($"Connecting to {_settings.MongoDbHost}:{_settings.MongoDbPort}");
            _database = new DB("opcuadata", host: _settings.MongoDbHost, port: _settings.MongoDbPort);
            Console.WriteLine("Connected!");

            await AddDefaultDatapoint();

            var datapoints = _database.Queryable<Datapoint>().ToList();

            Console.WriteLine($"Number of datapoints: {datapoints.Count}");

            foreach (var datapoint in datapoints)
            {
                Console.WriteLine($"Set Datapoint: {datapoint.Identifier}:{datapoint.Value}");
                await _client.SetDatapointAsync(new DatapointMessage() { Identifier = datapoint.Identifier, Value = datapoint.Value }, new CallOptions().WithWaitForReady(true));
            }

            while (true) { await Task.Delay(1000); };
        }

        private Channel CreateChannel()
        {
            Console.WriteLine($"Create channel to {_settings.DomainNameOpcuaServer}:{_settings.GrpcPortOpcuaServer}");
            Channel channel = new Channel($"{_settings.DomainNameOpcuaServer}:{_settings.GrpcPortOpcuaServer}", ChannelCredentials.Insecure);
            Console.WriteLine("Connected!");

            return channel;
        }

        private async Task AddDefaultDatapoint()
        {
            Datapoint defaultDatapoint = new Datapoint()
            {
                Identifier = "hello.world",
                Value = 100
            };

            var datapoint = _database.Find<Datapoint>().Many(f => f.Identifier.Equals("hello.world"));

            if (datapoint.Count == 0)
            {
                await defaultDatapoint.SaveAsync();
            }

            await _client.SetDatapointAsync(new DatapointMessage() { Identifier = defaultDatapoint.Identifier, Value = defaultDatapoint.Value }, new CallOptions().WithWaitForReady(true));
        }
    }
}