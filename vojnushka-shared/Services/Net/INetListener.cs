namespace VojnushkaShared.Net
{
    public interface INetListener : System.IDisposable
    {
        event PeerConnectDelegate OnPeerConnect;
        event PeerMessageDelegate OnPeerMessage;
        event PeerDisconnectDelegate OnPeerDisconnect;
        bool IsListening { get; }
        void Start(string ip, int port);
        void Stop();
        void Send(IPeer peer, byte[] data);
        void Broadcast(byte[] data);
    }

    public delegate void PeerConnectDelegate(IPeer peer);
    public delegate void PeerMessageDelegate(IPeer peer, byte[] data);
    public delegate void PeerDisconnectDelegate(IPeer peer);
}