using System.Collections.Generic;
using MessagePack;

namespace VojnushkaShared.NetEcs.Snapshot
{
    [MessagePackObject]
    public class SnapshotObjectData
    {
        [Key(0)] public int Id;
        [Key(1)] public List<SnapshotComponentData> Components = new();
    }
}