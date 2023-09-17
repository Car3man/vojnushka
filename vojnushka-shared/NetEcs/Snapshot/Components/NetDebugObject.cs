using MessagePack;
using VojnushkaShared.NetEcs.Core;

namespace VojnushkaShared.NetEcs.Snapshot
{
    [MessagePackObject]
    public class NetDebugObject : BaseNetComponent
    {
        [Key(0)] public int SomeNumber;
        
        public override byte[] PackTo()
        {
            return MessagePackSerializer.Serialize(this);
        }

        public override void ParseFrom(byte[] rawBytes)
        {
            var parsed = MessagePackSerializer.Deserialize<NetDebugObject>(rawBytes);
            SomeNumber = parsed.SomeNumber;
        }
    }
}