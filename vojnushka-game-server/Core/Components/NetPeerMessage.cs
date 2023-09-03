using Arch.Core;

namespace VojnushkaGameServer.Core;

public struct NetPeerMessage
{
    public EntityReference EntityRef;
    public ServerMessage Message;
}