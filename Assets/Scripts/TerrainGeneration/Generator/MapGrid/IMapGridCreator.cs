using MiniProceduralGeneration.Generator.MeshWork;

namespace MiniProceduralGeneration.Generator.Creator.Map
{

    public interface IMapGridCreator : IMapGridAttributes, IMapGridTileActions
    {
    }

    public interface IMapGridTileActions
    {

        void CreateChunkMap(ITerrainChunks terrainChunks);
        void ClearMap(ITerrainChunk[] chunkArray);

    }

    public interface IMapGridAttributes
    {

        int ChunkDistance { get; set; }

    }

}
