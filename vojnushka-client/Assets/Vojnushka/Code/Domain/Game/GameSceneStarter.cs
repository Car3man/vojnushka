using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using MessagePack;
using UnityEngine;
using Vojnushka.Infrastructure;
using VojnushkaShared.NetEcs.Snapshot;
using VojnushkaShared.NetEcs.Transform;

namespace Vojnushka.Game
{
    public class GameSceneStarter : MonoBehaviour
    {
        private GameWorld _gameWorld;

        [Inject]
        public void Construct(GameWorld gameWorld)
        {
            _gameWorld = gameWorld;
        }
        
        private void Start()
        {
            _gameWorld.Initialize();
        }

        private void Update()
        {
            _gameWorld.Update(Time.deltaTime);
        }

        private void OnDestroy()
        {
            _gameWorld?.Dispose();
        }
    }
}