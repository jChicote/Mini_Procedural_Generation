using MiniProceduralGeneration.Chunk;

namespace MiniProceduralGeneration.TerrainCore
{
    public interface ITerrainRunnerAction
    {

        #region - - - - - - Methods - - - - - -

        void IterateThroughChunkArraySelection(IChunkShell[] chunks);
        void ProcessChunk(IChunkShell chunk);

        #endregion Methods

    }

}
