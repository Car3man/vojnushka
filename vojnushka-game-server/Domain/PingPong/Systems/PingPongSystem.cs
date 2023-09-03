using Arch.Core;
using VojnushkaGameServer.Core;
using VojnushkaGameServer.Core.Utility;
using VojnushkaGameServer.Logger;

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
            if (netPeerMessage.Message.Type != ServerMessageType.Ping)
            {
                return;
            }

            var pingMessage = PingMessage.Parser.ParseFrom(netPeerMessage.Message.Data);
            _logger.Log($"Ping message: {pingMessage.Message}");
            
            var pongMessage = new PongMessage
            {
                Message = $"Hello from server: {pingMessage.Message}"
            };
            var requestEntity = world.Create();
            world.Add(requestEntity, new NetPeerRequest
            {
                EntityRef = netPeerMessage.EntityRef,
                Message = new ServerMessage
                {
                    Type = ServerMessageType.Pong,
                    Data = MessageUtility.MessageToByteString(pongMessage)
                }
            });
        });
    }

    public void OnStop(World world)
    {
        
    }
}