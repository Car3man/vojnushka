namespace VojnushkaGameServer.Network;

public interface IServer
{
    void Send(IPeer peer, ServerMessage message);
    void Broadcast(ServerMessage message);
    void OnPeerConnect(IPeer peer);
    void OnPeerMessage(IPeer peer, ServerMessage message);
    void OnPeerDisconnect(IPeer peer);
}