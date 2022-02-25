using MiniProceduralGeneration.ComputeShaders.Processors;
using MiniProceduralGeneration.TerrainCore;
using MiniProceduralGeneration.TerrainCore.Processor;
using UnityEngine;
using UnityEngine.Rendering;

namespace MiniProceduralGeneration.Chunk.Processors.ChunkMeshProcessor
{

    public interface IChunkMeshProcessor
    {

        #region - - - - - - Methods - - - - - -

        void InitChunkMeshProcessor(ITerrainAttributes terrainAttributes);
        void ProcessChunk(float[] noiseData);

        #endregion Methods

    }

    public class ChunkMeshProcessor : BaseProcessor, IChunkMeshProcessor
    {

        #region - - - - - - Fields - - - - - -

        private ITerrainAttributes m_TerrainAttributes;
        private IChunkMeshAttributes m_ChunkMeshAttributes;
        private IChunkDimensions m_ChunkDimensions;
        private IChunkShell m_ChunkShell;

        private MeshComputeBuffers meshBuffers;
        private float[] noiseData;

        #endregion Fields

        #region - - - - - - MonoBehaviour - - - - - -

        private void Awake()
        {
            this.m_ChunkMeshAttributes = this.GetComponent<IChunkMeshAttributes>();
            this.m_ChunkDimensions = this.GetComponent<IChunkDimensions>();
            this.m_ChunkShell = this.GetComponent<IChunkShell>();

            this.meshBuffers = new MeshComputeBuffers();
        }

        #endregion MonoBehaviour

        #region - - - - - - Method - - - - - -

        public void InitChunkMeshProcessor(ITerrainAttributes terrainAttributes)
            => this.m_TerrainAttributes = terrainAttributes;

        public void ProcessChunk(float[] noiseData)
        {
            this.noiseData = noiseData;

            CreateShaderBuffers();
            SetComputeShaderData();

            shaderProcessor.Dispatch(0, m_ChunkShell.Vertices.Length / 10, 1, 1);  // Processes terrain input to mesh data

            AsyncGPUReadback.Request(meshBuffers.vertBuffer, RetrieveVertexDataFromBuffer);
            AsyncGPUReadback.Request(meshBuffers.normalBuffer, RetrieveNormalDataFromBuffer);
            AsyncGPUReadback.Request(meshBuffers.uvBuffer, RetrieveUVsDataFromBuffer);
            AsyncGPUReadback.Request(meshBuffers.triangleBuffer, RetrieveTriangleDataFromBuffer);

            meshBuffers.noiseBuffer.Dispose();
            RetrieveDataFromComputeShader(m_ChunkShell);

            //ReleaseBuffersToGarbageCollection();
        }

        protected override void CreateShaderBuffers()
        {
            meshBuffers.vertBuffer = new ComputeBuffer(m_ChunkMeshAttributes.Vertices.Length, sizeof(float) * 3);
            meshBuffers.vertBuffer.SetData(m_ChunkMeshAttributes.Vertices);

            meshBuffers.normalBuffer = new ComputeBuffer(m_ChunkMeshAttributes.Vertices.Length, sizeof(float) * 3);
            meshBuffers.normalBuffer.SetData(m_ChunkMeshAttributes.Normals);

            meshBuffers.uvBuffer = new ComputeBuffer(m_ChunkMeshAttributes.Vertices.Length, sizeof(float) * 2);
            meshBuffers.uvBuffer.SetData(m_ChunkMeshAttributes.UVs);

            meshBuffers.noiseBuffer = new ComputeBuffer(noiseData.Length, sizeof(float));
            meshBuffers.noiseBuffer.SetData(noiseData);

            meshBuffers.triangleBuffer = new ComputeBuffer(m_ChunkMeshAttributes.Triangles.Length, sizeof(int));
            meshBuffers.triangleBuffer.SetData(m_ChunkMeshAttributes.Triangles);
        }

        protected override void SetComputeShaderData()
        {
            shaderProcessor.SetBuffer(0, "vertices", meshBuffers.vertBuffer);
            shaderProcessor.SetBuffer(0, "noiseData", meshBuffers.noiseBuffer);
            shaderProcessor.SetBuffer(0, "triangles", meshBuffers.triangleBuffer);
            shaderProcessor.SetBuffer(0, "normal", meshBuffers.normalBuffer);
            shaderProcessor.SetBuffer(0, "uv", meshBuffers.uvBuffer);

            print(m_TerrainAttributes);

            shaderProcessor.SetFloat("resolution", m_ChunkMeshAttributes.Vertices.Length);
            shaderProcessor.SetFloat("absoluteHeight", m_TerrainAttributes.AbsoluteHeight);
            shaderProcessor.SetFloat("maxHeight", m_TerrainAttributes.MaxHeight);
            shaderProcessor.SetFloat("minHeight", m_TerrainAttributes.MinHeight);
            shaderProcessor.SetFloat("fullChunkSize", m_TerrainAttributes.ActualChunkSize);
            shaderProcessor.SetFloat("renderChunkSize", m_TerrainAttributes.RenderChunkSize);

            shaderProcessor.SetInt("verticesPerSide", m_ChunkDimensions.Dimensions.VertexPerSide);
            shaderProcessor.SetInt("incrementStep", m_TerrainAttributes.LODIncrementStep);
        }

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
        private void RetrieveVertexDataFromBuffer(AsyncGPUReadbackRequest request)
        {
            m_ChunkShell.Vertices = request.GetData<Vector3>().ToArray();
            meshBuffers.vertBuffer.Release();
        }

        private void RetrieveNormalDataFromBuffer(AsyncGPUReadbackRequest request)
        {
            m_ChunkShell.Normals = request.GetData<Vector3>().ToArray();
            meshBuffers.normalBuffer.Release();
        }

        private void RetrieveUVsDataFromBuffer(AsyncGPUReadbackRequest request)
        {
            m_ChunkShell.UVs = request.GetData<Vector2>().ToArray();
            meshBuffers.uvBuffer.Release();
        }

        private void RetrieveTriangleDataFromBuffer(AsyncGPUReadbackRequest request)
        {
            m_ChunkShell.Triangles = request.GetData<int>().ToArray();
            meshBuffers.triangleBuffer.Release();
        }

        #endregion Method

    }

}
