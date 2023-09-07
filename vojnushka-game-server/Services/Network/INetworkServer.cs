using VojnushkaProto.Core;

namespace VojnushkaGameServer.Network;

public interface INetworkServer
{
    event PeerConnectDelegate OnPeerConnect;
    event PeerMessageDelegate OnPeerMessage;
    event PeerDisconnectDelegate OnPeerDisconnect;
    void Start();
    void Stop();
    void Send(IPeer peer, ServerProtoMsg message);
    void Broadcast(ServerProtoMsg message);
}

public delegate void PeerConnectDelegate(IPeer peer);
public delegate void PeerMessageDelegate(IPeer peer, ServerProtoMsg message);
public delegate void PeerDisconnectDelegate(IPeer peer);