namespace VojnushkaShared.Net
{
    public class NetConnectConfig : INetConnectConfig
    {
        public string Ip { get; }
        public int Port { get; }

        public NetConnectConfig(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }
    }
}