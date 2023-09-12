using Arch.Core;
using Arch.System;
using VojnushkaGameServer.Domain.MovingCube;
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
        INetListener listener, INetConnectConfig netConnectConfig)
    {
        _world = World.Create();
        _group = new Group<float>(
            new NetServerListenSystem(_world, logger, listener, netConnectConfig),
            new NetTickSystem(_world, true),
            new NetRpcReceiveSystem(_world, listener),
            new NetRpcSendSystem(_world, listener),
            // -- DEBUG new NetDebugRpcSystem(_world, logger, false, true),
            // -- DEBUG new NetDebugSnapshotSystem(_world, logger, true),
            // Game Logic
            // ----------
            new SpawnAndMoveMovingCubeSystem(_world),
            // ----------
            new NetSnapshotSendSystem(_world, listener),
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