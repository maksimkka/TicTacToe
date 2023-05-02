using Code.Cell;
using Code.Combinations;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Code.UI.EndGameScreen
{
    public sealed class s_ShowEndGameScreen : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<c_GameEndScreenData, r_ChangeWinImageAndShowEndGameScreen>> _winGameEndScreenFilter;
        private readonly EcsFilterInject<Inc<c_GameEndScreenData, r_ChangeDrawTextAndShowEndGameScreen>> _drawGameEndScreenFilter;

        public void Run(IEcsSystems systems)
        {
            ChangeTextAndImageIfWin();
            ChangeTextAndImageIfDraw();
        }

        private void ChangeTextAndImageIfWin()
        {
            foreach (var entity in _winGameEndScreenFilter.Value)
            {
                ref var gameEndScreenData = ref _winGameEndScreenFilter.Pools.Inc1.Get(entity);
                ref var changeWinImageAndShowEndGameScreen = ref _winGameEndScreenFilter.Pools.Inc2.Get(entity);
                
                gameEndScreenData.GameEndScreenGameObject.SetActive(true);
                gameEndScreenData.GameEndScreenGameObject.transform.DoUIAppearEffect();
                gameEndScreenData.WinPlayerImage.sprite = changeWinImageAndShowEndGameScreen.CurrentWinImage;
                
                _winGameEndScreenFilter.Pools.Inc2.Del(entity);
            }
        }

        private void ChangeTextAndImageIfDraw()
        {
            foreach (var entity in _drawGameEndScreenFilter.Value)
            {
                ref var gameEndScreenData = ref _drawGameEndScreenFilter.Pools.Inc1.Get(entity);

                gameEndScreenData.GameEndScreenGameObject.SetActive(true);
                gameEndScreenData.GameEndScreenGameObject.transform.DoUIAppearEffect();
                gameEndScreenData.GameEndText.text = "     DRAW";
                gameEndScreenData.WinPlayerImage.enabled = false;
                
                _drawGameEndScreenFilter.Pools.Inc2.Del(entity);
            }
        }
    }
}