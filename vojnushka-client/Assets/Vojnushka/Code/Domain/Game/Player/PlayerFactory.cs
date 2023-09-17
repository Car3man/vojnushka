using UnityEngine;

namespace Vojnushka.Game.Player
{
    public class PlayerFactory
    {
        public PlayerObject Instantiate(Vector3 position)
        {
            var playerPrefab = Resources.Load<PlayerObject>("Player");
            var playerInstance = Object.Instantiate(playerPrefab, position, Quaternion.identity);
            return playerInstance;
        }

        public void Destroy(PlayerObject playerObject)
        {
            Object.Destroy(playerObject.gameObject);
        }
    }
}