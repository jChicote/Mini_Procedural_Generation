using UnityEngine;

namespace MiniProceduralGeneration.TerrainCore.Infrastructure.ChunkDimensionsResolver
{

    public class MinimumLevelOfDetailIncrementFinder
    {

        #region - - - - - - Properties - - - - - -

        private ITerrainAttributes TerrainAttributes { get; set; }

        private int MinimumLevelOfDetail { get; set; }
        private int LevelOfDetail { get; set; }

        #endregion Properties

        #region - - - - - - Constructors - - - - - -

        public MinimumLevelOfDetailIncrementFinder(ITerrainAttributes attributes)
        {
            this.TerrainAttributes = attributes;
        }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        public int CalculateLevelOfDetailIncrement(int levelOfDetail)
        {
            LevelOfDetail = levelOfDetail;
            MinimumLevelOfDetail = FindMininmumAllowableLevelOfDetail(0, TerrainAttributes.RenderChunkSize);

            if (levelOfDetail > MinimumLevelOfDetail)
                LevelOfDetail = MinimumLevelOfDetail;

            // provides the step detail value for each side of mesh
            return (int)(LevelOfDetail == 0 ? 1 : LevelOfDetail * 2);
        }

        public int FindMininmumAllowableLevelOfDetail(int currentLevelOfDetail, int chunkWidth)
        {
            int nextLodStep = currentLevelOfDetail == 0 ? 1 : currentLevelOfDetail * 2;
            int vertexPerSide = Mathf.RoundToInt(chunkWidth / nextLodStep);
            float squaredSize = vertexPerSide * vertexPerSide;

            if (squaredSize % 2f == 0f)
            {
                currentLevelOfDetail++;
                return FindMininmumAllowableLevelOfDetail(currentLevelOfDetail, chunkWidth);
            }

            currentLevelOfDetail--;
            return currentLevelOfDetail;
        }

        #endregion Methods

    }

}