using MiniProceduralGeneration.Chunk;
using MiniProceduralGeneration.Controllers;
using MiniProceduralGeneration.Handler;
using MiniProceduralGeneration.TerrainCore;
using UnityEngine;

namespace MiniProceduralGeneration.MapGrid
{

    /// <summary>
    /// Creates Chunk Map for Terrain Generation.
    /// </summary>
    public class ChunkMapCreator : GameHandler, IMapGridCreator
    {

        #region - - - - Fields - - - -

        public GameObject chunkPrefab; // Remove
        public Transform targetObject; // Remove

        private ITerrainAttributes terrainAttributes;
        private ICalculateMapBorder calculateBorder;

        #endregion Fields

        #region - - - - Properties - - - -

        public int ChunkDistance { get; set; }

        #endregion Properties

        #region - - - - Methods - - - -

        public override object AwakeHandle(object request)
        {
            terrainAttributes = this.GetComponent<ITerrainAttributes>();
            calculateBorder = this.GetComponent<ICalculateMapBorder>();

            IMapGridInfoController infoController = FindObjectOfType<MapGridInfoController>().GetComponent<IMapGridInfoController>(); // Can be better ... make a generic interface getter
            infoController.GetMapGridAttributes(this);

            return base.AwakeHandle(request);
        }

        /// <summary>
        /// Creates a completed terrain map with a collection of seperate chunks
        /// </summary>
        public void CreateChunkMap(ITerrainChunkCollection terrainChunks)
        {
            ClearMap(terrainChunks.ChunkArray);

            // chunk creation scroll backtracks
            for (int z = ChunkDistance; z > -ChunkDistance - 1; z--)
            {
                for (int x = -ChunkDistance; x < ChunkDistance + 1; x++)
                {
                    terrainChunks.ChunkArray = AddToChunkArray(terrainChunks.ChunkArray,
                                                new Vector3((x * terrainAttributes.RenderChunkSize) + targetObject.position.x,
                                                    0,
                                                    (z * terrainAttributes.RenderChunkSize) + targetObject.position.z));
                }
            }

            calculateBorder.DefineMapBorders();
        }

        private IChunkShell[] AddToChunkArray(IChunkShell[] chunkArray, Vector3 newPosition)
        {
            IChunkShell[] tempArray = new IChunkShell[chunkArray.Length + 1];

            for (int i = 0; i < chunkArray.Length; i++)
                tempArray[i] = chunkArray[i];

            tempArray[tempArray.Length - 1] = CreateChunk(newPosition);
            return tempArray;
        }

        public IChunkShell CreateChunk(Vector3 newPosition)
            => Instantiate(chunkPrefab, newPosition, Quaternion.identity).GetComponent<IChunkShell>();

        public void ClearMap(IChunkShell[] chunkArray)
        {
            if (chunkArray.Length == 0) return;

            int index = chunkArray.Length - 1;
            while (index >= 0)
            {
                DestroyChunk(chunkArray[index]);
                index--;
            }
        }

        private void DestroyChunk(IChunkShell chunk)
            => chunk.OnDestroyChunk();

        #endregion Methods

    }

}
