using System.Text;
using Arch.Core;
using VojnushkaGameServer.Core;

namespace VojnushkaGameServer.Domain;

public class PingPongSystem : IWorldSystem
{
    public void OnStart(World world)
    {
        
    }

    public void OnTick(World world, float deltaTime)
    {
        var pingQuery = new QueryDescription()
            .WithAll<NetPeerMessage>();

        world.Query(pingQuery, (ref NetPeerMessage peerMessage) =>
        {
            var messageFromPeer = Encoding.UTF8.GetString(peerMessage.Data);
            var messageToPeer = $"Hello from server: {messageFromPeer}";
            
            var requestEntity = world.Create();
            world.Add(requestEntity, new NetPeerRequest
            {
                EntityRef = peerMessage.EntityRef,
                Data = Encoding.UTF8.GetBytes(messageToPeer)
            });
        });
    }

    public void OnStop(World world)
    {
        
    }
}