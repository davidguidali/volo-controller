using Grpc.Core;
using System.Threading.Tasks;

namespace Volo.Controller.Opcua
{
    public class OpcuaControllerApi : OpcuaControllerService.OpcuaControllerServiceBase
    {
        private OpcuaController _controller;

        public OpcuaControllerApi(OpcuaController controller)
        {
            _controller = controller;
        }

        public override async Task<OpcuaControllerResult> SetDatapoint(OpcuaControllerMessage request, ServerCallContext context)
        {
            await _controller.SetDatapoint(new Datapoint { Identifier = request.Identifier, Value = request.Value });
            return new OpcuaControllerResult() { ResultCode = OpcuaControllerResult.Types.ResultCode.Success, Message = "Datapoint has been set" };
        }
    }
}