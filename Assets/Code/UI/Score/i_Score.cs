using Code.Cell;
using Code.Constants;
using Code.Player.Init;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Code.UI.Score
{
    public sealed class i_Score : IEcsInitSystem
    {
        private readonly EcsPoolInject<c_ScoreData> c_ScoreData = default;
        private readonly EcsCustomInject<PlayerView[]> _playersView = default;

        private readonly int _currentCrossScore;
        private readonly int _currentZeroScore;

        public i_Score()
        {
            if (PlayerPrefs.HasKey(GameConstants.CrossScoreHashKey))
            {
                _currentCrossScore = PlayerPrefs.GetInt(GameConstants.CrossScoreHashKey);
            }

            if (PlayerPrefs.HasKey(GameConstants.ZeroScoreHashKey))
            {
                _currentZeroScore = PlayerPrefs.GetInt(GameConstants.ZeroScoreHashKey);
            }
        }

        public void Init(IEcsSystems systems)
        {
            foreach (var playerView in _playersView.Value)
            {
                var entity = systems.GetWorld().NewEntity();
                ref var scoreData = ref c_ScoreData.Value.Add(entity);
            
                if (playerView.PlayerType == PlayerType.PlayerOne)
                {
                    scoreData.ScoreHashKey = GameConstants.CrossScoreHashKey;
                    scoreData.CurrentScore = _currentCrossScore;
                    scoreData.FigureType = FigureTypeInsideTheCell.Cross;
                }
                
                else
                {
                    scoreData.ScoreHashKey = GameConstants.ZeroScoreHashKey;
                    scoreData.CurrentScore = _currentZeroScore;
                    scoreData.FigureType = FigureTypeInsideTheCell.Zero;
                }
                
                scoreData.ScoreText = playerView.ScoreText;
                scoreData.ScoreText.text = scoreData.CurrentScore.ToString();
                scoreData.PlayerMarker = playerView.TurnPlayerMarker;
            }
        }
    }
}