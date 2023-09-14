using System;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using VojnushkaShared.Logger;
using VojnushkaShared.NetEcs.Core;

namespace VojnushkaShared.NetEcs.Snapshot
{
    public class NetDebugSnapshotSystem : BaseSystem<World, float>
    {
        private readonly ILogger _logger;
        private readonly bool _isServer;
        private readonly Random _random;
        
        private readonly QueryDescription _netObjectQuery = new QueryDescription()
            .WithAll<NetObject, NetDebugObject>();

        private float _time;
        private float _lastChangeObjectTime;
        private float _lastObjectWatchTime;
        private bool _objectWasDestroyed;
        
        private const float ChangeObjectInterval = 1f;
        private const float ObjectLifetime = 30f;
        private const float WatchObjectInterval = 1f;

        public NetDebugSnapshotSystem(World world, ILogger logger, bool isServer) : base(world)
        {
            _logger = logger;
            _isServer = isServer;
            _random = new Random();
        }

        public override void Initialize()
        {
            if (!_isServer)
            {
                return;
            }

            var netObjectEntity = World.CreateNetObject();
            netObjectEntity.Add(new NetDebugObject
            {
                SomeNumber = _random.Next(0, 1000)
            });
            
            _logger.Log($"[{nameof(NetDebugSnapshotSystem)}] Created NetObject, " +
                        $"id - {netObjectEntity.Get<NetObject>().Id}, " +
                        $"someNumber - {netObjectEntity.Get<NetDebugObject>().SomeNumber}");
        }

        public override void Update(in float deltaTime)
        {
            if (_isServer)
            {
                UpdateOnServer();
            }
            else
            {
                UpdateOnClient();
            }
            
            _time += deltaTime;
        }

        private void UpdateOnServer()
        {
            if (_time - _lastChangeObjectTime >= ChangeObjectInterval)
            {
                World.Query(in _netObjectQuery, (ref NetObject netObject, ref NetDebugObject netDebugObject) =>
                {
                    netDebugObject.SomeNumber = _random.Next(0, 1000);
                    
                    _logger.Log($"[{nameof(NetDebugSnapshotSystem)}] Changed NetObject, " +
                                $"id - {netObject.Id}, " +
                                $"someNumber - {netDebugObject.SomeNumber}");
                });

                _lastChangeObjectTime = _time;
            }

            if (_time >= ObjectLifetime && !_objectWasDestroyed)
            {
                World.Destroy(in _netObjectQuery);
                _objectWasDestroyed = true;
                
                _logger.Log($"[{nameof(NetDebugSnapshotSystem)}] Destroyed NetObject");
            }
        }

        private void UpdateOnClient()
        {
            if (_time - _lastObjectWatchTime >= WatchObjectInterval)
            {
                World.Query(in _netObjectQuery, (ref NetObject netObject, ref NetDebugObject netDebugObject) =>
                {
                    _logger.Log($"[{nameof(NetDebugSnapshotSystem)}] Watching the NetObject, " +
                                $"id - {netObject.Id}, " +
                                $"someNumber - {netDebugObject.SomeNumber}");
                });

                _lastObjectWatchTime = _time;
            }
        }
    }
}