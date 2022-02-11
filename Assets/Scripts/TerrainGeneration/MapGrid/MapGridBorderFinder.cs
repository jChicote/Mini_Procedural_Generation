using MiniProceduralGeneration.Chunk;
using MiniProceduralGeneration.Generator.Utility;

namespace MiniProceduralGeneration.MapGrid
{

    public interface IMapGridBorderFinder
    {

        #region - - - - Properties - - - -

        public ITerrainChunk LeftChunk { get; set; }

        public ITerrainChunk RightChunk { get; set; }

        public ITerrainChunk TopChunk { get; set; }

        public ITerrainChunk BottomChunk { get; set; }

        public int RightmostEdgeCol { get; set; }

        public int LeftMostEdgeCol { get; set; }

        public int TopMostEdgeRow { get; set; }

        public int BottomMostEdgeRow { get; set; }

        #endregion Properties

        #region - - - - Methods  - - - -

        void DefineReferenceChunksInCardinalDirections(ITerrainChunk[] chunkArray, int mapEdgeSize);
        void FindMapBoundaryIndexes(int chunkDistance, int mapEdgeSize);

        #endregion Methods

    }

    public class MapGridBorderFinder : IMapGridBorderFinder
    {

        #region - - - - - - Properties - - - - - -
        public ITerrainChunk LeftChunk { get; set; }
        public ITerrainChunk RightChunk { get; set; }
        public ITerrainChunk TopChunk { get; set; }
        public ITerrainChunk BottomChunk { get; set; }
        public int RightmostEdgeCol { get; set; }
        public int LeftMostEdgeCol { get; set; }
        public int TopMostEdgeRow { get; set; }
        public int BottomMostEdgeRow { get; set; }

        #endregion Properties

        #region - - - - - - Methods - - - - - - -

        public void DefineReferenceChunksInCardinalDirections(ITerrainChunk[] chunkArray, int mapEdgeSize)
        {
            if (chunkArray.Length <= 1) return;

            LeftChunk = chunkArray[MapArrayUtility.GetIndexFromRowAndCol(mapEdgeSize, 0, LeftMostEdgeCol)];
            RightChunk = chunkArray[MapArrayUtility.GetIndexFromRowAndCol(mapEdgeSize, 0, RightmostEdgeCol)];
            TopChunk = chunkArray[MapArrayUtility.GetIndexFromRowAndCol(mapEdgeSize, TopMostEdgeRow, 0)];
            BottomChunk = chunkArray[MapArrayUtility.GetIndexFromRowAndCol(mapEdgeSize, BottomMostEdgeRow, 0)];
        }

        public void FindMapBoundaryIndexes(int chunkDistance, int mapEdgeSize)
        {
            RightmostEdgeCol = chunkDistance * 2;
            LeftMostEdgeCol = 0;
            TopMostEdgeRow = 0;
            BottomMostEdgeRow = mapEdgeSize - 1;
        }

        #endregion Methods

    }

}
