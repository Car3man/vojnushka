using System;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using UnityEngine;
using VojnushkaShared.Domain.Player;
using VojnushkaShared.NetEcs.Rpc;

namespace Vojnushka.Game.Player
{
    public class PlayerInputSystem : BaseSystem<World, float>
    {
        public PlayerInputSystem(World world) : base(world)
        {
        }

        public override void Update(in float deltaTime)
        {
            var playerInputVector = new System.Numerics.Vector2
            {
                X = Input.GetAxis("Horizontal"),
                Y = Input.GetAxis("Vertical")
            };

            var playerInput = this.World.Create();
            playerInput.Add(new SendRpcCommandRequest());
            playerInput.Add(new PlayerInputComponent
            {
                Time = DateTime.UtcNow,
                Move = playerInputVector
            });
        }
    }
}