using Arch.Core;
using Arch.System;

namespace VojnushkaShared.NetEcs.Rpc
{
    public class NetCleanUpReceivedRpcSystem : BaseSystem<World, float>
    {
        private readonly QueryDescription _queryDescription = new QueryDescription()
            .WithAll<ReceiveRpcCommandRequest>();
        
        public NetCleanUpReceivedRpcSystem(World world) : base(world)
        {
        }

        public override void Update(in float deltaTime)
        {
            World.Destroy(in _queryDescription);
        }
    }
}