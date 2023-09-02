using UnityEngine;
using UnityEngine.SceneManagement;

namespace Vojnushka.Domain.SceneStarters
{
    public class StartupSceneStarter : MonoBehaviour
    {
        private void Start()
        {
            SceneManager.LoadScene("Game");
        }
    }
}