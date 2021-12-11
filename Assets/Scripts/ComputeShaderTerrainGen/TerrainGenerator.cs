using UnityEngine;
using MiniProceduralGeneration.Generator.Noise;
using MiniProceduralGeneration.Generator.MeshWork;

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

    public struct TerrainChunkDimensions
    {
        public int vertexPerSide;
        public int squaredVertexSide;
    }

    /// <summary>
    /// 
    /// </summary>
    public class TerrainGenerator : MonoBehaviour
    {
        // Fields
        public ComputeShader computeTerrainGen; // Use interface
        public INoiseGenerator noiseGenerator; // Use interface
        public HeightLerpAssigner lerpAssigner; // Use interface

        [Header("Terrain Cahracteristics")]
        [SerializeField] private float maxHeight = 10;
        [SerializeField] private float minHeight = 0;

        [SerializeField] private int lodIncrementStep;
        [SerializeField] private int groundlevel;

        // width must contain a base value that follows the "divisibility rules"
        private const int width = 241; // Provides max base width value to be processed (value divisible by 2 + 1) 
        [Range(0, 6)]
        [SerializeField] private int levelOfDetail = 0;

        public TerrainChunk[] chunks;
        [SerializeField] public TerrainChunkDimensions chunkDimensions;


        private void Awake()
        {
            noiseGenerator = this.GetComponent<INoiseGenerator>();
        }

        //
        //
        //
        private void PopulateMeshAttributes()
        {
            chunkDimensions = new TerrainChunkDimensions();

            // determines the increment to step for each side of the chunk
            // and must be a multiple of 2.
            lodIncrementStep = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
            chunkDimensions.vertexPerSide = (width - 1) / lodIncrementStep + 1; // width removes 1 so width is a multiple of 2
            chunkDimensions.squaredVertexSide = chunkDimensions.vertexPerSide * chunkDimensions.vertexPerSide;

            print("LOD: " + lodIncrementStep);
            print("Vertex Per Side: " + chunkDimensions.vertexPerSide);
            print("Squard Vertex Size: " + chunkDimensions.squaredVertexSide);

            foreach (TerrainChunk chunk in chunks)
            {
                ITerrainChunk terrainChunk = chunk.GetComponent<ITerrainChunk>();
                terrainChunk.InitialiseMeshArrays(chunkDimensions);
            }
        }

        public void BuildTerrain()
        {
            //mesh = new Mesh();

            foreach (TerrainChunk chunk in chunks)
            {
                ITerrainMeshAttributeModifier chunkAttributes = chunk.GetComponent<ITerrainMeshAttributeModifier>();

                //noiseGenerator.GenerateSeed();
                float[] noiseData = noiseGenerator.SampleNoiseDataAtLocation(width, new Vector3(0, 0, 0));

                MeshComputeBuffers meshComputeBuffers = CreateNewMeshBuffers(noiseData, chunkAttributes);// Purpiose is too ambigious
                SetComputeShaderBuffers(meshComputeBuffers, chunkAttributes);
                computeTerrainGen.Dispatch(0, chunkAttributes.Vertices.Length / 10, 1, 1);

                RetrieveDataFromComputeShader(meshComputeBuffers, chunkAttributes);
                chunk.BuildMesh();
                DisposeBuffersIntoGarbageCollection(meshComputeBuffers);
            }
        }

        //
        // Simplify method to prevent repeating code statements
        //
        private MeshComputeBuffers CreateNewMeshBuffers(float[] noiseData, ITerrainMeshAttributeModifier chunkAttributes)
        {
            MeshComputeBuffers meshBuffers = new MeshComputeBuffers();

            meshBuffers.vertBuffer = new ComputeBuffer(chunkAttributes.Vertices.Length, sizeof(float) * 3);
            meshBuffers.vertBuffer.SetData(chunkAttributes.Vertices);

            meshBuffers.normalBuffer = new ComputeBuffer(chunkAttributes.Vertices.Length, sizeof(float) * 3);
            meshBuffers.normalBuffer.SetData(chunkAttributes.Normals);

            meshBuffers.uvBuffer = new ComputeBuffer(chunkAttributes.Vertices.Length, sizeof(float) * 2);
            meshBuffers.uvBuffer.SetData(chunkAttributes.UVs);

            meshBuffers.noiseBuffer = new ComputeBuffer(noiseData.Length, sizeof(float));
            meshBuffers.noiseBuffer.SetData(noiseData);

            meshBuffers.trisBuffer = new ComputeBuffer(chunkAttributes.Quads.Length, sizeof(float) * 6);
            meshBuffers.trisBuffer.SetData(chunkAttributes.Quads);

            return meshBuffers;
        }


        //
        //
        //
        private void SetComputeShaderBuffers(MeshComputeBuffers meshBuffers, ITerrainMeshAttributeModifier chunkAttributes)
        {
            computeTerrainGen.SetBuffer(0, "vertices", meshBuffers.vertBuffer);
            computeTerrainGen.SetBuffer(0, "noiseData", meshBuffers.noiseBuffer);
            computeTerrainGen.SetBuffer(0, "triangles", meshBuffers.trisBuffer);
            computeTerrainGen.SetBuffer(0, "normal", meshBuffers.normalBuffer);
            computeTerrainGen.SetBuffer(0, "uv", meshBuffers.uvBuffer);

            computeTerrainGen.SetFloat("resolution", chunkAttributes.Vertices.Length);
            computeTerrainGen.SetFloat("maxHeight", maxHeight);
            computeTerrainGen.SetFloat("minHeight", minHeight);
            computeTerrainGen.SetFloat("meshSize", width);

            computeTerrainGen.SetInt("meshLineSize", chunkDimensions.vertexPerSide);
            computeTerrainGen.SetInt("incrementStep", lodIncrementStep);
        }

        //
        //
        private void RetrieveDataFromComputeShader(MeshComputeBuffers meshBuffers, ITerrainMeshAttributeModifier chunkAttributes)
        {
            meshBuffers.vertBuffer.GetData(chunkAttributes.Vertices);
            meshBuffers.normalBuffer.GetData(chunkAttributes.Normals);
            meshBuffers.uvBuffer.GetData(chunkAttributes.UVs);
            meshBuffers.trisBuffer.GetData(chunkAttributes.Quads);
        }

        //
        //
        private void DisposeBuffersIntoGarbageCollection(MeshComputeBuffers meshBuffers)
        {
            meshBuffers.vertBuffer.Dispose();
            meshBuffers.normalBuffer.Dispose();
            meshBuffers.uvBuffer.Dispose();
            meshBuffers.noiseBuffer.Dispose();
            meshBuffers.trisBuffer.Dispose();
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

                    PopulateMeshAttributes();
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
