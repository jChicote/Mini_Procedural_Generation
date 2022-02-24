using MiniProceduralGeneration.Chunk;
using MiniProceduralGeneration.ComputeShaders.Processors;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

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
        IChunkMeshAttributes chunkModifier;
        private float[] noiseData;

        #endregion Fields

        #region - - - - - - Methods - - - - - -

        private void Awake()
        {
            terrainCharacteristics = this.GetComponent<ITerrainAttributes>();
            meshBuffers = new MeshComputeBuffers();
        }


        //  This one works for some odd readson
        public void ProcessChunkMesh(IChunkShell chunk, float[] noiseData)
        {
            this.chunkDimensions = chunk;
            this.chunkMeshAttributes = chunk;
            chunkModifier = chunk;
            this.noiseData = noiseData;

            CreateShaderBuffers();
            SetComputeShaderData();

            shaderProcessor.Dispatch(0, chunk.Vertices.Length / 10, 1, 1);  // Processes terrain input to mesh data
            RetrieveDataFromComputeShader(chunk);
            ReleaseBuffersToGarbageCollection();
        }

        //  This one proceduces chunk issues with LOD resoltions at full detail
        // THIS IS A DUMB SOLUTION IT IS CAUSING MULTIPLE INDEPENDANT COROUTINES TO RUN
        // BLOODY DESTROYING MY CPU, GPU AND MEMORY
        public IEnumerator ProcessChunkMesh(IChunkShell chunk, float[] noiseData, Action chunkAction)
        {
            yield return null;

            this.chunkDimensions = chunk;
            this.chunkMeshAttributes = chunk;
            chunkModifier = chunk;
            this.noiseData = noiseData;

            CreateShaderBuffers();
            SetComputeShaderData();


            //while (true)
            //{
            shaderProcessor.Dispatch(0, chunk.Vertices.Length / 10, 1, 1);  // Processes terrain input to mesh data

            //print(meshBuffers.vertBuffer.IsValid());
            var vertRequest = AsyncGPUReadback.Request(meshBuffers.vertBuffer, RetrieveVertexDataFromBuffer);
            yield return new WaitUntil(() => vertRequest.done);

            var normalRequest = AsyncGPUReadback.Request(meshBuffers.normalBuffer, RetrieveNormalDataFromBuffer);
            yield return new WaitUntil(() => vertRequest.done);

            var uvRequest = AsyncGPUReadback.Request(meshBuffers.uvBuffer, RetrieveUVsDataFromBuffer);
            yield return new WaitUntil(() => uvRequest.done);

            var triangleRequest = AsyncGPUReadback.Request(meshBuffers.triangleBuffer, RetrieveTriangleDataFromBuffer);
            yield return new WaitUntil(() => triangleRequest.done);

            //yield return Wait;
           // RetrieveDataFromComputeShader(chunk);

                //print("Has approached end");
                //meshBuffers.noiseBuffer.Dispose();
            //ReleaseBuffersToGarbageCollection();

            //}
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

        private void RetrieveVertexDataFromBuffer(AsyncGPUReadbackRequest request)
        {
            chunkModifier.Vertices = request.GetData<Vector3>().ToArray();
            //print(request.done);
            meshBuffers.vertBuffer.Release();
            //print(chunkModifier.Vertices.Length);
        }

        private void RetrieveNormalDataFromBuffer(AsyncGPUReadbackRequest request)
        {
            chunkModifier.Normals = request.GetData<Vector3>().ToArray();
            //print(request.done);
            meshBuffers.normalBuffer.Release();
            //print(chunkModifier.Normals.Length);
        }

        private void RetrieveUVsDataFromBuffer(AsyncGPUReadbackRequest request)
        {
            chunkModifier.UVs = request.GetData<Vector2>().ToArray();
            //print(request.done);
             meshBuffers.uvBuffer.Release();
            // print(chunkModifier.UVs.Length);
        }

        private void RetrieveTriangleDataFromBuffer(AsyncGPUReadbackRequest request)
        {
            chunkModifier.Triangles = request.GetData<int>().ToArray();
            //print(request.done);
            meshBuffers.triangleBuffer.Release();
            //print(chunkModifier.Triangles.Length);
            //ReleaseBuffersToGarbageCollection();
        }

        protected override void ReleaseBuffersToGarbageCollection()
        {
            //AsyncGPUReadback.WaitAllRequests();

            meshBuffers.vertBuffer.Dispose();
            meshBuffers.normalBuffer.Dispose();
            meshBuffers.uvBuffer.Dispose();
            meshBuffers.noiseBuffer.Dispose();
            meshBuffers.triangleBuffer.Dispose();
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
