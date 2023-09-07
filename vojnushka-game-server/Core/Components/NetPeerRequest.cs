using Arch.Core;
using VojnushkaProto.Core;

namespace VojnushkaGameServer.Core;

public struct NetPeerRequest
{
    public EntityReference EntityRef;
    public ServerProtoMsg Message;
}