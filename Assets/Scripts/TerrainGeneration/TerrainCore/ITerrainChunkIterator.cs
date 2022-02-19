using MiniProceduralGeneration.Chunk;

namespace MiniProceduralGeneration.TerrainCore
{
    public interface ITerrainChunkIterator
    {

        #region - - - - - - Methods - - - - - -

        void IterateThroughChunkArraySelection(IChunkShell[] chunks);
        void ProcessChunk(IChunkShell chunk);

        #endregion Methods

    }

}
