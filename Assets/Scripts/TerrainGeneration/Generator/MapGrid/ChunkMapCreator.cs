using MiniProceduralGeneration.Generator.MeshWork;
using MiniProceduralGeneration.Generator.Scroller;
using MiniProceduralGeneration.Handler;
using UnityEngine;

namespace MiniProceduralGeneration.Generator.Creator.Map
{

    public interface IMapGridCreator
    {

        public int ChunkDistance { get; set; }

        void CreateChunkMap(ITerrainChunks terrainChunks);
        void ClearMap(ITerrainChunk[] chunkArray);

    }

    /// <summary>
    /// Creates Chunk Map for Terrain Generation.
    /// </summary>
    public class ChunkMapCreator : GameHandler, IMapGridCreator
    {

        #region ------ Fields ------

        public GameObject chunkPrefab; // Remove
        public Transform targetObject; // Remove

        [SerializeField]
        private int chunkDistance = 2; // Change

        private ITerrainAttributes terrainAttributes;
        private ICalculateMapBorder calculateBorder;

        #endregion Fields

        #region ------ Properties ------

        public int ChunkDistance { get => chunkDistance; set => chunkDistance = value; }

        #endregion

        #region ------ Methods ------

        public override object Handle(object request)
        {
            terrainAttributes = this.GetComponent<ITerrainAttributes>();
            calculateBorder = this.GetComponent<ICalculateMapBorder>();

            return base.Handle(request);
        }

        /// <summary>
        /// Creates a completed terrain map with a collection of seperate chunks
        /// </summary>
        public void CreateChunkMap(ITerrainChunks terrainChunks)
        {
            ClearMap(terrainChunks.ChunkArray);

            // chunk scroll backtracks
            for (int z = chunkDistance; z > -chunkDistance - 1; z--)
            {
                for (int x = -chunkDistance; x < chunkDistance + 1; x++)
                {
                    terrainChunks.ChunkArray = AddToChunkArray(terrainChunks.ChunkArray,
                                                new Vector3((x * terrainAttributes.ChunkWidth) + targetObject.position.x, 0,
                                                    (z * terrainAttributes.ChunkWidth) + targetObject.position.z));
                }
            }

            calculateBorder.DefineMapBorders();
        }

        private ITerrainChunk[] AddToChunkArray(ITerrainChunk[] chunkArray, Vector3 newPosition)
        {
            ITerrainChunk[] tempArray = new ITerrainChunk[chunkArray.Length + 1];

            for (int i = 0; i < chunkArray.Length; i++)
                tempArray[i] = chunkArray[i];

            tempArray[tempArray.Length - 1] = CreateChunk(newPosition);
            return tempArray;
        }

        public ITerrainChunk CreateChunk(Vector3 newPosition)
            => Instantiate(chunkPrefab, newPosition, Quaternion.identity).GetComponent<ITerrainChunk>();

        public void ClearMap(ITerrainChunk[] chunkArray)
        {
            if (chunkArray.Length == 0) return;

            int index = chunkArray.Length - 1;
            while (index >= 0)
            {
                DestroyChunk(chunkArray[index]);
                index--;
            }
        }

        private void DestroyChunk(ITerrainChunk chunk)
            => chunk.OnDestroyChunk();

        #endregion Methods

    }

}
