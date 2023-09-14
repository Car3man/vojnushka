using Arch.Core;
using Arch.System;

namespace VojnushkaShared.NetEcs.Snapshot
{
    public class NetSnapshotTrailCleanUpSystem : BaseSystem<World, float>
    {
        private readonly QueryDescription _queryDescription = new QueryDescription()
            .WithAll<NetSnapshotTrail>();
        
        public NetSnapshotTrailCleanUpSystem(World world) : base(world)
        {
        }

        public override void Update(in float deltaTime)
        {
            World.Destroy(in _queryDescription);
        }
    }
}