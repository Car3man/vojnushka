using System.Numerics;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using VojnushkaShared.Domain.MovingCube;
using VojnushkaShared.NetEcs.Core;
using VojnushkaShared.NetEcs.Transform;

namespace VojnushkaGameServer.Domain.MovingCube
{
    public class SpawnAndMoveMovingCubeSystem : BaseSystem<World, float>
    {
        private float _time;

        private readonly QueryDescription _movingCubeQuery = new QueryDescription()
            .WithAll<NetTransform, MovingCubeComponent>();
        
        public SpawnAndMoveMovingCubeSystem(World world) : base(world)
        {
        }

        public override void Initialize()
        {
            var entity = World.CreateNetObject();
            entity.Add(new NetTransform
            {
                Position = Vector3.Zero
            });
            entity.Add(new MovingCubeComponent());
        }

        public override void Update(in float deltaTime)
        {
            float sin = (float)Math.Sin(_time) * 3f;

            World.Query(in _movingCubeQuery, (ref NetTransform netTransform) =>
            {
                netTransform.Position.X = sin;
            });
            
            _time += deltaTime;
        }
    }
}