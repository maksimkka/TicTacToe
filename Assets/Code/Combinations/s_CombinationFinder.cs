using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Code.Cell;
using Code.Constants;
using Code.Grid;
using Code.Player.Turn;
using Code.UI.EndGameScreen;
using Code.UI.Score;
using Cysharp.Threading.Tasks;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Code.Combinations
{
    public sealed class s_CombinationFinder : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<c_GridData, r_PlayerHasMadeAMove>, Exc<m_WinCombination, m_IsDraw>> _gridFilter;
        private readonly EcsFilterInject<Inc<c_GameEndScreenData>> _gameEndScreenFilter;
        private readonly EcsFilterInject<Inc<c_ScoreData>> _scoreDataFilter;

        private readonly EcsPoolInject<c_CellData> c_CellData;
        private readonly EcsPoolInject<m_WinCombination> r_WinCombination;
        private readonly EcsPoolInject<r_PlayerHasMadeAMove> r_PlayerHasMadeAMove;
        private readonly EcsPoolInject<r_ChangeWinImageAndShowEndGameScreen> r_ChangeWinImageAndShowEndGameScreen;
        private readonly EcsPoolInject<r_SendTheWinnerDetails> r_SendTheWinnerDetails;
        private readonly EcsPoolInject<r_Draw> r_Draw;

        private readonly CancellationTokenSource _tokenSources;

        public s_CombinationFinder(CancellationTokenSource tokenSources)
        {
            _tokenSources = tokenSources;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _gridFilter.Value)
            {
                ref var playerHasMadeAMove = ref r_PlayerHasMadeAMove.Value.Get(entity);

                FindCombinations(entity, playerHasMadeAMove.CurrentFigureType,
                    playerHasMadeAMove.CurrentWinImage);

                r_PlayerHasMadeAMove.Value.Del(entity);
            }
        }

        private void FindCombinations(int gridDataEntity, FigureTypeInsideTheCell figureType,
            Sprite currentWinImage)
        {
            if (FindHorizontalAndVerticalCombinations(gridDataEntity, figureType, currentWinImage)) return;
            CheckDiagonals(gridDataEntity, figureType, currentWinImage);

            if (r_Draw.Value.Has(gridDataEntity)) return;
            r_Draw.Value.Add(gridDataEntity);
        }

        private bool FindHorizontalAndVerticalCombinations(int gridDataEntity,
            FigureTypeInsideTheCell figureType, Sprite currentWinImage)
        {
            ref var gridData = ref _gridFilter.Pools.Inc1.Get(gridDataEntity);
            for (int i = 0; i < gridData.GridSideSize; i++)
            {
                FillArrays(gridDataEntity, i, true);
                if (CheckForVictory(figureType, gridDataEntity, currentWinImage)) return true;

                FillArrays(gridDataEntity, i, false);
                if (CheckForVictory(figureType, gridDataEntity, currentWinImage)) return true;
            }

            return false;
        }

        private void FillArrays(int gridDataEntity, int i, bool isVertical)
        {
            ref var gridData = ref _gridFilter.Pools.Inc1.Get(gridDataEntity);

            for (int j = 0; j < gridData.GridSideSize; j++)
            {
                var cellEntity = isVertical ? gridData.CellsEntity[i, j] : gridData.CellsEntity[j, i];
                ref var cellData = ref c_CellData.Value.Get(cellEntity);
                gridData.FiguresType[j] = cellData.CurrentFigureType;
                gridData.WinFigures[j] = cellData.FigureInCell;
            }
        }

        private bool CheckForVictory(FigureTypeInsideTheCell figureType, int gridDataEntity, Sprite currentWinImage)
        {
            ref var gridData = ref _gridFilter.Pools.Inc1.Get(gridDataEntity);
            if (gridData.FiguresType.All(x => x == figureType))
            {
                AddRequests(gridDataEntity, currentWinImage, figureType);
                AnimationOfTheFiguresOfTheWinningCombination(gridData.WinFigures);
                return true;
            }

            return false;
        }

        private void CheckDiagonals(int gridDataEntity, FigureTypeInsideTheCell figureType,
            Sprite currentWinImage)
        {
            if (FindDiagonalCombination(gridDataEntity, figureType, currentWinImage, true)) return;
            FindDiagonalCombination(gridDataEntity, figureType, currentWinImage, false);
        }

        private void FillArrayDiagonalCombinations(int index, ref c_GridData gridData, bool isDiagonalOne)
        {
            var cellEntity = isDiagonalOne
                ? gridData.CellsEntity[index, index]
                : gridData.CellsEntity[index, gridData.GridSideSize - 1 - index];
            ref var cellData = ref c_CellData.Value.Get(cellEntity);
            gridData.FiguresType[index] = cellData.CurrentFigureType;
            gridData.WinFigures[index] = cellData.FigureInCell;
        }

        private bool FindDiagonalCombination(int gridDataEntity, FigureTypeInsideTheCell figureType,
            Sprite currentWinImage,
            bool isDiagonalOne)
        {
            ref var gridData = ref _gridFilter.Pools.Inc1.Get(gridDataEntity);

            for (int i = 0; i < gridData.GridSideSize; i++)
            {
                FillArrayDiagonalCombinations(i, ref gridData, isDiagonalOne);
            }

            return CheckForVictory(figureType, gridDataEntity, currentWinImage);
        }

        private void AddRequests(int entity, Sprite currentWinSprite, FigureTypeInsideTheCell figureType)
        {
            r_WinCombination.Value.Add(entity);
            ChangeScoreRequest(figureType);
            ChangeWinImageRequest(currentWinSprite);
        }

        private void ChangeScoreRequest(FigureTypeInsideTheCell figureType)
        {
            foreach (var entity in _scoreDataFilter.Value)
            {
                ref var scoreData = ref _scoreDataFilter.Pools.Inc1.Get(entity);
                if (figureType != scoreData.FigureType) continue;
                
                r_SendTheWinnerDetails.Value.Add(entity);
            }
        }

        private void ChangeWinImageRequest(Sprite currentWinSprite)
        {
            foreach (var entity in _gameEndScreenFilter.Value)
            {
                ref var changeWinImageAndShowEndGameScreen = ref r_ChangeWinImageAndShowEndGameScreen.Value.Add(entity);
                changeWinImageAndShowEndGameScreen.CurrentWinImage = currentWinSprite;
            }
        }

        private async void AnimationOfTheFiguresOfTheWinningCombination(IEnumerable<GameObject> winningCells)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.4f), cancellationToken: _tokenSources.Token);

            foreach (var winningCell in winningCells)
            {
                winningCell.transform.DoPulse(AnimationsConstants.DurationCellFigure,
                    AnimationsConstants.TargetScaleCellFigure);
            }
        }
    }
}