using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Code.UI.EndGameScreen
{
    public sealed class i_EndGameScreen : IEcsInitSystem
    {
        private readonly EcsPoolInject<c_GameEndScreenData> c_GameEndScreenData;
        private readonly EcsCustomInject<EndScreenSettings> _endScreenSettings;
        
        public void Init(IEcsSystems systems)
        {
            var entity = systems.GetWorld().NewEntity();

            ref var gameEndScreenData = ref c_GameEndScreenData.Value.Add(entity);

            gameEndScreenData.GameEndScreenGameObject = _endScreenSettings.Value.gameObject;
            gameEndScreenData.WinPlayerImage = _endScreenSettings.Value.WinPlayerImage;
            gameEndScreenData.GameEndText = _endScreenSettings.Value.GameEndText;
        }
    }
}