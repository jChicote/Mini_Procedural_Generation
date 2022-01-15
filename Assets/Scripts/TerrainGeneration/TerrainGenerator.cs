using MiniProceduralGeneration.Generator.Creator.Map;
using MiniProceduralGeneration.Generator.Entities;
using MiniProceduralGeneration.Generator.MeshWork;
using MiniProceduralGeneration.Generator.Processor;
using MiniProceduralGeneration.Generator.Scroller;
using MiniProceduralGeneration.Handler;
using UnityEngine;

namespace MiniProceduralGeneration.Generator
{
    public interface ITerrainGenerator
    {
        void CalculateChunkDimensions();
        void InitialiseTerrainChunks();
        void BuildTerrain();
    }

    public interface ITerrainCharacteristics
    {
        float MaxHeight { get; set; }
        float MinHeight { get; set; }
        int MapSize { get; }
        int LODIncrementStep { get; }
        int VertexPerSide { get; }
        float LevelOfDetail { get; set; }
    }

    public interface ITerrainChunkArray
    {
        ITerrainChunk[] TerrainChunks { get; set; }
    }

    /// <summary>
    /// The primary terrain class that handles and coordinates building the terrain chunks.
    /// </summary>
    public class TerrainGenerator : GameHandler, ITerrainGenerator, ITerrainCharacteristics
    {
        #region ------ Fields ------

        public INoiseGenerator noiseGenerator;
        private ITerrainProcessor terrainProcessor;

        [Header("Terrain Chracteristics")]
        [SerializeField]
        private float maxHeight = 10;
        [SerializeField]
        private float minHeight = 0;
        [SerializeField]
        private int lodIncrementStep;
        [Range(0, 6)]

        // Lower the LOD the higher the resolution
        [SerializeField]
        private int levelOfDetail = 0;
        private int minimumLevelOfDetail;

        // width must contain a base value that follows the "divisibility rules" (add 1 for noise processing).
        [SerializeField]
        private int mapWidth = 241;

        private TerrainChunkDimensions chunkDimensions;

        private IMapCreator mapCreator;

        #endregion Fields

        #region ------ Properties ------

        public float MaxHeight { get => maxHeight; set => maxHeight = value; }
        public float MinHeight { get => minHeight; set => minHeight = value; }
        public int MapSize => mapWidth;
        public int LODIncrementStep => lodIncrementStep;
        public int VertexPerSide => chunkDimensions.VertexPerSide;
        public float LevelOfDetail { get => levelOfDetail; set => levelOfDetail = (int)value; }

        #endregion Properties

        #region ------ Methods ------

        private void Awake()
        {
            mapCreator = this.GetComponent<IMapCreator>();
            terrainProcessor = this.GetComponent<ITerrainProcessor>();
            noiseGenerator = this.GetComponent<INoiseGenerator>();

            ChunkMapCreator mapChunkCreator = this.GetComponent<ChunkMapCreator>();
            ChunkMapScroller mapScroller = this.GetComponent<ChunkMapScroller>();

            mapChunkCreator.SetNext(mapScroller);
            mapChunkCreator.Handle(true);
        }

        public void InitialiseTerrainChunks()
        {
            if (mapCreator.ChunkMap.TerrainChunks.Length == 0) return;

            CalculateChunkDimensions();

            print(mapCreator.ChunkMap.TerrainChunks.Length);
            foreach (ITerrainChunk chunk in mapCreator.ChunkMap.TerrainChunks)
            {
                chunk.InitialiseMeshArrays(chunkDimensions);
            }
        }

        /// <summary>
        /// Calculates base dimensions to be used for each chunk mesh
        /// </summary>
        public void CalculateChunkDimensions()
        {
            chunkDimensions = new TerrainChunkDimensions();
            CalculateLevelOfDetail();

            chunkDimensions.VertexPerSide = Mathf.RoundToInt(mapWidth / lodIncrementStep);

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
        }

        /// <summary>
        /// Finds the minimum allowable level of detail value of a given mesh size.
        /// </summary>
        /// <param name="currentLevelOfDetail"></param>
        public int FindMininmumAllowableLevelOfDetail(int currentLevelOfDetail)
        {
            int nextLodStep = currentLevelOfDetail == 0 ? 1 : currentLevelOfDetail * 2;
            int vertexPerSide = Mathf.RoundToInt(mapWidth / nextLodStep);
            float squaredSize = vertexPerSide * vertexPerSide;

            if (squaredSize % 2f == 0f)
            {
                currentLevelOfDetail++;
                return FindMininmumAllowableLevelOfDetail(currentLevelOfDetail);
            }

            currentLevelOfDetail--;
            return currentLevelOfDetail;
        }

        public void BuildTerrain()
        {
            float[] noiseData = new float[0];

            foreach (ITerrainChunk chunk in mapCreator.ChunkMap.TerrainChunks)
            {
                ProcessChunk(noiseData, chunk);
            }
        }

        public void ProcessChunk(float[] noiseData, ITerrainChunk chunk)
        {
            noiseData = noiseGenerator.SampleNoiseDataAtLocation(mapWidth, chunk.PositionWorldSpace);
            terrainProcessor.ProcessChunkMesh(chunk, noiseData);
            chunk.BuildMesh();

            // cleans buffers before next use.
            terrainProcessor.DisposeBuffersIntoGarbageCollection();
        }

        #endregion Methods
    }
}
