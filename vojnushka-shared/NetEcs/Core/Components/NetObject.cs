using MessagePack;

namespace VojnushkaShared.NetEcs.Core
{
    [MessagePackObject]
    public class NetObject : BaseNetComponent
    {
        [Key(0)] public int Id;
        [Key(1)] public int OwnerId;
        
        public override byte[] PackTo()
        {
            return MessagePackSerializer.Serialize(this);
        }

        public override void ParseFrom(byte[] rawBytes)
        {
            var parsed = MessagePackSerializer.Deserialize<NetObject>(rawBytes);
            Id = parsed.Id;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}";
        }
    }
}