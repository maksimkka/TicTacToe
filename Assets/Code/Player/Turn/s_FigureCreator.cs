using Code.Cell;
using Code.Combinations;
using Code.Constants;
using Code.Grid;
using Code.Player.Init;
using Code.UI.Score;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Code.Player.Turn
{
    public sealed class s_FigureCreator : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<c_Player, r_CurrentPlayerTurn>> _playerFilter;
        private readonly EcsFilterInject<Inc<c_GridData>, Exc<m_WinCombination>> _gridFilter;
        private readonly EcsFilterInject<Inc<c_ScoreData>> _scoreDataFilter;

        private readonly EcsPoolInject<r_Swap> r_Swap;

        private readonly EcsPoolInject<c_CellData> c_CellData;
        private readonly EcsPoolInject<r_CurrentPlayerTurn> r_CurrentPlayerTurn;
        private readonly EcsPoolInject<r_PlayerHasMadeAMove> r_PlayerHasMadeMove;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _playerFilter.Value)
            {
                ref var player = ref _playerFilter.Pools.Inc1.Get(entity);
                ref var currentPlayerTurn = ref r_CurrentPlayerTurn.Value.Get(entity);
                CreateFigure(ref player, ref currentPlayerTurn);
                r_CurrentPlayerTurn.Value.Del(entity);
            }
        }

        private void CreateFigure(ref c_Player player, ref r_CurrentPlayerTurn currentPlayerTurn)
        {
            if (player.IsCurrentTurn)
            {
                var posCell = currentPlayerTurn.CellTransform.position;
                var gameObject = Object.Instantiate(player.FigurePrefab, posCell, Quaternion.identity);
                var figureSettings = gameObject.GetComponent<FigureSettings>();

                gameObject.transform.DoTurnAnimation(AnimationsConstants.TargetScaleCellFigure, AnimationsConstants.DurationCellFigure);
                    
                FillCurrentObject(currentPlayerTurn.CurrentObjectHashCode, figureSettings.FigureType,
                    figureSettings.FigureSprite, gameObject);
            }
            
            player.IsCurrentTurn = !player.IsCurrentTurn;
            SwapTurnPlayerMarkers();
        }

        private void FillCurrentObject(int currentHashCode, FigureTypeInsideTheCell figureType, Sprite winImage,
            GameObject figure)
        {
            foreach (var entity in _gridFilter.Value)
            {
                ref var gridData = ref _gridFilter.Pools.Inc1.Get(entity);
                ref var playerHasMadeMove = ref r_PlayerHasMadeMove.Value.Add(entity);
                
                for (int i = 0; i < gridData.GridSideSize; i++)
                {
                    for (int j = 0; j < gridData.GridSideSize; j++)
                    {
                        var cellEntity = gridData.CellsEntity[i, j];
                        ref var cellData = ref c_CellData.Value.Get(cellEntity);
                        
                        if (cellData.CellHashCode != currentHashCode) continue;
                        cellData.CurrentFigureType = figureType;
                        cellData.FigureInCell = figure;
                        playerHasMadeMove.CurrentFigureType = figureType;
                        playerHasMadeMove.CurrentWinImage = winImage;
                    }
                }
            }
        }
        
        private void SwapTurnPlayerMarkers()
        {
            foreach (var entity in _scoreDataFilter.Value)
            {
                if (r_Swap.Value.Has(entity)) return;
                r_Swap.Value.Add(entity);
            }
        }
    }
}