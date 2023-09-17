using MessagePack;
using VojnushkaShared.NetEcs.Core;

namespace VojnushkaShared.NetEcs.Rpc
{
    [MessagePackObject]
    public class NetDebugRpc : BaseNetComponent
    {
        [Key(0)] public string Uid;
        
        public override byte[] PackTo()
        {
            return MessagePackSerializer.Serialize(this);
        }

        public override void ParseFrom(byte[] rawBytes)
        {
            var parsed = MessagePackSerializer.Deserialize<NetDebugRpc>(rawBytes);
            Uid = parsed.Uid;
        }
    }
}