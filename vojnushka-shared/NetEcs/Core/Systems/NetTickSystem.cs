using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;

namespace VojnushkaShared.NetEcs.Core
{
    public class NetTickSystem : BaseSystem<World, float>
    {
        private readonly bool _isServer;

        private readonly QueryDescription _queryDescription = new QueryDescription()
            .WithAll<NetTime>();

        public NetTickSystem(World world, bool isServer) : base(world)
        {
            _isServer = isServer;
        }

        public override void Initialize()
        {
            var entity = this.World.Create();
            entity.Add(new NetTime
            {
                Tick = 0
            });
        }

        public override void Update(in float deltaTime)
        {
            if (!_isServer)
            {
                return;
            }
            
            World.Query(in _queryDescription, (ref NetTime netTime) =>
            {
                netTime.Tick += 1;
            });
        }
    }
}