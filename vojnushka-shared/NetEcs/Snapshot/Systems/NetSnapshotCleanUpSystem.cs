using Arch.Core;
using Arch.System;

namespace VojnushkaShared.NetEcs.Snapshot
{
    public class NetSnapshotCleanUpSystem : BaseSystem<World, float>
    {
        private readonly QueryDescription _trailQuery = new QueryDescription()
            .WithAll<NetSnapshotTrail>();
        
        private readonly QueryDescription _eventQuery = new QueryDescription()
            .WithAny<NetObjectCreated, NetObjectDestroyed>();
        
        public NetSnapshotCleanUpSystem(World world) : base(world)
        {
        }

        public override void Update(in float deltaTime)
        {
            World.Destroy(in _trailQuery);
            World.Destroy(in _eventQuery);
        }
    }
}