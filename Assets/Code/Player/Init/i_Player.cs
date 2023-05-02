using Code.Constants;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Code.Player.Init
{
    public sealed class i_Player : IEcsInitSystem
    {
        private readonly EcsPoolInject<c_Player> c_Player;
        private readonly EcsCustomInject<PlayerSettings> _playerSettings;

        public void Init(IEcsSystems systems)
        {
            for (int i = 0; i < GameConstants.CountPlayers; i++)
            {
                var entity = systems.GetWorld().NewEntity();
                ref var player = ref c_Player.Value.Add(entity);

                var isCurrent = (i == 0);
                var prefab = isCurrent
                    ? _playerSettings.Value.CrossPrefab
                    : _playerSettings.Value.ZeroPrefab;
                
                player.FigurePrefab = prefab;
                player.IsCurrentTurn = isCurrent;
            }
        }
    }
}