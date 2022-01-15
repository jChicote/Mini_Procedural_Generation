using MiniProceduralGeneration.Generator.MeshWork;

namespace MiniProceduralGeneration.Generator.Entities
{

    public class ChunkMap
    {
        #region ------ Field ------

        private int chunkDistance = 2;

        #endregion Field

        #region ------ Properties ------

        public ITerrainCharacteristics Characteristics { get; set; }

        public ITerrainChunk[] TerrainChunks { get; set; }

        public int ChunkDistance
        {
            get => chunkDistance;
            set => chunkDistance = value;
        }

        public int MapSize => Characteristics.MapSize;

        public int MapEdgeSize => chunkDistance * 2 + 1;

        #endregion Properties
    }
}
