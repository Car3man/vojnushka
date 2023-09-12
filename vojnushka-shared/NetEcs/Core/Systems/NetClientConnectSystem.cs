using Arch.Core;
using Arch.System;
using VojnushkaShared.Logger;
using VojnushkaShared.Net;

namespace VojnushkaShared.NetEcs.Core
{
    public class NetClientConnectSystem : BaseSystem<World, float>
    {
        private readonly ILogger _logger;
        private readonly INetClient _netClient;
        private readonly INetConnectConfig _netConnectConfig;

        public NetClientConnectSystem(
            World world,
            ILogger logger,
            INetClient netClient,
            INetConnectConfig netConnectConfig)
            : base(world)
        {
            _logger = logger;
            _netClient = netClient;
            _netConnectConfig = netConnectConfig;
        }
        
        public override void Initialize()
        {
            _netClient.OnConnect += OnConnect;
            _netClient.OnMessage += OnMessage;
            _netClient.OnDisconnect += OnDisconnect;
            _netClient.Connect(_netConnectConfig);
        }

        public override void Dispose()
        {
            _netClient.OnConnect -= OnConnect;
            _netClient.OnMessage -= OnMessage;
            _netClient.OnDisconnect -= OnDisconnect;
        }

        private void OnConnect()
        {
            _logger.Log($"[{nameof(NetClientConnectSystem)}] OnConnect");
        }

        private void OnMessage(byte[] data)
        {
            // _logger.Log($"[{nameof(NetClientConnectSystem)}] OnMessage, data length - {data.Length}");
        }

        private void OnDisconnect()
        {
            _logger.Log($"[{nameof(NetClientConnectSystem)}] OnDisconnect");
        }
    }
}