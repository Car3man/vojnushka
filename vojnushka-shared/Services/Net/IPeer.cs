using System.Net;

namespace VojnushkaShared.Net
{
    public interface IPeer
    {
        public bool Connected { get; set; }
        string Id { get; set; }
        int IdNumber { get; set; }
        IPEndPoint EndPoint { get; set; }
    }
}