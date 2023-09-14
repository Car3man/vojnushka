using System.Collections.Generic;
using MessagePack;

namespace VojnushkaShared.NetEcs.Rpc
{
    [MessagePackObject]
    public struct RpcData
    {
        [Key(0)] public List<RpcComponentData> Components;
    }
}