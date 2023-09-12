using MessagePack;

namespace VojnushkaShared.NetEcs.Snapshot
{
    [MessagePackObject]
    public struct SnapshotComponentValueData
    {
        [Key(0)] public int KeyId;
        [Key(1)] public byte[] RawBytes;
    }
}