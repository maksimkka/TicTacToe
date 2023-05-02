using System.Collections.Generic;
using Code.Cell;
using Code.Combinations;
using Code.Grid;
using Code.Player.Init;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Player.Turn
{
    public sealed class s_AITurner : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<c_GridData, r_TurnAI>, Exc<m_WinCombination, m_IsDraw>> _girdAiFilter;
        private readonly EcsFilterInject<Inc<c_Player>> _playerFilter;
        private readonly EcsFilterInject<Inc<c_CellData>> _cellDataFilter;

        private readonly EcsPoolInject<c_CellData> c_CellData;
        private readonly EcsPoolInject<r_TurnAI> r_TurnAI;
        private readonly EcsPoolInject<r_TurnPlayer> r_TurnPlayer;
        private readonly EcsPoolInject<r_CurrentPlayerTurn> r_CurrentPlayerTurn;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _girdAiFilter.Value)
            {
                ref var r_turnAI = ref r_TurnAI.Value.Get(entity);
                FillEmptyCellArray(ref r_turnAI);
                TurnAi(ref r_turnAI, entity);
            }
        }
        private void FillEmptyCellArray(ref r_TurnAI r_turnAI)
        {
            r_turnAI.EmptyCellsEntity = new List<int>();

            foreach (var cellEntity in _cellDataFilter.Value)
            {
                ref var cellData = ref c_CellData.Value.Get(cellEntity);

                if (cellData.CurrentFigureType == FigureTypeInsideTheCell.Empty)
                {
                    r_turnAI.EmptyCellsEntity.Add(cellEntity);
                }
            }
        }

        private void TurnAi(ref r_TurnAI r_turnAI, int girdDataEntity)
        {
            int randomEmptyCellIndex = Random.Range(0, r_turnAI.EmptyCellsEntity.Count);
            ref var cellData = ref c_CellData.Value.Get(r_turnAI.EmptyCellsEntity[randomEmptyCellIndex]);
            SaveSelectCell(cellData.CellGameObject.transform, cellData.CellHashCode);
            r_TurnAI.Value.Del(girdDataEntity);
            r_TurnPlayer.Value.Add(girdDataEntity);
            
        }

        private void SaveSelectCell(Transform cellPos, int cellHashCode)
        {
            foreach (var entity in _playerFilter.Value)
            {
                ref var currentPlayerTurn = ref r_CurrentPlayerTurn.Value.Add(entity);
                currentPlayerTurn.CellTransform = cellPos;
                currentPlayerTurn.CurrentObjectHashCode = cellHashCode;
            }
        }
    }
}