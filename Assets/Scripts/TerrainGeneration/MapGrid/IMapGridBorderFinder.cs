using MiniProceduralGeneration.Chunk;

namespace MiniProceduralGeneration.MapGrid
{
    public interface IMapGridBorderFinder
    {

        #region - - - - Properties - - - -

        public IChunkShell LeftChunk { get; set; }

        public IChunkShell RightChunk { get; set; }

        public IChunkShell TopChunk { get; set; }

        public IChunkShell BottomChunk { get; set; }

        public int RightmostEdgeCol { get; set; }

        public int LeftMostEdgeCol { get; set; }

        public int TopMostEdgeRow { get; set; }

        public int BottomMostEdgeRow { get; set; }

        #endregion Properties

        #region - - - - Methods  - - - -

        void DefineReferenceChunksInCardinalDirections(IChunkShell[] chunkArray, int mapEdgeSize);
        void FindMapBoundaryIndexes(int chunkDistance, int mapEdgeSize);

        #endregion Methods

    }

}
