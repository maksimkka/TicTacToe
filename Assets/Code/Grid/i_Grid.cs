using Code.Cell;
using Code.Constants;
using Code.Player.Turn;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Code.Grid
{
    public sealed class i_Grid : IEcsInitSystem
    {
        private readonly EcsPoolInject<c_GridData> c_GridData = default;
        private readonly EcsPoolInject<c_CellData> c_CellData = default;
        private readonly EcsPoolInject<r_TurnPlayer> r_TurnPlayer = default;
        private readonly EcsCustomInject<GridSettings> _gridSettings = default;
        private EcsWorld _world;
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            var entity = _world.NewEntity();
            ref var gridData = ref c_GridData.Value.Add(entity);

            var transform = _gridSettings.Value.transform;
            gridData.GridSideSize = GameConstants.GridSideSize;
            gridData.CellPrefab = _gridSettings.Value.CellPrefab;
            gridData.CellSize = _gridSettings.Value.CellSize;
            gridData.CellSpacing = _gridSettings.Value.CellSpacing;
            gridData.GridTransform = transform;
            gridData.FiguresType = new FigureTypeInsideTheCell[gridData.GridSideSize];
            gridData.WinFigures = new GameObject[gridData.GridSideSize];

            r_TurnPlayer.Value.Add(entity);
            CreateCells(ref gridData);
        }
        
        private void CreateCells(ref c_GridData gridData)
        {
            var gridWorldPosition = gridData.GridTransform.position;
            
            gridData.CellsEntity = new int[gridData.GridSideSize, gridData.GridSideSize];

            for (int x = 0; x < gridData.GridSideSize; x++)
            {
                for (int y = 0; y < gridData.GridSideSize; y++)
                {
                    var cellWorldPosition = gridWorldPosition + new Vector3(
                        x * (gridData.CellSize + gridData.CellSpacing),
                        gridData.GridTransform.position.y,
                        y * (gridData.CellSize + gridData.CellSpacing));
                    
                    var cell = Object.Instantiate(gridData.CellPrefab, cellWorldPosition, Quaternion.identity);
                    var cellEntity = _world.NewEntity();
                    gridData.CellsEntity[x,y] = cellEntity;
                    
                    ref var cellData = ref c_CellData.Value.Add(cellEntity);
                    cellData.CellGameObject = cell;
                    cellData.CellHashCode = cell.GetHashCode();
                    cellData.CellGameObject.transform.parent = gridData.GridTransform;
                    cellData.CurrentFigureType = FigureTypeInsideTheCell.Empty;
                }
            }
        }
    }
}