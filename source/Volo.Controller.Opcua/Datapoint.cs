using MongoDB.Entities.Core;

namespace Volo.Controller.Opcua
{
    public class Datapoint : Entity
    {
        public string Identifier { get; set; }
        public float Value { get; set; }
    }
}