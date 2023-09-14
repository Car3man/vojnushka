using System;
using Arch.Core;
using Arch.System;
using UnityEngine;
using Vojnushka.Game.MovingCube.Systems;
using VojnushkaShared.Net;
using VojnushkaShared.NetEcs.Core;
using VojnushkaShared.NetEcs.Rpc;
using VojnushkaShared.NetEcs.Snapshot;
using VojnushkaShared.NetEcs.Transform;
using ILogger = VojnushkaShared.Logger.ILogger;

namespace Vojnushka.Game
{
    public class GameWorld : IDisposable
    {
        private readonly World _world;
        private readonly Group<float> _group;
    
        public GameWorld(
            ILogger logger,
            INetClient netClient, INetConnectConfig netConnectConfig,
            Mesh movingCubeMesh, Material movingCubeMaterial)
        {
            _world = World.Create();
            _group = new Group<float>(
                new NetClientConnectSystem(_world, logger, netClient, netConnectConfig),
                new NetTimeSystem(_world, logger, netClient),
                new NetSnapshotReceiveSystem(_world, logger, netClient),
                new NetRpcReceiveSystem(_world, netClient),
                new NetRpcSendSystem(_world, netClient),
                new NetInterpolateTransformSystem(_world, logger),
                // -- DEBUG new NetDebugRpcSystem(_world, logger, true, false),
                // -- DEBUG new NetDebugSnapshotSystem(_world, logger, false),
                // Game Logic
                // ----------
                new MovingCubeRenderSystem(_world, movingCubeMesh, movingCubeMaterial),
                // ----------
                new NetSnapshotTrailCleanUpSystem(_world),
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
}