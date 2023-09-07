namespace Vojnushka.Network
{
    public interface INetworkClient
    {
        bool Connected { get; }
        event ConnectDelegate OnConnect;
        event MessageDelegate OnMessage;
        event DisconnectDelegate OnDisconnect;
        void Connect(string query);
        void Send(byte[] data);
        void Disconnect();
    }
    
    public delegate void ConnectDelegate();
    public delegate void MessageDelegate(byte[] data);
    public delegate void DisconnectDelegate();
}