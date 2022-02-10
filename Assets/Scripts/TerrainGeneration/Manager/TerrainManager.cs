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

        #region - - - - - - Methods - - - - - -

        void InitialiseTerrainChunks();
        void BuildTerrain();

        #endregion Methods

    }

    public interface ITerrainAttributes
    {

        #region - - - - - - Properties - - - - - -

        float MaxHeight { get; set; }
        float MinHeight { get; set; }
        int ActualChunkSize { get; set; }
        int RenderChunkSize { get; }
        int LODIncrementStep { get; set; }
        int VertexPerSide { get; }
        float LevelOfDetail { get; set; }

        #endregion Properties

    }

    public interface ITerrainChunkCollection
    {

        #region - - - - - - Properties - - - - - -

        ITerrainChunk[] ChunkArray { get; set; }

        #endregion Properties

    }

    /// <summary>
    /// The primary terrain class that handles and coordinates building the terrain chunks.
    /// </summary>
    public class TerrainManager : GameHandler, ITerrainManager, ITerrainAttributes, ITerrainChunkCollection
    {

        #region - - - - - - Fields - - - - - -

        private IMapGridCreator mapCreator;
        public IChunkDimensionsUtility dimensionsUtility;
        private ITerrainInfoController controller;

        private TerrainChunkDimensions chunkDimensions;
        private TerrainRunnerAction terrainAction;

        #endregion Fields

        #region - - - - - - Properties - - - - - -

        public float MaxHeight { get; set; }
        public float MinHeight { get; set; }
        public int ActualChunkSize { get; set; }
        public int RenderChunkSize { get => ActualChunkSize; }
        public int LODIncrementStep { get; set; }

        public int VertexPerSide => chunkDimensions.VertexPerSide;
        public float LevelOfDetail { get; set; }
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

            print(ActualChunkSize);
            print(VertexPerSide);

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
