using System;
using MessagePack;

namespace VojnushkaShared.NetEcs.Core
{
    [MessagePackObject]
    public struct PingPongData
    {
        [Key(0)] public DateTime PingTime;
        [Key(1)] public DateTime PongTime;
    }
}