using MiniProceduralGeneration.Chunk;
using MiniProceduralGeneration.Noise;
using MiniProceduralGeneration.TerrainCore.Processor;
using UnityEngine;

namespace MiniProceduralGeneration.TerrainCore
{

    public interface ITerrainRunnerAction
    {

        #region - - - - - - Methods - - - - - -

        void IterateThroughChunkArraySelection(IChunkShell[] chunks);
        void ProcessChunk(IChunkShell chunk);

        #endregion Methods

    }

    public class TerrainRunnerAction : MonoBehaviour, ITerrainRunnerAction
    {

        #region - - - - Fields - - - -

        private float[] noiseData;
        private INoiseGenerator noiseGenerator;
        private ITerrainAttributes attributes;
        private ITerrainProcessor terrainProcessor;

        #endregion Fields

        #region - - - - Constructors - - - -

        public void StartTerrainRunnerAction(ITerrainAttributes attributes, ITerrainProcessor terrainProcessor, INoiseGenerator noiseGenerator)
        {
            this.attributes = attributes;
            this.terrainProcessor = terrainProcessor;
            this.noiseGenerator = noiseGenerator;
        }

        #endregion Constructors

        #region - - - - Methods - - - -

        public void IterateThroughChunkArraySelection(IChunkShell[] chunks)
        {
            foreach (IChunkShell chunk in chunks)
            {
                ProcessChunk(chunk);
            }
        }

        public void ProcessChunk(IChunkShell chunk)
        {
            if (noiseData is null)
                _ = new float[0];

            this.noiseData = noiseGenerator.SampleNoiseDataAtLocation(attributes.ActualChunkSize, chunk.PositionWorldSpace);
            terrainProcessor.ProcessChunkMesh(chunk, noiseData);
            chunk.BuildMesh();

            // cleans buffers before next use.
            terrainProcessor.DisposeBuffersIntoGarbageCollection();
        }

        #endregion Methods

    }

}
