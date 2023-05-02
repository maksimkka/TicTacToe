using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.UI.EndGameScreen
{
    [DisallowMultipleComponent]
    public sealed class RestartButtonView : MonoBehaviour
    {
        private Button _button;

        private void Start()
        {
            _button = gameObject.GetComponent<Button>();
            _button.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
        }
        

        public void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}