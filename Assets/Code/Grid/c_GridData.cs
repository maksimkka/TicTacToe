using Code.Cell;
using UnityEngine;

namespace Code.Grid
{
    public struct c_GridData
    {
        public FigureTypeInsideTheCell[] FiguresType;
        public GameObject[] WinFigures;
        public int[,] CellsEntity;
        public GameObject CellPrefab;
        public Transform GridTransform;
        public int GridSideSize;
        public float CellSize;
        public float CellSpacing;
    }
}