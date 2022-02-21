using MiniProceduralGeneration.Chunk;
using MiniProceduralGeneration.ComputeShaders.Processors;
using UnityEngine;

namespace MiniProceduralGeneration.TerrainCore.Processor
{

    /// <summary>
    /// Processes terrain data through specified compute shader.
    /// </summary>
    public class MeshTerrainProcessor : BaseProcessor, IMeshTerrainProcessor
    {

        #region - - - - - - Fields - - - - - -

        private ITerrainAttributes terrainCharacteristics;
        private MeshComputeBuffers meshBuffers;

        private IChunkMeshAttributes chunkMeshAttributes;
        private IChunkDimensions chunkDimensions;
        private float[] noiseData;

        #endregion Fields

        #region - - - - - - Methods - - - - - -

        private void Awake()
        {
            terrainCharacteristics = this.GetComponent<ITerrainAttributes>();
            meshBuffers = new MeshComputeBuffers();
        }

        public void ProcessChunkMesh(IChunkShell chunk, float[] noiseData)
        {
            this.chunkDimensions = chunk;
            this.chunkMeshAttributes = chunk;
            this.noiseData = noiseData;

            CreateShaderBuffers();
            SetComputeShaderData();
            shaderProcessor.Dispatch(0, chunk.Vertices.Length / 10, 1, 1);  // Processes terrain input to mesh data
            RetrieveDataFromComputeShader(chunk);

            ReleaseBuffersToGarbageCollection();
        }

        /// <summary>
        /// Creates mesh buffers to prepare structured buffers to specified array sizes
        /// and strides.
        /// </summary>
        protected override void CreateShaderBuffers()
        {
            meshBuffers.vertBuffer = new ComputeBuffer(chunkMeshAttributes.Vertices.Length, sizeof(float) * 3);
            meshBuffers.vertBuffer.SetData(chunkMeshAttributes.Vertices);

            meshBuffers.normalBuffer = new ComputeBuffer(chunkMeshAttributes.Vertices.Length, sizeof(float) * 3);
            meshBuffers.normalBuffer.SetData(chunkMeshAttributes.Normals);

            meshBuffers.uvBuffer = new ComputeBuffer(chunkMeshAttributes.Vertices.Length, sizeof(float) * 2);
            meshBuffers.uvBuffer.SetData(chunkMeshAttributes.UVs);

            meshBuffers.noiseBuffer = new ComputeBuffer(noiseData.Length, sizeof(float));
            meshBuffers.noiseBuffer.SetData(noiseData);

            meshBuffers.triangleBuffer = new ComputeBuffer(chunkMeshAttributes.Triangles.Length, sizeof(int));
            meshBuffers.triangleBuffer.SetData(chunkMeshAttributes.Triangles);
        }

        protected override void SetComputeShaderData()
        {
            shaderProcessor.SetBuffer(0, "vertices", meshBuffers.vertBuffer);
            shaderProcessor.SetBuffer(0, "noiseData", meshBuffers.noiseBuffer);
            shaderProcessor.SetBuffer(0, "triangles", meshBuffers.triangleBuffer);
            shaderProcessor.SetBuffer(0, "normal", meshBuffers.normalBuffer);
            shaderProcessor.SetBuffer(0, "uv", meshBuffers.uvBuffer);

            shaderProcessor.SetFloat("resolution", chunkMeshAttributes.Vertices.Length);
            shaderProcessor.SetFloat("absoluteHeight", terrainCharacteristics.AbsoluteHeight);
            shaderProcessor.SetFloat("maxHeight", terrainCharacteristics.MaxHeight);
            shaderProcessor.SetFloat("minHeight", terrainCharacteristics.MinHeight);
            shaderProcessor.SetFloat("fullChunkSize", terrainCharacteristics.ActualChunkSize);
            shaderProcessor.SetFloat("renderChunkSize", terrainCharacteristics.RenderChunkSize);

            shaderProcessor.SetInt("verticesPerSide", chunkDimensions.Dimensions.VertexPerSide);
            shaderProcessor.SetInt("incrementStep", terrainCharacteristics.LODIncrementStep);
        }

        /// <summary>
        /// Collects mesh data from compute shader to be outputted to terrain chunk variables.
        /// </summary>
        private void RetrieveDataFromComputeShader(IChunkMeshAttributes chunkModifier)
        {
            meshBuffers.vertBuffer.GetData(chunkModifier.Vertices);
            meshBuffers.normalBuffer.GetData(chunkModifier.Normals);
            meshBuffers.uvBuffer.GetData(chunkModifier.UVs);
            meshBuffers.triangleBuffer.GetData(chunkModifier.Triangles);
        }

        protected override void ReleaseBuffersToGarbageCollection()
        {
            meshBuffers.vertBuffer.Release();
            meshBuffers.normalBuffer.Release();
            meshBuffers.uvBuffer.Release();
            meshBuffers.noiseBuffer.Release();
            meshBuffers.triangleBuffer.Release();
        }

        #endregion Methods
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
