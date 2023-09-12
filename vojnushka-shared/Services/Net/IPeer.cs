using System.Net;

namespace VojnushkaShared.Net
{
    public interface IPeer
    {
        string Id { get; set; }
        int IdNumber { get; set; }
        IPEndPoint EndPoint { get; set; }
    }
}