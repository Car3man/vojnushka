using System;
using System.Collections.Generic;
using MessagePack;

namespace VojnushkaShared.NetEcs.Snapshot
{
    [MessagePackObject]
    public class SnapshotComponentData
    {
        [Key(0)] public Type? Type;
        [Key(1)] public List<SnapshotComponentValueData> Values = new();
    }
}