namespace VojnushkaShared.NetEcs.Core
{
    public enum MessageType
    {
        None = 0,
        Rpc = 1,
        Snapshot = 2,
        SnapshotAck = 3
    }
}