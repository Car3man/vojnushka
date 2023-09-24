using Scellecs.Morpeh;
using VojnushkaShared.Logger;
using VojnushkaShared.NetDriver;

namespace VojnushkaShared.Net
{
    public class NetListenSystem : ISystem
    {
        private readonly ILogger _logger;
        private readonly INetListener _netListener;
        private readonly INetConnectConfig _netConnectConfig;
        
        private const string LogTag = nameof(NetListenSystem);
        
        public World World { get; set; }

        public NetListenSystem(
            ILogger logger,
            INetListener netListener,
            INetConnectConfig netConnectConfig
            )
        {
            _logger = logger;
            _netListener = netListener;
            _netConnectConfig = netConnectConfig;
        }

        public async void OnAwake()
        {
            _netListener.OnPeerConnect += OnPeerConnect;
            _netListener.OnPeerMessage += OnPeerMessage;
            _netListener.OnPeerDisconnect += OnPeerDisconnect;
            await _netListener.StartAsync(_netConnectConfig);
            _logger.Log(LogTag, "Listening...");
        }

        public void OnUpdate(float deltaTime)
        {
        }

        public void Dispose()
        {
            _netListener.OnPeerConnect -= OnPeerConnect;
            _netListener.OnPeerMessage -= OnPeerMessage;
            _netListener.OnPeerDisconnect -= OnPeerDisconnect;
        }
        
        private void OnPeerConnect(IPeer peer)
        {
            _logger.Log(LogTag,$"OnPeerConnect, peer.Id=${peer.Id}");
        }

        private void OnPeerMessage(IPeer peer, byte[] data)
        {
            _logger.Log(LogTag,$"OnMessage, peer.Id=${peer.Id}, byte.Length=${data.Length}");
        }

        private void OnPeerDisconnect(IPeer peer)
        {
            _logger.Log(LogTag,$"OnPeerDisconnect, peer.Id={peer.Id}");
        }
    }
}