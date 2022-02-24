using MiniProceduralGeneration.Chunk.Processors.ChunkMeshProcessor;
using MiniProceduralGeneration.Generator.Entities;
using MiniProceduralGeneration.Noise;
using MiniProceduralGeneration.Seed;
using MiniProceduralGeneration.TerrainCore;
using UnityEngine;

namespace MiniProceduralGeneration.Chunk
{

    public class AsyncChunkShell : ChunkShell
    {

        #region - - - - - - Fields - - - - - -

        private ITerrainAttributes m_TerrainAttributes;
        private IChunkMeshProcessor m_ChunkMeshProcessor;
        private INoiseGenerator m_NoiseGenerator;

        #endregion Fields

        #region - - - - - - Methods - - - - - -

        public override void InitChunkShell(TerrainChunkDimensions chunkDimensions, ITerrainAttributes terrainAttributes, ISeedGenerator seedGenerator, INoiseOffsetGenerator offsetGenerator)
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

            m_ChunkMeshProcessor.ProcessChunk(noiseData);
            AssignDataToMesh();
            RenderTerrain();
        }

        #endregion Methods

    }

}
