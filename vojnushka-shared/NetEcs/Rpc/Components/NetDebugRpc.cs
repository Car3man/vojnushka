using MessagePack;
using VojnushkaShared.NetEcs.Core;

namespace VojnushkaShared.NetEcs.Rpc
{
    [MessagePackObject]
    public struct NetDebugRpc : IPackableComponent
    {
        [Key(0)] public string Uid;
        
        public byte[] PackTo()
        {
            return MessagePackSerializer.Serialize(this);
        }

        public void ParseFrom(byte[] rawBytes)
        {
            var parsed = MessagePackSerializer.Deserialize<NetDebugRpc>(rawBytes);
            Uid = parsed.Uid;
        }
    }
}