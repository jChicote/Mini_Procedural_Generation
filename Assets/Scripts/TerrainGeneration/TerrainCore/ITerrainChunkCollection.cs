using MiniProceduralGeneration.Chunk;

namespace MiniProceduralGeneration.TerrainCore
{
    public interface ITerrainChunkCollection
    {

        #region - - - - - - Properties - - - - - -

        IChunkShell[] ChunkArray { get; set; }

        #endregion Properties

    }

}
