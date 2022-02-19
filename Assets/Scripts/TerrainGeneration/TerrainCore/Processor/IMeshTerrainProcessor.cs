using MiniProceduralGeneration.Chunk;

namespace MiniProceduralGeneration.TerrainCore.Processor
{
    public interface IMeshTerrainProcessor
    {

        #region - - - - - - Methods - - - - - -

        void ProcessChunkMesh(IChunkShell chunk, float[] noiseData);

        #endregion Methods

    }

}
