namespace VojnushkaShared.Net
{
    public interface IServer
    {
        void Send(IPeer peer, byte[] data);
        void Broadcast(byte[] data);
        void OnPeerConnect(IPeer peer);
        void OnPeerMessage(IPeer peer, byte[] data);
        void OnPeerDisconnect(IPeer peer);
    }
}