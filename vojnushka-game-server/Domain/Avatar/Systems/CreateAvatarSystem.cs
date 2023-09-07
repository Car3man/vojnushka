using System.Numerics;
using Arch.Core;
using VojnushkaGameServer.Core;

namespace VojnushkaGameServer.Domain.Avatar;

public class CreateAvatarSystem : IPeerEventSystem
{
    private int _avatarCounter;
    
    public void OnStart(World world)
    {
    }

    public void OnStop(World world)
    {
    }

    public void OnPeerConnect(World world, EntityReference entityRef)
    {
        var avatarEntity = world.Create();
        world.Add(avatarEntity, new NetObject(entityRef));
        world.Add(avatarEntity, new AvatarComponent
        {
            Id = _avatarCounter++,
            Position = Vector3.Zero,
            Rotation = Vector3.Zero
        });
    }

    public void OnPeerDisconnect(World world, EntityReference entityRef)
    {
        var query = new QueryDescription()
            .WithAll<NetObject, AvatarComponent>();

        world.Query(query, (in Entity entity,
            ref NetObject netObject) =>
        {
            if (netObject.OwnerRef == entityRef)
            {
                world.Destroy(entity);
            }
        });
    }
}