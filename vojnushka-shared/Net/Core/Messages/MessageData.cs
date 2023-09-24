using MemoryPack;

namespace VojnushkaShared.Net
{
    [MemoryPackable]
    public partial struct MessageData
    {
        public MessageType Type;
        public byte[] Bytes;
    }
}