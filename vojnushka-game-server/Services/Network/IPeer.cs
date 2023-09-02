using System.Net;

namespace VojnushkaGameServer.Network;

public interface IPeer
{
    string Id { get; set; }
    IPEndPoint EndPoint { get; set; }
}