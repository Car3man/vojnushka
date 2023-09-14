using MessagePack;

namespace VojnushkaShared.NetEcs.Core
{
    [MessagePackObject]
    public class NetObject : IPackableComponent
    {
        [Key(0)] public int Id;
        
        public byte[] PackTo()
        {
            return MessagePackSerializer.Serialize(this);
        }

        public void ParseFrom(byte[] rawBytes)
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