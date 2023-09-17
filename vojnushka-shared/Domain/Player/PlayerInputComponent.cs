using System;
using System.Numerics;
using MessagePack;
using VojnushkaShared.NetEcs.Core;

namespace VojnushkaShared.Domain.Player
{
    [MessagePackObject]
    public class PlayerInputComponent : BaseNetComponent
    {
        [Key(0)] public DateTime Time;
        [Key(1)] public Vector2 Move;

        public override byte[] PackTo()
        {
            return MessagePackSerializer.Serialize(this);
        }

        public override void ParseFrom(byte[] rawBytes)
        {
            var parsed = MessagePackSerializer.Deserialize<PlayerInputComponent>(rawBytes);
            Time = parsed.Time;
            Move = parsed.Move;
        }
    }
}