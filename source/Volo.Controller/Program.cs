using System.Threading.Tasks;

namespace Volo.Controller.Opcua
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            OpcuaController opcuaController = new OpcuaController();
            await opcuaController.StartListening();
        }
    }
}