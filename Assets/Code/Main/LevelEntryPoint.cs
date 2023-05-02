using System;
using System.Collections.Generic;
using System.Threading;
using Code.Combinations;
using Code.Grid;
using Code.Player.Init;
using Code.Player.Turn;
using Code.UI.EndGameScreen;
using Code.UI.MainMenu;
using Code.UI.Score;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Main
{
    [DisallowMultipleComponent]
    public sealed class LevelEntryPoint : MonoBehaviour
    {
        private readonly Dictionary<SystemType, EcsSystems> _systems = new();
        private readonly CancellationTokenSource _tokenSources = new();

        private EcsWorld _world;
        private EcsSystems _updateSystem;
        private GridSettings _gridSettings;
        private Camera _camera;

        private void Start()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                LevelInstance.Instance.SetGameMode(GameMode.StayMenu);
            }

            InitECS();
        }

        private void DistributeDataBetweenGameModes()
        {
            var mode = LevelInstance.Instance.CurrentGameMode;

            switch (mode)
            {
                case GameMode.StayMenu:
                    AddMainMenuSystems();
                    InjectMainMenuObjects();
                    break;
                case GameMode.SinglePlayer:
                    AddGameSystems(GameMode.SinglePlayer);
                    InjectGameObjects();
                    break;
                case GameMode.TwoPlayers:
                    AddGameSystems(GameMode.TwoPlayers);
                    InjectGameObjects();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void InitECS()
        {
            _world = new EcsWorld();
            var systemTypes = Enum.GetValues(typeof(SystemType));
            foreach (var item in systemTypes)
            {
                _systems.Add((SystemType)item, new EcsSystems(_world));
            }

#if UNITY_EDITOR
            AddDebugSystems();
#endif

            DistributeDataBetweenGameModes();

            foreach (var system in _systems)
            {
                system.Value.Init();
            }
        }

        private void InjectGameObjects()
        {
            var gridSettings = FindObjectOfType<GridSettings>();
            var playerSettings = FindObjectOfType<PlayerSettings>();
            var endScreenSettings = FindObjectOfType<EndScreenSettings>(true);
            var playersView = FindObjectsOfType<PlayerView>(true);
            _camera = Camera.main;
            
            foreach (var system in _systems)
            {
                system.Value
                    .Inject(gridSettings, playerSettings, endScreenSettings, playersView, _camera);
            }
        }

        private void InjectMainMenuObjects()
        {
            var mainMenuButtons = FindObjectOfType<MainMenuButtons>();
            
            foreach (var system in _systems)
            {
                system.Value
                    .Inject(mainMenuButtons);
            }
        }

        private void Update()
        {
            _systems[SystemType.Update].Run();
        }

        private void AddDebugSystems()
        {
#if UNITY_EDITOR
            _systems[SystemType.Update].Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
#endif
        }

        private void AddMainMenuSystems()
        {
            _systems[SystemType.Init]
                .Add(new i_MainMenu());
        }

        private void AddGameSystems(GameMode mode)
        {
            _systems[SystemType.Init]
                .Add(new i_Grid())
                .Add(new i_Player())
                .Add(new i_EndGameScreen())
                .Add(new i_Score());

            _systems[SystemType.Update]
                .Add(new s_PlayerTurner(mode, _tokenSources))
                .Add(new s_AITurner())
                .Add(new s_FigureCreator())
                .Add(new s_TurnPlayerMarkersSwapper())
                .Add(new s_CombinationFinder(_tokenSources))
                .Add(new s_DrawChecker())
                .Add(new s_ShowEndGameScreen())
                .Add(new s_ChangeScore());
        }

        private void OnDestroy()
        {
            _world?.Destroy();
            foreach (var system in _systems)
            {
                system.Value.Destroy();
            }

            _tokenSources.Cancel();
            _tokenSources.Dispose();
        }
    }
}