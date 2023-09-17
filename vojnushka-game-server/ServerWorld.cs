using Arch.Core;
using Arch.System;
using VojnushkaGameServer.Domain.Player;
using VojnushkaShared.Logger;
using VojnushkaShared.Net;
using VojnushkaShared.NetEcs.Core;
using VojnushkaShared.NetEcs.Rpc;
using VojnushkaShared.NetEcs.Snapshot;

namespace VojnushkaGameServer;

internal class ServerWorld : IDisposable
{
    private readonly World _world;
    private readonly Group<float> _group;
    
    public ServerWorld(
        ILogger logger,
        INetListener netListener, INetConnectConfig netConnectConfig)
    {
        _world = World.Create();
        _group = new Group<float>(
            new NetServerListenSystem(_world, logger, netListener, netConnectConfig),
            new NetTimeSystem(_world, logger, netListener),
            new NetRpcReceiveSystem(_world, netListener),
            new NetRpcSendSystem(_world, netListener),
            // -- DEBUG new NetDebugRpcSystem(_world, logger, false, true),
            // -- DEBUG new NetDebugSnapshotSystem(_world, logger, true),
            // Game Logic
            // ----------
            new PlayerSpawnSystem(_world, netListener),
            new PlayerMoveSystem(_world),
            // ----------
            new NetSnapshotSendSystem(_world, netListener),
            new NetCleanUpReceivedRpcSystem(_world)
        );
    }

    public void Initialize()
    {
        _group.Initialize();
    }

    public void Update(in float deltaTime)
    {
        _group.Update(deltaTime);
    }

    public void Dispose()
    {
        _group.Dispose();
        _world.Dispose();
    }
}