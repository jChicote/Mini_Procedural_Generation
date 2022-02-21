using MiniProceduralGeneration.Chunk;
using MiniProceduralGeneration.Controllers.ActionControls;
using MiniProceduralGeneration.Generator.Entities;
using MiniProceduralGeneration.Handler;
using MiniProceduralGeneration.MapGrid;
using MiniProceduralGeneration.Noise;
using MiniProceduralGeneration.TerrainCore.Processor;
using MiniProceduralGeneration.Utility;

namespace MiniProceduralGeneration.TerrainCore
{

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
        private TerrainChunkIterator terrainChunkIterator;

        #endregion Fields

        #region - - - - - - Properties - - - - - -

        public float AbsoluteHeight { get; set; }
        public float MaxHeight { get; set; }
        public float MinHeight { get; set; }
        public int ActualChunkSize { get; set; }
        public int RenderChunkSize { get => ActualChunkSize - 1; }
        public int LODIncrementStep { get; set; }
        public int VertexPerSide => 0;
        public float LevelOfDetail { get; set; }
        public IChunkShell[] ChunkArray { get; set; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        private void Awake()
        {
            // Internally Resolves Dependency
            controller = FindObjectOfType<TerrainInfoController>(); // Can be Better
            controller.GetTerrainAttributes(this);

            mapCreator = this.GetComponent<IMapGridCreator>();
            IMeshTerrainProcessor terrainProcessor = this.GetComponent<IMeshTerrainProcessor>();
            INoiseGenerator noiseGenerator = this.GetComponent<INoiseGenerator>();

            ChunkArray = new IChunkShell[0];
            dimensionsUtility = new ChunkDimensionsUtility(this);

            terrainChunkIterator = this.GetComponent<TerrainChunkIterator>();
            terrainChunkIterator.StartTerrainRunnerAction(this, terrainProcessor, noiseGenerator);

            ChunkMapCreator mapChunkCreator = this.GetComponent<ChunkMapCreator>();
            ChunkMapScroller mapScroller = this.GetComponent<ChunkMapScroller>();

            mapChunkCreator.SetNext(mapScroller);
            mapChunkCreator.AwakeHandle(true);
        }

        public void BuildTerrain()
        {
            mapCreator.CreateChunkMap(this);
            //InitialiseTerrainChunks();

            //print(ActualChunkSize);
            //print(VertexPerSide);

            terrainChunkIterator.IterateThroughChunkArraySelection(ChunkArray);
        }

        public void RegenerateTerrain()
        {
            //InitialiseTerrainChunks();
            terrainChunkIterator.IterateThroughChunkArraySelection(ChunkArray);

        }

        public void InitialiseTerrainChunks()
        {
            if (ChunkArray.Length == 0) return;

            //chunkDimensions = dimensionsUtility.CalculateChunkDimensions();
            foreach (IChunkShell chunk in ChunkArray)
            {
                chunk.InitialiseMeshArrays(chunkDimensions);
            }
        }

        #endregion Methods

    }

}
