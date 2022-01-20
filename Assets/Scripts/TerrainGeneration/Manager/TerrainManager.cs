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
        int ChunkWidth { get; }
        int LODIncrementStep { get; set; }
        int VertexPerSide { get; }
        float LevelOfDetail { get; set; }
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

        private float maxHeight = 10;
        private float minHeight = 0;
        private int lodIncrementStep;
        private int levelOfDetail = 0;
        private int minimumLevelOfDetail;
        private int chunkWidth = 241;

        private TerrainChunkDimensions chunkDimensions;

        private IMapGridCreator mapCreator;
        private IChunkDimensionsUtility dimensionsUtility;

        #endregion Fields

        #region ------ Properties ------

        public float MaxHeight { get => maxHeight; set => maxHeight = value; }
        public float MinHeight { get => minHeight; set => minHeight = value; }
        public int ChunkWidth => chunkWidth;

        public int LODIncrementStep
        {
            get => lodIncrementStep;
            set => lodIncrementStep = value;
        }

        public int VertexPerSide => chunkDimensions.VertexPerSide;
        public float LevelOfDetail { get => levelOfDetail; set => levelOfDetail = (int)value; }
        public ITerrainChunk[] ChunkArray { get; set; }

        #endregion Properties

        #region ------ Methods ------

        private void Awake()
        {
            mapCreator = this.GetComponent<IMapGridCreator>();
            terrainProcessor = this.GetComponent<ITerrainProcessor>();
            noiseGenerator = this.GetComponent<INoiseGenerator>();

            ChunkArray = new ITerrainChunk[0];

            ChunkMapCreator mapChunkCreator = this.GetComponent<ChunkMapCreator>();
            ChunkMapScroller mapScroller = this.GetComponent<ChunkMapScroller>();

            mapChunkCreator.SetNext(mapScroller);
            mapChunkCreator.Handle(true);

            dimensionsUtility = new ChunkDimensionsUtility(this);
        }

        public void InitialiseTerrainChunks()
        {
            if (ChunkArray.Length == 0) return;

            chunkDimensions = dimensionsUtility.CalculateChunkDimensions();

            print(ChunkArray.Length);
            foreach (ITerrainChunk chunk in ChunkArray)
            {
                chunk.InitialiseMeshArrays(chunkDimensions);
            }
        }

        /// <summary>
        /// Calculates base dimensions to be used for each chunk mesh
        /// </summary>
        /*public void CalculateChunkDimensions()
        {
            chunkDimensions = new TerrainChunkDimensions();
            CalculateLevelOfDetail();

            chunkDimensions.VertexPerSide = Mathf.RoundToInt(chunkWidth / lodIncrementStep);
            chunkDimensions.SquaredVertexSide = chunkDimensions.VertexPerSide * chunkDimensions.VertexPerSide;
        }

        private void CalculateLevelOfDetail()
        {
            minimumLevelOfDetail = FindMininmumAllowableLevelOfDetail(0);

            if (levelOfDetail > minimumLevelOfDetail)
            {
                levelOfDetail = minimumLevelOfDetail;
            }

            // provides the step detail value for each side of mesh
            lodIncrementStep = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
        }*/

        /*/// <summary>
        /// Finds the minimum allowable level of detail value of a given mesh size.
        /// </summary>
        /// <param name="currentLevelOfDetail"></param>
        public int FindMininmumAllowableLevelOfDetail(int currentLevelOfDetail)
        {
            int nextLodStep = currentLevelOfDetail == 0 ? 1 : currentLevelOfDetail * 2;
            int vertexPerSide = Mathf.RoundToInt(chunkWidth / nextLodStep);
            float squaredSize = vertexPerSide * vertexPerSide;

            if (squaredSize % 2f == 0f)
            {
                currentLevelOfDetail++;
                return FindMininmumAllowableLevelOfDetail(currentLevelOfDetail);
            }

            currentLevelOfDetail--;
            return currentLevelOfDetail;
        }*/

        public void BuildTerrain()
        {
            float[] noiseData = new float[0];

            foreach (ITerrainChunk chunk in ChunkArray)
            {
                ProcessChunk(noiseData, chunk);
            }
        }

        public void ProcessChunk(float[] noiseData, ITerrainChunk chunk)
        {
            noiseData = noiseGenerator.SampleNoiseDataAtLocation(chunkWidth, chunk.PositionWorldSpace);
            terrainProcessor.ProcessChunkMesh(chunk, noiseData);
            chunk.BuildMesh();

            // cleans buffers before next use.
            terrainProcessor.DisposeBuffersIntoGarbageCollection();
        }

        #endregion Methods
    }
}
