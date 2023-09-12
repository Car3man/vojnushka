using Arch.Core;
using VojnushkaShared.NetEcs.Core;

namespace VojnushkaShared.NetEcs.Snapshot.Utility
{
    public static class NetTimeUtility
    {
        private static readonly QueryDescription NetTimeQuery = new QueryDescription()
            .WithAll<NetTime>();
        
        public static int GetLocalTick(this World world)
        {
            var currentTick = 0;
            
            world.Query(in NetTimeQuery, (ref NetTime netTime) =>
            {
                currentTick = netTime.Tick;
            });
            
            return currentTick;
        }
    }
}