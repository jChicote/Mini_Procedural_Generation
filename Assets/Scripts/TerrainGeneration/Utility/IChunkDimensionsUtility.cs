using MiniProceduralGeneration.Generator.Entities;

namespace MiniProceduralGeneration.Utility
{

    public interface IChunkDimensionsUtility
    {

        #region - - - - - - Methods - - - - - -

        TerrainChunkDimensions CalculateChunkDimensions();
        void CalculateLevelOfDetail();
        int FindMininmumAllowableLevelOfDetail(int currentLevelOfDetail, int chunkWidth);

        #endregion Methods

    }

}
