using UnityEngine;

namespace Code.Cell
{
    [DisallowMultipleComponent]
    public sealed class FigureSettings : MonoBehaviour
    {
        [field: SerializeField] public FigureTypeInsideTheCell FigureType { get; private set; }
        [field: SerializeField] public Sprite FigureSprite { get; private set; }
    }
}