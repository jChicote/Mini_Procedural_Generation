using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MiniProceduralGeneration.Generator.Noise;
using MiniProceduralGeneration.Generator.MeshWork;
using MiniProceduralGeneration.Generator.Procesor;

namespace MiniProceduralGeneration.Generator
{
    /*
     * 
     */
    public struct MeshPointData
    {
        public Vector3 vert;
        public Vector3 normal;
        public Vector2 uv;
    }

    /*
     * 
     */
    public struct QuadSet
    {
        public Vector3 triangleA;
        public Vector3 triangleB;
    }

    /*
     * 
     */
    public struct MeshComputeBuffers
    {
        public ComputeBuffer vertBuffer;
        public ComputeBuffer normalBuffer;
        public ComputeBuffer uvBuffer;
        public ComputeBuffer noiseBuffer;
        public ComputeBuffer trisBuffer;
    }

    [System.Serializable]
    public class TerrainChunkDimensions
    {
        public int vertexPerSide;
        public int squaredVertexSide;
    }

    public interface ITerrainCharacteristics
    {
        float MaxHeight { get; }
        float MinHeight { get; }
        int Width { get; }
        int LODIncrementStep { get; }
        int VertexPerSide { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TerrainGenerator : MonoBehaviour, ITerrainCharacteristics // DOES TWO THINGS
    {
        // Fields
        public INoiseGenerator noiseGenerator;
        private ITerrainProcessor terrainProcessor;
        public HeightLerpAssigner lerpAssigner; // Use interface

        [Header("Terrain Chracteristics")]
        [SerializeField] private float maxHeight = 10;
        [SerializeField] private float minHeight = 0;

        [SerializeField] private int lodIncrementStep;
        [SerializeField] private int groundlevel;

        // width must contain a base value that follows the "divisibility rules".
        // provides max base width value to be processed (value divisible by 2 + 1)
        private const int width = 241;  
        [Range(0, 6)]
        [SerializeField] private int levelOfDetail = 0;

        public GameObject[] chunkObjects;
        private ITerrainChunk[] terrainChunks;
        private TerrainChunkDimensions chunkDimensions;

        // Properties
        public float MaxHeight => maxHeight;
        public float MinHeight => minHeight;
        public int Width => width;
        public int LODIncrementStep => lodIncrementStep;
        public int VertexPerSide => chunkDimensions.vertexPerSide;

        private void Awake() // THIS DOES TWO THINGS
        {
            terrainProcessor = this.GetComponent<ITerrainProcessor>();
            noiseGenerator = this.GetComponent<INoiseGenerator>();
            terrainChunks = new ITerrainChunk[chunkObjects.Length];

            for (int i = 0; i < chunkObjects.Length; i++)
            {
                ITerrainChunk chunk = chunkObjects[i].GetComponent<ITerrainChunk>();
                terrainChunks[i] = chunk;
            }
        }

        private void CalculateChunkDimensions()
        {
            // Below calculates base dimensions to be used for each chunk mesh
            chunkDimensions = new TerrainChunkDimensions();
            lodIncrementStep = levelOfDetail == 0 ? 1 : levelOfDetail * 2; // provides the step detail value for each side of mesh
            chunkDimensions.vertexPerSide = (width - 1) / lodIncrementStep + 1; // width removes 1 so width is a multiple of 2
            chunkDimensions.squaredVertexSide = chunkDimensions.vertexPerSide * chunkDimensions.vertexPerSide;
        }

        private void InitialiseTerrainChunks()
        {
            foreach (ITerrainChunk chunk in terrainChunks)
            {
                chunk.InitialiseMeshArrays(chunkDimensions);
            }
        }

        public void BuildTerrain()
        {
            ITerrainMeshAttributeModifier chunkAttributes;
            float[] noiseData;

            foreach (TerrainChunk chunk in terrainChunks)
            {
                chunkAttributes = chunk.GetComponent<ITerrainMeshAttributeModifier>();

                noiseData = noiseGenerator.SampleNoiseDataAtLocation(width, new Vector3(0, 0, 0));
                terrainProcessor.ProcessChunkMesh(chunkAttributes, noiseData);
                chunk.BuildMesh();

                terrainProcessor.DisposeBuffersIntoGarbageCollection();
            }
        }


        // --------------------------------------------------------------------
        //                              GIZMOS GUI
        // --------------------------------------------------------------------


        // THis needs to be refracted with seperate UI functions
        // must be controllable seperately from a ui interface


        private void OnGUI()
        {
            if (GUI.Button(new Rect(0, 0, 100, 50), "Create"))
            {
                if (!noiseGenerator.HasCreatedSeed)
                {
                    print("Has Not created seed");
                    return;
                }

                CalculateChunkDimensions();
                InitialiseTerrainChunks();
                lerpAssigner.AssignLerpColors(maxHeight, minHeight);
                BuildTerrain();
            }

            if (GUI.Button(new Rect(150, 0, 200, 50), "Generate Seed"))
            {
            noiseGenerator.GenerateSeed();
            }
        }
    }
}
