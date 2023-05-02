using UnityEngine;

namespace Code.Cell
{
    public struct c_CellData
    {
        public FigureTypeInsideTheCell CurrentFigureType;
        public GameObject CellGameObject;
        public GameObject FigureInCell;
        public int CellHashCode;
    }
}