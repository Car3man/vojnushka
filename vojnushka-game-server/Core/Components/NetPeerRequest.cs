using Arch.Core;

namespace VojnushkaGameServer.Core;

public struct NetPeerRequest
{
    public EntityReference EntityRef;
    public byte[] Data;
}