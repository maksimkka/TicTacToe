using Code.Player.Init;
using TMPro;
using UnityEngine;

public class PlayerView : MonoBehaviour
{ 
    [field: SerializeField] public PlayerType PlayerType { get; private set; }
    [field: SerializeField] public TextMeshProUGUI ScoreText { get; private set; }
    [field: SerializeField] public GameObject TurnPlayerMarker { get; private set; }
}