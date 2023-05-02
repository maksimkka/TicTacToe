using UnityEngine;

namespace Code.Player.Init
{
    [DisallowMultipleComponent]
    public sealed class PlayerSettings : MonoBehaviour
    {
        [field: SerializeField] public GameObject CrossPrefab { get; private set; }
        [field: SerializeField] public GameObject ZeroPrefab { get; private set; }
    }
}