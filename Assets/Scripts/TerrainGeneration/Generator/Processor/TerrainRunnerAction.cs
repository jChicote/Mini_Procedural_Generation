using MiniProceduralGeneration.Generator.MeshWork;

namespace MiniProceduralGeneration.Generator.Processor
{

    public class TerrainRunnerAction
    {

        #region - - - - Fields - - - -

        private readonly float[] noiseData;
        private readonly INoiseGenerator noiseGenerator;
        private readonly ITerrainAttributes attributes;
        private readonly ITerrainProcessor terrainProcessor;

        #endregion Fields

        #region - - - - Constructors - - - -

        public TerrainRunnerAction(ITerrainAttributes attributes, ITerrainProcessor terrainProcessor, INoiseGenerator noiseGenerator)
        {
            this.attributes = attributes;
            this.terrainProcessor = terrainProcessor;
            this.noiseGenerator = noiseGenerator;
        }

        #endregion Constructors

        #region - - - - Methods - - - -

        public void IterateThroughChunkArraySelection(ITerrainChunk[] chunks)
        {
            foreach (ITerrainChunk chunk in chunks)
            {
                ProcessChunk(noiseData, chunk);
            }
        }

        private void ProcessChunk(float[] noiseData, ITerrainChunk chunk)
        {
            if (noiseData is null)
                _ = new float[0];

            noiseData = noiseGenerator.SampleNoiseDataAtLocation(attributes.ChunkWidth, chunk.PositionWorldSpace);
            terrainProcessor.ProcessChunkMesh(chunk, noiseData);
            chunk.BuildMesh();

            // cleans buffers before next use.
            terrainProcessor.DisposeBuffersIntoGarbageCollection();
        }

        #endregion Methods

    }

}
