using MessagePack;
using VojnushkaShared.NetEcs.Core;

namespace VojnushkaShared.Domain.MovingCube
{
    [MessagePackObject]
    public struct MovingCubeComponent : IPackableComponent
    {
        [Key(0)] public float X;
        [Key(1)] public float Y;
        
        public byte[] PackTo()
        {
            return MessagePackSerializer.Serialize(this);
        }

        public void ParseFrom(byte[] rawBytes)
        {
            var parsed = MessagePackSerializer.Deserialize<MovingCubeComponent>(rawBytes);
            X = parsed.X;
            Y = parsed.Y;
        }
    }
}