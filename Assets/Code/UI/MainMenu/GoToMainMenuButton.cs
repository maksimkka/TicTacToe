using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.UI.MainMenu
{
    [DisallowMultipleComponent]
    public sealed class GoToMainMenuButton : MonoBehaviour
    {
        private Button _button;

        private void Start()
        {
            _button = gameObject.GetComponent<Button>();
            _button.onClick.AddListener(() => SceneManager.LoadScene("Main"));
        }
        

        public void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}