using Arch.Core;
using VojnushkaGameServer.Core;
using VojnushkaGameServer.Logger;
using VojnushkaProto.Core;
using VojnushkaProto.PingPong;
using VojnushkaProto.Utility;

namespace VojnushkaGameServer.Domain.PingPong;

public class PingPongSystem : ITickSystem
{
    private readonly ILogger _logger;

    public PingPongSystem(ILogger logger)
    {
        _logger = logger;
    }
    
    public void OnStart(World world)
    {
        
    }

    public void OnTick(World world, float deltaTime)
    {
        var pingQuery = new QueryDescription()
            .WithAll<NetPeerMessage>();

        world.Query(pingQuery, (ref NetPeerMessage netPeerMessage) =>
        {
            if (netPeerMessage.Message.Type != ServerProtoMsgType.Ping)
            {
                return;
            }

            var pingMessage = PingProtoMsg.Parser.ParseFrom(netPeerMessage.Message.Data);
            _logger.Log($"Ping message: {pingMessage.Message}");
            
            var pongMessage = new PongProtoMsg
            {
                Message = $"Hello from server: {pingMessage.Message}"
            };
            var requestEntity = world.Create();
            world.Add(requestEntity, new NetPeerRequest
            {
                EntityRef = netPeerMessage.EntityRef,
                Message = new ServerProtoMsg
                {
                    Type = ServerProtoMsgType.Pong,
                    Data = MessageUtility.MessageToByteString(pongMessage)
                }
            });
        });
    }

    public void OnStop(World world)
    {
        
    }
}