using MiniProceduralGeneration.Generator.Entities;
using MiniProceduralGeneration.MapGrid;
using UnityEngine;

namespace MiniProceduralGeneration.TerrainCore.Infrastructure
{

    public class ChunkDimensionsResolver : MonoBehaviour
    {

        #region - - - - - - Fields - - - - - -

        private ITerrainAttributes attributes;
        private ITargetObjectTracker targetTracker;
        private IMapGridAttributes mapGridAttributes;
        private TerrainChunkDimensions chunkDimensions;
        private int minimumLevelOfDetail;
        private int finalLevelOfDetail;

        #endregion Fields

        #region - - - - - - Methods - - - - - -

        private void Awake()
        {
            attributes = this.GetComponent<ITerrainAttributes>();
            targetTracker = this.GetComponent<ITargetObjectTracker>();
            mapGridAttributes = this.GetComponent<IMapGridAttributes>();
        }

        public TerrainChunkDimensions GetChunkDimensions(Vector3 chunkPosition)
        {
            finalLevelOfDetail = (int)attributes.LevelOfDetail;
            finalLevelOfDetail = CalculateLODFromTileDistance(chunkPosition);
            CalculateLevelOfDetail();

            chunkDimensions.VertexPerSide = Mathf.RoundToInt(attributes.ActualChunkSize / attributes.LODIncrementStep);
            chunkDimensions.VertexPerSide += attributes.LODIncrementStep > 1 ? 1 : 0;

            chunkDimensions.SquaredVertexSide = chunkDimensions.VertexPerSide * chunkDimensions.VertexPerSide;

            return chunkDimensions;
        }

        public float CalculateDistanceToTileCenter(Vector3 chunkPosition)
        {
            float distance = Vector3.Magnitude(targetTracker.TargetPositionInWorldSpace - chunkPosition); // not exactly center

            return distance;
        }

        public int CalculateLODFromTileDistance(Vector3 chunkPosition)
        {
            int levelOfDetail = (int)CalculateDistanceToTileCenter(chunkPosition) / (mapGridAttributes.ChunkDistance * attributes.ActualChunkSize);
            return levelOfDetail;
        }

        public void CalculateLevelOfDetail()
        {
            minimumLevelOfDetail = FindMininmumAllowableLevelOfDetail(0, attributes.RenderChunkSize);

            if (attributes.LevelOfDetail > minimumLevelOfDetail)
                attributes.LevelOfDetail = minimumLevelOfDetail;

            // provides the step detail value for each side of mesh
            attributes.LODIncrementStep = (int)(attributes.LevelOfDetail == 0 ? 1 : finalLevelOfDetail * 2);
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
