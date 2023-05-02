using Code.UI.Score;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Code.Player.Turn
{
    public sealed class s_TurnPlayerMarkersSwapper : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<c_ScoreData, r_Swap>> _scoreDataFilter;

        public void Run(IEcsSystems systems)
        {
            SwapTurnPlayerMarkers();
        }
        
        private void SwapTurnPlayerMarkers()
        {
            foreach (var entity in _scoreDataFilter.Value)
            {
                ref var gameEndScreenData = ref _scoreDataFilter.Pools.Inc1.Get(entity);

                var isFirstPlayerTurn = gameEndScreenData.PlayerMarker.activeSelf;

                gameEndScreenData.PlayerMarker.SetActive(!isFirstPlayerTurn);
                _scoreDataFilter.Pools.Inc2.Del(entity);
            }
        }
    }
}