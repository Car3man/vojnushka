using System;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using VojnushkaShared.Logger;

namespace VojnushkaShared.NetEcs.Rpc
{
    public class NetDebugRpcSystem : BaseSystem<World, float>
    {
        private readonly ILogger _logger;
        private readonly bool _canSend;
        private readonly bool _canReceive;

        private readonly QueryDescription _queryDescription = new QueryDescription()
            .WithAll<ReceiveRpcCommandRequest, NetDebugRpc>();
        
        private float _sendTimer;
        private float _lastSendTime;

        private const float SendInterval = 5f;
        
        public NetDebugRpcSystem(
            World world, ILogger logger, bool canSend, bool canReceive) : base(world)
        {
            _logger = logger;
            _canSend = canSend;
            _canReceive = canReceive;
        }

        public override void Update(in float deltaTime)
        {
            if (_canSend)
            {
                if (_sendTimer - _lastSendTime > SendInterval)
                {
                    SendDebugRpc();
                    _lastSendTime = _sendTimer;
                }
                _sendTimer += deltaTime;
            }
        
            if (_canReceive)
            {
                ProcessReceivedDebugRpc();
            }
        }
        
        private void SendDebugRpc()
        {
            var uid = Guid.NewGuid().ToString();
            
            var entity = this.World.Create();
            entity.Add<SendRpcCommandRequest>();
            entity.Add(new NetDebugRpc
            {
                Uid = uid
            });
            
            _logger.Log($"[{nameof(NetDebugRpcSystem)}] SendDebugRpc, test rpc uid - {uid}");
        }
        
        private void ProcessReceivedDebugRpc()
        {
            World.Query(in _queryDescription, (ref NetDebugRpc testRpc) =>
            {
                _logger.Log($"[{nameof(NetDebugRpcSystem)}] ProcessReceivedDebugRpc, test rpc uid - {testRpc.Uid}");
            });
        }
    }
}