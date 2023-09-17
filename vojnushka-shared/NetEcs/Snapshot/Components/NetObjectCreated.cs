using Arch.Core;

namespace VojnushkaShared.NetEcs.Snapshot
{
    public class NetObjectCreated
    {
        public EntityReference Reference;

        public NetObjectCreated(EntityReference reference)
        {
            Reference = reference;
        }
    }
}