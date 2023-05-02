using System;
using System.Threading;
using Code.Cell;
using Code.Combinations;
using Code.Constants;
using Code.Grid;
using Code.Main;
using Code.Player.Init;
using Cysharp.Threading.Tasks;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Code.Player.Turn
{
    public sealed class s_PlayerTurner : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<c_GridData, r_TurnPlayer>, Exc<m_WinCombination, m_IsDraw>> _girdFilter;
        private readonly EcsFilterInject<Inc<c_Player>> _playerFilter;
        
        private readonly EcsPoolInject<c_CellData> c_CellData;
        private readonly EcsPoolInject<r_CurrentPlayerTurn> r_CurrentPlayerTurn;
        private readonly EcsPoolInject<r_TurnAI> r_TurnAI;
        private readonly EcsPoolInject<r_TurnPlayer> r_TurnPlayer;
        private readonly EcsCustomInject<Camera> _camera;
        private readonly GameMode _mode;
        private readonly CancellationTokenSource _tokenSources;
        
        public s_PlayerTurner(GameMode mode, CancellationTokenSource tokenSources)
        {
            _mode = mode;
            _tokenSources = tokenSources;
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _girdFilter.Value)
            {
                ref var gridData = ref _girdFilter.Pools.Inc1.Get(entity);
                FindingObjectUsingRay(ref gridData, entity);
            }
        }

        private void FindingObjectUsingRay(ref c_GridData gridData, int gridEntity)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = _camera.Value.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out var hit)) return;
                if (hit.transform.gameObject.layer != Layers.Cell) return;
                
                CheckRaycastObject(gridData, hit.transform.gameObject.GetHashCode(), gridEntity);
            }
        }

        private void CheckRaycastObject(c_GridData gridData, int hashCodeRayObject, int gridEntity)
        {
            for (int i = 0; i < gridData.GridSideSize; i++)
            {
                for (int j = 0; j < gridData.GridSideSize; j++)
                {
                    var cellEntity = gridData.CellsEntity[i, j];
                    ref var cellData = ref c_CellData.Value.Get(cellEntity);
                    if (cellData.CellHashCode != hashCodeRayObject) continue;
                    if (cellData.CurrentFigureType != FigureTypeInsideTheCell.Empty) return;
                    SaveSelectedCell(cellData.CellGameObject.transform, cellData.CellHashCode);
                    TransitTurnFromPlayerToAI(gridEntity);
                }
            }
        }
        
        private void SaveSelectedCell(Transform cellPos, int cellHashCode)
        {
            foreach (var entity in _playerFilter.Value)
            {
                ref var currentPlayerTurn = ref r_CurrentPlayerTurn.Value.Add(entity);
                currentPlayerTurn.CellTransform = cellPos;
                currentPlayerTurn.CurrentObjectHashCode = cellHashCode;
            }
        }

        private async void TransitTurnFromPlayerToAI(int gridEntity)
        {
            if (_mode != GameMode.SinglePlayer) return;
            
            r_TurnPlayer.Value.Del(gridEntity);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: _tokenSources.Token);
            r_TurnAI.Value.Add(gridEntity);
        }
    }
}