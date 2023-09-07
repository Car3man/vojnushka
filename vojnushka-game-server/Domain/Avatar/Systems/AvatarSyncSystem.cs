using Arch.Core;
using Arch.Core.Extensions;
using VojnushkaGameServer.Core;
using VojnushkaProto.Avatar;
using VojnushkaProto.Core;
using VojnushkaProto.Utility;

namespace VojnushkaGameServer.Domain.Avatar;

public class AvatarSyncSystem : ITickSystem
{
    public void OnStart(World world)
    {
    }

    public void OnStop(World world)
    {
    }

    public void OnTick(World world, float deltaTime)
    {
        var syncQuery = new QueryDescription()
            .WithAll<NetPeerMessage>();
        
        world.Query(syncQuery, (ref NetPeerMessage netPeerMessage) =>
        {
            if (netPeerMessage.Message.Type != ServerProtoMsgType.AvatarSync)
            {
                return;
            }
            
            var avatarSyncMessage = AvatarSyncProtoMsg.Parser.ParseFrom(netPeerMessage.Message.Data);
            var avatarEntityRef = GetAvatarEntityById(world, avatarSyncMessage.Id);
            if (avatarEntityRef == EntityReference.Null)
            {
                return;
            }
            
            var avatarNetObject = world.Get<NetObject>(avatarEntityRef);
            if (!IsPeerOwner(netPeerMessage.EntityRef, avatarNetObject))
            {
                return;
            }

            ref var avatar = ref world.Get<AvatarComponent>(avatarEntityRef);
            avatar.Position = avatarSyncMessage.Position.ToVector3();
            avatar.Rotation = avatarSyncMessage.Rotation.ToVector3();
        });
    }

    private EntityReference GetAvatarEntityById(World world, int id)
    {
        var avatarEntityRef = EntityReference.Null;
        var avatarQuery = new QueryDescription()
            .WithAll<NetObject, AvatarComponent>();
        world.Query(avatarQuery, (in Entity entity, ref AvatarComponent avatar) =>
        {
            if (avatar.Id == id)
            {
                avatarEntityRef = entity.Reference();
            }
        });
        return avatarEntityRef;
    }

    private bool IsPeerOwner(EntityReference requester, NetObject netObject)
    {
        return requester == netObject.OwnerRef;
    }
}