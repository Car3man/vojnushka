using System.Collections.Generic;
using MessagePack;
using VojnushkaShared.NetEcs.Core;

namespace VojnushkaShared.NetEcs.Rpc
{
    [MessagePackObject]
    public struct RpcData
    {
        [Key(0)] public List<RpcComponentData> Components;
    }
}