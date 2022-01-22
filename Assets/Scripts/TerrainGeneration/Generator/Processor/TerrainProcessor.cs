using MiniProceduralGeneration.Generator.MeshWork;
using UnityEngine;

namespace MiniProceduralGeneration.Generator.Processor
{
    public interface ITerrainProcessor
    {
        void ProcessChunkMesh(IChunkMeshAttributes chunkAttributes, float[] noiseData);
        void DisposeBuffersIntoGarbageCollection();
    }

    /// <summary>
    /// Processes terrain data through specified compute shader.
    /// </summary>
    public class TerrainProcessor : MonoBehaviour, ITerrainProcessor
    {
        // Fields
        public ComputeShader computeTerrainGen;
        private ITerrainAttributes terrainCharacteristics;
        private MeshComputeBuffers meshBuffers;

        private void Awake()
        {
            terrainCharacteristics = this.GetComponent<ITerrainAttributes>();
            meshBuffers = new MeshComputeBuffers();
        }

        public void ProcessChunkMesh(IChunkMeshAttributes chunkAttributes, float[] noiseData)
        {
            CreateNewMeshBuffers(noiseData, chunkAttributes);

            SetComputeShaderVariables(chunkAttributes);
            computeTerrainGen.Dispatch(0, chunkAttributes.Vertices.Length / 10, 1, 1);  // Processes terrain input to mesh data
            RetrieveDataFromComputeShader(chunkAttributes);
        }

        /// <summary>
        /// Creates mesh buffers to prepare structured buffers to specified array sizes
        /// and strides.
        /// </summary>
        /// <param name="noiseData">Generated noise</param>
        /// <param name="chunkAttributes">Interface to terrain attributes of mesh.</param>
        private void CreateNewMeshBuffers(float[] noiseData, IChunkMeshAttributes chunkAttributes)
        {
            meshBuffers.vertBuffer = new ComputeBuffer(chunkAttributes.Vertices.Length, sizeof(float) * 3);
            meshBuffers.vertBuffer.SetData(chunkAttributes.Vertices);

            meshBuffers.normalBuffer = new ComputeBuffer(chunkAttributes.Vertices.Length, sizeof(float) * 3);
            meshBuffers.normalBuffer.SetData(chunkAttributes.Normals);

            meshBuffers.uvBuffer = new ComputeBuffer(chunkAttributes.Vertices.Length, sizeof(float) * 2);
            meshBuffers.uvBuffer.SetData(chunkAttributes.UVs);

            meshBuffers.noiseBuffer = new ComputeBuffer(noiseData.Length, sizeof(float));
            meshBuffers.noiseBuffer.SetData(noiseData);

            meshBuffers.triangleBuffer = new ComputeBuffer(chunkAttributes.Triangles.Length, sizeof(int));
            meshBuffers.triangleBuffer.SetData(chunkAttributes.Triangles);
        }


        /// <summary>
        /// Sets the compute shader to recieve variables of input terrain data.
        /// </summary>
        /// <param name="chunkAttributes">Interface to terrain attributes of mesh.</param>
        private void SetComputeShaderVariables(IChunkMeshAttributes chunkAttributes)
        {
            computeTerrainGen.SetBuffer(0, "vertices", meshBuffers.vertBuffer);
            computeTerrainGen.SetBuffer(0, "noiseData", meshBuffers.noiseBuffer);
            computeTerrainGen.SetBuffer(0, "triangles", meshBuffers.triangleBuffer);
            computeTerrainGen.SetBuffer(0, "normal", meshBuffers.normalBuffer);
            computeTerrainGen.SetBuffer(0, "uv", meshBuffers.uvBuffer);

            computeTerrainGen.SetFloat("resolution", chunkAttributes.Vertices.Length);
            computeTerrainGen.SetFloat("maxHeight", terrainCharacteristics.MaxHeight);
            computeTerrainGen.SetFloat("minHeight", terrainCharacteristics.MinHeight);
            computeTerrainGen.SetFloat("meshSize", terrainCharacteristics.ChunkWidth);

            computeTerrainGen.SetInt("meshLineSize", terrainCharacteristics.VertexPerSide);
            computeTerrainGen.SetInt("incrementStep", terrainCharacteristics.LODIncrementStep);
        }

        /// <summary>
        /// Collects mesh data from compute shader to be outputted to terrain chunk variables.
        /// </summary>
        /// <param name="chunkAttributes">Interface modifier to targetted terrain chunk instance.</param>
        private void RetrieveDataFromComputeShader(IChunkMeshAttributes chunkModifier)
        {
            meshBuffers.vertBuffer.GetData(chunkModifier.Vertices);
            meshBuffers.normalBuffer.GetData(chunkModifier.Normals);
            meshBuffers.uvBuffer.GetData(chunkModifier.UVs);
            meshBuffers.triangleBuffer.GetData(chunkModifier.Triangles);
        }

        public void DisposeBuffersIntoGarbageCollection()
        {
            meshBuffers.vertBuffer.Dispose();
            meshBuffers.normalBuffer.Dispose();
            meshBuffers.uvBuffer.Dispose();
            meshBuffers.noiseBuffer.Dispose();
            meshBuffers.triangleBuffer.Dispose();
        }
    }

    public struct MeshComputeBuffers
    {
        public ComputeBuffer vertBuffer;
        public ComputeBuffer normalBuffer;
        public ComputeBuffer uvBuffer;
        public ComputeBuffer noiseBuffer;
        public ComputeBuffer triangleBuffer;
    }

}
