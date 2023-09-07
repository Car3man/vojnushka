using Arch.Core;
using Arch.Core.Extensions;
using VojnushkaGameServer.Core;
using VojnushkaGameServer.Domain.Avatar;
using VojnushkaProto.Avatar;
using VojnushkaProto.Common;
using VojnushkaProto.Core;
using VojnushkaProto.SessionSnapshot;
using VojnushkaProto.Utility;

namespace VojnushkaGameServer.Domain.SessionSnapshot;

public class SessionSnapshotSystem : ITickSystem
{
    public void OnStart(World world)
    {
    }

    public void OnStop(World world)
    {
    }

    public void OnTick(World world, float deltaTime)
    {
        var sessionSnapshotMessage = new SessionSnapshotProtoMsg
        {
            Avatars = TryGrabAvatars(world)
        };
        
        if (!ShouldBroadcastSnapshot(sessionSnapshotMessage))
        {
            return;
        }

        var requestEntity = world.Create();
        world.Add(requestEntity, new NetBroadcastRequest
        {
            Message = new ServerProtoMsg
            {
                Type = ServerProtoMsgType.SessionSnapshot,
                Data = MessageUtility.MessageToByteString(sessionSnapshotMessage)
            }
        });
    }

    private bool ShouldBroadcastSnapshot(SessionSnapshotProtoMsg sessionSnapshotMessage)
    {
        return sessionSnapshotMessage.Avatars != null;
    }

    private AvatarsSnapshotProtoMsg? TryGrabAvatars(World world)
    {
        var query = new QueryDescription()
            .WithAll<AvatarsSnapshotComponent>();
        var avatarSnapshotEntityRef = EntityReference.Null;
        world.Query(query, (in Entity entity) =>
        {
            avatarSnapshotEntityRef = entity.Reference();
        });
        if (avatarSnapshotEntityRef == EntityReference.Null)
        {
            return null;
        }
        var avatarSnapshot = world.Get<AvatarsSnapshotComponent>(avatarSnapshotEntityRef);
        var avatarsSnapshotMessage = new AvatarsSnapshotProtoMsg();
        avatarsSnapshotMessage.Id.Add(avatarSnapshot.Ids);
        foreach (var avatarPosition in avatarSnapshot.Positions)
        {
            avatarsSnapshotMessage.Positions.Add(new Vector3ProtoMsg()
            {
                X = avatarPosition.X,
                Y = avatarPosition.Y,
                Z = avatarPosition.Z
            });
        }
        foreach (var avatarRotation in avatarSnapshot.Rotations)
        {
            avatarsSnapshotMessage.Rotations.Add(new Vector3ProtoMsg()
            {
                X = avatarRotation.X,
                Y = avatarRotation.Y,
                Z = avatarRotation.Z
            });
        }
        return avatarsSnapshotMessage;
    }
}