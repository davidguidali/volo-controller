using System.Threading.Tasks;

namespace Volo.Controller
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            OpcuaController opcuaController = new OpcuaController();
            await opcuaController.SeedRandom();
        }
    }
}