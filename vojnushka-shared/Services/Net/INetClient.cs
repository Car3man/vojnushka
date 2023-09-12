namespace VojnushkaShared.Net
{
    public interface INetClient : System.IDisposable
    {
        bool Connected { get; }
        event ConnectDelegate OnConnect;
        event MessageDelegate OnMessage;
        event DisconnectDelegate OnDisconnect;
        void Connect(INetConnectConfig connectConfig);
        void Send(byte[] data);
        void Disconnect();
    }
    
    public delegate void ConnectDelegate();
    public delegate void MessageDelegate(byte[] data);
    public delegate void DisconnectDelegate();
}