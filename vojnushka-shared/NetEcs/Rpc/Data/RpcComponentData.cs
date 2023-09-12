using System;
using MessagePack;

namespace VojnushkaShared.NetEcs.Rpc
{
    [MessagePackObject]
    public struct RpcComponentData
    {
        [Key(0)] public Type Type;
        [Key(1)] public byte[] RawBytes;
    }
}