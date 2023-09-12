using UnityEngine;
using Vojnushka.Infrastructure;

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