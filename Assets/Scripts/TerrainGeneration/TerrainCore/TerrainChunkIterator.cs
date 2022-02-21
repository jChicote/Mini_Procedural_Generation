using MiniProceduralGeneration.Chunk;
using MiniProceduralGeneration.Generator.Entities;
using MiniProceduralGeneration.Noise;
using MiniProceduralGeneration.TerrainCore.Infrastructure.ChunkDimensionsResolver;
using MiniProceduralGeneration.TerrainCore.Processor;
using UnityEngine;

namespace MiniProceduralGeneration.TerrainCore
{

    public class TerrainChunkIterator : MonoBehaviour, ITerrainChunkIterator
    {

        #region - - - - Fields - - - -

        private ChunkDimensionsResolver chunkResolver;
        private TerrainChunkDimensions chunkDimensions;

        private float[] noiseData;
        private INoiseGenerator noiseGenerator;
        private ITerrainAttributes attributes;
        private IMeshTerrainProcessor terrainProcessor;

        #endregion Fields

        #region - - - - Constructors - - - -

        public void StartTerrainRunnerAction(ITerrainAttributes attributes, IMeshTerrainProcessor terrainProcessor, INoiseGenerator noiseGenerator)
        {
            this.attributes = attributes;
            this.terrainProcessor = terrainProcessor;
            this.noiseGenerator = noiseGenerator;

            chunkResolver = this.GetComponent<ChunkDimensionsResolver>();
        }

        #endregion Constructors

        #region - - - - Methods - - - -

        public void IterateThroughChunkArraySelection(IChunkShell[] chunks)
        {
            foreach (IChunkShell chunk in chunks)
            {
                ProcessChunk(chunk, false);
            }
        }

        public void ProcessChunk(IChunkShell chunk, bool forceUpdate)
        {
            if (noiseData is null)
                _ = new float[0];

            chunkDimensions = chunkResolver.GetChunkDimensions(chunk.PositionWorldSpace);

            if (!forceUpdate && chunk.Dimensions != null)
                if (chunk.Dimensions.LevelOfDetail == chunkDimensions.LevelOfDetail)
                    return;

            chunk.InitialiseMeshArrays(chunkDimensions);

            this.noiseData = noiseGenerator.SampleNoiseDataAtLocation(attributes.ActualChunkSize, chunk.PositionWorldSpace);
            terrainProcessor.ProcessChunkMesh(chunk, noiseData);
            chunk.BuildMesh();
        }

        #endregion Methods

    }

}
