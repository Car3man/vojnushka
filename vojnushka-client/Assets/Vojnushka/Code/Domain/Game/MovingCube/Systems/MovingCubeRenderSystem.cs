using Arch.Core;
using Arch.System;
using UnityEngine;
using Vojnushka.VectorConverters;
using VojnushkaShared.Domain.MovingCube;
using VojnushkaShared.NetEcs.Transform;

namespace Vojnushka.Game.MovingCube.Systems
{
    public class MovingCubeRenderSystem : BaseSystem<World, float>
    {
        private readonly Mesh _mesh;
        private readonly Material _material;

        private readonly QueryDescription _movingCubeQuery = new QueryDescription()
            .WithAll<NetTransform, MovingCubeComponent>();

        public MovingCubeRenderSystem(World world, Mesh mesh, Material material) : base(world)
        {
            _mesh = mesh;
            _material = material;
        }

        public override void Update(in float deltaTime)
        {
            World.Query(in _movingCubeQuery, (ref NetTransform netTransform) =>
            {
                var position = netTransform.InterpolatedPosition.GetUnityVector();
                var matrix = Matrix4x4.TRS(
                    position,
                    Quaternion.identity,
                    Vector3.one
                );
                Graphics.DrawMesh(_mesh, matrix, _material, 0);
            });
        }
    }
}