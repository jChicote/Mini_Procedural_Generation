using MiniProceduralGeneration.Generator.Entities;
using MiniProceduralGeneration.TerrainCore;
using UnityEngine;

namespace MiniProceduralGeneration.Utility
{

    public class ChunkDimensionsUtility : IChunkDimensionsUtility
    {

        #region - - - - Fields - - - -

        private readonly ITerrainAttributes attributes;
        private readonly TerrainChunkDimensions chunkDimensions;
        private int minimumLevelOfDetail;

        #endregion Fields

        #region - - - - Constructor - - - -

        public ChunkDimensionsUtility(ITerrainAttributes attributes)
        {
            this.attributes = attributes;
            this.chunkDimensions = new TerrainChunkDimensions();
        }

        #endregion Constructor

        #region - - - - Methods - - - -

        public TerrainChunkDimensions CalculateChunkDimensions()
        {
            CalculateLevelOfDetail();

            chunkDimensions.VertexPerSide = Mathf.RoundToInt(attributes.ActualChunkSize / attributes.LODIncrementStep);
            chunkDimensions.VertexPerSide += attributes.LODIncrementStep > 1 ? 1 : 0;

            chunkDimensions.SquaredVertexSide = chunkDimensions.VertexPerSide * chunkDimensions.VertexPerSide;

            Debug.Log("LODIncrementStep: " + attributes.LODIncrementStep);
            Debug.Log("VertexPerSide: " + chunkDimensions.VertexPerSide);
            Debug.Log("ChunkWidth: " + attributes.ActualChunkSize);
            return chunkDimensions;
        }

        public void CalculateLevelOfDetail()
        {
            minimumLevelOfDetail = FindMininmumAllowableLevelOfDetail(0, attributes.RenderChunkSize);

            if (attributes.LevelOfDetail > minimumLevelOfDetail)
                attributes.LevelOfDetail = minimumLevelOfDetail;

            // provides the step detail value for each side of mesh
            attributes.LODIncrementStep = (int)(attributes.LevelOfDetail == 0 ? 1 : attributes.LevelOfDetail * 2);
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
