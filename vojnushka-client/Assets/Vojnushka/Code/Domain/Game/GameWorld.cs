using System;
using System.Threading.Tasks;
using Scellecs.Morpeh;
using VojnushkaShared.Net;
using ILogger = VojnushkaShared.Logger.ILogger;

namespace Vojnushka.Game
{
    public class GameWorld : IDisposable
    {
        private readonly ILogger _logger;
        private readonly INetClient _netClient;
        private readonly INetConnectConfig _netConnectConfig;
        
        private readonly World _world;
        private readonly SystemsGroup _group;

        public GameWorld(
            ILogger logger,
            INetClient netClient, INetConnectConfig netConnectConfig)
        {
            _logger = logger;
            _netClient = netClient;
            _netConnectConfig = netConnectConfig;
            
            _world = World.Create();
            _world.UpdateByUnity = false;
            _group = _world.CreateSystemsGroup();
            
            RegisterSystems();
        }
        
        private void RegisterSystems()
        {
        
        }

        public async Task StartConnectAsync()
        {
            await _netClient.ConnectAsync(_netConnectConfig);
            _logger.Log("[GameWorld] Start connect success.");
        }

        public void Update(float deltaTime)
        {
            _world.Update(deltaTime);
        }

        public void Dispose()
        {
            if (!_world.IsDisposed)
            {
                _world.Dispose();
            }
        }
    }
}