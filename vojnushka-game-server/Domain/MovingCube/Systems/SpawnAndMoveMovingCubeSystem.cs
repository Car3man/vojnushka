using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using VojnushkaShared.Domain.MovingCube;
using VojnushkaShared.NetEcs.Core;

namespace VojnushkaGameServer.Domain.MovingCube
{
    public class SpawnAndMoveMovingCubeSystem : BaseSystem<World, float>
    {
        private float _time;

        private readonly QueryDescription _movingCubeQuery = new QueryDescription()
            .WithAll<MovingCubeComponent>();
        
        public SpawnAndMoveMovingCubeSystem(World world) : base(world)
        {
        }

        public override void Initialize()
        {
            var entity = this.World.Create();
            entity.Add(new NetObject());
            entity.Add(new MovingCubeComponent());
        }

        public override void Update(in float deltaTime)
        {
            float sin = (float)Math.Sin(_time) * 3f;

            World.Query(in _movingCubeQuery, (ref MovingCubeComponent movingCube) =>
            {
                movingCube.X = sin;
            });
            
            _time += deltaTime;
        }
    }
}