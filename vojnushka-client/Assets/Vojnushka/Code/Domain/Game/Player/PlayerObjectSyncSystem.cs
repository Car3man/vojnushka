using System.Collections.Generic;
using Arch.Core;
using Arch.System;
using Vojnushka.VectorConverters;
using VojnushkaShared.Domain.Player;
using VojnushkaShared.NetEcs.Core;
using VojnushkaShared.NetEcs.Snapshot;
using VojnushkaShared.NetEcs.Transform;

namespace Vojnushka.Game.Player
{
    public class PlayerObjectSyncSystem : BaseSystem<World, float>
    {
        private readonly PlayerFactory _playerFactory;
        private readonly Dictionary<int, PlayerObject> _playerObjects;

        private readonly QueryDescription _createEventQuery = new QueryDescription()
            .WithAll<NetObjectCreated>();

        private readonly QueryDescription _destroyEventQuery = new QueryDescription()
            .WithAll<NetObjectDestroyed>();

        private readonly QueryDescription _playerQuery = new QueryDescription()
            .WithAll<NetTransform, PlayerComponent>();

        public PlayerObjectSyncSystem(World world) : base(world)
        {
            _playerFactory = new PlayerFactory();
            _playerObjects = new Dictionary<int, PlayerObject>();
        }

        public override void Update(in float deltaTime)
        {
            ObserveCreateEvents();
            ObserveDestroyEvents();
            SyncPositions();
        }

        private void ObserveCreateEvents()
        {
            World.Query(in _createEventQuery, (ref NetObjectCreated netObjectCreated) =>
            {
                var netObjectReference = netObjectCreated.Reference;
                if (!World.Has<PlayerComponent>(netObjectReference))
                {
                    return;
                }

                var playerNetObject = World.Get<NetObject>(netObjectReference);
                var playerTransform = World.Get<NetTransform>(netObjectReference);
                var playerObject = _playerFactory.Instantiate(playerTransform.Position.GetUnityVector());
                _playerObjects[playerNetObject.Id] = playerObject;
            });
        }
        
        private void ObserveDestroyEvents()
        {
            World.Query(in _destroyEventQuery, (ref NetObjectDestroyed netObjectDestroyed) =>
            {
                var netObjectReference = netObjectDestroyed.Reference;
                if (!World.Has<PlayerComponent>(netObjectReference))
                {
                    return;
                }

                var playerNetObject = World.Get<NetObject>(netObjectReference);
                _playerFactory.Destroy(_playerObjects[playerNetObject.Id]);
                _playerObjects.Remove(playerNetObject.Id);
            });
        }

        private void SyncPositions()
        {
            World.Query(in _playerQuery, (ref NetObject netObject, ref NetTransform netTransform) =>
            {
                if (_playerObjects.TryGetValue(netObject.Id, out var playerObject))
                {
                    playerObject.transform.position = netTransform.Position.GetUnityVector();
                }
            });
        }
    }
}