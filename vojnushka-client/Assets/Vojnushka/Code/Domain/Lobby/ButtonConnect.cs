using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Vojnushka.Lobby
{
    public class ButtonConnect : MonoBehaviour
    {
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            SceneManager.LoadScene("Game");
        }
    }
}