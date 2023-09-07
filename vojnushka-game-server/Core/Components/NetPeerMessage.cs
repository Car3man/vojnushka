using Arch.Core;
using VojnushkaProto.Core;

namespace VojnushkaGameServer.Core;

public struct NetPeerMessage
{
    public EntityReference EntityRef;
    public ServerProtoMsg Message;
}