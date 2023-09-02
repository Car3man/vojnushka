namespace VojnushkaGameServer.Network;

public interface INetworkServer
{
    event PeerConnectDelegate OnPeerConnect;
    event PeerMessageDelegate OnPeerMessage;
    event PeerDisconnectDelegate OnPeerDisconnect;
    void Start();
    void Stop();
    void Send(IPeer peer, byte[] data);
    void Broadcast(byte[] data);
}

public delegate void PeerConnectDelegate(IPeer peer);
public delegate void PeerMessageDelegate(IPeer peer, byte[] data);
public delegate void PeerDisconnectDelegate(IPeer peer);