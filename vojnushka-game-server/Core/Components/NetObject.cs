using Arch.Core;

namespace VojnushkaGameServer.Core;

public struct NetObject
{
    public EntityReference OwnerRef;

    public NetObject(EntityReference ownerRef)
    {
        OwnerRef = ownerRef;
    }
}