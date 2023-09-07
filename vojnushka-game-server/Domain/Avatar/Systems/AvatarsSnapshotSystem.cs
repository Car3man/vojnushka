using System.Numerics;
using Arch.Core;
using VojnushkaGameServer.Core;

namespace VojnushkaGameServer.Domain.Avatar;

public class AvatarsSnapshotSystem : ITickSystem
{
    public void OnStart(World world)
    {
    }

    public void OnStop(World world)
    {
    }

    public void OnTick(World world, float deltaTime)
    {
        world.Destroy(new QueryDescription().WithAll<AvatarsSnapshotComponent>());
        
        var query = new QueryDescription()
            .WithAll<NetObject, AvatarComponent>();

        var length = world.CountEntities(query);
        if (length == 0)
        {
            return;
        }
        var index = 0;
        var ids = new int[length];
        var positions = new Vector3[length];
        var rotations = new Vector3[length];
        
        world.Query(query, (ref NetObject netObject, ref AvatarComponent avatarComponent) =>
        {
            var ownerPeer = world.Get<NetPeer>(netObject.OwnerRef);
            ids[index] = ownerPeer.IdNumber;
            positions[index] = avatarComponent.Position;
            rotations[index] = avatarComponent.Rotation;
            index++;
        });

        var snapshot = world.Create();
        world.Add(snapshot, new AvatarsSnapshotComponent
        {
            Ids = ids,
            Positions = positions,
            Rotations = rotations
        });
    }
}