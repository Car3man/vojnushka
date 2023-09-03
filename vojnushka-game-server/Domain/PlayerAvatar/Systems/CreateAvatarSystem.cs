using System.Numerics;
using Arch.Core;
using VojnushkaGameServer.Core;

namespace VojnushkaGameServer.Domain.PlayerAvatar;

public class CreateAvatarSystem : IPeerEventSystem
{
    public void OnStart(World world)
    {
    }

    public void OnStop(World world)
    {
    }

    public void OnPeerConnect(World world, EntityReference entityRef)
    {
        var avatarEntity = world.Create();
        world.Add(avatarEntity, new AvatarComponent
        {
            PlayerRef = entityRef,
            MoveSpeed = 1f,
            Position = Vector3.Zero
        });
    }

    public void OnPeerDisconnect(World world, EntityReference entityRef)
    {
        var query = new QueryDescription()
            .WithAll<AvatarComponent>();

        world.Query(query, (in Entity entity, ref AvatarComponent avatar) =>
        {
            if (avatar.PlayerRef == entityRef)
            {
                world.Destroy(entity);
            }
        });
    }
}