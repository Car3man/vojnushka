using VojnushkaGameServer.Network;

namespace VojnushkaGameServer.Core;

public interface IServerWorld
{
    event ServerWorldBroadcastRequestDelegate BroadcastRequest;
    event ServerWorldPeerRequestDelegate PeerRequest;
    void Start();
    void Stop();
    void Tick(float deltaTime);
    void AddPeer(IPeer peer);
    void AddPeerMessage(IPeer peer, byte[] data);
    void RemovePeer(IPeer peer);
}

public delegate void ServerWorldBroadcastRequestDelegate(byte[] data);
public delegate void ServerWorldPeerRequestDelegate(IPeer peer, byte[] data);