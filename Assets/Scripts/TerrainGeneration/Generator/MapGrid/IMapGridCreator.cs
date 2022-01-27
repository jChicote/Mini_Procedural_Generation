using MiniProceduralGeneration.Generator.MeshWork;

namespace MiniProceduralGeneration.Generator.Creator.Map
{

    public interface IMapGridCreator : IMapGridAttributes, IMapGridTileActions
    {
    }

    public interface IMapGridTileActions
    {

        void CreateChunkMap(ITerrainChunkCollection terrainChunks);
        void ClearMap(ITerrainChunk[] chunkArray);

    }

    public interface IMapGridAttributes
    {

        int ChunkDistance { get; set; }

    }

}
