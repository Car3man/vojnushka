using VojnushkaGameServer.Network;
using VojnushkaProto.Core;

namespace VojnushkaGameServer.Core;

public interface IServerWorld
{
    event ServerWorldBroadcastRequestDelegate BroadcastRequest;
    event ServerWorldPeerRequestDelegate PeerRequest;
    void Start();
    void Stop();
    void Tick(float deltaTime);
    void AddPeer(IPeer peer);
    void AddPeerMessage(IPeer peer, ServerProtoMsg message);
    void RemovePeer(IPeer peer);
}

public delegate void ServerWorldBroadcastRequestDelegate(ServerProtoMsg message);
public delegate void ServerWorldPeerRequestDelegate(IPeer peer, ServerProtoMsg message);