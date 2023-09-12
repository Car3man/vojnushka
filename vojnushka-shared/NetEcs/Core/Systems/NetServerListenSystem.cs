using Arch.Core;
using Arch.System;
using VojnushkaShared.Logger;
using VojnushkaShared.Net;

namespace VojnushkaShared.NetEcs.Core
{
    public class NetServerListenSystem : BaseSystem<World, float>
    {
        private readonly ILogger _logger;
        private readonly INetListener _netListener;
        private readonly INetConnectConfig _netConnectConfig;

        public NetServerListenSystem(
            World world,
            ILogger logger,
            INetListener netListener,
            INetConnectConfig netConnectConfig
            ) : base(world)
        {
            _logger = logger;
            _netListener = netListener;
            _netConnectConfig = netConnectConfig;
        }

        public override void Initialize()
        {
            _netListener.OnPeerConnect += OnPeerConnect;
            _netListener.OnPeerMessage += OnPeerMessage;
            _netListener.OnPeerDisconnect += OnPeerDisconnect;
            _netListener.Start(_netConnectConfig.Ip, _netConnectConfig.Port);
        }

        public override void Dispose()
        {
            _netListener.OnPeerConnect -= OnPeerConnect;
            _netListener.OnPeerMessage -= OnPeerMessage;
            _netListener.OnPeerDisconnect -= OnPeerDisconnect;
        }

        private void OnPeerConnect(IPeer peer)
        {
            _logger.Log($"[{nameof(NetServerListenSystem)}] OnPeerConnect, peer.Id - ${peer.Id}");
        }

        private void OnPeerMessage(IPeer peer, byte[] data)
        {
            // _logger.Log($"[{nameof(NetServerListenSystem)}] OnPeerMessage, peer.Id - ${peer.Id}, dataLength - ${data.Length}");
        }

        private void OnPeerDisconnect(IPeer peer)
        {
            _logger.Log($"[{nameof(NetServerListenSystem)}] OnPeerDisconnect, peer.Id - ${peer.Id}");
        }
    }
}