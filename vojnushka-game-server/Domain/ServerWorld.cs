using Scellecs.Morpeh;
using VojnushkaShared.Logger;
using VojnushkaShared.Net;
using VojnushkaShared.NetDriver;

namespace VojnushkaGameServer.Domain;

internal class ServerWorld : IDisposable
{
    private readonly ILogger _logger;
    private readonly INetListener _netListener;
    private readonly INetConnectConfig _netConnectConfig;
    
    private readonly World _world;
    private readonly SystemsGroup _group;

    private bool _disposed;

    public ServerWorld(
        ILogger logger,
        INetListener netListener, INetConnectConfig netConnectConfig)
    {
        _logger = logger;
        _netListener = netListener;
        _netConnectConfig = netConnectConfig;
        
        _world = World.Create();
        _group = _world.CreateSystemsGroup();
        _group.AddSystem(new NetListenSystem(_logger, _netListener, _netConnectConfig));
    }

    public async Task InitializeAndStart()
    {
        await _netListener.StartAsync(_netConnectConfig);
        _logger.Log("[ServerWorld] Start listen success.");
    }

    public void Update(float deltaTime)
    {
        _world.Update(deltaTime);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        
        _netListener.Dispose();
        _world.Dispose();
        
        _disposed = true;
    }
}