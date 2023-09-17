using System;
using MessagePack;

namespace VojnushkaShared.NetEcs.Core
{
    [MessagePackObject]
    public class BaseNetComponent : IPackableComponent
    {
        public virtual byte[] PackTo()
        {
            return Array.Empty<byte>();
        }

        public virtual void ParseFrom(byte[] rawBytes)
        {
        }
    }
}