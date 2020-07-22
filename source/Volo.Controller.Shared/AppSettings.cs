using Newtonsoft.Json;

namespace Volo.Controller.Shared
{
    public class AppSettings
    {
        [JsonProperty("grpcPort")]
        public int GrpcPort { get; set; }

        [JsonProperty("grpcHost")]
        public string GrpcHost { get; set; }

        [JsonProperty("grpcPortOpcuaServer")]
        public int GrpcPortOpcuaServer { get; set; }

        [JsonProperty("domainNameOpcuaServer")]
        public string DomainNameOpcuaServer { get; set; }

        [JsonProperty("mongoDbHost")]
        public string MongoDbHost { get; set; }

        [JsonProperty("mongoDbPort")]
        public int MongoDbPort { get; set; }
    }
}