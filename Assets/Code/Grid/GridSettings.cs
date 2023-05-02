using UnityEngine;

namespace Code.Grid
{
    [DisallowMultipleComponent]
    public sealed class GridSettings : MonoBehaviour
    {
        [field: SerializeField] public GameObject CellPrefab { get; private set; }
        [field: SerializeField] public float CellSize { get; private set; }
        [field: SerializeField] public float CellSpacing { get; private set; }
    }
}
