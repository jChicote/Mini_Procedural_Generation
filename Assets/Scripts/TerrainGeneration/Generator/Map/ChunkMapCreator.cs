using MiniProceduralGeneration.Generator.Entities;
using MiniProceduralGeneration.Generator.MeshWork;
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
    public class ChunkMapCreator : MonoBehaviour, IMapCreator
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

        private void Awake()
        {
            chunkMap = new ChunkMap
            {
                ChunkArrayInterface = this.GetComponent<ITerrainChunkArray>(),
                Characteristics = this.GetComponent<ITerrainCharacteristics>(),
            };
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
            ITerrainChunk[] tempArray = new ITerrainChunk[chunkMap.ChunkArrayInterface.TerrainChunks.Length + 1];

            for (int i = 0; i < chunkMap.ChunkArrayInterface.TerrainChunks.Length; i++)
            {
                tempArray[i] = chunkMap.ChunkArrayInterface.TerrainChunks[i];
            }

            tempArray[tempArray.Length - 1] = Instantiate(chunkPrefab, newPosition, Quaternion.identity).GetComponent<ITerrainChunk>();
            chunkMap.ChunkArrayInterface.TerrainChunks = tempArray;
        }

        private void ClearMap()
        {
            if (chunkMap.ChunkArrayInterface.TerrainChunks.Length == 0) return;

            int index = chunkMap.ChunkArrayInterface.TerrainChunks.Length - 1;

            while (index >= 0)
            {
                DestroyChunk(chunkMap.ChunkArrayInterface.TerrainChunks[index], index);
                index--;
            }

            chunkMap.ChunkArrayInterface.TerrainChunks = new ITerrainChunk[0];
        }

        private void DestroyChunk(ITerrainChunk chunk, int index)
        {
            chunkMap.ChunkArrayInterface.TerrainChunks[index] = null;
            chunk.OnDestroyChunk();
        }

        #endregion Methods
    }
}
