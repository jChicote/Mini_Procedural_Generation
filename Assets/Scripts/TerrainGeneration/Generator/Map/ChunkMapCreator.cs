using MiniProceduralGeneration.Generator.Entities;
using MiniProceduralGeneration.Generator.MeshWork;
using MiniProceduralGeneration.Handler;
using UnityEngine;

namespace MiniProceduralGeneration.Generator.Creator.Map
{

    public interface IMapCreator
    {
        ChunkMap ChunkMap { get; }
    }

    /// <summary>
    /// Creates Chunk Map for Terrain Generation.
    /// </summary>
    public class ChunkMapCreator : GameHandler, IMapCreator
    {
        #region ------ Fields ------

        public GameObject chunkPrefab;
        public Transform targetObject;
        private ChunkMap chunkMap;

        #endregion Fields

        #region ------ Properties ------

        public ChunkMap ChunkMap => chunkMap;

        #endregion

        #region ------ Methods ------

        public override object Handle(object request)
        {
            print("Handle 1 ran first");
            chunkMap = new ChunkMap
            {
                TerrainChunks = new ITerrainChunk[0],
                Characteristics = this.GetComponent<ITerrainCharacteristics>()
            };

            return base.Handle(request);
        }

        /// <summary>
        /// Creates a completed terrain map with a collection of seperate chunks
        /// </summary>
        public void CreateChunkMap()
        {
            ClearMap();

            // chunk scroll backtracks
            for (int z = chunkMap.ChunkDistance; z > -chunkMap.ChunkDistance - 1; z--)
            {
                for (int x = -chunkMap.ChunkDistance; x < chunkMap.ChunkDistance + 1; x++)
                {
                    CreateChunk(new Vector3((x * chunkMap.Characteristics.MapSize) + targetObject.position.x, 0, (z * chunkMap.Characteristics.MapSize) + targetObject.position.z));
                }
            }
        }

        public void CreateChunk(Vector3 newPosition)
        {
            ITerrainChunk[] tempArray = new ITerrainChunk[chunkMap.TerrainChunks.Length + 1];

            for (int i = 0; i < chunkMap.TerrainChunks.Length; i++)
            {
                tempArray[i] = chunkMap.TerrainChunks[i];
            }

            tempArray[tempArray.Length - 1] = Instantiate(chunkPrefab, newPosition, Quaternion.identity).GetComponent<ITerrainChunk>();
            chunkMap.TerrainChunks = tempArray;
        }

        private void ClearMap()
        {
            if (chunkMap.TerrainChunks.Length == 0) return;

            int index = chunkMap.TerrainChunks.Length - 1;

            while (index >= 0)
            {
                DestroyChunk(chunkMap.TerrainChunks[index], index);
                index--;
            }

            chunkMap.TerrainChunks = new ITerrainChunk[0];
        }

        private void DestroyChunk(ITerrainChunk chunk, int index)
        {
            chunkMap.TerrainChunks[index] = null;
            chunk.OnDestroyChunk();
        }

        #endregion Methods
    }
}
