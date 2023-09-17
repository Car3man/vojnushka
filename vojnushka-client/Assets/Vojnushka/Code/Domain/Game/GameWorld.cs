using System;
using Arch.Core;
using Arch.System;
using Vojnushka.Game.Player;
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
            INetClient netClient, INetConnectConfig netConnectConfig)
        {
            _world = World.Create();
            _group = new Group<float>(
                new NetClientConnectSystem(_world, logger, netClient, netConnectConfig),
                new NetTimeSystem(_world, logger, netClient),
                new NetSnapshotReceiveSystem(_world, netClient),
                new NetRpcReceiveSystem(_world, netClient),
                new NetRpcSendSystem(_world, netClient),
                new NetInterpolateTransformSystem(_world),
                // -- DEBUG new NetDebugRpcSystem(_world, logger, true, false),
                // -- DEBUG new NetDebugSnapshotSystem(_world, logger, false),
                // Game Logic
                // ----------
                new PlayerInputSystem(_world),
                new PlayerObjectSyncSystem(_world),
                // ----------
                new NetSnapshotCleanUpSystem(_world),
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