using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.EndGameScreen
{
    public sealed class EndScreenSettings : MonoBehaviour
    {
        [field: SerializeField]
        public Image WinPlayerImage { get; private set; }

        [field: SerializeField]
        public TextMeshProUGUI GameEndText { get; private set; }
    }
}