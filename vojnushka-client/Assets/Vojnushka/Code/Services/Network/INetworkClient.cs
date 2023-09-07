using VojnushkaProto.Core;

namespace Vojnushka.Network
{
    public interface INetworkClient
    {
        bool Connected { get; }
        event ConnectDelegate OnConnect;
        event MessageDelegate OnMessage;
        event DisconnectDelegate OnDisconnect;
        void Connect(string query);
        void Send(ServerProtoMsg serverMessage);
        void Disconnect();
    }
    
    public delegate void ConnectDelegate();
    public delegate void MessageDelegate(ServerProtoMsg message);
    public delegate void DisconnectDelegate();
}