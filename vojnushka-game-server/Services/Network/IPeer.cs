using System.Net;

namespace VojnushkaGameServer.Network;

public interface IPeer
{
    string Id { get; set; }
    int IdNumber { get; set; }
    IPEndPoint EndPoint { get; set; }
}