namespace VojnushkaGameServer.Network;

public interface INetworkServer
{
    event PeerConnectDelegate OnPeerConnect;
    event PeerMessageDelegate OnPeerMessage;
    event PeerDisconnectDelegate OnPeerDisconnect;
    void Start();
    void Stop();
    void Send(IPeer peer, ServerMessage message);
    void Broadcast(ServerMessage message);
}

public delegate void PeerConnectDelegate(IPeer peer);
public delegate void PeerMessageDelegate(IPeer peer, ServerMessage message);
public delegate void PeerDisconnectDelegate(IPeer peer);