using MiniProceduralGeneration.Chunk.Processors.ChunkMeshProcessor;
using MiniProceduralGeneration.Generator.Entities;
using MiniProceduralGeneration.Noise;
using MiniProceduralGeneration.Seed;
using MiniProceduralGeneration.TerrainCore;
using System.Collections;
using UnityEngine;

namespace MiniProceduralGeneration.Chunk
{

    public class AsyncChunkShell : ChunkShell
    {

        #region - - - - - - Fields - - - - - -

        private ITerrainAttributes m_TerrainAttributes;
        private IChunkMeshProcessor m_ChunkMeshProcessor;
        private INoiseGenerator m_NoiseGenerator;

        private Color GizmosColor;

        #endregion Fields

        #region - - - - - - Methods - - - - - -

        private void Awake()
        {
            GizmosColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        }

        public override void InitChunkShell(
            TerrainChunkDimensions chunkDimensions,
            ITerrainAttributes terrainAttributes,
            ISeedGenerator seedGenerator,
            INoiseOffsetGenerator offsetGenerator)
        {
            base.InitChunkShell(chunkDimensions, terrainAttributes, seedGenerator, offsetGenerator);

            this.m_TerrainAttributes = terrainAttributes;

            this.m_ChunkMeshProcessor = this.GetComponent<IChunkMeshProcessor>();
            this.m_NoiseGenerator = this.GetComponent<INoiseGenerator>();

            m_NoiseGenerator.StepOffsets = offsetGenerator.OctaveOffsets;
            m_NoiseGenerator.InitNoiseGenerator(offsetGenerator);

            m_ChunkMeshProcessor.InitChunkMeshProcessor(terrainAttributes);
        }

        public override void BuildMesh()
        {
            mesh = new Mesh();

            float[] noiseData = m_NoiseGenerator.SampleNoiseDataAtLocation
            (
                m_TerrainAttributes.ActualChunkSize,
                this.transform.position
            );

            StartCoroutine(AsyncProcessChunk(noiseData));
        }

        private IEnumerator AsyncProcessChunk(float[] noiseData)
        {
            yield return StartCoroutine(m_ChunkMeshProcessor.ProcessChunk(noiseData));

            AssignDataToMesh();
            RenderTerrain();
            meshRenderer.enabled = true;
        }

        private void OnDrawGizmos()
        {
            if (Normals != null)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    Gizmos.color = GizmosColor; new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
                    Gizmos.DrawLine(transform.position + vertices[i], (transform.position + vertices[i]) + (Normals[i] * 5f));
                }
            }
        }

        #endregion Methods

    }

}
