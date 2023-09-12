using System.Collections.Generic;
using MessagePack;

namespace VojnushkaShared.NetEcs.Snapshot
{
    [MessagePackObject]
    public struct SnapshotData
    {
        [Key(0)] public int Tick;
        [Key(1)] public int DependentOnTick;
        [Key(2)] public List<SnapshotObjectData> Objects;
    }
}