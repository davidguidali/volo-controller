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

            StartGrpcServer();

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
            Datapoint datapoint = new Datapoint()
            {
                Identifier = "hello.world",
                Value = 100
            };

            await SetDatapoint(datapoint);
        }

        private void StartGrpcServer()
        {
            var grpcServer = new Server
            {
                Services = { OpcuaControllerService.BindService(new OpcuaControllerApi(this)) },
                Ports = { new ServerPort(_settings.GrpcHost, _settings.GrpcPort, ServerCredentials.Insecure) }
            };

            grpcServer.Start();
        }

        public async Task SetDatapoint(Datapoint datapoint)
        {
            var datapointsInDatabase = await _database.Find<Datapoint>().ManyAsync(f => f.Identifier.Equals(datapoint.Identifier));

            if (datapointsInDatabase.Count == 0)
            {
                await datapoint.SaveAsync();
            }

            var result = await _client.SetDatapointAsync(new DatapointMessage() { Identifier = datapoint.Identifier, Value = datapoint.Value }, new CallOptions().WithWaitForReady(true));
        }
    }
}