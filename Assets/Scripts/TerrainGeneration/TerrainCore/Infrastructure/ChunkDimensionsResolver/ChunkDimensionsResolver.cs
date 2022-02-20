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
        private int calculatedLevelOfDetail;

        #endregion Fields

        #region - - - - - - Properties - - - - - -

        public int MinimumLevelOfDetail { get; set; }

        #endregion Properties

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

            calculatedLevelOfDetail = (int)attributes.LevelOfDetail;
            calculatedLevelOfDetail = tileDistanceCalculator.CalculateLevelOfDetailDFromTileDistance(chunkPosition);
            MinimumLevelOfDetail = minimumLevelFinder.FindMininmumAllowableLevelOfDetail(0, attributes.RenderChunkSize);
            attributes.LODIncrementStep = minimumLevelFinder.CalculateLevelOfDetailIncrement(calculatedLevelOfDetail);

            chunkDimensions.VertexPerSide = Mathf.RoundToInt(attributes.ActualChunkSize / attributes.LODIncrementStep);
            chunkDimensions.VertexPerSide += attributes.LODIncrementStep > 1 ? 1 : 0;
            chunkDimensions.SquaredVertexSide = chunkDimensions.VertexPerSide * chunkDimensions.VertexPerSide;
            chunkDimensions.LevelOfDetail = minimumLevelFinder.LevelOfDetail;

            return chunkDimensions;
        }

        #endregion Methods

    }

}
