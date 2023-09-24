using System.Net;

namespace VojnushkaShared.NetDriver
{
    public interface IPeer
    {
        public bool Connected { get; set; }
        string Id { get; set; }
        int IdNumber { get; set; }
        IPEndPoint EndPoint { get; set; }
    }
}