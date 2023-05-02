using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.MainMenu
{
    [DisallowMultipleComponent]
    public sealed class MainMenuButtons : MonoBehaviour
    {
        [field: SerializeField] public Button StartOnePlayer { get; private set; }
        [field: SerializeField] public Button StartTwoPlayers { get; private set; }

        private void OnDestroy()
        {
            StartOnePlayer.onClick.RemoveAllListeners();
            StartTwoPlayers.onClick.RemoveAllListeners();
        }
    }
}