using Scellecs.Morpeh;
using VojnushkaShared.Logger;
using VojnushkaShared.NetDriver;

namespace VojnushkaShared.Net
{
    public class NetConnectSystem : ISystem
    {
        private readonly ILogger _logger;
        private readonly INetClient _netClient;
        private readonly INetConnectConfig _netConnectConfig;
        
        private const string LogTag = nameof(NetConnectSystem);
        
        public World World { get; set; }

        public NetConnectSystem(
            ILogger logger,
            INetClient netClient,
            INetConnectConfig netConnectConfig
            )
        {
            _logger = logger;
            _netClient = netClient;
            _netConnectConfig = netConnectConfig;
        }

        public async void OnAwake()
        {
            _netClient.OnConnect += OnConnect;
            _netClient.OnMessage += OnMessage;
            _netClient.OnDisconnect += OnDisconnect;
            await _netClient.ConnectAsync(_netConnectConfig);
            _logger.Log(LogTag, "Connecting...");
        }

        public void OnUpdate(float deltaTime)
        {
        }

        public void Dispose()
        {
            _netClient.OnConnect -= OnConnect;
            _netClient.OnMessage -= OnMessage;
            _netClient.OnDisconnect -= OnDisconnect;
        }
        
        private void OnConnect()
        {
            _logger.Log(LogTag, "OnConnect");
        }

        private void OnMessage(byte[] data)
        {
            _logger.Log(LogTag,$"OnMessage, data.Length={data.Length}");
        }

        private void OnDisconnect()
        {
            _logger.Log(LogTag,"OnDisconnect");
        }
    }
}