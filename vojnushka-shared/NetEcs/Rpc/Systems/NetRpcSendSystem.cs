using System.Collections.Generic;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using MessagePack;
using VojnushkaShared.Net;
using VojnushkaShared.NetEcs.Core;

namespace VojnushkaShared.NetEcs.Rpc
{
    public class NetRpcSendSystem : BaseSystem<World, float>
    {
        private readonly bool _isServer;
        private readonly INetListener? _netListener;
        private readonly INetClient? _netClient;

        private readonly QueryDescription _queryDescription = new QueryDescription()
            .WithAll<SendRpcCommandRequest>();

        public NetRpcSendSystem(World world, INetListener netListener) : base(world)
        {
            _isServer = true;
            _netListener = netListener;
        }
        
        public NetRpcSendSystem(World world, INetClient netClient) : base(world)
        {
            _isServer = false;
            _netClient = netClient;
        }

        public override void Update(in float deltaTime)
        {
            if (!IsAllowToSendRpc())
            {
                return;
            }

            World.Query(_queryDescription, (in Entity entity) =>
            {
                var rpcDto = new RpcData
                {
                    Components = new List<RpcComponentData>()
                };
                
                var entityComponents = entity.GetAllComponents();
                foreach (var entityComponent in entityComponents)
                {
                    if (entityComponent is IPackableComponent packableComponent)
                    {
                        var componentDto = new RpcComponentData
                        {
                            Type = packableComponent.GetType(),
                            RawBytes = packableComponent.PackTo()
                        };
                        
                        rpcDto.Components.Add(componentDto);
                    }
                }

                if (rpcDto.Components.Count > 0)
                {
                    SendRpc(rpcDto);
                }
            });
            
            World.Destroy(_queryDescription);
        }

        private bool IsAllowToSendRpc()
        {
            if (_isServer)
            {
                return _netListener!.IsListening;
            }

            return _netClient!.Connected;
        }

        private void SendRpc(RpcData rpcData)
        {
            byte[] serializedRpc = MessagePackSerializer.Serialize(rpcData);
            byte[] serializedMessage = MessagePackSerializer.Serialize(new MessageData
            {
                Type = MessageType.Rpc,
                RawBytes = serializedRpc
            });
            
            if (_isServer)
            {
                _netListener!.Broadcast(serializedMessage);
            }
            else
            {
                _netClient!.Send(serializedMessage);
            }
        }
    }
}