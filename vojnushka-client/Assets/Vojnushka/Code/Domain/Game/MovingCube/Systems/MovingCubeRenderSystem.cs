using Arch.Core;
using Arch.System;
using UnityEngine;
using VojnushkaShared.Domain.MovingCube;

namespace Vojnushka.Game.MovingCube.Systems
{
    public class MovingCubeRenderSystem : BaseSystem<World, float>
    {
        private readonly Mesh _mesh;
        private readonly Material _material;

        private readonly QueryDescription _movingCubeQuery = new QueryDescription()
            .WithAll<MovingCubeComponent>();

        public MovingCubeRenderSystem(World world, Mesh mesh, Material material) : base(world)
        {
            _mesh = mesh;
            _material = material;
        }

        public override void Update(in float deltaTime)
        {
            World.Query(in _movingCubeQuery, (ref MovingCubeComponent movingCube) =>
            {
                var matrix = Matrix4x4.TRS(
                    new Vector3(movingCube.X, movingCube.Y, 0),
                    Quaternion.identity,
                    Vector3.one
                );
                Graphics.DrawMesh(_mesh, matrix, _material, 0);
            });
        }
    }
}