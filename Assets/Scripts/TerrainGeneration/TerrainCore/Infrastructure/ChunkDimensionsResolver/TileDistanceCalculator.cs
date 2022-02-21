using MiniProceduralGeneration.MapGrid;
using UnityEngine;

namespace MiniProceduralGeneration.TerrainCore.Infrastructure.ChunkDimensionsResolver
{

    public class TileDistanceCalculator
    {

        #region - - - - - - Properties - - - - - -

        private IMapGridAttributes mapGridAttributes;
        private ITargetObjectTracker targetTracker;
        private ITerrainAttributes terrainAttributes;

        private Vector2 targetPlanePosition;
        private Vector2 tileCenteredPosition;

        #endregion Properties

        #region - - - - - - Constructors - - - - - -

        public TileDistanceCalculator(
            IMapGridAttributes mapGridAttributes,
            ITargetObjectTracker targetTracker,
            ITerrainAttributes terrainAttributes)
        {
            this.mapGridAttributes = mapGridAttributes;
            this.targetTracker = targetTracker;
            this.terrainAttributes = terrainAttributes;
        }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        public float CalculateDistanceToTileCenter(Vector3 chunkPosition)
        {
            tileCenteredPosition = new Vector2
            (
                chunkPosition.x + (terrainAttributes.RenderChunkSize / 2),
                chunkPosition.z + (terrainAttributes.RenderChunkSize / 2)
            );

            targetPlanePosition = new Vector2
            (
                targetTracker.TargetPositionInWorldSpace.x,
                targetTracker.TargetPositionInWorldSpace.z
            );

            return Vector2.Distance(targetPlanePosition, tileCenteredPosition);
        }

        public int CalculateLevelOfDetailDFromTileDistance(Vector3 chunkPosition)
            => Mathf.Clamp((int)CalculateDistanceToTileCenter(chunkPosition) / terrainAttributes.RenderChunkSize, 0, 1000);

        #endregion Methods

    }

}