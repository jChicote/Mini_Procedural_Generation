using UnityEngine;
using MiniProceduralGeneration.Generator.MeshWork;

namespace MiniProceduralGeneration.Generator.Procesor
{
    public interface ITerrainProcessor
    {
        void ProcessChunkMesh(ITerrainMeshAttributeModifier chunkAttributes, float[] noiseData);
        void DisposeBuffersIntoGarbageCollection();
    }

    /// <summary>
    /// Processes terrain data through specified compute shader 
    /// </summary>
    public class TerrainProcessor : MonoBehaviour, ITerrainProcessor
    {
        public ComputeShader computeTerrainGen;
        private ITerrainCharacteristics terrainCharacteristics;
        private MeshComputeBuffers meshComputeBuffers;

        private void Awake()
        {
            terrainCharacteristics = this.GetComponent<ITerrainCharacteristics>();
        }

        public void ProcessChunkMesh(ITerrainMeshAttributeModifier chunkAttributes, float[] noiseData)
        {
            meshComputeBuffers = CreateNewMeshBuffers(noiseData, chunkAttributes);// Purpiose is too ambigious

            SetComputeShaderBuffers(meshComputeBuffers, chunkAttributes);
            computeTerrainGen.Dispatch(0, chunkAttributes.Vertices.Length / 10, 1, 1);
            RetrieveDataFromComputeShader(meshComputeBuffers, chunkAttributes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="noiseData"></param>
        /// <param name="chunkAttributes"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="meshBuffers"></param>
        /// <param name="chunkAttributes"></param>
        private void SetComputeShaderBuffers(MeshComputeBuffers meshBuffers, ITerrainMeshAttributeModifier chunkAttributes)
        {
            computeTerrainGen.SetBuffer(0, "vertices", meshBuffers.vertBuffer);
            computeTerrainGen.SetBuffer(0, "noiseData", meshBuffers.noiseBuffer);
            computeTerrainGen.SetBuffer(0, "triangles", meshBuffers.trisBuffer);
            computeTerrainGen.SetBuffer(0, "normal", meshBuffers.normalBuffer);
            computeTerrainGen.SetBuffer(0, "uv", meshBuffers.uvBuffer);

            computeTerrainGen.SetFloat("resolution", chunkAttributes.Vertices.Length);
            computeTerrainGen.SetFloat("maxHeight", terrainCharacteristics.MaxHeight);
            computeTerrainGen.SetFloat("minHeight", terrainCharacteristics.MinHeight);
            computeTerrainGen.SetFloat("meshSize", terrainCharacteristics.Width);

            computeTerrainGen.SetInt("meshLineSize", terrainCharacteristics.VertexPerSide);
            computeTerrainGen.SetInt("incrementStep", terrainCharacteristics.LODIncrementStep);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meshBuffers"></param>
        /// <param name="chunkAttributes"></param>
        private void RetrieveDataFromComputeShader(MeshComputeBuffers meshBuffers, ITerrainMeshAttributeModifier chunkAttributes)
        {
            meshBuffers.vertBuffer.GetData(chunkAttributes.Vertices);
            meshBuffers.normalBuffer.GetData(chunkAttributes.Normals);
            meshBuffers.uvBuffer.GetData(chunkAttributes.UVs);
            meshBuffers.trisBuffer.GetData(chunkAttributes.Quads);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meshBuffers"></param>
        public void DisposeBuffersIntoGarbageCollection()
        {
            meshComputeBuffers.vertBuffer.Dispose();
            meshComputeBuffers.normalBuffer.Dispose();
            meshComputeBuffers.uvBuffer.Dispose();
            meshComputeBuffers.noiseBuffer.Dispose();
            meshComputeBuffers.trisBuffer.Dispose();
        }
    }
}
