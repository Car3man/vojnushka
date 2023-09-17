using Arch.Core;
using Arch.System;
using VojnushkaShared.Domain.Player;
using VojnushkaShared.NetEcs.Core;
using VojnushkaShared.NetEcs.Rpc;
using VojnushkaShared.NetEcs.Transform;

namespace VojnushkaGameServer.Domain.Player;

public class PlayerMoveSystem : BaseSystem<World, float>
{
    private const float MoveSpeed = 1f;
    
    private readonly QueryDescription _playerInputQuery = new QueryDescription()
        .WithAll<ReceiveRpcCommandRequest, PlayerInputComponent>();
    
    private readonly QueryDescription _playerQuery = new QueryDescription()
        .WithAll<NetTransform, PlayerComponent>();
    
    public PlayerMoveSystem(World world) : base(world)
    {
    }

    public override void Update(in float deltaTime)
    {
        var foundedInputCommands = new Dictionary<int, PlayerInputComponent>();
        
        World.Query(in _playerInputQuery, (ref ReceiveRpcCommandRequest rpcRequest,
            ref PlayerInputComponent playerInput) =>
        {
            if (foundedInputCommands.TryGetValue(rpcRequest.SenderId, out var existPlayerInput))
            {
                if (playerInput.Time > existPlayerInput.Time)
                {
                    foundedInputCommands[rpcRequest.SenderId] = playerInput;
                }
            }
            else
            {
                foundedInputCommands[rpcRequest.SenderId] = playerInput;
            }
        });

        World.Query(in _playerQuery, (ref NetObject netObject, ref NetTransform netTransform) =>
        {
            if (foundedInputCommands.TryGetValue(netObject.OwnerId, out var playerInput))
            {
                netTransform.Position.X += playerInput.Move.X * MoveSpeed;
                netTransform.Position.Z += playerInput.Move.Y * MoveSpeed;
            }
        });
    }
}