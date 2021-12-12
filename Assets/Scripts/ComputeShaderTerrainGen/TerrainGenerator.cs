using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MiniProceduralGeneration.Generator.MeshWork;
using MiniProceduralGeneration.Generator.Processor;

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
        int Width { get; }
        int LODIncrementStep { get; }
        int VertexPerSide { get; }
        float LevelOfDetail { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TerrainGenerator : MonoBehaviour, ITerrainGenerator, ITerrainCharacteristics
    {
        // Fields
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
        [SerializeField] 
        private int levelOfDetail = 0;
        private const int mapWidth = 241; // width must contain a base value that follows the "divisibility rules" (add 1 for noise processing).

        public GameObject[] chunkObjects;
        private ITerrainChunk[] terrainChunks;
        private TerrainChunkDimensions chunkDimensions;

        // Properties
        public float MaxHeight { get => maxHeight; set => maxHeight = value; }
        public float MinHeight { get => minHeight; set => minHeight = value; }
        public int Width => mapWidth;
        public int LODIncrementStep => lodIncrementStep;
        public int VertexPerSide => chunkDimensions.vertexPerSide;
        public float LevelOfDetail { get => levelOfDetail; set => levelOfDetail = (int)value; }

        private void Awake()
        {
            terrainProcessor = this.GetComponent<ITerrainProcessor>();
            noiseGenerator = this.GetComponent<INoiseGenerator>();
            terrainChunks = new ITerrainChunk[chunkObjects.Length];

            // Collects chunk interfaces set through the inspector editor
            for (int i = 0; i < chunkObjects.Length; i++)
            {
                ITerrainChunk chunk = chunkObjects[i].GetComponent<ITerrainChunk>();
                terrainChunks[i] = chunk;
            }
        }

        public void CalculateChunkDimensions()
        {
            // Below calculates base dimensions to be used for each chunk mesh
            chunkDimensions = new TerrainChunkDimensions();
            lodIncrementStep = levelOfDetail == 0 ? 1 : levelOfDetail * 2; // provides the step detail value for each side of mesh
            chunkDimensions.vertexPerSide = (mapWidth - 1) / lodIncrementStep + 1; // width removes 1 so width is a multiple of 2
            chunkDimensions.squaredVertexSide = chunkDimensions.vertexPerSide * chunkDimensions.vertexPerSide;
        }

        public void InitialiseTerrainChunks()
        {
            foreach (ITerrainChunk chunk in terrainChunks)
            {
                chunk.InitialiseMeshArrays(chunkDimensions);
            }
        }

        public void BuildTerrain()
        {


            float[] noiseData;

            foreach (ITerrainChunk chunk in terrainChunks)
            {
                noiseData = noiseGenerator.SampleNoiseDataAtLocation(mapWidth, chunk.PositionWorldSpace);
                terrainProcessor.ProcessChunkMesh(chunk, noiseData);
                chunk.BuildMesh();

                // cleans buffers before next use.
                terrainProcessor.DisposeBuffersIntoGarbageCollection();
            }
        }
    }

    [System.Serializable]
    public class TerrainChunkDimensions
    {
        public int vertexPerSide;
        public int squaredVertexSide;
    }
}
