using System.Numerics;
using Arch.Core;
using Arch.Core.Extensions;
using VojnushkaShared.Domain.Player;
using VojnushkaShared.Net;
using VojnushkaShared.NetEcs.Core;

namespace VojnushkaGameServer.Domain.Player;

public class PlayerFactory
{
    private readonly World _world;

    public PlayerFactory(World world)
    {
        _world = world;
    }
    
    public Entity Instantiate(IPeer peer, Vector3 position)
    {
        var playerEntity = _world.CreateOwnedNetObject(peer.IdNumber);
        playerEntity.Add<PlayerComponent>();
        return playerEntity;
    }
}