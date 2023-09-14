using System.Numerics;
using MessagePack;
using VojnushkaShared.NetEcs.Core;

namespace VojnushkaShared.NetEcs.Transform
{
    [MessagePackObject]
    public class NetTransform : IPackableComponent
    {
        [Key(0)] public Vector3 Position;
        [IgnoreMember] public Vector3 PrevPosition;
        [IgnoreMember] public Vector3 LastCheckPosition;
        [IgnoreMember] public Vector3 InterpolatedPosition;
        
        public byte[] PackTo()
        {
            return MessagePackSerializer.Serialize(this);
        }

        public void ParseFrom(byte[] rawBytes)
        {
            var parsed = MessagePackSerializer.Deserialize<NetTransform>(rawBytes);
            Position = parsed.Position;
        }
    }
}