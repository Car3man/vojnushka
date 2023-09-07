using UnityEngine;
using UnityEngine.SceneManagement;

namespace Vojnushka.Startup
{
    public class StartupSceneStarter : MonoBehaviour
    {
        private void Start()
        {
            SceneManager.LoadScene("Lobby");
        }
    }
}