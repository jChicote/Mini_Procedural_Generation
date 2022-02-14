
using MiniProceduralGeneration.Chunk;
using MiniProceduralGeneration.TerrainCore;

namespace MiniProceduralGeneration.MapGrid
{

    public interface IMapGridCreator : IMapGridAttributes, IMapGridTileActions
    {
    }

    public interface IMapGridTileActions
    {

        void CreateChunkMap(ITerrainChunkCollection terrainChunks);
        void ClearMap(IChunkShell[] chunkArray);

    }

    public interface IMapGridAttributes
    {

        int ChunkDistance { get; set; }

    }

}
