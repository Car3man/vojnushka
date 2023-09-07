using System.Numerics;

namespace VojnushkaGameServer.Domain.Avatar;

public struct AvatarsSnapshotComponent
{
    public int[] Ids;
    public Vector3[] Positions;
    public Vector3[] Rotations;
}