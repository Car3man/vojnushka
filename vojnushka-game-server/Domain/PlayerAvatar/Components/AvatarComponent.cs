using System.Numerics;
using Arch.Core;

namespace VojnushkaGameServer.Domain.PlayerAvatar;

public struct AvatarComponent
{
    public EntityReference PlayerRef;
    public float MoveSpeed;
    public Vector3 Position;
}