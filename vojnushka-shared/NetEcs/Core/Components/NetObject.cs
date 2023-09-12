using MessagePack;

namespace VojnushkaShared.NetEcs.Core
{
    [MessagePackObject]
    public struct NetObject : IPackableComponent
    {
        [Key(0)] public int Id;
        [Key(1)] public int SomeNumber;
        
        public byte[] PackTo()
        {
            return MessagePackSerializer.Serialize(this);
        }

        public void ParseFrom(byte[] rawBytes)
        {
            var parsed = MessagePackSerializer.Deserialize<NetObject>(rawBytes);
            Id = parsed.Id;
            SomeNumber = parsed.SomeNumber;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(SomeNumber)}: {SomeNumber}";
        }
    }
}