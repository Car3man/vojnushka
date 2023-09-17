using Arch.Core;

namespace VojnushkaShared.NetEcs.Snapshot
{
    public class NetObjectDestroyed
    {
        public EntityReference Reference;

        public NetObjectDestroyed(EntityReference reference)
        {
            Reference = reference;
        }
    }
}