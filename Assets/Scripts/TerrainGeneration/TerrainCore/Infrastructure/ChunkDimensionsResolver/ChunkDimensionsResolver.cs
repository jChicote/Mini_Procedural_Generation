using MiniProceduralGeneration.Generator.Entities;
using MiniProceduralGeneration.MapGrid;
using UnityEngine;

namespace MiniProceduralGeneration.TerrainCore.Infrastructure.ChunkDimensionsResolver
{

    public class ChunkDimensionsResolver : MonoBehaviour
    {

        #region - - - - - - Fields - - - - - -

        private ITerrainAttributes attributes;
        private ITargetObjectTracker targetTracker;
        private IMapGridAttributes mapGridAttributes;

        private MinimumLevelOfDetailIncrementFinder minimumLevelFinder;
        private TileDistanceCalculator tileDistanceCalculator;
        private TerrainChunkDimensions chunkDimensions;
        private int finalLevelOfDetail;

        #endregion Fields

        #region - - - - - - Methods - - - - - -

        private void Awake()
        {
            attributes = this.GetComponent<ITerrainAttributes>();
            targetTracker = this.GetComponent<ITargetObjectTracker>();
            mapGridAttributes = this.GetComponent<IMapGridAttributes>();

            minimumLevelFinder = new MinimumLevelOfDetailIncrementFinder(attributes);
            tileDistanceCalculator = new TileDistanceCalculator(mapGridAttributes, targetTracker, attributes);
        }

        public TerrainChunkDimensions GetChunkDimensions(Vector3 chunkPosition)
        {
            chunkDimensions = new TerrainChunkDimensions();

            finalLevelOfDetail = (int)attributes.LevelOfDetail;
            finalLevelOfDetail = tileDistanceCalculator.CalculateLevelOfDetailDFromTileDistance(chunkPosition);
            attributes.LODIncrementStep = minimumLevelFinder.CalculateLevelOfDetailIncrement(finalLevelOfDetail);

            chunkDimensions.VertexPerSide = Mathf.RoundToInt(attributes.ActualChunkSize / attributes.LODIncrementStep);
            chunkDimensions.VertexPerSide += attributes.LODIncrementStep > 1 ? 1 : 0;
            chunkDimensions.SquaredVertexSide = chunkDimensions.VertexPerSide * chunkDimensions.VertexPerSide;

            return chunkDimensions;
        }

        #endregion Methods

    }

}
