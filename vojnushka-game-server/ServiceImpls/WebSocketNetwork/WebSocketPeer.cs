using System.Net;
using VojnushkaGameServer.Network;

namespace VojnushkaGameServer.WebSocketNetwork;

public class WebSocketPeer : IPeer
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
        EndPoint = IPEndPoint.Parse(ConvertToIpV4(ipPort));
    }

    private string ConvertToIpV4(string ipPort)
    {
        return ipPort.Replace("::1", "127.0.0.1");
    }
}