using System;
using System.Collections.Generic;
using System.Numerics;
using Arch.Core;
using Arch.System;
using VojnushkaShared.Logger;
using VojnushkaShared.NetEcs.Core;
using VojnushkaShared.NetEcs.Snapshot;

namespace VojnushkaShared.NetEcs.Transform
{
    public class NetInterpolateTransformSystem : BaseSystem<World, float>
    {
        private readonly ILogger _logger;
        private bool _warmedUp;
        
        private readonly QueryDescription _netSnapshotTrail = new QueryDescription()
            .WithAll<NetSnapshotTrail>();
        private readonly QueryDescription _netTransformQuery = new QueryDescription()
            .WithAll<NetTransform>();
        
        public NetInterpolateTransformSystem(World world, ILogger logger) : base(world)
        {
            _logger = logger;
        }

        public override void Update(in float deltaTime)
        {
            var lastTickTime = World.GetNetLastTickTime();
            var lastTickPing = World.GetNetLastTickPing();
            var expectedNextTickTime = lastTickTime.AddMilliseconds(lastTickPing);
            var leftToNextTickTime = (int)(expectedNextTickTime - DateTime.UtcNow).TotalMilliseconds;
            var t = 1f - Math.Clamp(leftToNextTickTime / (float)lastTickPing, 0f, float.MaxValue);
            
            if (TryGetSnapshotTrail(out var netSnapshotTrail) && netSnapshotTrail!.PrevSnapshot.HasValue)
            {
                World.Query(in _netTransformQuery, (ref NetTransform netTransform) =>
                {
                    netTransform.PrevPosition = netTransform.LastCheckPosition;
                });

                _warmedUp = true;
            }
            
            World.Query(in _netTransformQuery, (ref NetObject netObject, ref NetTransform netTransform) =>
            {
                netTransform.InterpolatedPosition = _warmedUp ?
                    Vector3.Lerp(netTransform.PrevPosition, netTransform.Position, t) :
                    netTransform.Position;
                netTransform.LastCheckPosition = netTransform.Position;
            });
        }

        private bool TryGetSnapshotTrail(out NetSnapshotTrail? outNetSnapshotTrail)
        {
            bool has = false;
            NetSnapshotTrail? netSnapshotTrail = null;
            World.Query(in _netSnapshotTrail, (ref NetSnapshotTrail entitySnapshotTrail) =>
            {
                netSnapshotTrail = entitySnapshotTrail;
                has = true;
            });
            outNetSnapshotTrail = netSnapshotTrail;
            return has;
        }
    }
}