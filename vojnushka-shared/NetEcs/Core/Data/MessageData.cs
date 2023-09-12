using MessagePack;

namespace VojnushkaShared.NetEcs.Core
{
    [MessagePackObject]
    public struct MessageData
    {
        [Key(0)] public MessageType Type;
        [Key(1)] public byte[] RawBytes;
    }
}