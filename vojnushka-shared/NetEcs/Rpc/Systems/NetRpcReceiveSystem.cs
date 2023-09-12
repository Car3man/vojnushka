using System;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using MessagePack;
using VojnushkaShared.Net;
using VojnushkaShared.NetEcs.Core;

namespace VojnushkaShared.NetEcs.Rpc
{
    public class NetRpcReceiveSystem : BaseSystem<World, float>
    {
        private readonly INetListener? _netListener;
        private readonly INetClient? _netClient;

        public NetRpcReceiveSystem(World world, INetListener netListener) : base(world)
        {
            _netListener = netListener;
            _netListener.OnPeerMessage += OnPeerMessage;
        }

        public NetRpcReceiveSystem(World world, INetClient netClient) : base(world)
        {
            _netClient = netClient;
            _netClient.OnMessage += OnServerMessage;
        }

        public override void Dispose()
        {
            if (_netListener != null)
            {
                _netListener.OnPeerMessage -= OnPeerMessage;
            }
            
            if (_netClient != null)
            {
                _netClient.OnMessage -= OnServerMessage;
            }
        }

        private void OnPeerMessage(IPeer peer, byte[] data)
        {
            ProcessReceivedRawMessage(data);
        }

        private void OnServerMessage(byte[] data)
        {
            ProcessReceivedRawMessage(data);
        }

        private void ProcessReceivedRawMessage(byte[] data)
        {
            if (!TryGetRpcMessage(data, out var message))
            {
                return;
            }

            var rpc = MessagePackSerializer.Deserialize<RpcData>(message.RawBytes);
            
            var entity = this.World.Create();
            entity.Add<ReceiveRpcCommandRequest>();
            
            foreach (var rpcComponent in rpc.Components)
            {
                var component = Activator.CreateInstance(rpcComponent.Type);
                var packableComponent = (IPackableComponent)component;
                packableComponent.ParseFrom(rpcComponent.RawBytes);
                entity.Add(component);
            }
        }

        private bool TryGetRpcMessage(byte[] data, out MessageData message)
        {
            var parsedMessage = MessagePackSerializer.Deserialize<MessageData>(data);
            if (parsedMessage.Type != MessageType.Rpc)
            {
                message = default;
                return false;
            }

            message = parsedMessage;
            return true;
        }
    }
}