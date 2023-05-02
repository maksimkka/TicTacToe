using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using TMPro;
using UnityEngine;

namespace Code.UI.Score
{
    public sealed class s_ChangeScore : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<c_ScoreData, r_SendTheWinnerDetails>> _scoreDataFilter;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _scoreDataFilter.Value)
            {
                ref var scoreData = ref _scoreDataFilter.Pools.Inc1.Get(entity);

                SavePlayerPrefs(scoreData.ScoreHashKey, scoreData.CurrentScore, scoreData.ScoreText);
                _scoreDataFilter.Pools.Inc2.Del(entity);
            }
        }

        private void SavePlayerPrefs(string key, int value, TextMeshProUGUI changedText)
        {
            value++;
            changedText.text = value.ToString();
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
        }
    }
}