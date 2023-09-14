using System;
using System.Collections.Generic;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using MessagePack;
using VojnushkaShared.Logger;
using VojnushkaShared.Net;

namespace VojnushkaShared.NetEcs.Core
{
    public class NetTimeSystem : BaseSystem<World, float>
    {
        private readonly ILogger _logger;
        private readonly INetListener? _netListener;
        private readonly INetClient? _netClient;
        private readonly bool _isServer;

        private float _time;
        private float _lastPingTime;
        private readonly float _pingFrequency = 1f;

        private readonly QueryDescription _queryDescription = new QueryDescription()
            .WithAll<NetTime>();

        public NetTimeSystem(World world, ILogger logger, INetListener netListener) : base(world)
        {
            _logger = logger;
            _netListener = netListener;
            _netListener.OnPeerMessage += OnPeerMessage;
            _isServer = true;
        }
        
        public NetTimeSystem(World world, ILogger logger, INetClient netClient) : base(world)
        {
            _logger = logger;
            _netClient = netClient;
            _netClient.OnMessage += OnServerMessage;
            _isServer = false;
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

        public override void Initialize()
        {
            var entity = this.World.Create();
            entity.Add(new NetTime());
        }

        public override void Update(in float deltaTime)
        {
            if (_isServer)
            {
                World.Query(in _queryDescription, (ref NetTime netTime) =>
                {
                    netTime.Tick += 1;
                    netTime.LastTickTime = DateTime.UtcNow;
                });
            }
            else
            {
                if (_time - _lastPingTime >= _pingFrequency)
                {
                    Ping();
                    _lastPingTime = _time;
                }
            }

            _time += deltaTime;
        }

        private void Ping()
        {
            var pingPongData = new PingPongData
            {
                PingTime = DateTime.UtcNow
            };
            var message = new MessageData
            {
                Type = MessageType.PingPong,
                RawBytes = MessagePackSerializer.Serialize(pingPongData)
            };
            _netClient!.Send(MessagePackSerializer.Serialize(message));
        }
        
        private void OnServerMessage(byte[] data)
        {
            if (!TryGetPingPongMessage(data, out var pingPongData))
            {
                return;
            }

            var dateDiff = pingPongData.PongTime - pingPongData.PingTime;
            var ping = (int)dateDiff.TotalMilliseconds;
            World.SetNetPing(ping);
            
            // _logger.Log($"[{nameof(NetTimeSystem)}] Ping calculated: {ping}");
        }

        private void OnPeerMessage(IPeer peer, byte[] data)
        {
            if (!TryGetPingPongMessage(data, out var pingPongData))
            {
                return;
            }
            
            pingPongData.PongTime = DateTime.UtcNow;
            var responseMessage = new MessageData
            {
                Type = MessageType.PingPong,
                RawBytes = MessagePackSerializer.Serialize(pingPongData)
            };
            _netListener!.Send(peer, MessagePackSerializer.Serialize(responseMessage));
        }
        
        private bool TryGetPingPongMessage(byte[] data, out PingPongData pingPongData)
        {
            var message = MessagePackSerializer.Deserialize<MessageData>(data);
            if (message.Type != MessageType.PingPong)
            {
                pingPongData = default;
                return false;
            }

            pingPongData = MessagePackSerializer.Deserialize<PingPongData>(message.RawBytes);
            return true;
        }
    }
}