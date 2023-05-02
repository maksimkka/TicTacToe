using Code.Main;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.UI.MainMenu
{
    public class i_MainMenu : IEcsInitSystem
    {
        private readonly EcsCustomInject<MainMenuButtons> _mainMenuButtons;
        public void Init(IEcsSystems systems)
        {
            _mainMenuButtons.Value.StartOnePlayer.onClick.AddListener(() => ClickStartGameButton(GameMode.SinglePlayer));
            _mainMenuButtons.Value.StartTwoPlayers.onClick.AddListener(() => ClickStartGameButton(GameMode.TwoPlayers));
        }
        private void ClickStartGameButton(GameMode gameMode)
        {
            LevelInstance.Instance.SetGameMode(gameMode);
            SceneManager.LoadScene("TicTacToe");
            PlayerPrefs.DeleteAll();
        }
    }
}