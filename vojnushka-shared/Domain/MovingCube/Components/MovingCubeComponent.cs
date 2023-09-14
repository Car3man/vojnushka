using System;
using MessagePack;
using VojnushkaShared.NetEcs.Core;

namespace VojnushkaShared.Domain.MovingCube
{
    [MessagePackObject]
    public class MovingCubeComponent : IPackableComponent
    {
        public byte[] PackTo()
        {
            return Array.Empty<byte>();
        }

        public void ParseFrom(byte[] rawBytes)
        {
        }
    }
}