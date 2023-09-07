using VojnushkaProto.Core;

namespace VojnushkaGameServer.Network;

public interface IServer
{
    void Send(IPeer peer, ServerProtoMsg message);
    void Broadcast(ServerProtoMsg message);
    void OnPeerConnect(IPeer peer);
    void OnPeerMessage(IPeer peer, ServerProtoMsg message);
    void OnPeerDisconnect(IPeer peer);
}