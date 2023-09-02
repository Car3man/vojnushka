using UnityEngine.SceneManagement;

namespace Vojnushka.Domain
{
    public class StartupLoader
    {
        public StartupLoader()
        {
            SceneManager.LoadScene("Startup");
        }
    }
}