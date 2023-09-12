using System.Net;
using VojnushkaShared.Net;

namespace VojnushkaGameServer.WebSocket
{
    internal class WebSocketPeer : IPeer
    {
        public readonly Guid Guid;

        public string Id { get; set; }
    
        public int IdNumber { get; set; }
    
        public IPEndPoint EndPoint { get; set; }

        public WebSocketPeer(Guid guid, string ipPort, int number)
        {
            Guid = guid;
            IdNumber = number;
            Id = Guid.ToString();

            var ipv4 = GetIpV4(ipPort);
            EndPoint = new IPEndPoint(GetIpV4Address(ipv4), GetIpV4Port(ipv4));
        }
        
        private string GetIpV4(string ipPort)
        {
            return ipPort.Replace("::1", "127.0.0.1");
        }

        private IPAddress GetIpV4Address(string ipPortV4)
        {
            var split = ipPortV4.Split(":");
            var ipAddressString = split[0];
            return IPAddress.Parse(ipAddressString);
        }
        
        private int GetIpV4Port(string ipPortV4)
        {
            var split = ipPortV4.Split(":");
            var portString = split[1];
            return int.Parse(portString);
        }
    }
}