using Code.Cell;
using TMPro;
using UnityEngine;

namespace Code.UI.Score
{
    public struct c_ScoreData
    {
        public FigureTypeInsideTheCell FigureType;
        public int CurrentScore;
        public string ScoreHashKey;
        public TextMeshProUGUI ScoreText;
        public GameObject PlayerMarker;
    }
}