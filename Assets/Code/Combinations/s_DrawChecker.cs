using Code.Cell;
using Code.Grid;
using Code.UI.EndGameScreen;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Code.Combinations
{
    public sealed class s_DrawChecker : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<c_GridData, r_Draw>, Exc<m_WinCombination>> _gridFilter;
        private readonly EcsFilterInject<Inc<c_GameEndScreenData>> _gameEndScreenFilter;
        private readonly EcsPoolInject<c_CellData> c_CellData;
        private readonly EcsPoolInject<r_ChangeDrawTextAndShowEndGameScreen> r_ChangeDrawTextAndShowEndGameScreen;
        private readonly EcsPoolInject<m_IsDraw> m_IsDraw;
        
        private readonly EcsPoolInject<r_Draw> r_Draw;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _gridFilter.Value)
            {
                ref var gridData = ref _gridFilter.Pools.Inc1.Get(entity);

                if (!IsAnyCellEmpty(ref gridData))
                {
                    m_IsDraw.Value.Add(entity);
                    AddDrawMarkerRequest();
                }
                
                r_Draw.Value.Del(entity);
            }
        }
        
        private bool IsAnyCellEmpty(ref c_GridData GridData)
        {
            for (int i = 0; i < GridData.GridSideSize; i++)
            {
                for (int j = 0; j < GridData.GridSideSize; j++)
                {
                    var cellEntity = GridData.CellsEntity[i, j];
                    ref var cellData = ref c_CellData.Value.Get(cellEntity);
                    
                    if (cellData.CurrentFigureType == FigureTypeInsideTheCell.Empty)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        private void AddDrawMarkerRequest()
        {
            foreach (var entity in _gameEndScreenFilter.Value)
            {
                r_ChangeDrawTextAndShowEndGameScreen.Value.Add(entity);
            }
        }
    }
}