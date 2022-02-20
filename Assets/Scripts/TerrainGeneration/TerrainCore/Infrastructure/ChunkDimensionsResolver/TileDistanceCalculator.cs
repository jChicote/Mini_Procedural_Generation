using MiniProceduralGeneration.MapGrid;
using UnityEngine;

namespace MiniProceduralGeneration.TerrainCore.Infrastructure.ChunkDimensionsResolver
{

    public class TileDistanceCalculator
    {

        #region - - - - - - Properties - - - - - -

        private IMapGridAttributes MapGridAttributes { get; set; }

        private ITargetObjectTracker TargetTracker { get; set; }

        private ITerrainAttributes TerrainAttributes { get; set; }

        #endregion Properties

        #region - - - - - - Constructors - - - - - -

        public TileDistanceCalculator(
            IMapGridAttributes mapGridAttributes,
            ITargetObjectTracker targetTracker,
            ITerrainAttributes terrainAttributes)
        {
            this.MapGridAttributes = mapGridAttributes;
            this.TargetTracker = targetTracker;
            this.TerrainAttributes = terrainAttributes;
        }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        public float CalculateDistanceToTileCenter(Vector3 chunkPosition)
        {
            Vector2 tileCenteredPosition = new Vector2
            (
                chunkPosition.x + (TerrainAttributes.RenderChunkSize / 2),
                chunkPosition.z + (TerrainAttributes.RenderChunkSize / 2)
            );

            Vector2 targetPlanePosition = new Vector2
            (
                TargetTracker.TargetPositionInWorldSpace.x,
                TargetTracker.TargetPositionInWorldSpace.z
            );

            float distance = Vector2.Distance(targetPlanePosition, tileCenteredPosition);

            return distance;
        }

        public int CalculateLevelOfDetailDFromTileDistance(Vector3 chunkPosition)
        {
            int levelOfDetail = (int)CalculateDistanceToTileCenter(chunkPosition) / TerrainAttributes.RenderChunkSize; // (MapGridAttributes.ChunkDistance * TerrainAttributes.ActualChunkSize) + 1;
            //Debug.Log((int)CalculateDistanceToTileCenter(chunkPosition));
            //Debug.Log(levelOfDetail);
            levelOfDetail = Mathf.Clamp(levelOfDetail, 0, 1000);
            return levelOfDetail;
        }

        #endregion Methods

    }

}