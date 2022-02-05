using MiniProceduralGeneration.Controllers.ActionControls;
using MiniProceduralGeneration.Generator.Creator.Map;
using MiniProceduralGeneration.Generator.Entities;
using MiniProceduralGeneration.Generator.MapGrid;
using MiniProceduralGeneration.Generator.MeshWork;
using MiniProceduralGeneration.Generator.Processor;
using MiniProceduralGeneration.Generator.Utility;
using MiniProceduralGeneration.Handler;

namespace MiniProceduralGeneration.Generator
{
    public interface ITerrainManager
    {
        void InitialiseTerrainChunks();
        void BuildTerrain();
    }

    public interface ITerrainAttributes
    {
        #region - - - - - - Properties - - - - - -

        float MaxHeight { get; set; }
        float MinHeight { get; set; }
        int ChunkWidth { get; set; }
        int LODIncrementStep { get; set; }
        int VertexPerSide { get; }
        int LevelOfDetail { get; set; }

        #endregion Properties
    }

    public interface ITerrainChunkCollection
    {
        ITerrainChunk[] ChunkArray { get; set; }
    }

    /// <summary>
    /// The primary terrain class that handles and coordinates building the terrain chunks.
    /// </summary>
    public class TerrainManager : GameHandler, ITerrainManager, ITerrainAttributes, ITerrainChunkCollection
    {
        #region ------ Fields ------

        private IMapGridCreator mapCreator;
        private IChunkDimensionsUtility dimensionsUtility;
        private ITerrainInfoController controller;

        private TerrainChunkDimensions chunkDimensions;
        private TerrainRunnerAction terrainAction;

        #endregion Fields

        #region - - - - - - Properties - - - - - -

        public float MaxHeight { get; set; }
        public float MinHeight { get; set; }
        public int ChunkWidth { get; set; }

        public int LODIncrementStep { get; set; }

        public int VertexPerSide => chunkDimensions.VertexPerSide;
        public int LevelOfDetail { get; set; }
        public ITerrainChunk[] ChunkArray { get; set; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        private void Awake()
        {
            // Internally Resolves Dependency
            controller = FindObjectOfType<TerrainInfoController>(); // Can be Better
            controller.GetTerrainAttributes(this);

            mapCreator = this.GetComponent<IMapGridCreator>();
            ITerrainProcessor terrainProcessor = this.GetComponent<ITerrainProcessor>();
            INoiseGenerator noiseGenerator = this.GetComponent<INoiseGenerator>();

            ChunkArray = new ITerrainChunk[0];
            dimensionsUtility = new ChunkDimensionsUtility(this);

            terrainAction = this.GetComponent<TerrainRunnerAction>();
            terrainAction.StartTerrainRunnerAction(this, terrainProcessor, noiseGenerator);

            ChunkMapCreator mapChunkCreator = this.GetComponent<ChunkMapCreator>();
            ChunkMapScroller mapScroller = this.GetComponent<ChunkMapScroller>();

            mapChunkCreator.SetNext(mapScroller);
            mapChunkCreator.AwakeHandle(true);
        }

        public void BuildTerrain()
        {
            mapCreator.CreateChunkMap(this);
            InitialiseTerrainChunks();

            terrainAction.IterateThroughChunkArraySelection(ChunkArray);
        }

        public void RegenerateTerrain()
        {
            InitialiseTerrainChunks();
            terrainAction.IterateThroughChunkArraySelection(ChunkArray);
        }

        public void InitialiseTerrainChunks()
        {
            if (ChunkArray.Length == 0) return;

            chunkDimensions = dimensionsUtility.CalculateChunkDimensions();
            foreach (ITerrainChunk chunk in ChunkArray)
            {
                chunk.InitialiseMeshArrays(chunkDimensions);
            }
        }

        #endregion Methods

    }
}
