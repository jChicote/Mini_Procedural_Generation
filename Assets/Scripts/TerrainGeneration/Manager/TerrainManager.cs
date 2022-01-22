using MiniProceduralGeneration.Controllers.ActionControls;
using MiniProceduralGeneration.Generator.Creator.Map;
using MiniProceduralGeneration.Generator.Entities;
using MiniProceduralGeneration.Generator.MeshWork;
using MiniProceduralGeneration.Generator.Processor;
using MiniProceduralGeneration.Generator.Scroller;
using MiniProceduralGeneration.Generator.Utility;
using MiniProceduralGeneration.Handler;

namespace MiniProceduralGeneration.Generator
{
    public interface ITerrainGenerator
    {
        //void CalculateChunkDimensions();
        void InitialiseTerrainChunks();
        void BuildTerrain();
    }

    public interface ITerrainAttributes
    {
        float MaxHeight { get; set; }
        float MinHeight { get; set; }
        int ChunkWidth { get; set; }
        int LODIncrementStep { get; set; }
        int VertexPerSide { get; }
        int LevelOfDetail { get; set; }
    }

    public interface ITerrainChunks
    {
        ITerrainChunk[] ChunkArray { get; set; }
    }

    /// <summary>
    /// The primary terrain class that handles and coordinates building the terrain chunks.
    /// </summary>
    public class TerrainManager : GameHandler, ITerrainGenerator, ITerrainAttributes, ITerrainChunks
    {
        #region ------ Fields ------

        public INoiseGenerator noiseGenerator;
        private ITerrainProcessor terrainProcessor;
        private IMapGridCreator mapCreator;
        private IChunkDimensionsUtility dimensionsUtility;
        private ITerrainInfoController controller;

        //private float maxHeight = 10;
        //private float minHeight = 0;
        //private int lodIncrementStep;
        //private int levelOfDetail = 0;
        //private int minimumLevelOfDetail;
        //private int chunkWidth = 241;

        //private ITerrainChunk[] chunkArray;

        private TerrainChunkDimensions chunkDimensions;

        #endregion Fields

        #region ------ Properties ------

        public float MaxHeight { get; set; }
        public float MinHeight { get; set; }
        public int ChunkWidth { get; set; }

        public int LODIncrementStep { get; set; }

        public int VertexPerSide => chunkDimensions.VertexPerSide;
        public int LevelOfDetail { get; set; }
        public ITerrainChunk[] ChunkArray { get; set; }

        #endregion Properties

        #region ------ Methods ------

        private void Awake()
        {
            // Internally Resolves Dependency
            controller = FindObjectOfType<TerrainInfoController>(); // Can be Better
            controller.GetTerrainAttributes(this);

            mapCreator = this.GetComponent<IMapGridCreator>();
            terrainProcessor = this.GetComponent<ITerrainProcessor>();
            noiseGenerator = this.GetComponent<INoiseGenerator>();

            ChunkArray = new ITerrainChunk[0];
            dimensionsUtility = new ChunkDimensionsUtility(this);

            ChunkMapCreator mapChunkCreator = this.GetComponent<ChunkMapCreator>();
            ChunkMapScroller mapScroller = this.GetComponent<ChunkMapScroller>();

            mapChunkCreator.SetNext(mapScroller);
            mapChunkCreator.Handle(true);
        }

        public void BuildTerrain()
        {
            float[] noiseData = new float[0];

            mapCreator.CreateChunkMap(this);
            InitialiseTerrainChunks();

            foreach (ITerrainChunk chunk in ChunkArray)
            {
                ProcessChunk(noiseData, chunk);
            }
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

        public void ProcessChunk(float[] noiseData, ITerrainChunk chunk)
        {
            noiseData = noiseGenerator.SampleNoiseDataAtLocation(ChunkWidth, chunk.PositionWorldSpace);
            terrainProcessor.ProcessChunkMesh(chunk, noiseData);
            chunk.BuildMesh();

            // cleans buffers before next use.
            terrainProcessor.DisposeBuffersIntoGarbageCollection();
        }

        #endregion Methods
    }
}
