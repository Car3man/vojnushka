using MessagePack;
using VojnushkaShared.NetEcs.Core;

namespace VojnushkaShared.NetEcs.Snapshot
{
    [MessagePackObject]
    public class NetDebugObject : IPackableComponent
    {
        [Key(0)] public int SomeNumber;
        
        public byte[] PackTo()
        {
            return MessagePackSerializer.Serialize(this);
        }

        public void ParseFrom(byte[] rawBytes)
        {
            var parsed = MessagePackSerializer.Deserialize<NetDebugObject>(rawBytes);
            SomeNumber = parsed.SomeNumber;
        }
    }
}