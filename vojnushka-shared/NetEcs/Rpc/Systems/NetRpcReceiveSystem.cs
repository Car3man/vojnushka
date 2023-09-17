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
        private readonly bool _isServer;

        public NetRpcReceiveSystem(World world, INetListener netListener) : base(world)
        {
            _isServer = true;
            _netListener = netListener;
        }

        public NetRpcReceiveSystem(World world, INetClient netClient) : base(world)
        {
            _isServer = false;
            _netClient = netClient;
        }

        public override void Initialize()
        {
            if (_isServer)
            {
                _netListener!.OnPeerMessage += OnPeerMessage;
            }
            else
            {
                _netClient!.OnMessage += OnServerMessage;
            }
        }

        public override void Dispose()
        {
            if (_isServer)
            {
                _netListener!.OnPeerMessage -= OnPeerMessage;
            }
            else
            {
                _netClient!.OnMessage -= OnServerMessage;
            }
        }

        private void OnPeerMessage(IPeer peer, byte[] data)
        {
            ProcessReceivedRawMessage(peer.IdNumber, data);
        }

        private void OnServerMessage(byte[] data)
        {
            ProcessReceivedRawMessage(-1, data);
        }

        private void ProcessReceivedRawMessage(int senderId, byte[] data)
        {
            if (!TryGetRpcMessage(data, out var message))
            {
                return;
            }

            var rpc = MessagePackSerializer.Deserialize<RpcData>(message.RawBytes);
            
            var entity = this.World.Create();
            entity.Add(new ReceiveRpcCommandRequest
            {
                SenderId = senderId
            });
            
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